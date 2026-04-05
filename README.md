# 🍃 FoodRush — Food Ordering System

A full-stack web application for online food ordering with three user roles — Customer, Admin, and Delivery Boy. Built with ASP.NET MVC and SQL Server.

---

## 🚀 Features

### 👤 Customer
- Register and login securely
- Browse food menu with categories and ratings
- Add items to cart, update quantity, remove items
- Enter delivery address and phone number at checkout
- Real-time countdown timer for delivery tracking
- OTP-based delivery verification
- Rate food quality and delivery experience separately
- View order history and submitted reviews

### 🛠️ Admin
- Secure admin dashboard with revenue and order analytics
- Revenue bar chart for last 7 days
- Top 5 food items by order count
- Add, edit, and soft-delete food items
- Manage delivery partners — add, activate, deactivate
- View delivery partner activity and customer reviews
- Assign orders to delivery boys with estimated time
- View full customer details — address, phone, OTP
- View all food ratings and reviews per item

### 🛵 Delivery Boy
- Login with separate credentials
- View assigned orders with customer address and phone
- Countdown timer showing time remaining for delivery
- OTP verification to mark order as delivered
- View delivery history
- View own customer reviews and ratings

---

## 🏗️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | ASP.NET MVC Razor Views, Bootstrap 5, Bootstrap Icons |
| Backend | C# ASP.NET MVC (.NET Framework 4.8) |
| Database | Microsoft SQL Server |
| Data Access | ADO.NET (SqlConnection, SqlCommand, SqlDataReader) |
| Architecture | MVC + DAL (Data Access Layer) pattern |
| IDE | Visual Studio 2022 |

---

## 📁 Project Structure

```
FoodOrderingSystem/
│
├── Controllers/
│   ├── AccountController.cs      # Login, Register, Logout
│   ├── AdminController.cs        # Dashboard, Food, Delivery management
│   ├── CartController.cs         # Cart, Orders, Ratings, Reviews
│   ├── DeliveryController.cs     # Deliveries, OTP, History
│   ├── FoodController.cs         # Menu, Food detail
│   └── HomeController.cs         # Landing page
│
├── Models/
│   ├── User.cs                   # User data model
│   ├── Order.cs                  # Order with address, OTP, status
│   ├── OrderDetail.cs            # Individual items in an order
│   ├── FoodItem.cs               # Food menu item
│   ├── DeliveryBoy.cs            # Delivery partner model
│   ├── Rating.cs                 # Food and delivery ratings
│   └── CartItem.cs               # Temporary cart item
│
├── DAL/
│   ├── DatabaseHelper.cs         # SQL Server connection manager
│   ├── UserDAL.cs                # User login, register queries
│   ├── FoodDAL.cs                # Food CRUD queries
│   ├── OrderDAL.cs               # Order, OTP, dashboard queries
│   ├── DeliveryDAL.cs            # Delivery boy queries
│   └── RatingDAL.cs              # Rating and review queries
│
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml        # Master layout with role-based navbar
│   ├── Account/                  # Login, Register pages
│   ├── Admin/                    # Dashboard, ManageFood, ManageDelivery etc
│   ├── Cart/                     # Cart, OrderHistory, OrderDetail, MyReviews
│   ├── Delivery/                 # MyDeliveries, VerifyOTP, History, Reviews
│   └── Food/                     # Menu, Detail pages
│
├── App_Start/
│   ├── RouteConfig.cs            # URL routing rules
│   ├── BundleConfig.cs           # CSS/JS bundling
│   └── FilterConfig.cs           # Global error handling
│
├── Content/
│   └── Images/                   # Food item images
│
└── Web.config                    # DB connection string, app settings
```

---

## 🗄️ Database Schema

### Tables

**Users**
| Column | Type | Description |
|--------|------|-------------|
| UserId | INT PK | Auto generated ID |
| Name | NVARCHAR | Full name |
| Email | NVARCHAR | Login email |
| Password | NVARCHAR | Login password |
| Phone | NVARCHAR | Contact number |
| Role | NVARCHAR | User / Admin |
| CreatedOn | DATETIME | Registration date |

**Orders**
| Column | Type | Description |
|--------|------|-------------|
| OrderId | INT PK | Auto generated ID |
| UserId | INT FK | Links to Users |
| OrderCode | NVARCHAR | Unique code like ORD-2026-1234 |
| OrderDate | DATETIME | When order was placed |
| TotalAmount | DECIMAL | Order total |
| Status | NVARCHAR | Pending / OutForDelivery / Delivered |
| OTP | NVARCHAR | 4 digit delivery OTP |
| OTPAttempts | INT | Wrong OTP count — locks at 3 |
| DeliveryBoyId | INT FK | Links to DeliveryBoys |
| DeliveryTime | INT | Estimated minutes |
| DeliveryStartTime | DATETIME | When delivery started |
| CustomerPhone | NVARCHAR | Phone entered at checkout |
| DeliveryAddress | NVARCHAR | Address entered at checkout |

**OrderDetails**
| Column | Type | Description |
|--------|------|-------------|
| OrderDetailId | INT PK | Auto generated ID |
| OrderId | INT FK | Links to Orders |
| FoodId | INT FK | Links to FoodItems |
| Quantity | INT | How many ordered |
| Price | DECIMAL | Price at time of order |

**FoodItems**
| Column | Type | Description |
|--------|------|-------------|
| FoodId | INT PK | Auto generated ID |
| FoodName | NVARCHAR | Name of food |
| Description | NVARCHAR | Short description |
| Price | DECIMAL | Price in rupees |
| Category | NVARCHAR | Snacks, Drinks etc |
| ImagePath | NVARCHAR | Image filename |
| IsAvailable | BIT | 1=visible, 0=hidden (soft delete) |
| AverageRating | FLOAT | Calculated average rating |

**DeliveryBoys**
| Column | Type | Description |
|--------|------|-------------|
| DeliveryBoyId | INT PK | Auto generated ID |
| Name | NVARCHAR | Full name |
| Email | NVARCHAR | Login email |
| Password | NVARCHAR | Login password |
| Phone | NVARCHAR | Contact number |
| IsAvailable | BIT | Active or inactive |
| TotalDeliveries | INT | Completed deliveries count |
| JoinedOn | DATETIME | When added by admin |

**Ratings**
| Column | Type | Description |
|--------|------|-------------|
| RatingId | INT PK | Auto generated ID |
| OrderId | INT FK | Links to Orders |
| UserId | INT FK | Links to Users |
| DeliveryBoyId | INT FK | Links to DeliveryBoys (nullable) |
| Stars | INT | 1 to 5 |
| Comment | NVARCHAR | Written review |
| RatingType | NVARCHAR | Food or Delivery |
| RatedOn | DATETIME | When rated |

---

## ⚙️ How to Run Locally

### Prerequisites
- Visual Studio 2022
- SQL Server (LocalDB or full)
- .NET Framework 4.8

### Steps

**1. Clone the repository**
```bash
git clone https://github.com/EDITH96929/FoodOrderingSystem.git
```

**2. Create the database**

Open SQL Server Management Studio and run:

```sql
CREATE DATABASE FoodOrderingDB;
USE FoodOrderingDB;

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    Password NVARCHAR(100),
    Phone NVARCHAR(20),
    Role NVARCHAR(20) DEFAULT 'User',
    CreatedOn DATETIME DEFAULT GETDATE()
);

CREATE TABLE FoodItems (
    FoodId INT PRIMARY KEY IDENTITY,
    FoodName NVARCHAR(100),
    Description NVARCHAR(500),
    Price DECIMAL(10,2),
    Category NVARCHAR(50),
    ImagePath NVARCHAR(200),
    IsAvailable BIT DEFAULT 1,
    AverageRating FLOAT DEFAULT 0
);

CREATE TABLE DeliveryBoys (
    DeliveryBoyId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    Password NVARCHAR(100),
    Phone NVARCHAR(20),
    IsAvailable BIT DEFAULT 1,
    TotalDeliveries INT DEFAULT 0,
    JoinedOn DATETIME DEFAULT GETDATE()
);

CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    OrderCode NVARCHAR(30),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(10,2),
    Status NVARCHAR(30) DEFAULT 'Pending',
    OTP NVARCHAR(10),
    OTPAttempts INT DEFAULT 0,
    DeliveryBoyId INT FOREIGN KEY REFERENCES DeliveryBoys(DeliveryBoyId) NULL,
    DeliveryTime INT NULL,
    DeliveryStartTime DATETIME NULL,
    CustomerPhone NVARCHAR(20) NULL,
    DeliveryAddress NVARCHAR(500) NULL
);

CREATE TABLE OrderDetails (
    OrderDetailId INT PRIMARY KEY IDENTITY,
    OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
    FoodId INT FOREIGN KEY REFERENCES FoodItems(FoodId),
    Quantity INT,
    Price DECIMAL(10,2)
);

CREATE TABLE Ratings (
    RatingId INT PRIMARY KEY IDENTITY,
    OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    DeliveryBoyId INT NULL,
    Stars INT,
    Comment NVARCHAR(500),
    RatingType NVARCHAR(20) DEFAULT 'Food',
    RatedOn DATETIME DEFAULT GETDATE()
);

-- Insert default admin account
INSERT INTO Users (Name, Email, Password, Role)
VALUES ('Admin', 'admin@foodrush.com', 'admin123', 'Admin');
```

**3. Update connection string in Web.config**
```xml
<connectionStrings>
  <add name="FoodDB"
       connectionString="Server=.;Database=FoodOrderingDB;Integrated Security=True;"
       providerName="System.Data.SqlClient"/>
</connectionStrings>
```

**4. Run the project**
- Open `FoodOrderingSystem.sln` in Visual Studio
- Press `F5` or click the green Run button
- Browser opens automatically

### Default Login Credentials
| Role | Email | Password |
|------|-------|----------|
| Admin | admin@foodrush.com | admin123 |
| User | Register from homepage | — |
| Delivery | Added by admin panel | — |

---

## 🔄 How the App Works

```
User visits site
      ↓
Login (AccountController checks Users table)
      ↓
Session stores UserId, UserName, UserRole
      ↓
Role-based redirect:
  User      → Food Menu
  Admin     → Dashboard  
  Delivery  → My Deliveries
```

**Order Flow:**
```
User adds food to cart (stored in Session)
      ↓
Enters phone + address at checkout
      ↓
Order saved to Orders table with OTP generated
      ↓
Admin sees order on Dashboard → assigns delivery boy
      ↓
Delivery boy sees order with customer address
      ↓
Customer sees countdown timer + OTP
      ↓
Delivery boy arrives → customer shares OTP
      ↓
Delivery boy enters OTP → order marked Delivered
      ↓
Customer can now rate food and delivery
```

---

## 🏛️ Architecture

This project follows the **MVC + DAL** pattern:

- **Model** — defines data structure (what fields exist)
- **View** — Razor HTML pages shown to user (.cshtml)
- **Controller** — handles requests, calls DAL, returns View
- **DAL** — all SQL queries in one place, separated from business logic

This separation means database code is never mixed with UI code. If the database changes, only DAL files need updating.

---

## 🔐 Security Features

- Role-based access — every controller method checks session role
- SQL injection prevention — all queries use parameterized SqlCommand
- OTP locking — wrong OTP locks after 3 attempts
- Soft delete — food items hidden not deleted to protect order history
- Address privacy — delivery address only visible to assigned delivery boy

---

## 👨‍💻 Built By


| Name | Role |
|------|------|
| Sunil Kumar Swain | Full Stack Development |

---

## 📄 License

This project is built for educational purposes as part of college curriculum.
