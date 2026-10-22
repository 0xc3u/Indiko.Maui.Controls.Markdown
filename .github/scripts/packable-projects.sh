#!/usr/bin/env bash
#
# Prints the projects that get published to NuGet, one path per line.
#
# Run it with no arguments to see the list the pipeline would publish. The
# hand-maintained matrix this replaces had to be kept in step with the
# projects by hand, which is how a sibling repository came to announce a
# package it never published.
#
# The rule is: a project under src/ ships when it says so, and is not named
# in EXCLUDED.
#
#   - src/ only, so the sample app never comes into consideration.
#
#   - <IsPackable>True</IsPackable> must be written out. The SDK defaults it
#     to true, so trusting the default would publish anything that simply
#     never said. The failure modes are not symmetric: opting in too narrowly
#     forgets to publish something, and the workflow_dispatch republishes it
#     minutes later, while opting in too widely puts a package on NuGet.org
#     under your name forever.
#
set -euo pipefail

# Projects that declare themselves packable but must not be published yet.
# Nothing is held back here; the sample app does not declare itself packable
# and so never reaches this list.
EXCLUDED=()

is_excluded() {
    local name="$1"
    local excluded
    for excluded in "${EXCLUDED[@]+"${EXCLUDED[@]}"}"; do
        [[ "$name" == "$excluded" ]] && return 0
    done
    return 1
}

for project in src/*/*.csproj; do
    [[ -e "$project" ]] || continue

    name="$(basename "$project" .csproj)"

    is_excluded "$name" && continue

    grep -qiE '<IsPackable>[[:space:]]*true[[:space:]]*</IsPackable>' "$project" || continue

    echo "$project"
done
