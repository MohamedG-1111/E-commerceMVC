# 🛒 E-Commerce MVC

A modern **E-Commerce Web Application** built with **ASP.NET Core MVC** following **Clean Architecture** principles. The project provides a secure, scalable, and user-friendly shopping experience with authentication, authorization, product management, shopping cart, online payments, and order management.

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

  * **Admin** – Full access to manage the entire system.
  * **Employee** – Manage customer orders.
  * **CompanyUser** – Receive exclusive discounts on products.
  * **Customer** – Browse products, manage cart, and place orders.

---

### 🛍️ Product Management

* Browse Products
* Product Details
* Product Categories
* Product Search
* Product Filtering
* Product Images

---

### 🛒 Shopping Cart

* Add Products to Cart
* Remove Products from Cart
* Increase / Decrease Quantity
* AJAX Cart Updates
* Live Cart Counter
* Redis Cache Integration

---

### 💳 Payment

* Secure online payments using **Stripe**
* Stripe Checkout Integration
* Payment Success & Cancel Pages
* Secure Payment Processing

---

### 📦 Order Management

* Place Orders
* View Order History
* Order Status Tracking
* Employee Order Management

---

### 🛡️ Administration

* Manage Products
* Manage Categories
* Manage Orders
* Manage Users
* Manage Roles
* Assign Roles to Users

---

### 💻 User Experience

* Responsive Design
* Bootstrap 5
* Partial Views
* AJAX Operations
* Server-side & Client-side Validation
* Clean and Modern UI

---

### 🔐 Security

* ASP.NET Core Identity
* Authentication & Authorization
* Password Hashing
* Email Verification
* Anti-Forgery Token Protection
* Role-Based Access Control

---

# 🏗️ Architecture

The project follows the **Clean Architecture** pattern to separate concerns and improve maintainability.

```text
Presentation Layer (MVC)
│
├── Controllers
├── Views
├── ViewComponents
└── Extensions
│
Business Layer (BLL)
│
├── Services
├── Interfaces
├── DTOs
├── ViewModels
└── Mapping
│
Data Access Layer (DAL)
│
├── DbContext
├── Entities
├── Configurations
├── Repositories
└── Migrations
```

---

# 🛠️ Technologies Used

* ASP.NET Core MVC
* C#
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* Redis Cache
* Stripe Payment Gateway
* AutoMapper
* Bootstrap 5
* jQuery
* AJAX
* LINQ
* Dependency Injection

---

# 📂 Project Structure

```text
E-CommerceMVC
│
├── BLL
│   ├── Services
│   ├── Interfaces
│   ├── DTOs
│   ├── ViewModels
│   └── Mapping
│
├── DAL
│   ├── Data
│   ├── Entities
│   ├── Repositories
│   ├── Configurations
│   └── Migrations
│
├── Controllers
├── Extensions
├── Views
├── wwwroot
├── appsettings.json
└── Program.cs
```

---

# ⚙️ Getting Started

## 1. Clone the Repository

```bash
git clone https://github.com/MohamedG-1111/E-commerceMVC.git
```

---

## 2. Navigate to the Project

```bash
cd E-commerceMVC
```

---

## 3. Configure SQL Server

Update the connection string in **appsettings.json**.

```json
"ConnectionStrings": {
  "DefaultConnection": "Your SQL Server Connection String"
}
```

---

## 4. Configure Redis

Configure your Redis connection in **appsettings.json**.

---

## 5. Configure Stripe

Add your Stripe API keys in **appsettings.json**.

```json
"Stripe": {
  "PublishableKey": "YOUR_PUBLISHABLE_KEY",
  "SecretKey": "YOUR_SECRET_KEY"
}
```

---

## 6. Configure Email

Update your SMTP settings in **appsettings.json**.

---

## 7. Apply Database Migrations

Package Manager Console

```powershell
Update-Database
```

or .NET CLI

```bash
dotnet ef database update
```

---

## 8. Run the Application

```bash
dotnet run
```

---

# 👥 User Roles

| Role            | Permissions                                                               |
| --------------- | ------------------------------------------------------------------------- |
| **Admin**       | Full system access, manage users, products, categories, orders, and roles |
| **Employee**    | Manage customer orders                                                    |
| **CompanyUser** | Receive special discounts during checkout                                 |
| **Customer**    | Browse products, manage cart, place orders, and complete payments         |

---

# 📌 Main Functionalities

* User Registration
* Email Confirmation
* Login & Logout
* Password Recovery
* Product Catalog
* Product Search
* Product Categories
* Shopping Cart
* Redis Cart Storage
* AJAX Cart Updates
* Stripe Online Payments
* Order Management
* Role-Based Authorization
* User Management

---

# 🔮 Future Improvements

* Wishlist
* Product Reviews & Ratings
* Coupons & Promo Codes
* Invoice Generation (PDF)
* Sales Dashboard
* Product Recommendations
* Multi-language Support
* Dark Mode
* Real-time Notifications

---

# 👨‍💻 Author

**Mohamed Gomaa Ghwail**

GitHub:
**https://github.com/MohamedG-1111**

---

# ⭐ Support

If you found this project helpful, please consider giving it a **Star ⭐** on GitHub. Your support is greatly appreciated!
