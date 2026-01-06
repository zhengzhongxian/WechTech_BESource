# WechTech Backend API

A modern, scalable backend API for an e-commerce platform built with .NET 8, featuring product management, order processing, payment integration, user authentication, and more.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Technologies](#technologies)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [API Documentation](#api-documentation)
- [Configuration](#configuration)
- [Database](#database)
- [Authentication](#authentication)
- [Payment Integration](#payment-integration)
- [Contributing](#contributing)

## 🎯 Overview

WechTech Backend Source is a comprehensive e-commerce API built following the layered architecture pattern with clear separation of concerns:

- **API Layer** - RESTful endpoints and request handling
- **Service Layer** - Business logic and core functionality
- **Repository Layer** - Data access and persistence

The API is fully documented with Swagger/OpenAPI and includes features for managing products, orders, customers, payments, and more.

## 🏗️ Architecture

The project follows a **3-tier layered architecture**:

```
┌─────────────────────────┐
│   WebTechnology.API     │  Controllers, Configurations
├─────────────────────────┤
│ WebTechnology.Service   │  Business Logic, Services
├─────────────────────────┤
│WebTechnology.Repository │  Data Access, Entities, DTOs
└─────────────────────────┘
```

### Key Architectural Components:

- **Unit of Work Pattern** - Manages transactions and multiple repositories
- **Generic Repository Pattern** - Reusable CRUD operations
- **Dependency Injection** - Service configuration and dependency management
- **AutoMapper** - Object-to-object mapping for DTOs
- **JWT Authentication** - Secure token-based authentication
- **Background Services** - Async cleanup and maintenance tasks

## ✨ Features

### Product Management
- Product catalog with categories and brands
- Product pricing and variants
- Product trends and trending items
- Product reviews and ratings
- Inventory and unit management
- Image management with cloud storage

### Order Management
- Shopping cart functionality
- Order creation and processing
- Order status tracking
- Order history and details
- Order statistics and analytics

### Payment System
- PayOS payment gateway integration
- Payment history and records
- Webhook handling for payment confirmations
- Multiple payment status tracking

### User Management
- User registration and authentication
- Role-based access control (RBAC)
- User profile management
- Customer details and preferences
- Password reset functionality
- Email verification and notifications

### Promotions & Discounts
- Voucher management
- Coupon system
- Customer voucher tracking
- Discount application

### Admin Features
- Admin dashboard statistics
- User and role management
- Product and inventory management
- Order management
- Payment processing

### Additional Features
- Email notifications
- Cloud image storage (Cloudinary)
- Product dimensions and specifications
- Parent product categorization
- Pagination and filtering
- Comprehensive logging

## 🛠️ Technologies

### Core Framework
- **.NET 8** - Latest .NET framework
- **ASP.NET Core** - Web API framework

### Database & ORM
- **Entity Framework Core 8** - ORM and database management
- **MySQL 8.0** - Relational database

### API Documentation
- **Swagger/Swashbuckle** - OpenAPI documentation
- **ReDoc** - API documentation UI

### Authentication & Security
- **JWT (JSON Web Tokens)** - Token-based authentication
- **Microsoft.IdentityModel** - Identity and access management

### External Services
- **PayOS SDK** - Payment processing
- **Cloudinary** - Image storage and optimization
- **Email Service** - SMTP email notifications

### Additional Libraries
- **AutoMapper** - Object mapping
- **Newtonsoft.Json** - JSON serialization

## 📦 Prerequisites

- **.NET 8 SDK** or later
- **MySQL 8.0** or compatible
- **Visual Studio 2022** (recommended) or VS Code
- **Git** for version control

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/zhengzhongxian/WechTech_BESource.git
cd WechTech_BESource
```

### 2. Set Up Environment Configuration

Create an `appsettings.json` file in the `WebTechnology` project with the following structure:

```json
{
  "ConnectionStrings": {
    "Connection": "Server=your_server;Database=webtech_db;User=your_user;Password=your_password;Port=3306;"
  },
  "JwtSettings": {
    "SecretKey": "your_secret_key_min_256_bits",
    "Issuer": "WebTechnology",
    "Audience": "WebTechnologyUsers",
    "ExpirationMinutes": 60
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "your_email@gmail.com",
    "SenderPassword": "your_app_password",
    "SenderName": "WechTech"
  },
  "Cloudinary": {
    "CloudName": "your_cloud_name",
    "ApiKey": "your_api_key",
    "ApiSecret": "your_api_secret"
  },
  "Payos": {
    "ClientId": "your_client_id",
    "ApiKey": "your_api_key",
    "ChecksumKey": "your_checksum_key"
  }
}
```

### 3. Install Dependencies

```bash
cd WebTechnology
dotnet restore
```

### 4. Database Setup

Apply migrations to create the database:

```bash
dotnet ef database update
```

Or create migrations:

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The API will be available at:
- **Swagger UI**: `https://localhost:7000/api-docs`
- **ReDoc**: `https://localhost:7000/redoc`
- **API Base URL**: `https://localhost:7000`

## 📁 Project Structure

### WebTechnology.API
```
Controllers/
├── AuthController
├── ProductController
├── OrderController
├── PaymentController
├── CustomerController
├── AdminController
├── ReviewController
├── VoucherController
├── CouponController
└── ... (other controllers)

Configurations/
├── ServiceConfiguration
├── AuthenticationConfiguration
├── SwaggerConfiguration
└── CustomUnauthorizedMiddleware

Program.cs
```

### WebTechnology.Service
```
Services/
├── Interfaces/ (Service contracts)
└── Implementations/ (Service implementations)
    ├── ProductService
    ├── OrderService
    ├── PaymentService
    ├── AuthService
    ├── CustomerService
    └── ... (other services)

CoreHelpers/
├── Multimedia/ (Cloudinary integration)
├── Generations/ (OTP, tokens, order numbers)
├── Extensions/ (Pagination)
└── MailBody.cs

Models/
├── ServiceResponse
├── AuthResponse
├── JwtSettings
├── EmailSetting
└── ... (DTOs and models)
```

### WebTechnology.Repository
```
Models/
├── Entities/ (Database entities)
│   ├── Product
│   ├── Order
│   ├── Customer
│   ├── User
│   └── ...
└── Pagination/

DTOs/
├── Products/
├── Orders/
├── Users/
├── Payments/
└── ...

Repositories/
├── Interfaces/ (Repository contracts)
└── Implementations/ (Repository implementations)

UnitOfWork/
├── IUnitOfWork.cs
└── UnitOfWork.cs

Migrations/
CoreHelpers/
├── Profiles/ (AutoMapper mappings)
├── Enums/
└── ValidationCustom/
```

## 📚 API Documentation

The API is fully documented using Swagger/OpenAPI. Once the application is running:

1. **Swagger UI** - Interactive API documentation
   - URL: `https://localhost:7000/api-docs`
   - Try out API endpoints directly from the browser

2. **ReDoc** - Alternative API documentation
   - URL: `https://localhost:7000/redoc`
   - Clean, reader-friendly API documentation

### Main Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | User login |
| POST | `/api/auth/register` | User registration |
| GET | `/api/products` | Get product list |
| POST | `/api/products` | Create new product (Admin) |
| GET | `/api/orders` | Get user orders |
| POST | `/api/orders` | Create new order |
| POST | `/api/payments` | Process payment |
| GET | `/api/customers` | Get customer info |
| POST | `/api/reviews` | Submit product review |

## 🔐 Configuration

### Environment Variables

Store sensitive information in `appsettings.json` or use .NET User Secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Connection" "your_connection_string"
dotnet user-secrets set "JwtSettings:SecretKey" "your_secret_key"
```

### Key Settings

- **Database Connection** - MySQL connection string
- **JWT Settings** - Token configuration (secret, issuer, audience, expiration)
- **Email Settings** - SMTP configuration for email notifications
- **Cloudinary** - Cloud storage credentials for images
- **PayOS** - Payment gateway credentials

## 🗄️ Database

The project uses **Entity Framework Core** with **MySQL**. 

### Key Entities:
- **User/Customer** - User accounts and customer information
- **Product** - Product catalog
- **Order/OrderDetail** - Order management
- **Payment** - Payment records
- **Category/Brand** - Product classification
- **Cart/CartItem** - Shopping cart
- **Review/Comment** - Product reviews
- **Voucher/Coupon** - Promotions and discounts
- **Role/Permission** - Access control

### Migrations

Migrations are version-controlled in the `Migrations` folder. Each migration includes:
- Schema changes (CreateTable, AddColumn, etc.)
- Data seeding
- Index creation
- Relationship constraints

## 🔑 Authentication

The API uses **JWT (JSON Web Token)** for authentication:

1. User logs in with credentials
2. Server generates a JWT token
3. Client includes token in `Authorization: Bearer <token>` header
4. Token is validated on each protected request
5. Token expires after configured duration

### Protected Endpoints

Protected endpoints require:
```
Authorization: Bearer <your_jwt_token>
```

### Roles & Claims

- **Admin** - Full system access
- **Staff** - Limited admin access
- **Customer** - User/customer access
- **Guest** - Limited public access

## 💳 Payment Integration

### PayOS Payment Gateway

The API integrates with **PayOS** for secure payment processing:

1. **Payment Initiation** - Create payment links
2. **Order Processing** - Link payments to orders
3. **Webhook Handling** - Receive payment confirmations
4. **Status Tracking** - Monitor payment status

### Payment Flow

```
1. Create Order → 2. Generate Payment Link → 3. Customer Pays → 4. Webhook Confirmation → 5. Update Order Status
```

## 🔄 Background Services

### UserAuthCleanupService

Automatically cleans up expired authentication tokens and temporary data.

## 📝 API Response Format

Standard API response format:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {},
  "statusCode": 200
}
```

Error response:

```json
{
  "success": false,
  "message": "Error description",
  "errors": ["Error detail 1", "Error detail 2"],
  "statusCode": 400
}
```

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Create a feature branch (`git checkout -b feature/AmazingFeature`)
2. Commit changes (`git commit -m 'Add AmazingFeature'`)
3. Push to branch (`git push origin feature/AmazingFeature`)
4. Open a Pull Request

## 📄 License

This project is part of WechTech Backend Source. See the repository for more details.

## 📧 Contact & Support

For issues, questions, or support, please refer to the project's issue tracker on GitHub:
- Repository: https://github.com/zhengzhongxian/WechTech_BESource

## 🔗 Resources

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [JWT Authentication](https://tools.ietf.org/html/rfc7519)
- [PayOS Documentation](https://payos.vn/)
- [Cloudinary Documentation](https://cloudinary.com/documentation)

---

**Last Updated**: 2025

**Framework Version**: .NET 8

**Database**: MySQL 8.0+
