# Sales API

A RESTful API built with **ASP.NET 8** for managing **users, products, and sales**, featuring **JWT authentication**, **SQLite database**, and a **layered architecture** that separates responsibilities for better maintainability and scalability.

---

# Overview

This project is a REST API developed with **ASP.NET 8** to manage users, products, and sales. It uses **JWT Bearer Authentication** for secure access and **SQLite** as the database engine.

The application follows a **Layered Architecture**, where each layer has a single responsibility, making the code easier to maintain, test, and extend.

---

# Project Structure

```text
SalesApi/
├── Controllers/          # Handles HTTP requests
├── Services/             # Business logic
├── Domain/
│   ├── Entities/         # Database models
│   └── Interfaces/       # Contracts (interfaces)
├── Infrastructure/
│   ├── Data/             # Entity Framework DbContext
│   └── Repositories/     # Database access
├── DTOs/                 # Data Transfer Objects
└── Middleware/           # Global exception handling
```

---

# Request Flow

```text
HTTP Request
     │
     ▼
Controller
     │  Validate request and call Service
     ▼
Service
     │  Apply business rules and call Repository
     ▼
Repository
     │  Execute database operations using EF Core
     ▼
SQLite Database
```

---

# Domain Entities

The application contains the following entities:

## User

Represents a system user.

**Properties**

* Name
* Email
* Password (hashed)
* Role (Admin or Customer)
* Active/Inactive status

---

## Product

Represents a product available for sale.

**Properties**

* Name
* Description
* Price
* Stock
* Category
* Active/Inactive status

---

## Sale

Represents a customer purchase.

**Properties**

* Associated User
* Status

  * Pending
  * Confirmed
  * Cancelled
* Total Amount

---

## SaleItem

Represents an individual item within a sale.

**Properties**

* Product
* Quantity
* Unit Price

---

# Entity Relationships

```text
User
 └── 1 ──────── N Sales

Sale
 └── 1 ──────── N SaleItems

SaleItem
 └── N ──────── 1 Product
```

---

# JWT Authentication

Authentication flow:

1. Client sends:

```http
POST /api/auth/login
```

with:

* Email
* Password

2. The API:

* Verifies the password using **BCrypt**
* Generates a signed JWT token

3. The token contains:

* User ID
* Name
* Email
* Role (Admin or Customer)

4. The client sends the token in every protected request:

```http
Authorization: Bearer <token>
```

5. The API automatically validates the JWT before accessing protected endpoints.

---

# Role-Based Authorization

| Action                | Admin | Customer |
| --------------------- | :---: | :------: |
| List all users        |   ✅   |     ❌    |
| View/Edit own profile |   ✅   |     ✅    |
| Product CRUD          |   ✅   |     ❌    |
| View product catalog  |   ✅   |     ✅    |
| Create own sales      |   ✅   |     ✅    |
| View all sales        |   ✅   |     ❌    |
| Confirm/Cancel sales  |   ✅   |     ❌    |

---

# Business Rules

## UserService

Responsible for user management.

Business rules:

* Prevents duplicate email addresses
* Hashes passwords using BCrypt before saving

---

## ProductService

Responsible for product management.

Business rules:

* Product CRUD operations
* Stock management

---

## SaleService

Responsible for the sales process.

Business rules:

* Validates product stock before creating a sale
* Automatically decreases stock when a sale is created
* Restores stock when a sale is cancelled or deleted
* Calculates the total sale amount by summing all sale items

---

# DTOs (Data Transfer Objects)

DTOs separate the API contract from the internal database models.

Example:

When creating a user:

```text
CreateUserDto
```

contains the password in plain text.

The response uses:

```text
UserResponseDto
```

which **never exposes the password**, improving security.

---

# Default Admin User (Seed Data)

The database is automatically seeded with an administrator account.

| Field    | Value                                           |
| -------- | ----------------------------------------------- |
| Email    | [admin@salesapi.com](mailto:admin@salesapi.com) |
| Password | Admin@123                                       |

---

# Technologies

| Technology              | Purpose                  |
| ----------------------- | ------------------------ |
| ASP.NET 8               | Web Framework            |
| Entity Framework Core 8 | ORM                      |
| SQLite                  | Database                 |
| JWT Bearer              | Stateless Authentication |
| BCrypt                  | Secure Password Hashing  |
| Swagger / OpenAPI       | API Documentation        |

---

# Features

* RESTful API
* JWT Authentication
* Role-based Authorization
* Layered Architecture
* Repository Pattern
* Entity Framework Core
* SQLite Database
* Global Exception Middleware
* Swagger Documentation
* Secure Password Hashing (BCrypt)
* DTO Pattern
* Automatic Database Seeding

---

# Getting Started

## Clone the repository

```bash
git clone https://github.com/yourusername/SalesApi.git
```

## Restore packages

```bash
dotnet restore
```

## Run the project

```bash
dotnet run
```

The API will automatically create the SQLite database (if it does not exist) and seed the default administrator account.

---

# API Documentation

Once the application is running, access Swagger at:

```text
https://localhost:<port>/swagger
```

---

# License

This project is licensed under the MIT License.

