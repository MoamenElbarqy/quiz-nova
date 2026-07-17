---
name: Verification
description: Enforces code quality and build standards for backend and frontend components of QuizNova.
triggers:
  - After modifying backend C# files in QuizNova.Api, QuizNova.Application, QuizNova.Domain, or QuizNova.Infrastructure.
  - After modifying frontend files (TS, HTML, CSS) in QuizNova.Client.
---

# Code Verification Skill

Use this skill to ensure that the codebase remains healthy, formatted, and buildable after any changes.

## Backend Verification (ASP.NET Core)

When changes are made to the backend projects, execute the following commands from the repository root:

1. **Format Check:**

   ```bash
   dotnet format QuizNova.slnx --verify-no-changes
   ```

   _Auto-fix formatting issues:_

   ```bash
   dotnet format QuizNova.slnx
   ```

2. **Build Verification:**

   ```bash
   dotnet build --warningsaserrors
   ```

## Frontend Verification (Angular)

When changes are made to the `src/QuizNova.Client` directory, execute the following commands within that directory:

1. **Format Check:**

   ```bash
   npm run format:check
   ```

   _Auto-fix formatting:_

   ```bash
   npm run format
   ```

2. **TypeScript Linting:**

   ```bash
   npm run lint
   ```

   _If errors occur, attempt to fix them automatically:_

   ```bash
   npm run lint -- --fix
   ```

3. **CSS Linting:**

   ```bash
   npm run lint:css
   ```

   _If errors occur, attempt to fix them automatically:_

   ```bash
   npm run lint:css -- --fix
   ```

4. **Testing:**

   ```bash
   npm run test
   ```

5. **Production Build:**

   ```bash
   npm run build
   ```

## End-to-End Verification (Playwright)

When validating user flows and visual correctness, execute within `src/QuizNova.Client`:

1. **Run Chromium E2E Tests (No Video):**

   ```bash
   VIDEO=off npx playwright test --project=chromium
   ```

   _Note: In CI (GitHub Actions), video recording is automatically enabled to provide high-quality 1080p visual proof of functionality for recruiters and VCs._

## Pre-commit Hooks

The project uses Husky + lint-staged to auto-format and lint staged files before every commit:

- **Frontend:** Prettier, ESLint, and Stylelint run on staged `.ts`, `.html`, `.css` files
- **Backend:** `dotnet format --verify-no-changes` checks the entire solution
- If the hook fails, the commit is blocked — fix the issues and try again
- To skip hooks (emergency only): `git commit --no-verify`

## CI Enforcement

GitHub Actions (`.github/workflows/build-and-test.yml`) enforces on every push/PR:

| Check | Command |
|---|---|
| Backend formatting | `dotnet format --verify-no-changes` |
| Backend build | `dotnet build --warningsaserrors` |
| Frontend lint | `npm run lint` |
| Frontend formatting | `npm run format:check` |
| Frontend build | `npm run build` |

## Guidelines

- Always run these verifications before concluding a task that involved code changes.
- If a verification step fails, address the issues before proceeding or completing the task.
- Ensure that the environment (Node.js, .NET SDK) is properly configured before running these commands.
