# Order Management API

## Technologies Used

- .NET Core 8.0
- PostgreSQL
- Entity Framework Core
- JWT Authentication
- Serilog
- AutoMapper
- Swagger/OpenAPI

## Prerequisites

- .NET Core SDK 8.0 or later
- PostgreSQL

## Getting Started

1. Clone the repository:
```bash
git clone [repository-url]
cd CustomerPortal
```

2. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=customerportaldb;Username=postgres;Password=your_password"
  }
}
```

3. Apply database migrations:
```bash
dotnet ef database update
```

4. Run the application:
```bash
dotnet run
```

The API will be available at:
- HTTP: http://localhost:5017
- HTTPS: https://localhost:7235
- Swagger UI: https://localhost:7235/swagger

## API Documentation

The API documentation is available through Swagger UI. After running the application, navigate to:
```
https://localhost:7235/swagger
```

### Authentication

The API uses JWT Bearer authentication. To access protected endpoints:

1. Register a new customer: `POST /api/customers`
2. Login: `POST /api/auth/login`
3. Use the returned token in the Authorization header:
```
Authorization: Bearer {token}
```

Once you ran the script, you will be automatically seed a admin user which is under my name. You can use this payload request for logging into admin account.
```json
{
  "email": "jiazheng010509@gmail.com",
  "password": "Admin@123"
}
```

## Configuration

Configuration settings are able to changed in `appsettings.json`:

1. **Database Connection**
2. **JWT Settings**
3. **Logging Configuration**
