# 📄 Professional Resume Builder System

A monolithic, full-stack web application built with **ASP.NET MVC 5 (.NET Framework)**. This system allows users to dynamically create and manage complex resumes (CVs), including personal details, profile photos, and multiple entries for education and work experience. It features robust administrative controls and heavily utilizes SQL Stored Procedures for secure data transactions.

## 🏗️ Architecture & Core Logic

* **Master-Details Form Handling:** The core `ResumeController` manages a complex `ApplicantViewModel`. It saves a "Master" record (`Applicant`) alongside multiple "Detail" records (lists of `Education` and `Experience`) within a single, unified view.
* **Atomic Database Transactions:** Multi-table inserts and updates (Applicant + Educations + Experiences) are wrapped in `db.Database.BeginTransaction()` to ensure complete data integrity. If one insertion fails, the entire resume save rolls back.
* **Stored Procedure Integration:** Bypasses standard Entity Framework `SaveChanges()` for critical operations. Uses explicit raw SQL execution (`sp_InsertApplicant`, `sp_InsertEducation`, `sp_UpdateDegree`, etc.) for highly optimized, secure database interactions.
* **Role-Based Access Control (RBAC):** Built on top of **ASP.NET Identity (OWIN)**. Standard users can only view and edit their own resumes, while "Admin" users have global access to manage all resumes, system lookup tables, and user roles.

## 📂 Key Modules & Features

### 🎓 Resume Generation (`ResumeController`)
* **Dynamic UI:** Add/remove multiple Education and Experience rows dynamically on the client side before submission.
* **File Uploads:** Secure handling of user profile photos, saving them to the server's `~/Uploads` directory and storing the relative path in the database.
* **Data Validation:** Server-side checks to dynamically ignore/remove Experience validation if the user checks the "No Experience" flag (`HasExperience = false`).

### ⚙️ System Configuration (`DegreesController`, `InstitutionsController`)
* **Lookup Management:** Admin-only interfaces to perform CRUD operations on standard Degrees and Institutions used in the dropdown menus of the Resume Builder.

### 🔐 Identity & Admin (`AccountController`, `AdminController`, `ManageController`)
* **Authentication:** Full suite of login, registration, password reset, and two-factor authentication functionalities.
* **User Management:** Admin dashboard to view registered users and dynamically reassign their security roles (e.g., promoting a standard User to Admin).

## 🛠️ Technical Stack

* **Framework:** ASP.NET MVC 5 (.NET Framework)
* **Authentication:** ASP.NET Identity (OWIN)
* **ORM / Data Access:** Entity Framework 6 (Code-First) mixed with direct ADO.NET Stored Procedure executions.
* **Database:** Microsoft SQL Server
* **Frontend:** Razor View Engine (`.cshtml`), HTML5, CSS3, Bootstrap (assumed for MVC5), and jQuery (for partial view rendering and dynamic form rows).

## 🚀 Getting Started

### Prerequisites
* Visual Studio 2019 / 2022 with the **ASP.NET and web development** workload installed.
* SQL Server (LocalDB or standard instance).
