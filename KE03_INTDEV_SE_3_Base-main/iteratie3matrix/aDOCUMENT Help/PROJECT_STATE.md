# MATRIXCOURIER — PROJECT STATE

Last Updated:
2026-06-11

Current Version:
v0.1.1

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
[x] Orders (List Page)
[ ] Order Details
[ ] Scanner
[ ] Map
[ ] Notifications
[ ] Profile
[ ] Testing
[ ] Final Polish

Current Focus:

Building the first MatrixCourier Order List screen using mock data and adapting the .NET MAUI Sample Content architecture. Preparing for Order Detail navigation and page development.

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

Current project is based on the MAUI Sample Content Shell structure.

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

[ ] OrderDetailPage
[ ] OrderDetailPageModel
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
✔ Fixed duplicate PageModel/class issues caused by accidental file overwrite
✔ Fixed OrderListPage XAML binding issues
✔ Verified OrderRepository mock data loading
✔ Verified OrderListPage data binding
✔ Confirmed Shell navigation to Orders page
✔ Application builds successfully
✔ Orders page displays repository data correctly

---

====================================================
CURRENT BUGS
============

BUG-001

Description:
OrderRepository currently contains temporary debug data.

Status:
Open

Notes:
Repository currently returns Order #999 test data.
Must be replaced with realistic mock data before Order Detail development.

====================================================
NEXT TASKS
==========

Priority 1

[x] Create Order model
[x] Create Order repository
[x] Create Order list page
[x] Verify order data binding

Priority 2

[ ] Create OrderDetailPage
[ ] Create OrderDetailPageModel
[ ] Register Order Detail route
[ ] Navigate from OrderListPage to OrderDetailPage
[ ] Load selected order by ID

Priority 3

[ ] Replace Order #999 debug data
[ ] Expand Product model
[ ] Create Account model
[ ] Create Address model
[ ] Begin SQL integration planning

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
Allows learning MAUI, MVVM, Shell navigation, dependency injection, and data binding before introducing Dapper and SQL complexity.

---

2026-06-11

Decision:
Build MatrixCourier incrementally using mock repositories.

Reason:
Allows validation of navigation, bindings, Shell routes and PageModels before introducing database complexity.

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
