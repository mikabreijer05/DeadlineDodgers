# MatrixCourier / TryoutDatabase
Version: 1.0
Last Updated: YYYY-MM-DD

---

# Database Overview

Database Name:
TryoutDatabase

Technology:
SQL Server

Data Access:
Dapper

Architecture:
.NET MAUI Frontend
Service Layer
Dapper Repository Layer
SQL Server Database

---

# Naming Convention

The database uses SQL table names.

Examples:

Database Table      → Model Class
----------------------------------------
Product             → Product
Category            → Category
Distributor         → Distributor
DeliveryCompany     → DeliveryCompany
Order               → Order
Status              → Status
Account             → Account
Address             → Address
Review              → Review

Avoid creating MAUI-specific model names when a matching
database table already exists.

Example:

❌ DeliveryOrder
✔ Order

❌ DeliveryStatus
✔ Status

❌ DeliveryAddress
✔ Address

---

# Core Tables

## Product

Primary Key:
ProductId

Columns:

- ProductId
- ProdName
- ProdPrice
- ProdDescription
- ProdImage
- ProdQuantity
- DDCId
- ProdDeliveryTime
- KortingId
- ProdCost
- ProdCatId
- ProdDimensions

Relationships:

Product
  → DistributorDeliveryCompany
  → ProductDiscount
  → Category

---

## Category

Primary Key:
CatId

Columns:

- CatId
- CatName

Sample Data:

1 = Voertuig
2 = Drug
3 = Onderdeel

---

## Status

Primary Key:
StatusId

Columns:

- StatusId
- Status

Values:

1 = Pending
2 = Processing
3 = Shipped
4 = Delivered
5 = Cancelled

---

## Order

Primary Key:
OrderId

Columns:

- OrderId
- CouponId
- DeliveryTogether
- AddressId
- AccountId
- OrderDate
- StatusId

Relationships:

Order
  → Account
  → Address
  → Coupon
  → Status
  → OrderProduct

---

## OrderProduct

Bridge Table

Columns:

- OrderId
- ProductId
- Quantity

Purpose:

Many-to-many relation between Orders and Products.

---

## Account

Primary Key:
AccId

Columns:

- AccId
- AccName
- CustName
- AccountPunten

Examples:

Neo
Morpheus
Trinity

---

## Address

Primary Key:
AddressId

Columns:

- Street
- HouseNumber
- PostalCode
- City
- Country

---

## AccountAddress

Bridge Table

Columns:

- AAId
- Account
- Address

Purpose:

Connects accounts to addresses.

---

## Review

Primary Key:
RevId

Columns:

- RevId
- ProdId
- ReviewRating
- ReviewTitle
- ReviewDescription
- ReviewAccount
- ReviewDate
- ReviewStatus

Statuses:

Pending
Approved
Rejected

---

## Coupon

Primary Key:
CouponId

Columns:

- CouponType
- CouponValue
- CouponBeginDatum
- CouponEindDatum

---

## Distributor

Primary Key:
DistId

Columns:

- DistId
- ProdDistributor

Sample Data:

OligarchInc
Resistance
UnitedNationsInc

---

## DeliveryCompany

Primary Key:
DelComId

Columns:

- DelComName

Sample Data:

PostNL
FedEx
DHL

---

## DistributorDeliveryCompany

Bridge Table

Columns:

- DDCId
- DistId
- DelComId

Purpose:

Links distributors to delivery companies.

---

## ProductDiscount

Primary Key:
DiscountId

Columns:

- DiscountType
- DiscountValue
- DiscountStartDate
- DiscountEndDate

---

## CustomerServiceTicket

Primary Key:
TicketId

Columns:

- CustomerId
- TicketTitle
- TicketDescription
- TicketStatus
- TicketDateCreated
- TicketPriority

Statuses:

Resolved
In Progress

Priorities:

Low
Moderate
High

---

## ContactMethod

Primary Key:
CMId

Values:

1 = Mail
2 = Bellen

---

## Contact

Primary Key:
ContactId

Columns:

- TicketId
- CMId
- ContactTitle
- ContactDescription

Purpose:

Stores communication history for tickets.

---

## CoWorker

Primary Key:
CoWorkerId

Columns:

- CoWorkerName

---

## DistributorDelivery

Primary Key:
DDId

Columns:

- OrderDate
- ArrivalDate
- DistributorDeliveryStatus
- CoWorkerId

Statuses:

Shipping
Arrived

---

## DistributorDeliveryProduct

Bridge Table

Columns:

- DeliveryId
- ProdId
- AantalProd

Purpose:

Products contained within distributor deliveries.

---

# Existing Sample Products

ProductId | Product
----------|---------
1 | Hovercraft
2 | Blue Pill
3 | Red Pill
4 | M5 Boutje

---

# Existing Sample Customers

Account | Customer
---------|---------------------
Neo | Thomas E. Anderson
Morpheus | Laurence Fishburne
Trinity | Trinity Tiff

---

# Dapper Mapping Notes

Product query aliases should match model names.

Example:

SELECT
    ProductId       AS Id,
    ProdName        AS Name,
    ProdPrice       AS Price,
    ProdDescription AS Description,
    ProdImage       AS ImageUrl,
    ProdQuantity    AS Quantity,
    ProdDimensions  AS Dimensions
FROM Product

Always alias SQL names to C# property names.

---

# Current MAUI Project Status

Framework:
.NET 9 MAUI

Platform:
Android

Toolkit:
CommunityToolkit.Maui
CommunityToolkit.Mvvm

Messaging:
WeakReferenceMessenger

Database Access:
Dapper

Pattern:
MVVM

Navigation:
Shell

Theme:
Matrix Green Cyberpunk UI

Primary Color:
#00FF41

---

# Future Database Tasks

[ ] Replace mock services with SQL-backed services
[ ] Create SQLOrderRepository
[ ] Create SQLCustomerRepository
[ ] Create SQLReviewRepository
[ ] Create SQLTicketRepository
[ ] Create SQLDashboardRepository
[ ] Add CRUD support
[ ] Add validation
[ ] Add transaction handling
[ ] Add logging

---

# Important Rule

Whenever a model is created:

1. Check if the database already contains a matching table.
2. Use the database naming first.
3. Avoid inventing duplicate model names.
4. Keep SQL table names and C# model names aligned whenever possible.