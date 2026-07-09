# 🛒 E-Commerce MVC

A modern E-Commerce Web Application built with ASP.NET Core MVC following Clean Architecture principles. The project provides a secure, scalable, and user-friendly shopping experience with authentication, authorization, product management, shopping cart, online payments, and order management.

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
- Redis Cache Integration

### 💳 Payment
- Secure online payments using Stripe
- Stripe Checkout Integration
- Payment Success & Cancel Pages
- Secure Payment Processing

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

## 🛠️ Technologies Used
- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Redis Cache
- Stripe Payment Gateway
- Hangfire (Background Jobs)
- AutoMapper
- Bootstrap 5
- jQuery
- AJAX
- LINQ
- Dependency Injection

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

### 4. Configure Redis
Configure your Redis connection in `appsettings.json`.

### 5. Configure Stripe
Add your Stripe API keys in `appsettings.json`.

```json
"Stripe": {
  "PublishableKey": "YOUR_PUBLISHABLE_KEY",
  "SecretKey": "YOUR_SECRET_KEY"
}
```

### 6. Configure Email
Update your SMTP settings in `appsettings.json`.

### 7. Configure Hangfire
Hangfire uses the same SQL Server connection string to store and track jobs. Make sure `UseHangfireDashboard` and `AddHangfireServer` are registered in `Program.cs`, then the recurring job will be scheduled automatically on application startup.

### 8. Apply Database Migrations

Package Manager Console
```powershell
Update-Database
```

or .NET CLI
```bash
dotnet ef database update
```

### 9. Run the Application
```bash
dotnet run
```

## 👥 User Roles

| Role | Permissions |
|------|-------------|
| Admin | Full system access, manage users, products, categories, orders, and roles |
| Employee | Manage customer orders |
| CompanyUser | Receive special discounts during checkout |
| Customer | Browse products, manage cart, place orders, and complete payments |

## 📌 Main Functionalities

- User Registration
- Email Confirmation
- Login & Logout
- Password Recovery
- Product Catalog
- Product Search
- Product Categories
- Shopping Cart
- Redis Cart Storage
- AJAX Cart Updates
- Stripe Online Payments
- Order Management
- Background Jobs (Unpaid Order Cleanup)
- Role-Based Authorization
- User Management

## ✉️ Email Confirmation

New accounts are not fully active until the user confirms their email address.

**Flow:**
1. User registers with email and password through ASP.NET Core Identity.
2. A confirmation token is generated using `UserManager.GenerateEmailConfirmationTokenAsync`.
3. A confirmation link (containing the `userId` and encoded `token`) is emailed to the user via the configured SMTP settings.
4. The user clicks the link, which hits a `ConfirmEmail` action that calls `UserManager.ConfirmEmailAsync`.
5. Until the email is confirmed, login is blocked for that account (`SignInOptions.RequireConfirmedEmail = true`), preventing use of unverified accounts.

The same email pipeline is reused for **Forgot Password / Reset Password**, generating a password reset token and sending it through the same email service.

## 📦 Order Management

Orders move through a set of statuses (e.g. `Pending`, `Confirmed`, `Cancelled`, `Delivered`) alongside a separate `PaymentStatus` (e.g. `Pending`, `Paid`, `Rejected`, `Refunded`).

**Flow:**
1. Customer checks out the cart and an `Order` is created with `OrderStatus = Pending`.
2. Payment is processed through Stripe; on success the `PaymentStatus` is updated to `Paid`, otherwise it stays `Pending`/`Rejected`.
3. Employees/Admins can view and update order status from the management dashboard.
4. Customers can track their order history and current status from their account page.
5. Orders left unpaid for too long are automatically handled by the **Background Job** described below, instead of staying pending indefinitely.

## ⏱️ Background Jobs

Recurring background work is handled with **Hangfire**, registered on startup and stored in SQL Server so scheduled jobs survive app restarts.

### Unpaid Orders Cleanup
A recurring job runs on a monthly schedule and cancels/cleans up orders that have been sitting unpaid for over a month.

```csharp
RecurringJob.AddOrUpdate<IOrderCleanupService>(
    "cleanup-unpaid-orders",
    service => service.CleanUpUnpaidOrdersOlderThanOneMonthAsync(CancellationToken.None),
    Cron.Monthly()
);
```

```csharp
private const int UnpaidOrderExpirationInDays = 30;

public async Task CleanUpUnpaidOrdersOlderThanOneMonthAsync(CancellationToken cancellationToken)
{
    var oneMonthAgo = DateTime.UtcNow.AddDays(-UnpaidOrderExpirationInDays);

    var unpaidOrders = await unitOfWork.Repository<Order>().GetAsQuery()
        .Where(x =>
            (x.OrderStatus == OrderStatus.Pending || x.OrderStatus == OrderStatus.Confirmed) &&
            (x.PaymentStatus == PaymentStatus.Pending ||
             x.PaymentStatus == PaymentStatus.Rejected ||
             x.PaymentStatus == PaymentStatus.Refunded) &&
            x.OrderDate < oneMonthAgo)
        .ToListAsync(cancellationToken);

    foreach (var order in unpaidOrders)
    {
        order.OrderStatus = OrderStatus.Cancelled;
    }

    await unitOfWork.SaveChangesAsync(cancellationToken);
}
```

Job execution and history (successes, failures, retries) can be monitored through the **Hangfire Dashboard**, exposed at `/hangfire`.

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

## 👨‍💻 Author
**Mohamed Gomaa Ghwail**
GitHub: https://github.com/MohamedG-1111

## ⭐ Support
If you found this project helpful, please consider giving it a Star ⭐ on GitHub. Your support is greatly appreciated!
