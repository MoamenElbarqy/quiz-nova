<div align="center">

# 🚀 QuizNova

**Full-Stack Quiz Management System**

Built with **.NET 10** · **Angular 21** · **PostgreSQL**

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-21-DD0031?style=flat-square&logo=angular)](https://angular.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-18-4169E1?style=flat-square&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)
[![CI](https://img.shields.io/github/actions/workflow/status/MoamenElbarqy/quiz-nova/ci.yml?branch=main&style=flat-square&label=CI)](https://github.com/MoamenElbarqy/quiz-nova/actions)

</div>

---

## 📑 Table of Contents

- [📸 Screenshots](#-screenshots)
- [🏗️ Architecture Overview](#️-architecture-overview)
- [✨ Key Features](#-key-features)
- [⚙️ Technical Deep Dives](#️-technical-deep-dives)
  - [🧩 Polymorphic Question & Answer System](#-polymorphic-question--answer-system)
  - [⚡ Caching Pipeline & Event-Driven Invalidation](#-caching-pipeline--event-driven-invalidation)
  - [Eliminating Data Clumps with Value Objects](#eliminating-data-clumps-with-value-objects)
  - [Overcoming Angular Dynamic Component Limitations](#overcoming-angular-dynamic-component-limitations)
- [🖥️ Frontend Architecture](#️-frontend-architecture)
- [🔍 Code Quality & Consistency](#-code-quality--consistency)
- [🛠️ Tech Stack](#️-tech-stack)
- [🧪 Testing Strategy](#-testing-strategy)
- [🚦 Getting Started](#-getting-started)
- [🔄 CI/CD Pipeline](#-cicd-pipeline)
- [📁 Project Structure](#-project-structure)

---

## 📸 Screenshots

### 🗄️ Relational Schema

<div align="center">
  <img src="docs/images/relational-schema.png" alt="QuizNova Relational Schema" width="70%"/>
  <p><em>Full database relational schema — Colleges, Courses, Users, Quizzes, Questions, Attempts &amp; Answers</em></p>
</div>

---

### 🔌 REST API — Swagger Endpoint Explorer

<table>
  <tr>
    <td width="50%">
      <img src="docs/images/swagger/swagger-admin-auth-college.png" alt="Swagger — Admin, Auth & College endpoints" width="100%"/>
      <p align="center"><em>Admin · Auth · College endpoints</em></p>
    </td>
    <td width="50%">
      <img src="docs/images/swagger/swagger-course-grading.png" alt="Swagger — Course & Grading endpoints" width="100%"/>
      <p align="center"><em>Course · Grading endpoints</em></p>
    </td>
  </tr>
  <tr>
    <td width="50%">
      <img src="docs/images/swagger/swagger-instructor-attempts.png" alt="Swagger — Instructor & QuizAttempt endpoints" width="100%"/>
      <p align="center"><em>Instructor · QuizAttempt endpoints</em></p>
    </td>
    <td width="50%">
      <img src="docs/images/swagger/swagger-quiz-student.png" alt="Swagger — Quiz & Student endpoints" width="100%"/>
      <p align="center"><em>Quiz · Student endpoints</em></p>
    </td>
  </tr>
  <tr>
    <td width="50%">
      <img src="docs/images/swagger/swagger-quiz-student-2.png" alt="Swagger — Quiz & Student endpoints (continued)" width="100%"/>
      <p align="center"><em>Quiz · Student endpoints (cont.)</em></p>
    </td>
    <td width="50%"></td>
  </tr>
</table>

### 🖥️ Application Pages

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

QuizNova is built on **Clean Architecture** — every layer has a single responsibility and all dependencies point inward, keeping the domain pure and independently testable.

```
┌────────────────────────────────────────────────────────┐
│                    Presentation                        │
│          API Controllers  ·  Angular Client            │
├────────────────────────────────────────────────────────┤
│                    Application                         │
│       Commands / Queries (CQRS)  ·  MediatR            │
│   FluentValidation  ·  Pipeline Behaviours             │
│   Caching  ·  Logging  ·  Exception Handling           │
├────────────────────────────────────────────────────────┤
│                      Domain                            │
│     Enriched Entities  ·  Value Objects                │
│     Result Pattern  ·  Domain Events                   │
├────────────────────────────────────────────────────────┤
│                   Infrastructure                       │
│   EF Core  ·  PostgreSQL  ·  ASP.NET Core Identity     │
│   Hybrid Cache  ·  JWT Auth  ·  Refresh Tokens         │
│   OpenTelemetry  ·  Serilog  ·  Seq                    │
└────────────────────────────────────────────────────────┘
```

All application logic flows through the **MediatR** pipeline (CQRS). **Commands** mutate state and return semantic results (`Result<Created>`, `Result<Updated>`). **Queries** are side-effect-free reads that may be cached. **Pipeline Behaviours** handle cross-cutting concerns transparently.

### Result Pattern — Exception-Free Error Handling

The domain uses a custom **`Result<T>`** monad — no exceptions for control flow. Every operation returns either a value or a list of strongly-typed `Error` objects categorised by `ErrorKind` (Validation, NotFound, Conflict, Forbidden).

```csharp
public static Result<Quiz> Create(Guid id, ...) {
    if (string.IsNullOrWhiteSpace(title))
        return QuizErrors.TitleRequired;    // implicit conversion to Result<Quiz>

    return new Quiz(id, ...);               // implicit conversion to Result<Quiz>
}
```

### Enriched Domain Model

Entities are **not anemic data bags** — they own validation, invariants, and business rules. Factory methods (`Create`) enforce all invariants at construction time — **no invalid objects can exist**.

```csharp
// Quiz owns its own business rules — cannot modify after it has started
public Result<Updated> Update(string title, DateTimeOffset startsAtUtc, DateTimeOffset endsAtUtc) {
    if (Status != QuizStatus.Scheduled)
        return QuizErrors.CannotUpdateStartedOrCompletedQuiz;
    ...
}
```

### Domain Events

Significant domain transitions raise **Domain Events** that decouple side-effects from the core business operation:

| Event | Raised When | Handler |
|---|---|---|
| `QuizCreatedEvent` | A new quiz is created | `QuizCreatedCacheInvalidationHandler` |
| `QuizAttemptSubmittedEvent` | A student submits an attempt | `QuizAttemptSubmittedCacheInvalidationHandler` |
| — | A question is graded | `QuestionGradedCacheInvalidationHandler` |

### 🗣️ Ubiquitous Language

> [!TIP]
> Full domain term dictionary: **[Ubiquitous Language Guide](docs/ubiquitous-language.md)**

---

## ✨ Key Features

### 🔐 Role-Based Access Control

Three distinct roles, each with strictly scoped permissions:

- **Admins** — Manage colleges, instructors, students, courses, and audit all quiz attempts system-wide.
- **Instructors** — Full quiz lifecycle: draft, publish, edit metadata, manage questions, view analytics, and manually grade essay responses.
- **Students** — Browse enrolled courses, attempt quizzes, track results, and review all answers after submission.

### 📝 Advanced Quiz Engine

- **Three question types** — MCQ (multiple choice with choices), True/False, and **Essay** (free-text, manually graded).
- **Polymorphic question hierarchy** — Two-level type system: `AutoGradedQuestion<TAnswer>` (MCQ, True/False) for instant scoring, and `ManuallyGradedQuestion` (Essay) for instructor-reviewed responses.
- **Automated grading via `CorrectionCondition`** — Each auto-graded type defines a generic predicate `Func<TAnswer, bool>` encapsulating its own correctness logic — extensible without touching any existing code.
- **Manual grading workflow** — Instructors see a **Pending Grades** dashboard, navigate to each student's essay, and assign a `Score` validated against the question's `Marks` ceiling.
- **Attempt lifecycle** — Tracks start time, submission time, and enforces duration limits. Submission raises a domain event that triggers cache invalidation.
- **Quiz status machine** — `Scheduled → Available Now → Completed`, computed from `StartsAtUtc` / `EndsAtUtc`.

### 🎓 Enrollment System

Students are enrolled in **Courses** (which belong to a **College**). A course has one assigned instructor. Quizzes are linked to courses — students only see quizzes for their enrolled courses.

| Operation | Who |
|---|---|
| Create / delete course | Admin |
| Assign instructor to course | Admin |
| Enroll / remove student from course | Admin |
| Create quizzes for course | Instructor |

### 📊 Observability Stack

- **Centralized structured logging** — Enriched with request context, shipped to **Seq**.
- **Distributed tracing** — Instrumented with **OpenTelemetry**.
- **Metrics pipeline** — Scraped by **Prometheus**, visualized in **Grafana**.
- **Global Exception Handler** — Catches unhandled exceptions at the API layer and returns RFC 9457 Problem Details.

---

## ⚙️ Technical Deep Dives

### 🧩 Polymorphic Question & Answer System

The domain models questions and answers through a **two-level polymorphic hierarchy**, adhering strictly to the **Liskov Substitution Principle (LSP)**.

#### Question Hierarchy

```text
                    ┌──────────────────┐
                    │    Question      │  ← abstract base
                    │  • QuestionText  │
                    │  • DisplayOrder  │
                    │  • Marks         │
                    └────────┬─────────┘
                             │
              ┌──────────────┴──────────────┐
              │                             │
  ┌───────────┴────────────┐   ┌────────────┴───────────┐
  │  AutoGradedQuestion    │   │  ManuallyGradedQuestion │
  │  <TAnswer> (abstract)  │   │       (abstract)        │
  │  • CorrectionCondition │   │  • Score (nullable)     │
  │    Func<TAnswer, bool> │   │  • SetScore(int)        │
  │  • Solve(TAnswer, ...) │   └────────────┬────────────┘
  └────────────┬───────────┘                │
    ┌──────────┴──────────┐          ┌──────┴──────┐
 ┌──┴──────┐       ┌──────┴──────┐  │    Essay    │
 │   Mcq   │       │     Tf      │  │  • MaxWords │
 │ Choices │       │CorrectChoice│  └─────────────┘
 │CorrectId│       │  (bool)     │
 └─────────┘       └─────────────┘
```

#### Answer Hierarchy

```text
                    ┌──────────────────┐
                    │  QuestionAnswer  │  ← abstract base
                    │  • StudentId     │
                    │  • QuestionId    │
                    │  • QuizAttemptId │
                    └────────┬─────────┘
                             │
              ┌──────────────┴──────────────┐
              │                             │
  ┌───────────┴────────────┐   ┌────────────┴─────────────┐
  │   AutoGradedAnswer     │   │  ManuallyGradedAnswers   │
  │  • IsCorrect (bool)    │   │  • Score (nullable int)  │
  └────────────┬───────────┘   │  • UpdateMarks(int?)     │
    ┌──────────┴──────────┐    └──────────────────────────┘
 ┌──┴──────┐       ┌──────┴───────
 ─┐
 │McqAnswer│       │  TfAnswer     │
 │SelectedChoiceId │ StudentChoice │
 └─────────┘       └───────────────┘
```

#### LSP in Practice

The quiz attempt handler dispatches through the abstract `Question` type and always gets back a valid `QuestionAnswer` — **zero knowledge of the concrete type required**:

```csharp
return question switch {
    Mcq mcq => mcq.Solve(mcqAnswer, studentId, attemptId),
    Tf  tf  => tf.Solve(tfAnswer,  studentId, attemptId),
    _       => Error.Validation("Quiz.Question.TypeMismatch", "..."),
};
```

#### Unified Grading via `CorrectionCondition`

Each auto-graded type owns its own generic correctness predicate — **the grading pipeline never changes when new types are added (OCP respected end-to-end)**:

```csharp
// MCQ — correct if selected choice matches the answer key
public override Func<Guid, bool> CorrectionCondition
    => studentChoiceId => studentChoiceId == CorrectChoiceId;

// True/False — correct if student's boolean matches
public override Func<bool, bool> CorrectionCondition
    => studentChoice => studentChoice == CorrectChoice;
```

Adding a new auto-graded type only requires: inherit `AutoGradedQuestion<TAnswer>`, implement `CorrectionCondition` + `Solve()`, and add an EF Core discriminator mapping.

#### Manual Grading — Essay Questions

`Essay` holds a nullable `Score` (pending review). Instructors set it after evaluating the student's response, validated against the question's `Marks` ceiling:

```csharp
public Result<Updated> SetScore(int score)
{
    if (score < 0)    return ManuallyGradedQuestionError.NegativeScore;
    if (score > Marks) return ManuallyGradedQuestionError.ScoreExceedsMarks;
    Score = score;
    return Result.Updated;
}
```

`ManuallyGradedAnswers` mirrors this on the answer side via `UpdateMarks()`.

---

### ⚡ Caching Pipeline & Event-Driven Invalidation

QuizNova uses a **Hybrid Cache** backed by PostgreSQL distributed cache (`Community.Microsoft.Extensions.Caching.PostgreSql`) — giving you both in-process (L1) and distributed (L2) caching without Redis.

#### Cache-Aside Pattern via MediatR Behaviour

Any query can opt-in to caching by implementing `ICachedQuery`:

```csharp
// Query opts-in by implementing ICachedQuery
public record GetAllQuizzesQuery : IQuery<...>, ICachedQuery
{
    public string CacheKey => "quizzes:all";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
```

The `CachingBehavior<TRequest, TResponse>` pipeline behaviour intercepts the request, checks the cache, and short-circuits the handler if a cached value exists — transparent to the handler itself.

#### Event-Driven Cache Invalidation

When state changes, domain events trigger `ICacheInvalidator` to remove stale entries:

```text
CreateQuiz command
    → QuizCreatedEvent raised in domain
        → QuizCreatedCacheInvalidationHandler
            → ICacheInvalidator.InvalidateAsync("quizzes:all")
```

This keeps the cache **always consistent** without polling or TTL-only expiry.

---

### Eliminating Data Clumps with Value Objects

Primitive-heavy signatures are refactored into cohesive **Value Objects**, eliminating duplicate validation and reducing method bloat:

```csharp
// Before — Primitive Obsession
public static Result<User> Create(Guid id, string name, string email, string password, string phoneNumber, Role role)

// After — Decoupled into cohesive objects
protected User(Guid id, PersonalInformation personalInformation, UserRole userRole, List<RefreshToken> refreshTokens)
```

`PersonalInformation` encapsulates `Name`, `Email`, `PhoneNumber` with its own validation. `UserRole` encapsulates role assignment. `RefreshToken` is a self-contained value object with its own expiry logic.

---

### Overcoming Angular Dynamic Component Limitations

`ngComponentOutlet` was ideal for OCP-friendly dynamic question rendering, but Angular [does not support output bindings](https://github.com/angular/angular/issues/15360) through it. When the **Edit Quiz** feature required `(formReady)` and `(blurEvent)` outputs from dynamically rendered question forms, we replaced the outlet with an explicit `@switch` block to directly bind outputs while keeping the **NgRx Signal Store** integration intact — solving the problem without sacrificing reactivity.

---

## 🖥️ Frontend Architecture

### Atomic Component Philosophy

Each feature is broken into small, single-responsibility components (e.g., `mcq-attempt.ts`, `quiz-attempt-header.ts`, `questions-navigator.ts`). Passing data through deep `@Input()/@Output()` chains across dozens of atomics is impractical — **this design directly drives the NgRx Signal Store as the shared state layer**.

### NgRx Signal Store

Reactive, fine-grained state management built on Angular Signals — no RxJS boilerplate.

```text
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

Custom `withRequestStatus()` provides a reusable `idle → pending → fulfilled | error` lifecycle across all stores. Every feature that makes async calls inherits this pattern for free.

### Stores by Feature

| Store | Feature |
|---|---|
| `create-quiz.store.ts` | Multi-step quiz creation with live question management |
| `edit-quiz.store.ts` | Optimistic question updates, metadata editing |
| `quiz-attempt.store.ts` | Question navigation, answer buffering, submission |
| `review-quiz.store.ts` | Post-attempt answer review with correctness display |

### Optimistic UI Updates

```text
User Action → Update Store Immediately → Send HTTP Request
                                              │
                              ┌───────────────┴────────────────┐
                              ▼                                ▼
                       ✅ Success                        ❌ Failure
                       (keep state)                   (rollback + notify)
```

### Route Guards & Auth Interceptor

- **`role.guard.ts`** — Protects routes by checking the user's role from the stored JWT. Redirects unauthenticated users to login; redirects wrong-role users to their own dashboard.
- **`auth.interceptor.ts`** — Automatically attaches the Bearer token to every outgoing request. Handles 401 responses by attempting a token refresh via `RefreshToken` before retrying.

### Services Layer

| Service | Responsibility |
|---|---|
| `quiz.service.ts` | Quiz CRUD, question management |
| `quiz-attempt.service.ts` | Start attempt, submit answers, grade manually |
| `courses.service.ts` | Course & enrollment operations |
| `admin.service.ts` / `instructor.service.ts` / `student.service.ts` | User management per role |
| `college.service.ts` | College listing |
| `question-component-mapper.service.ts` | Maps question type discriminator → concrete Angular component |

---

## 🔍 Code Quality & Consistency

Style violations are **build errors**, not warnings — enforced locally and in CI.

### Backend — StyleCop + EditorConfig

- `TreatWarningsAsErrors = true` — any StyleCop violation fails the build.
- `.editorconfig` fine-tunes rules (`SA1309` off for `_privateField` naming, `SA1101` off for `this.` prefix).
- Consistent import ordering enforced via `dotnet_sort_system_directives_first`.
- **Central Package Management** (`Directory.Packages.props`) — all NuGet version pins live in one file; no per-project version drift.

### Frontend — Stylelint + Property Ordering

Strict CSS property declaration order enforced across every rule:

```text
position → display/flex/grid → sizing → margin → padding → border → background → typography → transitions → misc
```

### Frontend — Unified Path Aliases

```typescript
// ✅ Clean & consistent — no ../../.. chains
import { CoursesService } from "@shared/services/courses.service";
import { withRequestStatus } from "@StoreFeatures/with-request-status.feature";
```

### Frontend — Strict TypeScript

```text
strict · noImplicitReturns · noUnusedLocals · noUnusedParameters
noUncheckedSideEffectImports · strictTemplates · strictInjectionParameters
```

---

## 🛠️ Tech Stack

### Backend

| Technology | Purpose |
|---|---|
| **.NET 10** (C#) | Web API framework |
| **Entity Framework Core 10** | ORM & migrations |
| **PostgreSQL 18** | Relational database |
| **ASP.NET Core Identity** | User management & password hashing |
| **MediatR** | CQRS pipeline & mediator |
| **FluentValidation** | Request validation |
| **JWT + Refresh Tokens** | Authentication & authorization |
| **Hybrid Cache + PostgreSQL Cache** | Two-level distributed caching (L1 in-process + L2 PostgreSQL) |
| **OpenTelemetry** | Distributed tracing |
| **Serilog + Seq** | Structured logging |
| **Prometheus + Grafana** | Metrics & dashboards |
| **Swagger + Scalar** | API documentation |

### Frontend

| Technology | Purpose |
|---|---|
| **Angular 21** | SPA framework (Signals-based) |
| **NgRx Signal Store** | Reactive state management |
| **PrimeNG 21** | UI component library |
| **Vanilla CSS** | Custom styling with CSS variables |
| **Stylelint** | CSS linting with property ordering |

### Infrastructure

| Technology | Purpose |
|---|---|
| **Docker & Docker Compose** | Multi-container orchestration |
| **GitHub Actions** | CI pipeline + Backend deploy pipeline |
| **GitHub Container Registry (GHCR)** | Docker image hosting |
| **StyleCop Analyzers** | C# code style enforcement |
| **Seq** | Log aggregation & search |
| **Prometheus** | Metrics collection |
| **Grafana** | Monitoring dashboards |

---

## 🧪 Testing Strategy

QuizNova has a **four-layer test pyramid** covering the domain, application, API surface, and infrastructure end-to-end:

```text
                    ┌───────────────────────────┐
                    │   Integration Tests        │  ← HTTP-level, real DB (TestContainers)
                    │ QuizNova.Api.IntegrationTests│
                    ├───────────────────────────┤
                    │  Subcutaneous Tests        │  ← MediatR pipeline, real handlers
                    │QuizNova.Application.       │
                    │   SubcutaneousTests        │
                    ├───────────────────────────┤
                    │  Application Unit Tests    │  ← Behaviours, Mappers
                    │QuizNova.Application.       │
                    │    UnitTests               │
                    ├───────────────────────────┤
                    │   Domain Unit Tests        │  ← Pure domain logic, no IO
                    │ QuizNova.Domain.UnitTests  │
                    └───────────────────────────┘
```

| Project | What it tests |
|---|---|
| `QuizNova.Domain.UnitTests` | Quiz business rules, MCQ grading, attempt lifecycle, user creation |
| `QuizNova.Application.UnitTests` | Pipeline behaviours (validation, logging), mapping correctness |
| `QuizNova.Application.SubcutaneousTests` | Full MediatR command/query pipelines with real handlers |
| `QuizNova.Api.IntegrationTests` | HTTP endpoints for Auth, Admin, College, Course, and Grading controllers |

Tests run automatically on every push/PR via the CI pipeline.

> [!NOTE]
> The `QuizNova.Tests.Common` project provides shared test fixtures, builders, and database seeding utilities used across all test projects.

---

## 🚦 Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended — brings up the entire stack)

### ⚡ Quick Start (Docker)

```bash
git clone https://github.com/MoamenElbarqy/quiz-nova.git
cd quiz-nova
docker compose up -d
```

| Service | URL |
|---|---|
| 🌐 Web App | [`http://localhost:4200`](http://localhost:4200) |
| 📘 Swagger API | [`http://localhost:8080/swagger`](http://localhost:8080/swagger) |
| 📙 Scalar API | [`http://localhost:8080/scalar`](http://localhost:8080/scalar) |
| 📋 Seq Logs | [`http://localhost:5341`](http://localhost:5341) |
| 📊 Prometheus | [`http://localhost:9090`](http://localhost:9090) |
| 📈 Grafana | [`http://localhost:3000`](http://localhost:3000) |

### 🔧 Running Locally (Without Docker)

**Backend**

```bash
# Set connection string in appsettings.Development.json or user secrets
dotnet restore
dotnet run --project src/QuizNova.Api
```

**Frontend**

```bash
cd src/QuizNova.Client
npm install
npm run dev
```

---

## 🔄 CI/CD Pipeline

QuizNova has two GitHub Actions workflows:

### CI — `ci.yml`

Runs on every push / PR to `main`. Both jobs run **in parallel** for fast feedback:

```text
┌─────────────────────────┐     ┌──────────────────────────────┐
│   Backend Build & Test  │     │      Frontend Build           │
│                         │     │                               │
│  dotnet restore         │     │  npm ci                       │
│  dotnet build (Release) │     │  npm run lint:css (Stylelint) │
│  dotnet test            │     │  npm run build                │
│  TreatWarningsAsErrors  │     │                               │
└─────────────────────────┘     └──────────────────────────────┘
```

A StyleCop violation or a failing test breaks the build.

### Deploy — `deploy-backend.yml`

Triggers on push to `main` **only when backend source files change** (path filtering). Builds the API Docker image and pushes it to **GitHub Container Registry (GHCR)**:

```text
Checkout → Log in to GHCR → Build Docker Image → Push ghcr.io/<repo>-api:latest
```

This means frontend-only changes do not trigger an unnecessary Docker rebuild.

---

## 📁 Project Structure

```text
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

Developed with ❤️ by Moamen

</div>
