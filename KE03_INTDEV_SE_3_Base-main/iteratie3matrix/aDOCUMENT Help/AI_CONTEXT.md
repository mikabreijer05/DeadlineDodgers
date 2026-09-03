MATRIXCOURIER — PROJECT STATE

Last Updated: 2026-06-18
Version: v0.1.6

PROJECT OVERVIEW

Project: MatrixCourier
Type: School project

Framework:

.NET 9 MAUI (Android-first)
MVVM architecture (CommunityToolkit.Mvvm)
CommunityToolkit.Maui
Syncfusion Toolkit

Theme:
Matrix-inspired courier system with dark UI and green terminal styling.

CURRENT STATUS
Completed
Project setup completed
MVVM structure implemented
Dependency Injection configured
Shell navigation configured
Login flow implemented
Order system working
Order List Page working
Order Detail Page working
Status system implemented and fixed
Vehicle selection workflow implemented
Vehicle inspection workflow implemented
DeliverySession state system implemented
Local SQL database working (TryoutDatabase)
Repository pattern established
DI registration completed
Navigation issues resolved (Shell routing fixed)
Partially Implemented
Loading workflow exists but not strict
Vehicle system basic but functional
Status mapping improved but still evolving
Not Yet Implemented
Package scanning system
Barcode scanning (ZXing.Net.Maui)
Full state machine workflow enforcement
Vehicle repository
Full production DB integration
ORDER WORKFLOW (CURRENT)

OrderListPage
→ OrderDetailPage
→ Start Route
→ Status updates
→ (planned) scanning phase

Status system:

Uses Status table from SQL DB
Status mapping fixed using dictionary approach
VEHICLE WORKFLOW (CURRENT)

Login
→ VanSelectionPage
→ System auto-assigns Vans[0]
→ VehicleInspectionPage
→ Start Loading

Rules:

No manual van selection
Inspection is mandatory
DeliverySession controls progression
DELIVERYSESSION (STATE SYSTEM)

Central workflow state manager:

Fields:

SelectedVan
HasInspectionPassed
ShiftStarted
IsLoading
IsScanning
IsOnRoute

Purpose:
Controls courier lifecycle state progression.

DATABASE STATUS

Database: TryoutDatabase (LOCAL DEVELOPMENT)

Tables available:

Order
OrderProduct
Product
Status
Account
Address
Category
Distributor system tables
Support tables
Important Note
Local SQL Server is currently used due to remote DB instability
Repository logic unchanged during migration
System fully functional locally
DATABASE ENVIRONMENTS
Local (ACTIVE)
Windows Authentication
SQL Server Express / LocalDB
Trusted Connection enabled
Production (TEAM SETUP)
SQL Authentication required
Username + password login
Separate SQLDAL implementation

Important:
Do NOT mix authentication modes.

DEPENDENCY INJECTION

Registered services:

Repositories:

OrderRepository
StatusRepository

PageModels:

OrderListPageModel
OrderDetailPageModel
VanSelectionPageModel
VehicleInspectionPageModel

Services:

DeliverySession (Singleton)

Pages:

OrderListPage
OrderDetailPage
VanSelectionPage
VehicleInspectionPage
NAVIGATION RULES
ONLY Shell navigation
ONLY absolute routes

Example:

///order
///vehicleinspection

No relative navigation allowed.

CURRENT BUGS

BUG-001:

Mock data still exists in some repositories
COMPLETED FEATURES
Order system fully functional
Status system fixed
Navigation stabilized
Vehicle workflow implemented
Inspection system implemented
DeliverySession introduced
Local DB migration completed successfully
DI issues resolved
NEXT TASKS
Priority 1
Implement strict workflow state machine
Prevent skipping steps in process
Priority 2
Implement package scanning system
Add barcode scanning (ZXing.Net.Maui)
Priority 3
Replace all mock data with real DB integration
Add VehicleRepository
PROJECT RULES / DECISIONS
No manual van selection
Vehicle inspection is mandatory
DeliverySession controls workflow state
Strict sequential workflow required
Package scanning belongs in loading phase
Only Shell absolute routing allowed
Local DB is temporary fallback
Do not mix SQL authentication modes