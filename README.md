# 🛒 E-Commerce MVC

A modern E-Commerce Web Application built with **ASP.NET Core MVC** following **Clean Architecture** principles. The project provides a secure, scalable, and user-friendly shopping experience with authentication, authorization, product management, shopping cart, online payments, SMS notifications, and order management.

🔗 **Live Demo:** [https://smartbooks.runasp.net/](https://smartbooks.runasp.net/)

---

## 🚀 Features

### 👤 Authentication & Authorization
- ASP.NET Core Identity
- User Registration
- Login & Logout
- Email Confirmation
- Forgot Password
- Reset Password
- Role-Based Authorization:
  - **Admin** – Full access to manage the entire system.
  - **Employee** – Manage customer orders.
  - **CompanyUser** – Receive exclusive discounts on products.
  - **Customer** – Browse products, manage cart, and place orders.

### 🛍️ Product Management
- Browse Products
- Product Details
- Product Categories
- Product Search
- Product Filtering
- Product Images

### 🛒 Shopping Cart
- Add Products to Cart
- Remove Products from Cart
- Increase / Decrease Quantity
- AJAX Cart Updates
- Live Cart Counter
- **Redis Cache Integration** (hosted on [Upstash](https://upstash.com))

### 💳 Payment
- Secure online payments using Stripe
- Stripe Checkout Integration
- Payment Success & Cancel Pages
- Secure Payment Processing

### 📩 Notifications (SMS)
- SMS notifications powered by **Twilio**
- Order confirmation and status update alerts sent directly to the customer's phone

### 📦 Order Management
- Place Orders
- View Order History
- Order Status Tracking
- Employee Order Management
- Automatic Cancellation of Unpaid Orders (via Background Job)

### ⏱️ Background Jobs
- Scheduled recurring jobs using **Hangfire**
- Automatic cleanup of unpaid/pending orders older than one month
- Job monitoring via Hangfire Dashboard

### 🛡️ Administration
- Manage Products
- Manage Categories
- Manage Orders
- Manage Users
- Manage Roles
- Assign Roles to Users

### 💻 User Experience
- Responsive Design
- Bootstrap 5
- Partial Views
- AJAX Operations
- Server-side & Client-side Validation
- Clean and Modern UI

### 🔐 Security
- ASP.NET Core Identity
- Authentication & Authorization
- Password Hashing
- Email Verification
- Anti-Forgery Token Protection
- Role-Based Access Control

---

## 🏗️ Architecture

The project follows the Clean Architecture pattern to separate concerns and improve maintainability.

```
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

## 🛠️ Technologies Used
- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Redis Cache (hosted on Upstash)
- Stripe Payment Gateway
- Twilio (SMS Notifications)
- Hangfire (Background Jobs)
- AutoMapper
- Bootstrap 5
- jQuery
- AJAX
- LINQ
- Dependency Injection

---

## 📂 Project Structure

```
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

## ⚙️ Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/MohamedG-1111/E-commerceMVC.git
```

### 2. Navigate to the Project
```bash
cd E-commerceMVC
```

### 3. Configure SQL Server
Update the connection string in `appsettings.json`.

```json
"ConnectionStrings": {
  "DefaultConnection": "Your SQL Server Connection String"
}
```

### 4. Configure Redis (Upstash)
This project uses [Upstash](https://upstash.com) as a managed Redis provider. Create a free Redis database on Upstash, then add its connection string to `appsettings.json`:

```json
"Redis": {
  "ConnectionString": "YOUR_UPSTASH_REDIS_CONNECTION_STRING"
}
```

### 5. Configure Stripe
Add your Stripe API keys in `appsettings.json`.

```json
"Stripe": {
  "PublishableKey": "YOUR_PUBLISHABLE_KEY",
  "SecretKey": "YOUR_SECRET_KEY"
}
```

### 6. Configure Twilio
Add your Twilio credentials in `appsettings.json` to enable SMS notifications.

```json
"Twilio": {
  "AccountSid": "YOUR_TWILIO_ACCOUNT_SID",
  "AuthToken": "YOUR_TWILIO_AUTH_TOKEN",
  "FromPhoneNumber": "YOUR_TWILIO_PHONE_NUMBER"
}
```

### 7. Configure Email
Update your SMTP settings in `appsettings.json`.

### 8. Configure Hangfire
Hangfire uses the same SQL Server connection string to store and track jobs. Make sure `UseHangfireDashboard` and `AddHangfireServer` are registered in `Program.cs`, then the recurring job will be scheduled automatically on application startup.

### 9. Apply Database Migrations

Package Manager Console
```powershell
Update-Database
```

or .NET CLI
```bash
dotnet ef database update
```

### 10. Run the Application
```bash
dotnet run
```

---

## 👥 User Roles

| Role | Permissions |
|------|-------------|
| Admin | Full system access, manage users, products, categories, orders, and roles |
| Employee | Manage customer orders |
| CompanyUser | Receive special discounts during checkout |
| Customer | Browse products, manage cart, place orders, and complete payments |

---

## 📌 Main Functionalities

- User Registration
- Email Confirmation
- Login & Logout
- Password Recovery
- Product Catalog
- Product Search
- Product Categories
- Shopping Cart
- Redis Cart Storage (Upstash)
- AJAX Cart Updates
- Stripe Online Payments
- Twilio SMS Notifications
- Order Management
- Background Jobs (Unpaid Order Cleanup)
- Role-Based Authorization
- User Management

---

## ✉️ Email Confirmation

New accounts are not fully active until the user confirms their email address.

**Flow:**
1. User registers with email and password through ASP.NET Core Identity.
2. A confirmation token is generated using `UserManager.GenerateEmailConfirmationTokenAsync`.
3. A confirmation link (containing the `userId` and encoded `token`) is emailed to the user via the configured SMTP settings.
4. The user clicks the link, which hits a `ConfirmEmail` action that calls `UserManager.ConfirmEmailAsync`.
5. Until the email is confirmed, login is blocked for that account (`SignInOptions.RequireConfirmedEmail = true`), preventing use of unverified accounts.

The same email pipeline is reused for **Forgot Password / Reset Password**, generating a password reset token and sending it through the same email service.

---

## 📩 SMS Notifications (Twilio)

Alongside email, the system sends **SMS notifications** to customers using **Twilio** to keep them updated in real time.

**Typical triggers:**
- Order placed / confirmed
- Payment received
- Order status changes (e.g. shipped, delivered, cancelled)

The Twilio client is registered as a service and injected wherever a notification needs to be sent, keeping the messaging logic isolated from the rest of the business layer.

---

## 📦 Order Management

Orders move through a set of statuses (e.g. `Pending`, `Confirmed`, `Cancelled`, `Delivered`) alongside a separate `PaymentStatus` (e.g. `Pending`, `Paid`, `Rejected`, `Refunded`).

**Flow:**
1. Customer checks out the cart and an `Order` is created with `OrderStatus = Pending`.
2. Payment is processed through Stripe; on success the `PaymentStatus` is updated to `Paid`, otherwise it stays `Pending`/`Rejected`.
3. A confirmation SMS/email is sent to the customer via Twilio/SMTP.
4. Employees/Admins can view and update order status from the management dashboard.
5. Customers can track their order history and current status from their account page.
6. Orders left unpaid for too long are automatically handled by the **Background Job** described below, instead of staying pending indefinitely.

---

## ⏱️ Background Jobs

Recurring background work is handled with **Hangfire**, registered on startup and stored in SQL Server so scheduled jobs survive app restarts.

### Unpaid Orders Cleanup
A recurring job runs on a monthly schedule and automatically cancels any order that has stayed unpaid for more than 30 days — checking both the order status (`Pending`/`Confirmed`) and the payment status (`Pending`/`Rejected`/`Refunded`), so orders don't sit in limbo indefinitely.

Job execution and history (successes, failures, retries) can be monitored through the **Hangfire Dashboard**, exposed at `/hangfire`.

---

## 🔮 Future Improvements
- Wishlist
- Product Reviews & Ratings
- Coupons & Promo Codes
- Invoice Generation (PDF)
- Sales Dashboard
- Product Recommendations
- Multi-language Support
- Dark Mode
- Real-time Notifications

---

## 👨‍💻 Author
**Mohamed Gomaa Ghwail**
GitHub: [https://github.com/MohamedG-1111](https://github.com/MohamedG-1111)

## ⭐ Support
If you found this project helpful, please consider giving it a Star ⭐ on GitHub. Your support is greatly appreciated!
