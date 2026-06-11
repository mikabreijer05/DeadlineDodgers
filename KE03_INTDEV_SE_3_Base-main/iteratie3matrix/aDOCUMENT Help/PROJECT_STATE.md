# MATRIXCOURIER — PROJECT STATE

Last Updated:
YYYY-MM-DD

Current Version:
v0.0.0

====================================================
PROJECT OVERVIEW
================

Project Name:
MatrixCourier

Framework:
.NET 9 MAUI

Architecture:
MVVM

Libraries:

* CommunityToolkit.Maui
* CommunityToolkit.Mvvm
* WeakReferenceMessenger
* ZXing.Net.Maui (scanner)

Target Platform:

* Android

Theme:
Matrix-inspired courier application

====================================================
CURRENT STATUS
==============

Project Phase:

[ ] Initial Setup
[ ] Database Models
[ ] Services
[ ] Navigation
[ ] Dashboard
[ ] Orders
[ ] Order Details
[ ] Scanner
[ ] Map
[ ] Notifications
[ ] Profile
[ ] Testing
[ ] Final Polish

Current Focus:

Describe what you are actively working on.

Example:

"Building OrderListPage and connecting it to OrderService."

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

[ ] Order.cs
[ ] OrderProduct.cs
[ ] Product.cs
[ ] Status.cs
[ ] Account.cs
[ ] Address.cs
[ ] Category.cs
[ ] Notification.cs
[ ] ScanItem.cs
[ ] ScannedPackage.cs

Services/

[ ] IOrderService.cs
[ ] OrderService.cs
[ ] IProductService.cs
[ ] ProductService.cs
[ ] INotificationService.cs

ViewModels/

[ ] BaseViewModel.cs
[ ] DashboardViewModel.cs
[ ] OrderListViewModel.cs
[ ] OrderDetailViewModel.cs
[ ] MapViewModel.cs
[ ] NotificationsViewModel.cs
[ ] ProfileViewModel.cs

Views/

[ ] DashboardPage.xaml
[ ] OrderListPage.xaml
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

Registered Routes:

[x] dashboard
[x] orders
[x] orderDetail
[x] map
[x] alerts
[x] profile

Notes:

orderDetail receives:

?id={OrderId}

====================================================
DEPENDENCY INJECTION
====================

Registered Services:

[ ] IOrderService
[ ] IProductService
[ ] INotificationService

Registered ViewModels:

[ ] DashboardViewModel
[ ] OrderListViewModel
[ ] OrderDetailViewModel
[ ] MapViewModel
[ ] NotificationsViewModel
[ ] ProfileViewModel

====================================================
THEME
=====

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

====================================================
COMPLETED FEATURES
==================

Example:

2026-06-11

✔ Created AppShell
✔ Registered routes
✔ Created BaseViewModel
✔ Added CommunityToolkit.Mvvm

---

Add newest completed work at the top.

====================================================
CURRENT BUGS
============

Example:

BUG-001

Description:
Order list not refreshing after navigation.

Status:
Open

Notes:
Possibly issue with ObservableCollection.

---

BUG-002

Description:
Scanner overlay not closing.

Status:
Investigating

====================================================
NEXT TASKS
==========

Priority 1

[ ]
[ ]
[ ]

Priority 2

[ ]
[ ]
[ ]

Priority 3

[ ]
[ ]
[ ]

====================================================
DECISIONS LOG
=============

Record important project decisions.

Example:

2026-06-11

Decision:
Use database naming instead of prototype naming.

Reason:
Keeps models aligned with SQL schema.

---

2026-06-12

Decision:
Use ZXing.Net.Maui.

Reason:
Native MAUI barcode scanning support.

====================================================
HELP REQUEST TEMPLATE
=====================

When asking an AI for help:

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
