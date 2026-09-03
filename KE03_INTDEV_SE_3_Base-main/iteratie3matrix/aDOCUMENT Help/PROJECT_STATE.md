MATRIXCOURIER — PROJECT STATE
Last Updated: 2026-07-01
Version: v0.4.0

============================================================
PROJECT OVERVIEW
============================================================

Project:
MatrixCourier

Framework:
.NET 9 MAUI (Android-first)

Architecture:
MVVM (CommunityToolkit MVVM)

Libraries:

* CommunityToolkit.Maui
* CommunityToolkit.Mvvm
* Syncfusion Toolkit
* Microsoft.Maui.Media

Theme:

Dark terminal-inspired courier application with courier-oriented terminology.

============================================================
CURRENT STATUS
============================================================

Project Phase:

Courier workflow prototype complete.

Implemented

✔ Dependency Injection
✔ Repository Pattern
✔ Shell Navigation
✔ Login Flow
✔ Van Assignment
✔ Vehicle Inspection
✔ Damage Reporting
✔ DeliverySession state management
✔ Cart Loading
✔ Package Loading Validation
✔ Simulated Package Scanning
✔ Deliveries overview
✔ Delivery Detail page
✔ Start Route workflow
✔ Mark as Delivered workflow
✔ Camera proof-of-delivery
✔ Status updates in database
✔ Automatic return to Deliveries after completion

Current development focus:

Improving Delivery Detail into a complete courier dashboard.

============================================================
COURIER WORKFLOW
============================================================

Login
↓
Van Assignment
↓
Vehicle Inspection
↓
Damage Check
↓
Cart Loading
↓
Package Scanning
↓
Deliveries
↓
Delivery Details
↓
Start Route
↓
Travel to Customer
↓
Take Proof Photo
↓
Mark Delivered
↓
Return to Deliveries

============================================================
DELIVERY WORKFLOW
============================================================

Current database status flow

Nieuw
↓
In behandeling
↓
Klaar om te verzenden
↓
Verzonden (Start Route)
↓
Afgeleverd (Mark Delivered)

Database status mapping

1 = Nieuw
2 = In behandeling
3 = Klaar om te verzenden
4 = Verzonden
5 = Afgeleverd
6 = Geannuleerd
7 = Retour
8 = Gedeeltelijk in behandeling

============================================================
DELIVERY SESSION
============================================================

Stored state

✔ SelectedVan
✔ HasInspectionPassed
✔ ShiftStarted
✔ IsLoading
✔ IsScanning
✔ CartScanned
✔ CartBarcode
✔ IsCartComplete

============================================================
CART LOADING WORKFLOW
============================================================

Vehicle Inspection
↓
Cart Scanner
↓
Assigned cart loaded
↓
Driver scans packages
↓
All packages validated
↓
Continue to Deliveries

Implemented

✔ DeliveryRepository
✔ CartLoad model
✔ CartItem model
✔ Vehicle lookup
✔ Delivery lookup
✔ Package validation
✔ Quantity validation
✔ Skip Scanning option

============================================================
DELIVERIES PAGE
============================================================

Internal name:

OrderListPage

User-facing name:

Deliveries

Implemented

✔ Available Deliveries
✔ Active Deliveries
✔ Automatic refresh
✔ Status filtering
✔ Courier terminology
✔ Navigation to Delivery Detail

Status filtering

Available Deliveries
StatusId = 3

Active Deliveries
StatusId = 4

Delivered
StatusId = 5

Delivered orders automatically disappear from courier view.

============================================================
DELIVERY DETAIL PAGE
============================================================

Internal name:

OrderDetailPage

User-facing name:

Delivery

Information displayed

✔ Delivery Number
✔ Order Date
✔ Current Status
✔ Customer Account Name
✔ Customer Name
✔ Full Delivery Address
✔ Delivery Type
✔ Total Package Count

Actions

✔ Start Route
✔ Mark as Delivered (Camera)

Current behaviour

Start Route

• Updates StatusId to 4 (Verzonden)

Mark as Delivered

• Opens device camera
• Requires a picture before continuing
• Saves proof photo locally
• Updates StatusId to 5 (Afgeleverd)
• Returns automatically to Deliveries page

============================================================
PACKAGE HANDLING
============================================================

Current implementation

✔ Simulated barcode scanning
✔ Quantity validation
✔ Package counting
✔ Cart validation

Current Delivery page

Displays only the total package count.

Important design decision

The courier should NOT see product names or package contents.

Future implementation

Each package will be identified by its barcode instead of its product.

============================================================
DATABASE STRUCTURE
============================================================

Core tables

✔ Order
✔ OrderProduct
✔ Product
✔ Delivery
✔ DeliveryOrderProduct
✔ Status
✔ Account
✔ Address
✔ Vehicle

Relationships

Order
↓
OrderProduct
↓
Product

Delivery
↓
DeliveryOrderProduct
↓
OrderProduct

============================================================
REPOSITORIES
============================================================

Implemented

✔ SQLDAL
✔ OrderRepository
✔ StatusRepository
✔ DeliveryRepository
✔ VehicleRepository
✔ AccountRepository
✔ AddressRepository

Planned

* PackageRepository

============================================================
MODELS
============================================================

Implemented

✔ Order
✔ OrderListItem
✔ OrderProductItem
✔ Product
✔ Status
✔ Account
✔ Address
✔ Van
✔ DeliverySession
✔ CartLoad
✔ CartItem
✔ VehicleDamageReport

Planned

* DeliveryProof
* Package
* BarcodeScan

============================================================
PAGEMODELS
============================================================

✔ OrderListPageModel
✔ OrderDetailPageModel
✔ VanSelectionPageModel
✔ VehicleInspectionPageModel
✔ VehicleDamagePageModel
✔ CartScannerPageModel

============================================================
PAGES
============================================================

✔ LoginPage
✔ VanSelectionPage
✔ VehicleInspectionPage
✔ VehicleDamagePage
✔ CartScannerPage
✔ OrderListPage (Deliveries)
✔ OrderDetailPage (Delivery)

============================================================
DEPENDENCY INJECTION
============================================================

Repositories

✔ SQLDAL
✔ OrderRepository
✔ StatusRepository
✔ DeliveryRepository
✔ VehicleRepository
✔ AccountRepository
✔ AddressRepository

PageModels

✔ OrderListPageModel
✔ OrderDetailPageModel
✔ CartScannerPageModel
✔ VanSelectionPageModel
✔ VehicleInspectionPageModel
✔ VehicleDamagePageModel

Singletons

✔ DeliverySession

Pages

✔ All pages registered

============================================================
NAVIGATION
============================================================

Shell absolute routing only.

Current flow

//login
↓
//vanselection
↓
//vehicleinspection
↓
//cartscanner
↓
//orders
↓
order?id={OrderId}

============================================================
CURRENT KNOWN ISSUES
============================================================

BUG-001

Package scanning still uses simulated barcode.

Status:
Open

------------------------------------------------------------

BUG-002

Delivery Detail page still contains legacy UI from the original Order Detail page.

Status:
In Progress

------------------------------------------------------------

BUG-003

Proof photos are only stored locally.

Status:
Open

------------------------------------------------------------

BUG-004

OrderProduct currently represents products instead of individually trackable delivery packages.

Status:
Open

============================================================
NEXT TASKS
============================================================

Priority 1

• Redesign Delivery Detail page into a professional courier dashboard
• Improve page layout
• Remove legacy order terminology
• Improve package overview

------------------------------------------------------------

Priority 2

Implement package barcode validation

• Show expected package barcodes
• Scan packages during delivery
• Prevent delivery until all expected packages are scanned

------------------------------------------------------------

Priority 3

Integrate ZXing.Net.MAUI

• Replace simulated scanner
• Enable real barcode scanning

------------------------------------------------------------

Priority 4

Store proof-of-delivery photos permanently

• Database
or
• Cloud storage

------------------------------------------------------------

Priority 5

Implement a strict workflow state machine preventing users from skipping mandatory steps.

============================================================
DESIGN DECISIONS
============================================================

✔ User sees "Deliveries"; database remains "Order"
✔ Couriers never see product names or package contents
✔ Package verification will use barcodes only
✔ Foreign keys are translated into readable information through repositories
✔ Vehicle inspection is mandatory
✔ DeliverySession controls workflow state
✔ Start Route = StatusId 4
✔ Mark as Delivered = StatusId 5
✔ Camera proof is mandatory before completing delivery
✔ After delivery the app automatically returns to the Deliveries page
✔ Repository pattern keeps SQL logic out of ViewModels
✔ Absolute Shell routing is used throughout the application

============================================================
LONG-TERM GOAL
============================================================

MatrixCourier should function as a complete professional courier application.

Login
↓
Vehicle Assignment
↓
Vehicle Inspection
↓
Cart Loading
↓
Package Verification
↓
Deliveries
↓
Navigation
↓
Delivery
↓
Package Verification
↓
Proof Photo
↓
Delivery Completed
↓
Route Finished