# Tech Solution (TS) - E-Commerce Platform

A full-featured e-commerce web application built with ASP.NET Core 8 and Entity Framework Core. This platform provides a complete shopping experience with product management, shopping carts, order processing, and secure payment integration via Stripe.

## Overview

Tech Solution is a multi-tenant e-commerce platform designed for managing product catalogs, user orders, and payments. It supports multiple user roles including customers, employees, admins, and companies, with role-based access control throughout the application.

## Features

### User Management
- **ASP.NET Identity Integration**: Secure user authentication and authorization
- **Role-Based Access Control**: Admin, Employee, Customer, and Company roles
- **User Registration & Login**: Customizable identity pages with security features
- **Account Management**: Profile management and password recovery

### Product Management
- **Product Catalog**: Browse and search products with detailed information
- **Product Details**: ISBN, Author, Title, Description, and pricing information
- **Dynamic Pricing**: Support for list prices, discounts, and category pricing
- **Image Management**: Product image storage and management

### Shopping & Orders
- **Shopping Cart**: Add/remove products, manage quantities with range validation (1-1000 items)
- **Order Management**: Complete order lifecycle tracking
- **Order Status Tracking**: Pending, Approved, Processing, Shipped, Cancelled, Refunded
- **Order Details**: Individual line items with pricing calculations

### Payment Processing
- **Stripe Integration**: Secure payment processing
- **Payment Status Management**: Pending, Approved, Rejected, and Delayed Payment states
- **Payment History**: Track payment transactions and status

### Admin Features
- **Order Management**: View and manage all orders
- **Product Administration**: Create, edit, and delete products
- **User Management**: Manage user accounts and roles
- **Order Status Updates**: Change order and payment statuses
- **Data Management**: Seed and manage company and category data

### Company Features
- **Multi-Company Support**: Support for multiple companies or tenants
- **Company Profiles**: Company name and identification
- **Company-Specific Orders**: Orders associated with company users

### Categories
- **Product Categories**: Organize products by categories
- **Category Management**: Admin controls for category maintenance
- **Pre-Seeded Data**: Initial category and company data

## Technology Stack

### Core Framework
- **ASP.NET Core 8.0**: Modern web framework for building web applications
- **Entity Framework Core 9.0**: ORM for database operations and migrations
- **C# 12**: Latest C# language features

### Database
- **Entity Framework Core**: Code-first database migrations
- **In-Memory Database**: Default development database (easily configurable)
- **SQL Server Support**: Ready for production SQL Server deployment
- **SQLite Support**: Lightweight database option

### Authentication & Security
- **ASP.NET Core Identity**: User authentication and authorization
- **Identity UI**: Built-in identity pages for login, registration, account management
- **Role-Based Authorization**: Custom role configuration

### Payment Integration
- **Stripe.NET 48.3.0**: Stripe payment processing library

### Additional Tools
- **Razor Pages**: For identity management pages
- **Razor Views**: For dynamic UI rendering
- **Code Generation**: Visual Studio scaffolding support

## Project Structure

```
TS.sln                          # Solution file
├── TSWeb/                      # Main web application
│   ├── Program.cs              # Application startup configuration
│   ├── appsettings.json        # Configuration settings
│   ├── TS.csproj              # Project file
│   ├── Areas/
│   │   ├── Admin/             # Admin functionality
│   │   ├── Customer/          # Customer functionality
│   │   └── Identity/          # Authentication pages
│   ├── Views/                 # Razor views
│   └── wwwroot/               # Static files (CSS, JS, images)
│
├── TS.Models/                 # Entity models and ViewModels
│   ├── Product.cs             # Product entity
│   ├── Category.cs            # Product categories
│   ├── ShoppingCart.cs        # Shopping cart items
│   ├── OrderHeader.cs         # Order headers
│   ├── OrderDetail.cs         # Order line items
│   ├── OrderItem.cs           # Order item representation
│   ├── Company.cs             # Company/tenant entity
│   ├── ApplicationUser.cs      # Extended identity user
│   ├── ErrorViewModel.cs      # Error view model
│   ├── ViewModels/            # View-specific models
│   │   ├── ProductVM.cs
│   │   ├── ShoppingCartVM.cs
│   │   └── OrderDetailVM.cs
│   └── TSModels.csproj        # Models project file
│
├── TS.DataAccess/             # Data access layer
│   ├── Data/
│   │   └── ApplicationDbContext.cs  # Entity Framework context
│   ├── Migrations/            # Database migrations
│   ├── Repository/            # Repository implementations
│   │   ├── Repository.cs      # Generic repository
│   │   ├── CategoryRepository.cs
│   │   ├── ProductRepository.cs
│   │   ├── OrderHeaderRepository.cs
│   │   ├── OrderDetailRepository.cs
│   │   ├── ShoppingCartRepository.cs
│   │   ├── ApplicationUserRepository.cs
│   │   ├── CompanyRepository.cs
│   │   ├── UnitOfWork.cs      # Unit of Work pattern
│   │   └── IRepository/       # Repository interfaces
│   └── TS.DataAccess.csproj   # Data access project file
│
└── TS.Utility/                # Utility classes and helpers
    ├── SD.cs                  # Static data (roles, constants)
    ├── StripeSettings.cs      # Stripe configuration
    ├── EmailSender.cs         # Email sending functionality
    └── TS.Utility.csproj      # Utility project file
```

## Architecture

The application follows a **layered architecture** with separation of concerns:

1. **Presentation Layer** (TSWeb): ASP.NET Core MVC/Razor Pages
2. **Models Layer** (TS.Models): Entity models and ViewModels
3. **Data Access Layer** (TS.DataAccess): Repository pattern with Unit of Work
4. **Utility Layer** (TS.Utility): Cross-cutting concerns and helpers

### Design Patterns Used

- **Repository Pattern**: Abstracts data access logic
- **Unit of Work Pattern**: Manages transactions and repository coordination
- **Dependency Injection**: Loose coupling and testability
- **Entity Framework Core**: ORM for data persistence

## Getting Started

### Prerequisites

- **.NET 8 SDK** or later
- **Visual Studio 2022** (Community, Professional, or Enterprise) or **Visual Studio Code**
- **Git** for version control

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd .NET-Website
   ```

2. **Open the solution**
   ```bash
   # Using Visual Studio
   start TS.sln
   
   # Or using Visual Studio Code
   code .
   ```

3. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

4. **Build the solution**
   ```bash
   dotnet build
   ```

## Configuration

### appsettings.json

The application uses `appsettings.json` for configuration. Key settings include:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "Stripe": {
    "SecretKey": "your-stripe-secret-key",
    "PublishableKey": "your-stripe-publishable-key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Stripe Configuration

1. **Get your Stripe keys** from [https://dashboard.stripe.com/](https://dashboard.stripe.com/)
2. **Update `appsettings.json`** or `appsettings.Development.json` with:
   - `Stripe:SecretKey`: Your Stripe secret key
   - `Stripe:PublishableKey`: Your Stripe publishable key
3. The application will automatically configure Stripe settings on startup

### Database Configuration

Currently configured to use an **In-Memory Database**. To switch to SQL Server or SQLite:

Edit `Program.cs` and modify the DbContext configuration:

```csharp
// For SQL Server:
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))

// For SQLite:
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
```

Then update the connection string in `appsettings.json`.

## Running the Application

### Development Mode

```bash
cd TSWeb
dotnet run
```

The application will typically start at `https://localhost:7000` or `https://localhost:5000`

### Visual Studio

1. Set **TSWeb** as the startup project
2. Press **F5** or click **Start Debugging**

### Creating & Applying Migrations

If you change the database schema:

```bash
# From the TS.DataAccess directory
dotnet ef migrations add MigrationName -p TS.DataAccess -s ../TSWeb

# Apply migrations
dotnet ef database update -p TS.DataAccess -s ../TSWeb
```

## User Roles & Permissions

### Admin
- Full access to product management
- Order management and status updates
- User and company management
- Payment status updates

### Employee
- Read-only access to orders
- Limited product viewing

### Customer
- Browse products
- Manage shopping cart
- Place and track orders
- Update profile

### Company
- Company-specific operations
- Company user management (depending on configuration)

## Key Features Explained

### Shopping Cart System
The shopping cart stores product selections with quantities. Users can:
- Add products with quantity selection (1-1000 items)
- View cart contents with calculated prices
- Modify quantities or remove items
- Proceed to checkout

### Order Processing
Orders flow through multiple states:
1. **Pending**: Initial order creation
2. **Approved**: Order approved for processing
3. **Processing**: Order being prepared
4. **Shipped**: Order sent to customer
5. **Cancelled**: Order cancelled by user/admin
6. **Refunded**: Refund processed

### Payment System
Payments are integrated with Stripe and track their own status:
- **Pending**: Payment awaiting processing
- **Approved**: Payment successfully processed
- **Rejected**: Payment failed
- **ApprovedForDelayedPayment**: Payment approved but processing delayed

## Database Models

### Core Entities

- **Product**: Product information (title, ISBN, author, pricing)
- **Category**: Product categorization
- **ShoppingCart**: User shopping cart items
- **OrderHeader**: Order summary information
- **OrderDetail**: Individual order line items
- **ApplicationUser**: Extended ASP.NET Identity user
- **Company**: Company/tenant information

Each entity includes appropriate relationships, validations, and constraints.

## Email Functionality

The application includes an `EmailSender` utility for sending emails. Configure your email settings in the configuration to enable:
- Order confirmations
- Password reset emails
- Account verification emails

## API Patterns

The application uses standard RESTful controller patterns with area-based organization:
- `/Admin/*`: Administrative endpoints
- `/Customer/*`: Customer-facing endpoints
- `/Identity/*`: Authentication endpoints

## Logging

Logging is configured in `appsettings.json`. By default, logs are set to "Information" level. Configure as needed for development or production environments.

## Security Considerations

1. **HTTPS**: Always use HTTPS in production
2. **Stripe Keys**: Keep secret keys in environment variables or Azure Key Vault, never commit to source control
3. **Authentication**: All protected routes require authentication
4. **Authorization**: Role-based and policy-based authorization throughout
5. **CORS**: Configure as needed for your production environment

## Development Workflow

1. Create a feature branch
2. Make changes in isolated feature areas
3. Add database migrations if schema changes
4. Test thoroughly
5. Submit pull request with documentation

## Troubleshooting

### Database Issues
- Clear the in-memory database by restarting the application
- For persistent databases, use migrations to update schema

### Stripe Integration
- Verify API keys are correct in configuration
- Check Stripe dashboard for API errors
- Use Stripe test keys during development

### Authentication Issues
- Ensure Identity pages are properly scaffolded
- Check cookie authentication configuration in Program.cs
- Verify user roles are properly assigned

## Future Enhancements

Potential improvements for the application:
- Product reviews and ratings
- Wishlist functionality
- Advanced search and filtering
- Email notifications
- Inventory management
- Admin dashboard with analytics
- API endpoints for mobile apps
- Real-time order tracking

## License

[Specify your license here - e.g., MIT, Apache 2.0, etc.]

## Contributing

[Add contribution guidelines as needed]

## Contact

[Add contact information as needed]

---

**Note**: This is a portfolio project demonstrating full-stack .NET development with modern ASP.NET Core practices, Entity Framework Core, authentication, and payment integration.
