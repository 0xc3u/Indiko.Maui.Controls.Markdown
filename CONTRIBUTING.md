## Contributing

Contributions to the `MarkdownView` project are very welcome! Whether you want to add new features, improve existing ones, fix bugs, or enhance documentation, your help is highly appreciated.

---

# How to Contribute

Thank you for considering contributing to our project! Please follow these guidelines to ensure a smooth process.

## 1. Work on a Feature Branch

Always create a new branch for your feature or fix. This keeps the main branch clean and makes it easier to manage changes.

```bash
git checkout -b feature/your-feature-name
```

## 2. Start a Pull Request

Once your feature is complete, push your branch to the repository and start a pull request to merge it into the main branch. Ensure all tests pass and your code follows the project's coding standards.

```bash
git push origin feature/your-feature-name
```

Then, create a pull request on GitHub and provide a clear description of your changes.

## 3. Use Semantic Release Prefixes for Commits

When committing your changes, use semantic release prefixes to categorize your commits. This helps in generating automated release notes and versioning.

The commit contains the following structural elements to communicate intent to the consumers of your library:

- **fix:** a commit of the type fix patches a bug in your codebase (this correlates with PATCH in Semantic Versioning).
- **feat:** a commit of the type feat introduces a new feature to the codebase (this correlates with MINOR in Semantic Versioning).
- **BREAKING CHANGE:** a commit that has a footer BREAKING CHANGE:, or appends a ! after the type/scope, introduces a breaking API change (correlating with MAJOR in Semantic Versioning). A BREAKING CHANGE can be part of commits of any type.
- Types other than fix: and feat: are allowed. For example, @commitlint/config-conventional (based on the Angular convention) recommends:
  - **build:** Changes that affect the build system or external dependencies
  - **chore:** Other changes that don't modify src or test files
  - **ci:** Changes to our CI configuration files and scripts
  - **docs:** Documentation only changes
  - **style:** Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
  - **refactor:** A code change that neither fixes a bug nor adds a feature
  - **perf:** A code change that improves performance
  - **test:** Adding missing tests or correcting existing tests

Footers other than BREAKING CHANGE: <description> may be provided and follow a convention similar to git trailer format. Additional types are not mandated by the Conventional Commits specification and have no implicit effect in Semantic Versioning (unless they include a BREAKING CHANGE). A scope may be provided to a commit’s type, to provide additional contextual information and is contained within parenthesis, e.g., feat(parser): add ability to parse arrays.

Example commit messages:

```bash
git commit -m "fix: resolve issue with user authentication"
git commit -m "feat: add new payment gateway integration"
git commit -m "BREAKING CHANGE: update API endpoints"
```

## 4. Write Meaningful Commit Messages

Commit messages should be concise yet descriptive. They should explain the "what" and "why" of your changes.

- **Good Example:** `fix: correct typo in user profile page`
- **Bad Example:** `fixed stuff`

## Additional Tips

- Ensure your code adheres to the project's coding standards and guidelines.
- Include tests for new features or bug fixes.
- Keep your commits atomic; a single commit should represent a single logical change.
- Update the documentation to reflect any new features or changes.

We appreciate your contributions and look forward to your pull requests!

Happy coding!
