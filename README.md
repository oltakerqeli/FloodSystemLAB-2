 🌊 Flood System – Early Warning & Decision Support System

A web-based platform designed to support flood monitoring, incident reporting, weather analysis, and emergency response. The system enables citizens to report incidents, authorities to monitor flood risks, and administrators to manage alerts and notifications in real time.


### Source Code Repository
GitHub:[ https://github.com/oltakerqeli/FloodSystemLAB-2](https://github.com/oltakerqeli/FloodSystemLAB-2.git)

### Project Management Board
Trello: [https://trello.com/b/XXXXXXXX/flood-system](https://trello.com/b/kgpJJLuw/floodsystemlab-2)

📋 Table of Contents

1. Overview
2. Features
3. Technology Stack
4. Project Structure
5. Prerequisites
6. Installation and Configuration
7. Database Setup
8. Running the Application
9. API Documentation
10. Decision Support System (DSS)
11. Real-Time Communication
12. Authentication and Roles
13. Testing
14. Authors
15. License


1. 🌟 Overview

Flood System is an Early Warning and Decision Support System that combines weather information, location management, flood reporting, notifications, and dashboards into a single platform.

The system is divided into four modules:

* **Module 1:** Authentication & User Management
* **Module 2:** Reporting & File Management
* **Module 3:** Location, Weather & Decision Support System
* **Module 4:** Dashboard & System Management



2. 🚀 Features

## Authentication & User Management

* User registration and login
* JWT authentication
* Refresh token support
* Password recovery
* Role management
* User activation/deactivation

## Reporting Module

* Flood reporting
* Drain blockage reporting
* Report status management
* Report history

## Weather & Decision Support System

* Weather retrieval
* Weather history
* Risk assessment
* Alerts generation
* Traffic analysis
* Safe route recommendations

## Dashboard Module

* Notifications
* Audit logs
* Dynamic reports
* Import and export functionality
* System settings

## Real-Time Communication

* SignalR WebSockets
* Live notifications
* Dashboard updates without page refresh

---

3. 🛠 Technology Stack

## Backend

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* MongoDB
* SignalR
* JWT Authentication
* Swagger

## Frontend

* React
* Vite
* Axios
* Leaflet.js
* CSS

## External Services

* OpenWeatherMap API



4. 📂 Project Structure

FloodSystemLAB-2
│
├── FloodSystem.API
│   ├── Controllers
│   ├── Services
│   ├── Repositories
│   ├── Models
│   ├── DTOs
│   ├── Data
│   ├── Middleware
│   ├── Hubs
│   ├── MongoDB
│   ├── Migrations
│   └── Program.cs
│
├── FloodSystem.Client
│   ├── src
│   │   ├── components
│   │   ├── pages
│   │   ├── contexts
│   │   ├── hooks
│   │   ├── assets
│   │   └── services
│   └── public
│
└── README.md


5. 📦 Prerequisites

Before running the application, install:

* .NET SDK
* Node.js
* SQL Server
* MongoDB
* Git

---

6. ⚙ Installation and Configuration

## Clone Repository

```bash
git clone https://github.com/oltakerqeli/FloodSystemLAB-2.git
cd FloodSystemLAB-2
```

---

## Configure SQL Server

Update connection string inside:

```text
appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=FloodSystemDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## Configure MongoDB

```json
"MongoDB": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "FloodSystemDB"
}
```

---

## Configure OpenWeatherMap

```json
"OpenWeatherMap": {
  "ApiKey": "YOUR_API_KEY"
}
```

---

7. 🗄 Database Setup

Navigate to API project:

```bash
cd FloodSystem.API
```

Restore packages:

```bash
dotnet restore
```

Apply migrations:

```bash
dotnet ef database update
```

---

8. Running App
8.1 ▶ Running Backend

```bash
cd FloodSystem.API
dotnet run
```

API:

```
http://localhost:5164
```

Swagger:

```
http://localhost:5164/swagger
```

---

8.2  ▶ Running Frontend

Navigate to client:

```bash
cd FloodSystem.Client
```

Install packages:

```bash
npm install
```

Start React application:

```bash
npm run dev
```

Frontend URL:

```
http://localhost:5173
```

---

9.📡 API Documentation

## Authentication

* POST /api/Auth/register
* POST /api/Auth/login
* POST /api/Auth/refresh-token
* POST /api/Auth/logout
* GET /api/Auth/me
* POST /api/Auth/forgot-password
* POST /api/Auth/verify-reset-code
* POST /api/Auth/reset-password

---

## Users

* GET /api/Users
* GET /api/Users/{id}
* PUT /api/Users/{id}/activate
* PUT /api/Users/{id}/deactivate
* PUT /api/Users/{id}/assign-role

---

## Reports

* POST /api/Reports/flood
* GET /api/Reports/flood
* POST /api/Reports/drain
* GET /api/Reports/drain
* GET /api/Reports/flood/all
* GET /api/Reports/drain/all
* PATCH /api/Reports/{id}/status

---

## Locations

* GET /api/Locations
* POST /api/Locations
* GET /api/Locations/{id}
* PUT /api/Locations/{id}
* DELETE /api/Locations/{id}
* GET /api/Locations/{id}/zones

---

## Zones

* GET /api/Zones
* POST /api/Zones
* GET /api/Zones/{id}
* PUT /api/Zones/{id}
* DELETE /api/Zones/{id}
* POST /api/Zones/{zoneId}/locations/{locationId}
* DELETE /api/Zones/{zoneId}/locations/{locationId}
* GET /api/Zones/{zoneId}/locations

---

## Weather

* GET /api/Weather/{locationId}/latest
* GET /api/Weather/{locationId}/history
* GET /api/Weather/{locationId}/risk
* POST /api/Weather/fetch/{locationId}
* POST /api/Weather/fetch-all

---

## Alerts

* GET /api/Alerts
* GET /api/Alerts/active
* GET /api/Alerts/location/{locationId}
* GET /api/Alerts/{id}

---

## Traffic

* GET /api/Traffic/safe-routes
* GET /api/Traffic/location/{locationId}/latest
* GET /api/Traffic/location/{locationId}/history

---

## Search

* POST /api/Search/alerts
* POST /api/Search/locations
* POST /api/Search/weather
* POST /api/Search/traffic
* POST /api/Search/zones

---

## Dashboard

* GET /api/Dashboard/audit-logs
* GET /api/Dashboard/notifications
* POST /api/Dashboard/notifications
* PATCH /api/Dashboard/notifications/{id}/read
* GET /api/Dashboard/settings
* PATCH /api/Dashboard/settings/{id}
* POST /api/Dashboard/export
* POST /api/Dashboard/import
* POST /api/Dashboard/dynamic-report



10. ⚙ Decision Support System (DSS)

The system evaluates flood risk according to rainfall values:

| Rainfall | Risk Level | Traffic Status |
| -------- | ---------- | -------------- |
| ≤ 5 mm   | LOW        | OPEN           |
| 5–10 mm  | MEDIUM     | CAUTION        |
| >10 mm   | HIGH       | BLOCKED        |

Color representation:

* 🟢 Low Risk
* 🟡 Medium Risk
* 🔴 High Risk



11. 🔔 Real-Time Communication

SignalR WebSockets are used to provide:

* Real-time notifications
* Dashboard updates
* Alert broadcasting
* Live data synchronization

Polling is not used.



12. 🔐 User Roles

### Admin

* Manage users
* Manage notifications
* Configure system settings
* Generate reports

### Authority

* Access all reports
* Monitor alerts
* Update statuses

### User

* Submit reports
* View weather information
* Track personal reports



13. 🧪 Testing

## Backend Testing

Run:

```bash
dotnet run
```

Open:

```
http://localhost:5164/swagger
```

Steps:

1. Login using API.
2. Copy JWT token.
3. Click Authorize.
4. Paste token.
5. Test endpoints.

---

## Frontend Testing

Run:

```bash
npm run dev
```

Open:

```
http://localhost:5173
```

Explore:

* Dashboard
* Map
* Weather information
* Reports
* Notifications



14 👥 Authors

| Name  | Responsibility                              | Module   |
| ----- | ------------------------------------------- | -------- |
| Erda  | Authentication & User Management            | Module 1 |
| Elona | Reporting & File Management                 | Module 2 |
| Olta  | Location, Weather & Decision Support System | Module 3 |
| Anila | Dashboard & System Management               | Module 4 |



15. 📄 License

This project was developed for academic purposes.

© 2026 Flood System Team
