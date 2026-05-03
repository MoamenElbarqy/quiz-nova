# 🚀 QuizNova

**QuizNova** is a high-performance, enterprise-grade quiz management ecosystem. It provides a seamless platform for educational institutions to manage courses, design complex assessments, and track student performance through real-time analytics.

Built with the latest bleeding-edge technologies (** .NET 10** and **Angular 21**), the project serves as a reference implementation for **Clean Architecture**, **Domain-Driven Design (DDD)**, and **Reactive Frontend State Management**.

---

## 🏗️ Architecture Overview

The system is built on a **Modular Monolith** foundation following **Clean Architecture** principles to ensure high maintainability and testability.

- **Domain:** Contains enterprise logic, entities, and value objects. Implements the **Result Pattern** for robust error handling without exceptions.
- **Application:** Implements **CQRS** using **MediatR**. Handles orchestration, validation (FluentValidation), and mapping.
- **Infrastructure:** Manages data persistence (EF Core + PostgreSQL), Authentication (JWT + Refresh Tokens), and external services.
- **API:** A versioned RESTful API utilizing modern .NET features like Global Exception Handling and OpenApi/Scalar documentation.

---

## 🛠️ Tech Stack

### Backend
- **Framework:** .NET 10 (C#)
- **ORM:** Entity Framework Core 10
- **Database:** PostgreSQL
- **Communication:** MediatR (CQRS Pattern)
- **Validation:** FluentValidation
- **Auth:** JWT Bearer + Refresh Token Strategy
- **Observability:** OpenTelemetry, Serilog, Seq, and Prometheus

### Frontend
- **Framework:** Angular 21 (Signals-based architecture)
- **State Management:** NGRX Signal Store
- **UI Components:** PrimeNG 21 & PrimeIcons
- **Styles:** Vanilla CSS with modern CSS Variables
- **Testing:** Vitest

### Infrastructure & DevOps
- **Containerization:** Docker & Docker Compose
- **API Docs:** Swagger (OpenAPI) & Scalar
- **Logging:** Structured logging with Seq

---

## ✨ Key Features

### 🔐 Multi-Tenant RBAC
- **Admins:** Manage colleges, instructors, students, and system-wide audits.
- **Instructors:** Full lifecycle quiz management (Drafting, Publishing, Analytics).
- **Students:** Interactive quiz attempts, time-limited assessments, and detailed result reviews.

### 📝 Advanced Quiz Engine
- **Polymorphic Questions:** Support for Multiple Choice Questions (MCQ) and True/False (T/F).
- **Real-time Grading:** Automated scoring engine with immediate feedback.
- **Attempt Lifecycle:** Tracking started/submitted timestamps and attempt duration.

### 📊 Observability & Monitoring
- **Centralized Logging:** All logs are enriched and sent to **Seq** for real-time analysis.
- **Metrics:** Instrumented with **OpenTelemetry**; metrics exported to **Prometheus**.

---

## 🚦 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js (v22+)](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Quick Start with Docker
The easiest way to run the entire stack (API, Client, DB, Seq, Prometheus) is using Docker Compose:

```bash
docker-compose up -d
```

**Access the services:**
- **Web App:** `http://localhost:4200`
- **API Swagger:** `http://localhost:5000/swagger`
- **Seq Logs:** `http://localhost:5341`

### Local Development (Manual)

1. **Database:** Start a PostgreSQL instance and update `appsettings.json`.
2. **Backend:**
   ```bash
   cd src/QuizNova.Api
   dotnet run
   ```
3. **Frontend:**
   ```bash
   cd src/QuizNova.Client
   npm install
   npm start
   ```

---

## 🧪 Testing
The project prioritizes quality through automated testing:
- **Backend:** Unit and Integration tests using xUnit.
- **Frontend:** Fast component testing using **Vitest**.

```bash
# Run backend tests
dotnet test

# Run frontend tests
npm run test
```

---

## 📈 Roadmap & Future Enhancements
- [ ] Integration with AI for automated question generation.
- [ ] Support for more question types (Short Answer, Matching).
- [ ] Real-time proctoring features.
- [ ] Mobile application using Flutter or Compose Multiplatform.

---

## 📄 License
Distributed under the MIT License. See `LICENSE` for more information.

---
*Developed with ❤️ as a showcase of modern software engineering.*
