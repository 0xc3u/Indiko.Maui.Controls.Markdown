#!/usr/bin/env bash
#
# Derives the next semantic version from the conventional commits made since
# the last version tag, and writes the release notes for them.
#
# Runs as a plain script rather than inline YAML so it can be executed on a
# developer machine against the real history -- `.github/scripts/next-version.sh`
# with no arguments prints what the next release would be and stops. A release
# pipeline whose version logic can only be observed by cutting a release is a
# pipeline nobody can check before it runs.
#
# Replaying it over the published history is the test that matters, and the
# reason the logic is not inline YAML:
#
#   PREV_TAG=v4.2.0 HEAD_REF=v4.3.0 .github/scripts/next-version.sh
#
# Outputs, when GITHUB_OUTPUT is set:
#   previous        the tag it compared against, e.g. v4.3.1
#   bump            major | minor | patch | none
#   version         the version to release, without the v, e.g. 4.3.2
#   tag             the tag to create, e.g. v4.3.2
#   release_needed  true | false
#   notes_file      path to the generated notes
#
set -euo pipefail

NOTES_FILE="${NOTES_FILE:-$(mktemp)}"
TAG_PREFIX="v"

# Conventional Commits, as the repository already uses them:
#   feat          -> minor
#   fix, perf     -> patch
#   <type>!:      -> major
#   BREAKING CHANGE in the body -> major
# Everything else (chore, ci, docs, refactor, test, style, build) releases
# nothing. That is deliberate: a release is a promise to consumers, and a
# commit that changes no shipped behaviour should not make one.
BUMP="none"
FEATURES=""
FIXES=""
BREAKING=""

previous_tag() {
    # Version sort, so v4.10.0 ranks above v4.9.0 -- lexical sort gets this
    # wrong and would re-release an older line.
    #
    # The grep is not redundant with the glob: the glob also matches the
    # prerelease tags this repository used early on (v1.0.0-preview101), and
    # the arithmetic below would read "0-preview101" as a patch number and
    # produce a version no one can parse. Three numbers and nothing else.
    git tag --list "${TAG_PREFIX}[0-9]*" --sort=-v:refname | grep -E "^${TAG_PREFIX}[0-9]+\.[0-9]+\.[0-9]+$" | head -n 1
}

# PREV_TAG and HEAD_REF exist so the script can be replayed over past
# releases and checked against what was actually published. A version
# calculation nobody has reproduced is a guess with a shell around it.
PREVIOUS="${PREV_TAG:-$(previous_tag || true)}"

if [[ -z "$PREVIOUS" ]]; then
    # A repository with no version tag yet starts the history at 0.0.0, so the
    # first feat produces 0.1.0 and the first fix 0.0.1.
    PREVIOUS="${TAG_PREFIX}0.0.0"
    RANGE=""
else
    RANGE="${PREVIOUS}..${HEAD_REF:-HEAD}"
fi

REPO="${GITHUB_REPOSITORY:-$(git remote get-url origin 2>/dev/null \
    | sed -E 's#(git@|https://)github\.com[:/]##; s#\.git$##')}"
SERVER="${GITHUB_SERVER_URL:-https://github.com}"

# %x1f separates fields, %x1e separates commits: a commit body contains
# newlines, so line-based reading would split one commit into several.
while IFS=$'\x1f' read -r -d $'\x1e' sha subject body; do
    # git writes a newline after each record separator, so every sha but the
    # first arrives with it attached. Left in, it lands in the middle of the
    # commit link and breaks the markdown.
    sha="${sha//[$'\n\r']/}"
    [[ -z "$sha" ]] && continue

    # Skip the release commits this pipeline makes itself, or every release
    # would count the previous one as a reason for the next.
    [[ "$subject" =~ ^chore\(release\): ]] && continue

    short="${sha:0:7}"
    link="([${short}](${SERVER}/${REPO}/commit/${sha}))"

    # "feat(ai): subject" becomes "**ai:** subject". The type is already the
    # heading the entry sits under, so repeating it on every line is noise.
    description="${subject#*: }"
    scope=""
    if [[ "$subject" =~ ^[a-zA-Z]+\(([^\)]*)\)!?: ]]; then
        scope="${BASH_REMATCH[1]}"
    fi

    if [[ -n "$scope" ]]; then
        entry="* **${scope}:** ${description} ${link}"
    else
        entry="* ${description} ${link}"
    fi

    is_breaking=false
    # "feat!:" and "fix(scope)!:" both mark a break.
    [[ "$subject" =~ ^[a-zA-Z]+(\([^\)]*\))?!: ]] && is_breaking=true
    # The footer form, which is what the specification actually requires.
    [[ "$body" == *"BREAKING CHANGE"* || "$body" == *"BREAKING-CHANGE"* ]] && is_breaking=true

    if $is_breaking; then
        BREAKING+="${entry}"$'\n'
        BUMP="major"
    fi

    case "$subject" in
        feat:*|feat\(*)
            FEATURES+="${entry}"$'\n'
            [[ "$BUMP" == "none" || "$BUMP" == "patch" ]] && BUMP="minor"
            ;;
        fix:*|fix\(*|perf:*|perf\(*)
            FIXES+="${entry}"$'\n'
            [[ "$BUMP" == "none" ]] && BUMP="patch"
            ;;
    esac
done < <(git log --format="%H%x1f%s%x1f%b%x1e" ${RANGE:+"$RANGE"})

IFS=. read -r MAJOR MINOR PATCH <<< "${PREVIOUS#"$TAG_PREFIX"}"

case "$BUMP" in
    major) MAJOR=$((MAJOR + 1)); MINOR=0; PATCH=0 ;;
    minor) MINOR=$((MINOR + 1)); PATCH=0 ;;
    patch) PATCH=$((PATCH + 1)) ;;
    none)  : ;;
esac

VERSION="${MAJOR}.${MINOR}.${PATCH}"
TAG="${TAG_PREFIX}${VERSION}"
RELEASE_NEEDED=true
[[ "$BUMP" == "none" ]] && RELEASE_NEEDED=false

{
    if [[ -n "$BREAKING" ]]; then
        printf '### ⚠ BREAKING CHANGES\n\n%s\n' "$BREAKING"
    fi
    if [[ -n "$FEATURES" ]]; then
        printf '### Features\n\n%s\n' "$FEATURES"
    fi
    if [[ -n "$FIXES" ]]; then
        printf '### Bug Fixes\n\n%s\n' "$FIXES"
    fi
    if [[ -z "$BREAKING$FEATURES$FIXES" ]]; then
        printf 'No user-facing changes.\n'
    fi
} > "$NOTES_FILE"

if [[ -n "${GITHUB_OUTPUT:-}" ]]; then
    {
        echo "previous=$PREVIOUS"
        echo "bump=$BUMP"
        echo "version=$VERSION"
        echo "tag=$TAG"
        echo "release_needed=$RELEASE_NEEDED"
        echo "notes_file=$NOTES_FILE"
    } >> "$GITHUB_OUTPUT"
fi

echo "previous:       $PREVIOUS"
echo "bump:           $BUMP"
echo "version:        $VERSION"
echo "release needed: $RELEASE_NEEDED"
echo "--- notes ---"
cat "$NOTES_FILE"
