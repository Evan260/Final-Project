# Village Rentals Management System
A desktop equipment rental management application built for CPSY200 Software Analysis course final project.

## Tech Stack
- **Framework**: .NET 9 WinForms
- **UI Library**: Guna UI (Modern UI controls)
- **Charts**: LiveCharts (Data visualization)
- **Database**: SQLite with Entity Framework Core
- **Language**: C#

## Features
- **Equipment Management**: Add, remove, and track rental equipment inventory
- **Customer Management**: Register customers, update profiles, ban problematic customers
- **Rental Processing**: Create multi-item rentals, track return dates, calculate costs
- **Category Management**: Organize equipment into categories (Power tools, Yard equipment, etc.)
- **Reports & Analytics**: Generate sales reports and visual charts
- **Dashboard**: Overview of business metrics and recent activity

## Setup Instructions
### Prerequisites
- Visual Studio 2022 or later
- .NET 9 SDK

### Installation
1. Clone the repository to your local machine.
2. Place the .env file in the project root directory (where the .sln file is located).
3. Build the project by pressing `Ctrl + Shift + B`.

### First Run
- The application will automatically create the SQLite database
- Sample data will be populated for testing purposes
