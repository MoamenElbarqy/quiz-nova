# 📖 Ubiquitous Language

This document defines the core terminology used throughout the **QuizNova** ecosystem. These terms are used consistently across our Domain Model, Database Schema, and User Interface to ensure a shared understanding between developers and stakeholders.

## 👥 Roles & Actors

| Term | Professional Definition |
| :--- | :--- |
| **Admin** | A member of the college administrative staff responsible for system-wide management, including the oversight of colleges, departments, and user accounts. |
| **Instructor** | An academic staff member (e.g., Professor or Doctor) responsible for teaching a specific course and managing the full lifecycle of its quizzes. |
| **Student** | A learner enrolled in a course who attempts quizzes and reviews their academic performance. |

## 📝 Quiz & Assessment

| Term | Professional Definition |
| :--- | :--- |
| **Quiz** | A structured assessment comprising a collection of questions, assigned to a specific course with defined start and end periods. |
| **Question** | An individual assessment item. Our system uses a polymorphic design to support multiple formats. |
| **MCQ** | **Multiple Choice Question**: A question format where the student must select exactly one correct answer from a set of predefined options. |
| **TF** | **True/False**: A binary-choice question format where the student evaluates the veracity of a specific statement. |
| **Mark** | The weight or numeric value assigned to a question, representing its contribution to the total quiz score. |

## ⏱️ Execution & Results

| Term | Professional Definition |
| :--- | :--- |
| **Attempt** | A single, time-bound instance of a student engaging with a specific quiz. |
| **Submission** | The act of finalizing an attempt, either manually by the student or automatically by the system when the time limit expires. |
| **Result** | The calculated score and feedback generated after an attempt is submitted and graded. |

---

> [!NOTE]
> This language is "ubiquitous" because it is used everywhere: in the code (e.g., `Quiz.cs`), in the database tables, and in the UI labels.
