# Security Policy

## Reporting a Vulnerability

If you discover a security vulnerability in this project, please report it responsibly.

**Do NOT open a public issue.** Instead, please email the maintainer directly at:

- **Email:** vladislavzinkov8@gmail.com

Include the following in your report:

1. A description of the vulnerability
2. Steps to reproduce
3. Potential impact
4. Suggested fix (if any)

We aim to acknowledge reports within **48 hours** and provide a fix or mitigation within **7 days** for critical issues.

## Supported Versions

| Version | Supported |
| ------- | --------- |
| main    | ✓         |

## Security Best Practices for Contributors

When contributing to this project, please follow these guidelines:

1. **Never commit secrets** — API keys, passwords, tokens, or credentials must never appear in source code. Use environment variables or Unity's ScriptableObjects with `.gitignore` exclusions.
2. **Validate all input** — Any user-facing input (UI fields, file imports, network data) must be validated and sanitized before use.
3. **Use parameterized queries** — If the project integrates with any database, always use parameterized queries to prevent SQL injection.
4. **Keep dependencies updated** — Dependabot is configured to alert on known vulnerabilities in dependencies.
5. **Avoid debug endpoints in production** — Ensure any debug/testing endpoints or cheats are disabled in release builds.
6. **Principle of least privilege** — Request only the minimum permissions necessary for any system integration.

## Tooling

This project uses the following security tooling:

- **[Gitleaks](https://github.com/gitleaks/gitleaks)** — Pre-commit hook to detect hardcoded secrets
- **[pre-commit](https://pre-commit.com/)** — Git hook framework for automated checks
- **[Dependabot](https://docs.github.com/en/code-security/dependabot)** — Automated dependency vulnerability scanning
- **GitHub Actions security workflow** — CI-based secret scanning on every push and PR
