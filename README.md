# 📚 VCCA — Architecture Documentation

> **Welcome!** 👋
> This documentation covers **Vertical Cleaner Clean Architecture (VCCA)** — the architectural pattern used across our projects. It will walk you through how a VCCA-structured system is built, from the big picture down to the nitty-gritty. Don't worry if you're new to these concepts; everything is explained step by step.

---

## 🗺️ Where to Start

| Document | What You'll Learn | Start Here If... |
|---|---|---|
| [01 — Big Picture](.docs/01-big-picture.md) | What the system does and how the pieces fit together | You're brand new |
| [02 — Layer Guide](.docs/02-layers.md) | What each layer is responsible for | You want to understand the architecture |
| [03 — Request Pipeline](.docs/03-request-pipeline.md) | How a button click becomes a database query | You're writing your first feature |
| [04 — Blazor & UI](.docs/04-blazor-ui.md) | How the frontend is structured | You're working on UI components |
| [05 — Database & Data Access](.docs/05-database.md) | How we read and write data | You're working with data |
| [06 — Domain & Business Rules](.docs/06-domain.md) | Where business logic lives | You're implementing business rules |
| [07 — Authentication](.docs/07-auth.md) | How login and permissions work | You're adding a secured feature |
| [08 — SAP Integration](.docs/08-sap.md) | How we talk to SAP | You're working on SAP features |
| [09 — Code Conventions](.docs/09-conventions.md) | Naming rules, patterns, do's and don'ts | Before writing any code |
| [10 — Building & Running](.docs/10-building.md) | How to run the project locally | Day one setup |
| [11 — NetSuite Integration](.docs/11-netsuite.md) | How we talk to NetSuite | You're working on NetSuite features |
| [12 — Odoo Integration](.docs/12-odoo.md) | How we talk to Odoo | You're working on Odoo features |

---

## 🧱 Tech Stack at a Glance

```
Language:      C# / .NET 8
Frontend:      Blazor Server
UI Components: Radzen
Database:      Microsoft SQL Server (EF Core + Dapper)
Mediator:      MediatR
Mapping:       Mapster
Guards:        Ardalis.Guards
Auth:          Cookie Authentication (Claims-based)
External:      SAP via B1SLayer · NetSuite REST API (OAuth2 / SuiteQL) · Odoo JSON-RPC
```

---

## 💡 The #1 Rule

> **Every layer has a job. Don't skip layers.**

A user clicks a button → the UI talks to a Web Repository → which calls a Web Handler → which sends a MediatR message → which the Application layer handles → which calls the database.

This chain always stays intact. You'll see it everywhere in this documentation.

---

## 📍 To Do

> **There is always room for improvement.**

This architecture is still a work in progress. There are [Architectural Debts](.docs/99-architectural-debts.md) that need to be reviewed and addressed.

Some key things we are planning to add:
- **Web API support** – so we can expose backend functionality through HTTP endpoints.
- **Headless setup** – allowing JavaScript frontends (like React or Angular) to connect to a C# API.

---

*Last updated: 2026 — maintained by Ian Nicolas Antonio. Engineered with Charles Maverick Herrera and Arnel De Asas.*
