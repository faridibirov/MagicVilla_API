# ASP.NET Core Web API and MVC Integration

## Overview
This project showcases the development of a comprehensive RESTful API using ASP.NET Core Web API and its integration with an ASP.NET MVC application. The API handles all backend operations, while the MVC application serves as the frontend, consuming the API for various functionalities.

## Features

### Web API
- **Comprehensive API Development**: Built with ASP.NET Core Web API and .NET 8, the project demonstrates the creation of robust RESTful APIs.
- **Swagger Documentation**: Integrated Swagger and Swashbuckle for clear and interactive API documentation.
- **API Versioning**: Implemented version control to support multiple versions of the API simultaneously.
- **Repository Pattern**: Utilized the Repository Pattern with Entity Framework Core for efficient and maintainable data access.
- **Secure Authentication and Authorization**: Integrated .NET Identity for secure user authentication and role-based authorization.
- **Database Management**: Employed Entity Framework Core with Code-First Migrations for database schema management.
- **Advanced Functionalities**: Implemented features like file and image uploads, refresh tokens for session management, and clean coding practices.
- **Exception Handling**: Implemented robust error handling using filters and middleware.
- **Azure Deployment**: The API is fully deployed to Microsoft Azure, showcasing cloud deployment practices.

### MVC Application
- **API Consumption**: The ASP.NET MVC application consumes the Web API using `HttpClient`, demonstrating how to interact with and utilize RESTful services.
- **User Interface**: Provides a user-friendly interface for managing data handled by the API, including CRUD operations.
- **Authentication and Authorization**: Leverages the API's authentication mechanisms to secure the MVC application.
- **Data Display and Management**: Fetches and displays data from the API, allowing users to interact with backend services seamlessly.
- **File Uploads**: Integrates with the API to handle file and image uploads, providing a complete end-to-end solution.
- **Deployment**: The MVC application is also deployed alongside the API, ensuring a cohesive deployment strategy on Microsoft Azure.

## Technologies Used
- **ASP.NET Core Web API**: For building the backend RESTful service.
- **ASP.NET Core MVC**: For developing the frontend application.
- **Entity Framework Core**: For data access and database management.
- **Swagger and Swashbuckle**: For API documentation.
- **.NET Identity**: For managing user authentication and authorization.
- **JWT (JSON Web Tokens)**: For securing API endpoints with token-based authentication.
- **Azure**: For deploying both the API and MVC application.

## Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/faridibirov/MagicVilla_API.git
cd MagicVilla_API
```

### 2. Configure the Database and API Keys
Update the connection strings, API keys, and other settings in the appsettings.json files for both the API and MVC projects.

### 3. Run the API and MVC Applications
- **API: Start the Web API project to handle backend operations.
- **MVC: Start the MVC project to interact with the API through the user interface.

### Contribution
Feel free to contribute to this project by submitting issues or pull requests. Your feedback and improvements are welcome!

