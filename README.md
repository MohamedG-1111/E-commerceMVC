# 🛒 E-Commerce MVC

A modern **E-Commerce Web Application** built with **ASP.NET Core MVC** following the **3-Tier Architecture**. The project demonstrates scalable backend development practices, secure authentication, online payments, caching, background processing, and third-party service integration.

---

## 🚀 Features

### 👤 Authentication & Authorization

* ASP.NET Core Identity
* User Registration
* Login & Logout
* Email Confirmation
* Forgot Password
* Reset Password
* Role-Based Authorization:

  * **Admin** – Full system management
  * **Employee** – Manage customer orders
  * **Company User** – Delayed payment support and exclusive discounts
  * **Customer** – Browse products, manage cart, and place orders

---

### 🛍️ Product Management

* Browse Products
* Product Details
* Product Categories
* Product Search
* Server-Side Pagination
* Product Images

---

### 🛒 Shopping Cart

* Add Products to Cart
* Remove Products from Cart
* Update Product Quantity
* AJAX Cart Operations
* Live Cart Counter
* Redis (Upstash) Integration

---

### 💳 Payments

* Stripe Checkout Integration
* Secure Online Payments
* Payment Confirmation
* Stripe Webhook Integration
* Payment Status Tracking

---

### 📦 Order Management

* Place Orders
* Order History
* Order Tracking
* Order Status Management
* Employee Order Dashboard
* Company User Delayed Payment Support

---

### ✉️ Notifications

* Email Confirmation
* Password Reset Email
* Order Status Email Notifications
* SMS Notifications using Twilio

---

### ⏱️ Background Jobs (Hangfire)

* Automatic cleanup of unpaid orders
* Email processing
* SMS processing
* Hangfire Dashboard for job monitoring

---

### ⚡ Performance

* Redis Distributed Caching
* Server-Side Pagination
* AJAX Requests
* Optimized Entity Framework Core Queries

---

### 🛡️ Security

* ASP.NET Core Identity
* Authentication & Authorization
* Role-Based Access Control
* Password Hashing
* Anti-Forgery Token Protection
* Email Verification

---

## 🏗️ Architecture

The application follows the **3-Tier Architecture**.

```text
Presentation Layer (MVC)
│
├── Controllers
├── Views
├── View Components
└── Extensions

Business Layer (BLL)
│
├── Services
├── Interfaces
├── DTOs
├── ViewModels
├── Mapping
└── Result Pattern

Data Access Layer (DAL)
│
├── DbContext
├── Entities
├── Configurations
├── Repositories
└── Unit of Work
```

---

## 🛠️ Technologies

* ASP.NET Core MVC
* C#
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* Redis (Upstash)
* Stripe
* Twilio
* Hangfire
* AutoMapper
* LINQ
* AJAX
* jQuery
* Bootstrap 5

---

## 💡 Design Patterns & Concepts

* 3-Tier Architecture
* Repository Pattern
* Unit of Work Pattern
* Dependency Injection
* Result Pattern
* Distributed Caching (Redis)
* Background Jobs
* Server-Side Pagination
* Stripe Webhooks

---

## ⚙️ Getting Started

### 1. Clone Repository

```bash
git clone https://github.com/MohamedG-1111/E-commerceMVC.git
```

---

### 2. Navigate to Project

```bash
cd E-commerceMVC
```

---

### 3. Configure SQL Server

Update the connection string inside **appsettings.json**

```json
"ConnectionStrings": {
  "DefaultConnection": "YOUR_CONNECTION_STRING"
}
```

---

### 4. Configure Redis (Upstash)

Add your Redis connection string.

```json
"Redis": {
  "ConnectionString": "YOUR_REDIS_CONNECTION_STRING"
}
```

---

### 5. Configure Stripe

```json
"Stripe": {
  "PublishableKey": "YOUR_PUBLISHABLE_KEY",
  "SecretKey": "YOUR_SECRET_KEY"
}
```

---

### 6. Configure Twilio

```json
"Twilio": {
  "AccountSid": "YOUR_ACCOUNT_SID",
  "AuthToken": "YOUR_AUTH_TOKEN",
  "PhoneNumber": "YOUR_PHONE_NUMBER"
}
```

---

### 7. Configure SMTP

Add your email settings for:

* Email Confirmation
* Password Reset
* Order Notifications

---

### 8. Configure Hangfire

Register Hangfire services and dashboard.

```csharp
builder.Services.AddHangfire(...);

builder.Services.AddHangfireServer();

app.UseHangfireDashboard();
```

---

### 9. Apply Database Migrations

Package Manager Console

```powershell
Update-Database
```

or

```bash
dotnet ef database update
```

---

### 10. Run the Application

```bash
dotnet run
```

---

## 👥 User Roles

| Role         | Permissions                 |
| ------------ | --------------------------- |
| Admin        | Full system management      |
| Employee     | Manage orders               |
| Company User | Delayed payment & discounts |
| Customer     | Shopping & checkout         |

---

## 📌 Main Functionalities

* User Registration
* Email Confirmation
* Login & Logout
* Password Recovery
* Product Catalog
* Product Search
* Categories
* Server-Side Pagination
* Shopping Cart
* Redis Distributed Cache
* Stripe Payments
* Stripe Webhooks
* Order Tracking
* SMS Notifications
* Email Notifications
* Hangfire Background Jobs
* Role-Based Authorization

---

## 🔮 Future Improvements

* Wishlist
* Product Reviews
* Coupons & Promo Codes
* Invoice PDF Generation
* Product Recommendations
* Sales Dashboard
* Multi-language Support
* Dark Mode
* Real-time Notifications

---

## 🌐 Live Demo

https://smartbooks.runasp.net/

---

## 💻 GitHub Repository

https://github.com/MohamedG-1111/E-commerceMVC

---

[## 👨‍💻 Author

**Mohamed Gomaa Ghwail**

GitHub:
https://github.com/MohamedG-1111

LinkedIn:
https:https://www.linkedin.com/in/mohamedgomaa-dev24/

---

## ⭐ Support

If you found this project helpful, please consider giving it a **Star ⭐** on GitHub.
