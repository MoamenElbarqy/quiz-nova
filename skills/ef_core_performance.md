---
name: EF Core Performance
description: Standardizes data access patterns for optimal performance and memory usage in QuizNova.
triggers:
  - When implementing Query handlers or read-only database access.
  - When implementing Command handlers that modify data.
---

# EF Core Performance Skill

Use this skill to ensure that database interactions are efficient and follow the Command Query Responsibility Segregation (CQRS) principles.

## 1. Read-Only Queries

When implementing a **Query** (read-only flow), always use `.AsNoTracking()`.

- **Instruction:** If you are retrieving entities to return as DTOs or for display only, append `.AsNoTracking()` to the `IQueryable`.
- **Reason:** This disables the EF Core change tracker, reducing memory overhead and improving execution speed.
- **Example:**

  ```csharp
  var quizzes = await dbContext.Quizzes
      .AsNoTracking()
      .Where(q => q.CourseId == courseId)
      .ToListAsync(ct);
  ```

## 2. DTO Projections (Preferred)

Prefer projecting directly to DTOs using `.Select()` instead of fetching full entities.

- **Instruction:** Map directly to your DTO in the query whenever possible.
- **Reason:** This minimizes the amount of data transferred from the database (SQL `SELECT` will only include specific columns). Projections are automatically "no-tracking."
- **Example:**

  ```csharp
  var summaries = await dbContext.Quizzes
      .Where(q => q.InstructorId == instructorId)
      .Select(q => new QuizSummaryDto(q.Id, q.Title))
      .ToListAsync(ct);
  ```

## 3. Command Handlers (Tracking Required)

Do **NOT** use `AsNoTracking()` when you intend to modify or delete the entity.

- **Instruction:** In **Command** handlers, retrieve the entity normally so that EF Core can track changes.
- **Reason:** EF Core needs to track the entity state to generate the correct `UPDATE` or `DELETE` statements when `SaveChangesAsync()` is called.
- **Example:**

  ```csharp
  var quiz = await dbContext.Quizzes.FindAsync(request.Id, ct);
  quiz.UpdateTitle(request.NewTitle); // Change tracking handles the update
  await dbContext.SaveChangesAsync(ct);
  ```

## Guidelines

- Always prioritize **Projections** (Rule 2) for reading data.
- Use `AsNoTracking()` (Rule 1) only if you must return the full entity for some reason.
- Never use `AsNoTracking()` if you call any method that modifies the entity state.
