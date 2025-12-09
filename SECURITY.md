# Security Policy (Pre-release)

> ⚠️ MissSolitude is still in its infancy and not production-ready. Security controls such as authentication, authorization, input validation, and monitoring are minimal or absent.

## Supported versions

No releases are supported at this time. Expect breaking changes while the project is under active, exploratory development.

## Reporting vulnerabilities

- Please open a private issue or contact the maintainers before sharing details publicly. If private channels are unavailable, create an issue marked clearly as a security concern and avoid including exploit details.
- Provide as much context as possible (affected endpoints, reproduction steps, logs, and environment details) to help us investigate quickly.
- Do not submit sensitive data, credentials, or production secrets when reporting.

## Security expectations (current state)

- The API is intended for local/testing environments only.
- User input is not fully validated and may be vulnerable to common classes of bugs until hardened.
- There is no authentication or authorization; any caller can access the endpoints when the service is reachable.
- Database credentials and other secrets should be provided via environment variables or development configuration only—avoid reusing production secrets.

We appreciate responsible disclosure and patience while the project matures.