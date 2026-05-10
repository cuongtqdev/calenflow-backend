<div align="center">

<img src="https://capsule-render.vercel.app/api?type=waving&color=gradient&customColorList=6,11,20&height=180&section=header&text=CalenFlow&fontSize=70&fontAlignY=35&desc=Smart%20Interview%20Scheduling%20Platform&descAlignY=60&animation=fadeIn&fontColor=ffffff" width="100%" />

<br/>

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![SignalR](https://img.shields.io/badge/SignalR-Real--time-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![OData](https://img.shields.io/badge/OData-Query-E8692A?style=for-the-badge)](https://www.odata.org/)
[![License](https://img.shields.io/badge/License-MIT-22C55E?style=for-the-badge)](LICENSE)

<br/>

> **📅 CalenFlow** — A web-based interview scheduling platform that streamlines the hiring process  
> for companies, interviewers, and candidates with real-time notifications and AI-powered CV analysis.

<br/>

[✨ Features](#-features) · [🏗️ Architecture](#️-architecture) · [🚀 Quick Start](#-quick-start) · [📁 Project Structure](#-project-structure)

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#️-tech-stack)
- [Architecture](#️-architecture)
- [Project Structure](#-project-structure)
- [Quick Start](#-quick-start)
- [Configuration](#️-configuration)
- [User Roles](#-user-roles)

---

## 🌟 Overview

**CalenFlow** is an interview scheduling web application built with **ASP.NET Core MVC** (.NET 8) as an academic capstone project. It automates the full hiring lifecycle — from candidate invitation and interview scheduling, to real-time status notifications and AI-assisted CV screening.

The system supports three distinct user roles (Admin, Hiring Manager, Candidate) with differentiated dashboards, calendar views, and notification flows.

---

## ✨ Features

### 📅 Interview Scheduling
- Hiring managers publish available time slots for candidates to book
- Candidates send meeting requests (Online via Google Meet / Offline)
- Automatic status tracking: `Pending → Active → Completed / Cancelled`

### 🔄 Reschedule Management
- Candidates can submit reschedule requests with a reason and proposed new date
- Hiring managers receive real-time notifications and approve or decline with one click
- Interview record automatically updates upon acceptance

### 🔔 Real-time Notifications (SignalR)
- Instant push notifications for: new meeting requests, interview invites, reschedule requests, hire/reject decisions
- Notifications grouped per user email using SignalR groups
- Persistent notification history stored in the database

### 🤖 AI-Powered CV Analysis
- Candidates upload their CV (PDF)
- AI service automatically extracts skills, scores strengths & weaknesses, and ranks applicants
- Helps hiring managers reduce manual screening time

### 🔐 Authentication
- Cookie-based authentication (username/password)
- **Google OAuth2** single sign-on
- Role-based access control: `Admin` / `Hiring` / `Candidate`
- Session management with configurable idle timeout

### 📊 Dashboard & Calendar
- Hiring Manager: today's interview overview, candidate pipeline, reschedule inbox
- Candidate: upcoming interviews, request meeting board, reschedule history
- Calendar view with full interview list per manager

---

## 🛠️ Tech Stack

| Technology | Version | Purpose |
|-----------|---------|---------|
| **ASP.NET Core MVC** | `.NET 8` | Web framework (MVC pattern) |
| **Entity Framework Core** | `8.0` | ORM |
| **SQL Server** | Latest | Primary database |
| **SignalR** | Built-in | Real-time push notifications |
| **OData** | — | Dynamic server-side filtering & pagination |
| **Google OAuth2** | — | Social authentication |
| **Cookie Authentication** | Built-in | Session management |
| **PDF Text Extractor** | — | CV upload & parsing |
| **AI CV Analysis Service** | HTTP Client | Automated candidate scoring |

---

## 🏗️ Architecture

CalenFlow is built using **N-Tier Architecture** with clear separation across 4 projects:

```
┌──────────────────────────────────────────────────────┐
│               CalenFlowApp  (Presentation)           │
│  MVC Controllers · Razor Views · SignalR Hub         │
│  Cookie Auth · Google OAuth2 · Session Management   │
└──────────────────────┬───────────────────────────────┘
                       │ (depends on)
┌──────────────────────▼───────────────────────────────┐
│                  Service  (Business Logic)            │
│  IInterviewService · ICandidateService               │
│  IHiringService · INotificationService               │
│  ICVAIService (AI scoring) · IRescheduleService      │
└──────────────────────┬───────────────────────────────┘
                       │ (depends on)
┌──────────────────────▼───────────────────────────────┐
│               Repository  (Data Access)              │
│  Generic Repository Pattern                          │
│  Interview · Hiring · Candidate · Reschedule         │
│  Notification · User · Company repositories          │
└──────────────────────┬───────────────────────────────┘
                       │ (depends on)
┌──────────────────────▼───────────────────────────────┐
│            BusinessObjects  (Domain Models)          │
│  User · Candidate · Hiring · Interview               │
│  Reschedule · Notification · Company                 │
│  CalenFlowContext (EF Core DbContext)                │
└──────────────────────────────────────────────────────┘
```

### Real-time Notification Flow

```
[Action Trigger]  e.g. Candidate sends reschedule request
       │
       ▼
[Service Layer]  saves Notification to DB
       │
       ▼
[SignalR Hub]  pushes to target user's group (grouped by email)
       │
       ▼
[Client Browser]  receives toast notification instantly
```

---

## 📁 Project Structure

```
CalenFlow/
├── BusinessObjects/              # Domain Layer
│   └── Models/
│       ├── User.cs               # Shared user entity (Candidate + Hiring)
│       ├── Candidate.cs          # Candidate profile & CV URL
│       ├── Hiring.cs             # Hiring manager profile
│       ├── Interview.cs          # Interview record (date, time, type, status)
│       ├── HiringAvailable.cs    # Manager's available time slots
│       ├── Reschedule.cs         # Reschedule requests
│       ├── Notification.cs       # Notification records
│       ├── Company.cs            # Company entity
│       ├── AnalysisResult.cs     # AI CV analysis result model
│       └── CalenFlowContext.cs   # EF Core DbContext
│
├── DataAccessObjects/            # DAO Layer (raw EF queries)
│
├── Repository/                   # Repository Pattern implementations
│
├── Service/                      # Business Logic Layer
│   ├── IInterviewService / InterviewService
│   ├── ICandidateService / CandidateService
│   ├── IHiringService / HiringService
│   ├── IRescheduleService / RescheduleService
│   ├── INotificationService / NotificationService
│   ├── IUserService / UserService
│   └── ICVAIService / CVAIService   ← AI CV analyzer
│
└── CalenFlowApp/                 # Presentation Layer
    ├── Controllers/
    │   ├── AuthController.cs     # Login, logout, Google OAuth2 callback
    │   ├── ManagerController.cs  # Hiring manager dashboard & actions
    │   ├── UserController.cs     # Candidate dashboard & actions
    │   ├── CVController.cs       # CV upload & AI analysis trigger
    │   ├── NotificationController.cs  # Notification fetch & mark-read
    │   ├── HiringController.cs   # Available slot management
    │   ├── GuestController.cs    # Public landing page
    │   └── CompanyController.cs  # Company info
    ├── Hubs/
    │   └── NotificationHub.cs    # SignalR hub — groups by user email
    ├── Views/                    # Razor views per role
    └── Program.cs                # DI registration & middleware pipeline
```

---

## 🚀 Quick Start

### Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or full instance)
- Google OAuth2 credentials (for social login)

### 1. Clone the repository

```bash
git clone https://github.com/cuongtqdev/calenflow-backend.git
cd calenflow-backend
```

### 2. Configure the database connection

Edit `CalenFlowApp/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "CalenFlow": "Server=.;Database=CalenFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Apply database migrations / update database

```bash
# From the solution root
dotnet ef database update -p DataAccessObjects -s CalenFlowApp
```

### 4. Run the application

```bash
dotnet run --project CalenFlowApp/CalenFlowApp.csproj

# App runs at: https://localhost:7xxx
```

---

## ⚙️ Configuration

Edit `CalenFlowApp/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "CalenFlow": "Server=.;Database=CalenFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Authentication": {
    "Google": {
      "ClientId": "your_google_client_id",
      "ClientSecret": "your_google_client_secret"
    }
  }
}
```

> [!TIP]
> Create `appsettings.Development.json` to override values locally without touching the base config. This file is gitignored by default.

> [!CAUTION]
> Never commit real Google credentials. Use environment variables or User Secrets in development:
> `dotnet user-secrets set "Authentication:Google:ClientId" "your_id"`

---

## 👤 User Roles

| Role | Access | Key Capabilities |
|------|--------|-----------------|
| **Admin** | Full system | User management, company oversight |
| **Hiring Manager** | Manager dashboard | Post time slots, invite candidates, manage interviews, hire/reject |
| **Candidate** | Candidate dashboard | Browse companies, request meetings, upload CV, reschedule |

### Key Workflows

**Hiring Manager flow:**
1. Post available time slots (`HiringAvailable`)
2. Invite candidates by email → real-time notification sent
3. View and manage interview calendar
4. Approve/decline reschedule requests
5. Mark candidate as `Hired` or `Rejected` → notification sent

**Candidate flow:**
1. Browse available companies and time slots
2. Send meeting request → real-time notification to manager
3. Upload CV for AI analysis
4. Request reschedule with reason → manager notified
5. Receive hire/reject decision notification

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

<div align="center">

<img src="https://capsule-render.vercel.app/api?type=waving&color=gradient&customColorList=6,11,20&height=100&section=footer" width="100%" />

<br/>

**Made with ❤️ for academic purposes — FPT University**

*📅 Schedule smarter. Hire better. CalenFlow.*

</div>
