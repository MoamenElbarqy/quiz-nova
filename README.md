<div align="center">

# 🚀 QuizNova

**Quiz Management System**

Built with **.NET 10** · **Angular 21** · **PostgreSQL**

[![Live Demo](https://img.shields.io/badge/🌐_Live_Demo-Click_Here-00C896?style=for-the-badge)](https://your-live-demo-url.com)

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-21-DD0031?style=flat-square&logo=angular)](https://angular.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-18-4169E1?style=flat-square&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)
[![CI](https://img.shields.io/github/actions/workflow/status/your-username/QuizNova/ci.yml?branch=main&style=flat-square&label=CI)](https://github.com/your-username/QuizNova/actions)

</div>

---

## 📸 Application Screenshots

### API Documentation — Swagger & Scalar

<table>
  <tr>
    <td width="50%">
      <img src="docs/images/swagger-overview.png" alt="Swagger API Overview" width="100%"/>
      <p align="center"><em>Swagger — API Overview</em></p>
    </td>
    <td width="50%">
      <img src="docs/images/swagger-endpoints.png" alt="Swagger Endpoints" width="100%"/>
      <p align="center"><em>Swagger — Endpoint Details</em></p>
    </td>
  </tr>
  <tr>
    <td width="50%">
      <img src="docs/images/scalar-overview.png" alt="Scalar API Reference" width="100%"/>
      <p align="center"><em>Scalar — API Reference</em></p>
    </td>
    <td width="50%">
      <img src="docs/images/scalar-endpoints.png" alt="Scalar Endpoint Details" width="100%"/>
      <p align="center"><em>Scalar — Endpoint Details</em></p>
    </td>
  </tr>
</table>

### Frontend — Application Pages

<table>
  <tr>
    <td width="50%">
      <img src="docs/images/landing-page.png" alt="Landing Page" width="100%"/>
      <p align="center"><em>Landing Page</em></p>
    </td>
    <td width="50%">
      <img src="docs/images/admin-dashboard.png" alt="Admin Dashboard" width="100%"/>
      <p align="center"><em>Admin Dashboard</em></p>
    </td>
  </tr>
  <tr>
    <td width="50%">
      <img src="docs/images/instructor-quiz-create.png" alt="Instructor — Create Quiz" width="100%"/>
      <p align="center"><em>Instructor — Create Quiz</em></p>
    </td>
    <td width="50%">
      <img src="docs/images/student-quiz-attempt.png" alt="Student — Quiz Attempt" width="100%"/>
      <p align="center"><em>Student — Quiz Attempt</em></p>
    </td>
  </tr>
</table>

---

## 🏗️ Architecture Overview

QuizNova is built on a **Modular Monolith** foundation following strict **Clean Architecture** principles. Every layer has a single responsibility and dependencies always point inward.

```
┌────────────────────────────────────────────────────────┐
│                    Presentation                        │
│          API Controllers  ·  Angular Client            │
├────────────────────────────────────────────────────────┤
│                    Application                         │
│       Commands / Queries (CQRS)  ·  MediatR            │
│       FluentValidation  ·  Mapping                     │
├────────────────────────────────────────────────────────┤
│                      Domain                            │
│     Enriched Entities  ·  Value Objects                │
│     Result Pattern  ·  Domain Events                   │
├────────────────────────────────────────────────────────┤
│                   Infrastructure                       │
│       EF Core  ·  PostgreSQL  ·  JWT Auth              │
│       OpenTelemetry  ·  Serilog  ·  Seq                │
└────────────────────────────────────────────────────────┘
```

### Clean Architecture + CQRS + MediatR

All application logic flows through the **MediatR** pipeline. Commands and Queries are separated (CQRS) to ensure clear intent and enable independent optimization of read and write paths.

```mermaid
flowchart LR
    A["HTTP Request"] --> B["Controller"]
    B --> C["MediatR"]
    C --> D["Command / Query Handler"]
    D --> E["Domain"]
    E --> F["Result&lt;T&gt;"]

    style A fill:#3b82f6,color:#fff,stroke:none
    style B fill:#6366f1,color:#fff,stroke:none
    style C fill:#8b5cf6,color:#fff,stroke:none
    style D fill:#a855f7,color:#fff,stroke:none
    style E fill:#d946ef,color:#fff,stroke:none
    style F fill:#10b981,color:#fff,stroke:none
```

- **Commands** mutate state and return semantic results (`Result<Created>`, `Result<Updated>`, `Result<Deleted>`).
- **Queries** are side-effect-free reads optimized for performance.
- **Pipeline behaviors** handle cross-cutting concerns (validation, logging) transparently.

### Result Pattern — Exception-Free Error Handling

The domain layer uses a custom **`Result<T>`** monad instead of exceptions for control flow. Every domain operation returns a `Result` that is either a success value or a list of strongly-typed `Error` objects.

```csharp
// Domain returns a Result — no exceptions thrown
public static Result<Quiz> Create(Guid id, ...) {
    if (string.IsNullOrWhiteSpace(title))
        return QuizErrors.TitleRequired;    // implicit conversion to Result<Quiz>

    return new Quiz(id, ...);               // implicit conversion to Result<Quiz>
}

// Consumer uses Match or checks IsSuccess
result.Match(
    onValue: quiz => Results.Created(quiz),
    onError: errors => Results.BadRequest(errors)
);
```

### Enriched Domain Model

Entities are **not anemic data bags**. They own their own validation, invariants, and business rules. State mutations happen exclusively through behavior-rich methods on the entities themselves.

```csharp
// Quiz owns its business rules — cannot modify a quiz that already started
public Result<Updated> Update(string title, DateTimeOffset startsAtUtc, DateTimeOffset endsAtUtc) {
    if (Status != QuizStatus.Scheduled)
        return QuizErrors.CannotUpdateStartedOrCompletedQuiz;
    ...
}
```

- `Quiz` validates its schedule, enforces minimum question counts, and manages its lifecycle status.
- `Question` validates marks, display order, and text through the abstract base.
- Factory methods (`Create`) enforce all invariants at construction time — **no invalid objects can exist**.

---

## ⚙️ Technical Details

### Eliminating Data Clumps (Code Smell Refactoring)

To ensure a clean, maintainable, and expressive domain model, we actively identify and refactor code smells such as **Data Clumps**—groups of variables (e.g., name, email, and phone number) that are frequently passed around together. By encapsulating these primitives into cohesive Value Objects or dedicated classes, we reduce method signature bloat, prevent primitive obsession, and improve overall architecture.

**Before (Primitive Obsession & Data Clump):**

```csharp
public static Result<User> Create(
    Guid id,
    string name,
    string email,
    string password,
    string phoneNumber,
    Role role)
```

**After (Decoupled into Cohesive Objects):**

```csharp
protected User(
    Guid id,
    PersonalInformation personalInformation,
    UserRole userRole,
    List<RefreshToken> refreshTokens)
    : base(id)
{
    PersonalInformation = personalInformation;
    UserRole = userRole;
    _refreshTokens = refreshTokens;
}
```

This approach centralizes the validation rules for personal details within the `PersonalInformation` object, eliminating duplicate logic across the application and ensuring that the `User` entity remains focused purely on core domain rules.

---

## 🧩 Polymorphic Question System

Questions use a **Table-Per-Hierarchy (TPH) inheritance** strategy, making the system easily extensible to new question types without modifying existing code.

```
                    ┌──────────────┐
                    │   Question   │  ← abstract base
                    │  (abstract)  │
                    └──────┬───────┘
                           │
               ┌───────────┼───────────┐
               │                       │
        ┌──────┴──────┐         ┌──────┴───────┐
        │     MCQ     │         │  True/False  │
        │             │         │              │
        │ • Choices[] │         │ • CorrectAns │
        │ • CorrectId │         │   (bool)     │
        └─────────────┘         └──────────────┘
               │
               │  future
        ┌──────┴──────────┐
        │  Fill-in-Blank  │
        │  Matching       │
        │  Ordering       │
        │  ...            │
        └─────────────────┘
```

**Currently supported:**

| Type             | Description                                                |
| ---------------- | ---------------------------------------------------------- |
| **MCQ**          | Multiple-choice with N choices and a single correct answer |
| **True / False** | Binary-choice question                                     |

**Adding a new question type** only requires:

1. Create a new class inheriting from `Question`.
2. Add its EF Core discriminator mapping.
3. Add its answer type inheriting from `QuestionAnswer`.

No existing code needs to change — the **Open/Closed Principle** is respected through polymorphic dispatch:

```csharp
return question switch {
    Mcq mcq     => mcq.Update(questionText, displayOrder, marks, correctChoiceId, choices),
    Tf  tf      => tf.Update(questionText, displayOrder, marks, tfCorrectChoice),
    _           => Error.Validation("Quiz.Question.UpdateTypeMismatch", "..."),
};
```

---

## ⚡ Frontend Architecture

### NgRx Signal Store

The frontend uses **NgRx Signal Store** for reactive, fine-grained state management built on Angular Signals — no RxJS-heavy boilerplate.

```
┌─────────────────────────────────────────────────┐
│                  Component                      │
│     Reads signals  ·  Dispatches store methods  │
├─────────────────────────────────────────────────┤
│              NgRx Signal Store                  │
│  withState()  · withMethods()  · withComputed() │
│           withRequestStatus()                   │
├─────────────────────────────────────────────────┤
│                   Service                       │
│         HTTP calls  ·  returns Observable       │
└─────────────────────────────────────────────────┘
```

- **Custom store features** like `withRequestStatus()` provide reusable state patterns (`idle → pending → fulfilled | error`) across all stores.
- Stores expose **computed signals** for derived state, keeping templates declarative and performant.

### Optimistic UI Updates

Mutations (create, update, delete) apply changes to the local store **immediately** before the server responds. If the server request fails, the store rolls back to the previous state — delivering a snappy, instant-feeling user experience.

```
User Action → Update Store Immediately → Send HTTP Request
                                              │
                              ┌───────────────┴────────────────┐
                              ▼                                ▼
                       ✅ Success                        ❌ Failure
                       (keep state)                   (rollback + notify)
```

---

## 🔍 Code Quality & Consistency

Every line of code — backend and frontend — is governed by automated analyzers and linters that run both locally and in CI. Style violations are **build errors**, not warnings.

### Backend — StyleCop Analyzers + EditorConfig

**StyleCop Analyzers** are installed globally via `Directory.Build.props` and apply to every C# project automatically:

```xml
<!-- Directory.Build.props — applied to ALL projects -->
<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>

<PackageReference Include="StyleCop.Analyzers">
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
```

- `TreatWarningsAsErrors` = **true** — any style violation fails the build.
- `.editorconfig` fine-tunes rules (e.g. `SA1309` off to allow `_privateField` naming, `SA1101` off to drop mandatory `this.` prefix).
- `dotnet_sort_system_directives_first` and `dotnet_separate_import_directive_groups` enforce consistent import ordering.

### Frontend — Stylelint + Property Ordering

CSS is linted with **Stylelint** using `stylelint-config-standard` and the `stylelint-order` plugin. A strict **property declaration order** is enforced:

```
position → display/flex/grid → sizing → margin → padding → border → background → typography → transitions → misc
```

This means every CSS rule across the entire codebase follows the same visual structure:

```css
/* Every CSS rule reads the same way — top to bottom */
.card {
  position: relative; /* 1. Positioning  */
  display: flex; /* 2. Layout       */
  width: 100%; /* 3. Sizing       */
  margin-block: 1rem; /* 4. Spacing      */
  padding: 1.5rem; /* 5. Padding      */
  border-radius: 12px; /* 6. Borders      */
  background: var(--surface); /* 7. Background   */
  color: var(--text); /* 8. Typography   */
  transition: transform 0.2s; /* 9. Transitions  */
}
```

### Frontend — Unified Path Aliases

Imports use **TypeScript path aliases** defined in `tsconfig.json` for clean, consistent, and refactor-safe imports:

```json
{
  "paths": {
    "@shared/*": ["src/app/shared/*"],
    "@Core/*": ["src/app/core/*"],
    "@Features/*": ["src/app/features/*"],
    "@Environments/*": ["src/environments/*"],
    "@StoreFeatures/*": ["src/store-features/*"]
  }
}
```

This eliminates fragile relative paths like `../../../shared/services/` and makes imports immediately readable:

```typescript
// ✅ Clean & consistent
import { CoursesService } from "@shared/services/courses.service";
import { NavigationButtons } from "@shared/components/navigation-buttons/navigation-buttons";
import { withRequestStatus } from "@StoreFeatures/with-request-status.feature";

// ❌ Fragile & unreadable
import { CoursesService } from "../../../shared/services/courses.service";
```

### Frontend — Strict TypeScript

The compiler is configured with maximum strictness — no room for implicit `any`, unused variables, or unchecked side effects:

```
strict: true · noImplicitReturns · noUnusedLocals · noUnusedParameters
noUncheckedSideEffectImports · strictTemplates · strictInjectionParameters
```

---

## 🛠️ Tech Stack

### Backend

| Technology                   | Purpose                        |
| ---------------------------- | ------------------------------ |
| **.NET 10** (C#)             | Web API framework              |
| **Entity Framework Core 10** | ORM & migrations               |
| **PostgreSQL 18**            | Relational database            |
| **MediatR**                  | CQRS pipeline & mediator       |
| **FluentValidation**         | Request validation             |
| **JWT + Refresh Tokens**     | Authentication & authorization |
| **OpenTelemetry**            | Distributed tracing            |
| **Serilog + Seq**            | Structured logging             |
| **Prometheus + Grafana**     | Metrics & dashboards           |
| **Swagger + Scalar**         | API documentation              |

### Frontend

| Technology            | Purpose                            |
| --------------------- | ---------------------------------- |
| **Angular 21**        | SPA framework (Signals-based)      |
| **NgRx Signal Store** | Reactive state management          |
| **PrimeNG 21**        | UI component library               |
| **Vanilla CSS**       | Custom styling with CSS variables  |
| **Stylelint**         | CSS linting with property ordering |

### Infrastructure

| Technology                  | Purpose                       |
| --------------------------- | ----------------------------- |
| **Docker & Docker Compose** | Multi-container orchestration |
| **GitHub Actions**          | CI/CD pipeline                |
| **StyleCop Analyzers**      | C# code style enforcement     |
| **Seq**                     | Log aggregation & search      |
| **Prometheus**              | Metrics collection            |
| **Grafana**                 | Monitoring dashboards         |

---

## ✨ Key Features

### 🔐 Role-Based Access Control

- **Admins** — Manage colleges, instructors, students, courses, and system-wide audits.
- **Instructors** — Full quiz lifecycle: draft, publish, edit, and view analytics.
- **Students** — Attempt quizzes, track results, review answers.

### 📝 Advanced Quiz Engine

- **Polymorphic questions** — MCQ and True/False out of the box, with an extensible architecture for future types.
- **Automated grading** — Real-time scoring with immediate feedback.
- **Attempt lifecycle** — Tracks start time, submission time, and enforces time limits.
- **Quiz status management** — Scheduled → Available Now → Completed, computed from dates.

### 📊 Observability Stack

- **Centralized logging** — Structured logs enriched with context, shipped to **Seq**.
- **Metrics pipeline** — Instrumented with **OpenTelemetry**, scraped by **Prometheus**, visualized in **Grafana**.

---

## 🚦 Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended)

### ⚡ Quick Start (Docker)

Spin up the entire stack with a single command:

```bash
git clone https://github.com/your-username/QuizNova.git
cd QuizNova/QuizNova-main
docker compose up -d
```

**Access the services:**

| Service        | URL                                                              |
| -------------- | ---------------------------------------------------------------- |
| 🌐 Web App     | [`http://localhost:4200`](http://localhost:4200)                 |
| 📘 Swagger API | [`http://localhost:8080/swagger`](http://localhost:8080/swagger) |
| 📙 Scalar API  | [`http://localhost:8080/scalar`](http://localhost:8080/scalar)   |
| 📋 Seq Logs    | [`http://localhost:5341`](http://localhost:5341)                 |
| 📊 Prometheus  | [`http://localhost:9090`](http://localhost:9090)                 |
| 📈 Grafana     | [`http://localhost:3000`](http://localhost:3000)                 |

## 🔄 CI/CD Pipeline

The project uses **GitHub Actions** to run automated checks on every push and pull request to `main`.

```
┌───────────────────────────────────────────────────────────────────┐
│                    GitHub Actions — CI                            │
├──────────────────────────┬────────────────────────────────────────┤
│      Backend Job         │           Frontend Job                 │
│                          │                                        │
│  1. Setup .NET 10        │  1. Setup Node.js 22                   │
│  2. dotnet restore       │  2. npm ci                             │
│  3. dotnet build         │  3. npm run lint:css (Stylelint)       │
│     (Release mode)       │  4. npm run build                      │
│     + StyleCop enforced  │                                        │
│     + TreatWarnings      │                                        │
│       AsErrors           │                                        │
└──────────────────────────┴────────────────────────────────────────┘
```

- **Backend build** compiles in `Release` mode with `TreatWarningsAsErrors` — any StyleCop or compiler warning fails the pipeline.
- **Frontend build** runs **Stylelint** for CSS property ordering before compiling the Angular app.
- Both jobs run in **parallel** for fast feedback.

---

## 📁 Project Structure

```
QuizNova/
├── src/
│   ├── QuizNova.Api/              # Presentation — Controllers, DTOs, Middleware
│   ├── QuizNova.Application/      # Application — Commands, Queries, Validators
│   ├── QuizNova.Domain/           # Domain — Entities, Value Objects, Results
│   ├── QuizNova.Infrastructure/   # Infrastructure — EF Core, Auth, External Services
│   └── QuizNova.Client/           # Frontend — Angular 21 SPA
│       └── src/
│           ├── app/
│           │   ├── core/           # Guards, interceptors, app-level config
│           │   ├── features/       # Feature modules (admin, instructor, student)
│           │   └── shared/         # Shared components, models, services, styles
│           └── store-features/     # Reusable NgRx Signal Store features
├── .github/workflows/ci.yml        # GitHub Actions CI pipeline
├── .editorconfig                   # Cross-IDE formatting rules
├── Directory.Build.props           # Global build settings + StyleCop
├── Directory.Packages.props        # Central Package Management
├── containers/                     # Docker volume configs (Seq, Prometheus)
├── compose.yaml                    # Full-stack Docker Compose
└── README.md
```

---

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

---

<div align="center">

_Developed with ❤️ by Moamen_

</div>
