# MATRIXCOURIER — PROJECT STATE

Last Updated:
2026-06-11

Current Version:
v0.1.0

====================================================
PROJECT OVERVIEW
================

Project Name:
MatrixCourier

Framework:
.NET 9 MAUI

Architecture:
MVVM (Sample Content PageModel Pattern)

Libraries:

* CommunityToolkit.Maui
* CommunityToolkit.Mvvm
* WeakReferenceMessenger (planned)
* ZXing.Net.Maui (planned)

Target Platform:

* Android

Theme:
Matrix-inspired courier application

====================================================
CURRENT STATUS
==============

Project Phase:

[x] Initial Setup
[x] Database Models
[x] Services
[x] Navigation
[ ] Dashboard
[x] Orders
[ ] Order Details
[ ] Scanner
[ ] Map
[ ] Notifications
[ ] Profile
[ ] Testing
[ ] Final Polish

Current Focus:

Building the first MatrixCourier Order List screen using mock data and adapting the .NET MAUI Sample Content architecture.

====================================================
DATABASE ALIGNMENT
==================

Database Name:
TryoutDatabase

Confirmed Tables:

[x] Status
[x] Category
[x] Product
[x] ProductDiscount
[x] Distributor
[x] DeliveryCompany
[x] DistributorDeliveryCompany
[x] Account
[x] Address
[x] Order
[x] OrderProduct
[x] Coupon
[x] Review
[x] CustomerServiceTicket
[x] Contact
[x] ContactMethod
[x] CoWorker
[x] DistributorDelivery
[x] DistributorDeliveryProduct

Naming Decision:

Use database names whenever possible.

Examples:

Order
OrderProduct
Product
Status

Avoid:

DeliveryOrder
DeliveryItem
DeliveryStatus

====================================================
PROJECT STRUCTURE
=================

Models/

[x] Order.cs
[ ] OrderProduct.cs
[x] Product.cs
[x] Status.cs
[ ] Account.cs
[ ] Address.cs
[ ] Category.cs
[ ] Notification.cs
[ ] ScanItem.cs
[ ] ScannedPackage.cs

Repositories / Data/

[x] OrderRepository.cs

Services/

[ ] IOrderService.cs
[ ] OrderService.cs
[ ] IProductService.cs
[ ] ProductService.cs
[ ] INotificationService.cs

PageModels/

[x] OrderListPageModel.cs
[ ] DashboardPageModel.cs
[ ] OrderDetailPageModel.cs
[ ] MapPageModel.cs
[ ] NotificationsPageModel.cs
[ ] ProfilePageModel.cs

Pages/

[ ] DashboardPage.xaml
[x] OrderListPage.xaml
[ ] OrderDetailPage.xaml
[ ] MapPage.xaml
[ ] NotificationsPage.xaml
[ ] ProfilePage.xaml

Controls/

[ ] PageHeader.xaml
[ ] MatrixRainView.xaml
[ ] PriorityBadge.xaml
[ ] StatusBadge.xaml
[ ] ScanItemRow.xaml
[ ] ScannedPackageRow.xaml
[ ] BarcodeScannerView.xaml

====================================================
SHELL ROUTES
============

Current Navigation:

[x] main
[x] orders
[x] manage

Notes:

Current project is based on MAUI Sample Content Shell structure.

Orders page currently replaces the original Projects page.

Future route planned:

orderDetail?id={OrderId}

====================================================
DEPENDENCY INJECTION
====================

Registered Repositories:

[x] OrderRepository

Registered PageModels:

[x] OrderListPageModel

Registered Pages:

[x] OrderListPage

Planned Registrations:

[ ] IOrderService
[ ] IProductService
[ ] INotificationService

====================================================
THEME
=====

Target Theme:

Background:
#000000

Foreground:
#00FF41

Accent:
#00CC33

CardBackground:
#0F0F0F

BorderColor:
#1A1A1A

MutedForeground:
#7FFF7F

Warning:
#FACC15

Classified:
#FF5555

Current Status:

[ ] Theme conversion started
[ ] Matrix styling applied
[ ] Custom controls created

====================================================
COMPLETED FEATURES
==================

2026-06-11

✔ Reviewed Sample Content architecture
✔ Confirmed PageModel pattern instead of ViewModel pattern
✔ Created Product model
✔ Created Status model
✔ Created Order model
✔ Created OrderRepository with mock data
✔ Created OrderListPage
✔ Created OrderListPageModel
✔ Registered OrderRepository in Dependency Injection
✔ Registered OrderListPageModel in Dependency Injection
✔ Registered OrderListPage in Dependency Injection
✔ Updated AppShell to include Orders page
✔ Fixed InitializeComponent issue caused by page/class mismatch
✔ Application builds successfully

---

====================================================
CURRENT BUGS
============

No active bugs currently documented.

====================================================
NEXT TASKS
==========

Priority 1

[x] Create Order model
[x] Create Order repository
[x] Create Order list page

Priority 2

[ ] Create OrderDetailPage
[ ] Create OrderDetailPageModel
[ ] Add Shell route for order details

Priority 3

[ ] Create Product model expansion
[ ] Create Account model
[ ] Create Address model

====================================================
DECISIONS LOG
=============

2026-06-11

Decision:
Use database naming instead of prototype naming.

Reason:
Keeps models aligned with SQL schema.

---

2026-06-11

Decision:
Reuse .NET MAUI Sample Content architecture.

Reason:
Provides working navigation, dependency injection, page models, and MVVM patterns.

---

2026-06-11

Decision:
Keep PageModels instead of renaming to ViewModels.

Reason:
Consistent with Sample Content architecture and reduces refactoring effort.

---

2026-06-11

Decision:
Use mock repositories before SQL integration.

Reason:
Allows learning MAUI, MVVM, Shell navigation, and data binding before introducing Dapper and SQL complexity.

====================================================
HELP REQUEST TEMPLATE
=====================

Current Task:

Problem:

Files Involved:

Expected Behaviour:

Actual Behaviour:

Error Messages:

Recent Changes:

Relevant Code:

====================================================
END OF PROJECT STATE
====================
