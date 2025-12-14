
# QuizAPI – Technical Assignment

QuizAPI is an **ASP.NET Core Web API** application developed as a solution for a technical assignment focused on designing a system for creating, managing, and exporting quizzes.

The goal of this solution is not only to implement the required functionality, but also to demonstrate:
- clear API design
- correct usage of Entity Framework Core (Code First)
- separation of concerns
- extensible architecture (plugin-based export system)
- clean and readable code suitable for long-term maintenance

---

## 🎯 Problem Description

Rejd frequently creates quizzes and needs an API that allows him to:

- create and manage quizzes
- reuse (recycle) previously created questions
- export quiz questions into a file for printing and distribution

Solving or answering quizzes is **explicitly out of scope**.  
The application focuses solely on **quiz management and export**.

---

## 🧩 Architectural Approach

The application is structured into clearly separated layers:

- **Controllers** – HTTP API endpoints
- **Domain** – Core business entities (`Quiz`, `Question`, `QuizQuestion`)
- **Data** – Entity Framework Core `DbContext` and database configuration
- **Models (DTOs)** – Request and response models
- **Export** – Plugin-based export system using MEF

This separation ensures:
- controllers remain thin and focused
- domain logic is isolated
- API models are decoupled from database entities
- export formats can be extended without modifying existing code

---

## 🛠️ Technologies Used

- **.NET 8 / ASP.NET Core Web API**
- **Entity Framework Core (Code First)**
- **SQLite**
- **MEF (Managed Extensibility Framework)**
- **Swagger / OpenAPI**

---

## 📦 Project Structure

```

QuizAPI
│
├── Controllers
│   ├── QuizzesController.cs
│   └── QuestionsController.cs
│
├── Data
│   └── QuizDbContext.cs
│
├── Domain
│   ├── Quiz.cs
│   ├── Question.cs
│   └── QuizQuestion.cs
│
├── Export
│   ├── IQuizExporter.cs
│   ├── CsvQuizExporter.cs
│   └── QuizExportService.cs
│
├── Models
│   ├── Requests
│   └── Responses
│
├── Migrations
│
├── Program.cs
└── appsettings.json

```

---

## 🔄 Data Model Overview

- **Quiz**
  - Has a title
  - Contains multiple questions
- **Question**
  - Contains question text and the correct answer
  - Can belong to multiple quizzes
- **QuizQuestion**
  - Join entity enabling a many-to-many relationship
  - Allows quiz–question reuse (recycling)

This design enables:
- reusing questions across multiple quizzes
- deleting quizzes without deleting questions
- future extensibility (e.g. question ordering)

---

## 🔁 Question Recycling

When creating or updating a quiz:
- new questions can be added
- existing questions can be reused by ID
- duplicate questions are avoided
- questions are never deleted when a quiz is removed

This fulfills the requirement of **question recycling** and ensures data integrity.

---

## 📤 Export System (MEF)

The export functionality is implemented as a **plugin-based system** using MEF.

### Key Concepts:
- All exporters implement the `IQuizExporter` interface
- Exporters are discovered dynamically at runtime
- Adding a new export format does not require modifying existing code

### Currently supported formats:
- **CSV**

### Example CSV output:

```

1. What is the capital of France?
2. Who invented the telephone?
3. How many continents are there?

````

Only question texts are exported, as required.

---

## 🌐 API Endpoints

### Quizzes
| Method | Endpoint | Description |
|------|--------|------------|
| GET | `/api/quizzes` | Get all quizzes (without questions) |
| GET | `/api/quizzes/{id}` | Get quiz details with questions |
| POST | `/api/quizzes` | Create a new quiz |
| PUT | `/api/quizzes/{id}` | Update an existing quiz |
| DELETE | `/api/quizzes/{id}` | Delete a quiz |

### Questions
| Method | Endpoint | Description |
|------|--------|------------|
| GET | `/api/questions?search=` | Search questions by text |

### Export
| Method | Endpoint | Description |
|------|--------|------------|
| GET | `/api/quizzes/export/formats` | Get available export formats |
| GET | `/api/quizzes/{id}/export?format=csv` | Export quiz |

---

## 🚀 Running the Application

### Prerequisites
- .NET SDK 8+
- Visual Studio 2022+ or VS Code

### Steps

1. Clone the repository:
```bash
git clone https://github.com/antonelagagro/QuizAPI.git
````

2. Open the project in Visual Studio

3. Apply database migrations:

```bash
dotnet ef database update
```

4. Run the application:

```bash
dotnet run
```

5. Open Swagger:

```
https://localhost:{port}/swagger
```

---

## 📝 Notes for Reviewers

* SQLite database files are excluded from version control
* The project follows async/await best practices
* DTO pattern is used to decouple API contracts from persistence
* The export system is designed for easy extension
* Swagger is used as primary API documentation

---

## 👩‍💻 Author

**Antonela Gagro**
Software Developer

