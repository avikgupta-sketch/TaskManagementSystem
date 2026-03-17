# TaskManagementSystem
TMS is a backend REST API designed to streamline task management 
in a team environment. It allows SuperAdmins to manage users, 
Admins to create and assign tasks, and Users to work on their 
assigned tasks and communicate through comments.

Built with a focus on clean architecture, JWT-based security, 
and role-based authorization.
ASP.NET Core Web API (.NET 8) | Backend REST API framework |
| SQL Server | Relational database |
| Entity Framework Core | ORM for database operations |
| JWT (JSON Web Tokens) | Authentication |
| BCrypt.Net | Secure password hashing |
| AutoMapper | Object-to-object mapping |
| Swagger / OpenAPI | API documentation & testing |

JWT Bearer Token based authentication
- Role-based authorization with three roles — `SuperAdmin`, `Admin`, and `User`
- Every endpoint is protected with `[Authorize(Roles = "...")]`
- Passwords are securely hashed using **BCrypt** before storing in the database
- No plain-text passwords stored anywhere
