# 12 — Odoo Integration

[← Back to Index](../README.md)

---

## Overview

Odoo is an external ERP system that VCCA communicates with via **JSON-RPC**. All Odoo communication is **outbound only** — we call Odoo, Odoo doesn't call us.

**Location:** `Integration.Odoo/`

Unlike NetSuite (which uses OAuth2/JWT), Odoo embeds credentials directly in every JSON-RPC call. There is no token exchange — the `db`, `uid`, and `password` are passed as part of the request body on every call.

| Method | What it Does |
|---|---|
| `SearchReadAsync` | Fetch a list of records from an Odoo model |
| `SearchReadOneAsync` | Fetch the first matching record |
| `CreateAsync` | Create a new record — returns the new record ID |
| `WriteAsync` | Update existing records by their IDs |
| `UnlinkAsync` | Delete records by their IDs |

---

## The Rule

> **All Odoo calls must go through `Integration.Odoo`.** No exceptions.

```
Blazor Component
  → Web Handler → IMediator.Send()
    → Application Handler
      → IXxxOdooIntegration   ← contract defined in Application.UseCases
        → Integration.Odoo        ← Odoo calls only happen here
          → Odoo External API (JSON-RPC)
```

---

## Project Structure

```
Integration.Odoo/
├── Entities/
│   ├── OdooSession.cs          # Credential model (BaseUrl, Database, Uid, Password)
│   ├── OdooResponse.cs         # Generic JSON-RPC response wrapper { result, error }
│   └── OdooError.cs            # Error shape { code, message, data }
├── Repositories/
│   ├── IOdooConnection.cs      # Connection contract — credentials + JsonRpcUrl
│   └── IOdooActions.cs         # Full action surface: SearchRead, Create, Write, Unlink
├── Services/
│   ├── OdooConnection.cs       # Singleton — reads config/env vars, holds credentials
│   └── OdooActions.cs          # Transient — builds JSON-RPC envelopes, HTTP dispatch
├── Implementations/
│   ├── OdooImplementationDI.cs # Registers feature implementations into DI
│   └── [Feature]/              # Feature integration implementations go here
└── OdooServicesDI.cs           # Root DI: AddOdooServicesIntegration()
```

---

## 1 — Setting Up Odoo Integration

### Register in the startup project

In `Web.BlazorServer/Program.cs`, call:

```csharp
builder.Services.AddOdooServicesIntegration();
```

This registers two services:
- `IOdooConnection → OdooConnection` — **Singleton** (holds credentials, thread-safe)
- `IOdooActions → OdooActions` — **Transient** (owns HTTP dispatch and JSON-RPC execution)

### Provide credentials

**Option A — `appsettings.json` section `"Odoo"` (non-production only):**

```json
{
  "Odoo": {
    "BaseUrl": "https://your-odoo-instance.com",
    "Database": "lotus_dbti_prod",
    "Uid": 2,
    "Password": "your-password"
  }
}
```

> ⚠️ Never commit `Uid` or `Password` to source control. Use environment variables in all environments.

**Option B — environment variables (all environments):**

| Variable | Purpose |
|---|---|
| `ODOO_BASE_URL` | Odoo server base URL (e.g. `https://your-odoo.com`) |
| `ODOO_DATABASE` | Odoo database name |
| `ODOO_UID` | User ID (integer — found in Odoo Settings > Users) |
| `ODOO_PASSWORD` | User password |

`OdooConnection` tries the config section first; falls back to environment variables; throws `ArgumentNullException` if neither is present.

### How authentication works

Odoo JSON-RPC uses **credential-per-request** authentication — there is no OAuth2 or session token. The `Database`, `Uid`, and `Password` are embedded directly in the `args` array of every call:

```json
{
  "jsonrpc": "2.0",
  "method": "call",
  "params": {
    "service": "object",
    "method": "execute_kw",
    "args": ["lotus_dbti_prod", 2, "your-password", "model.name", "method", [...], {...}]
  }
}
```

`OdooActions` constructs this envelope automatically. Callers only provide the model name, method arguments, and options.

---

## 2 — How to Create an Odoo Integration

Adding a new feature that reads from or writes to Odoo takes four steps.

### Step 1 — Define the interface in `Application.UseCases`

Create `Application.UseCases/Repositories/Integration/[Feature]/IXxxOdooIntegration.cs`:

```csharp
// Application.UseCases/Repositories/Integration/Transaction/Timecard/ITimecardOdooIntegration.cs
namespace Application.UseCases.Repositories.Integration.Transaction.Timecard;

public interface ITimecardOdooIntegration
{
    Task<IEnumerable<TimecardDTO>> GetTimecardsAsync(string dateFrom, string dateTo);
    Task<int> PostTimecardAsync(TimecardDTO payload);
}
```

This interface lives in `Application.UseCases` — it is the boundary. The Application layer depends on the contract, not on anything in `Integration.Odoo`.

### Step 2 — Implement the class in `Integration.Odoo`

Create `Integration.Odoo/Implementations/[Feature]/[Feature]Integration.cs`:

```csharp
// Integration.Odoo/Implementations/Transactions/TimecardIntegration.cs
using Application.UseCases.Repositories.Integration.Transaction.Timecard;
using Integration.Odoo.Repositories;

namespace Integration.Odoo.Implementations.Transactions;

internal class TimecardIntegration(IOdooActions odoo) : ITimecardOdooIntegration
{
    public Task<IEnumerable<TimecardDTO>> GetTimecardsAsync(string dateFrom, string dateTo)
        => odoo.SearchReadAsync<TimecardDTO>(
               "time.card.line",
               domain: [[["time", ">=", dateFrom], ["time", "<=", dateTo]]],
               fields: ["time", "biometric_name", "card_type", "location"]);

    public async Task<int> PostTimecardAsync(TimecardDTO dto)
        => await odoo.CreateAsync("time.card.line", dto);
}
```

Rules:
- Always `internal` — the interface is the public surface
- Inject `IOdooActions` — never inject `IOdooConnection` directly
- One class per feature domain, one method per operation

**`IOdooActions` quick reference:**

```csharp
// Fetch
Task<List<T>> SearchReadAsync<T>(string model, object[][] domain, string[] fields, int? limit = null, int? offset = null);
Task<T?>      SearchReadOneAsync<T>(string model, object[][] domain, string[] fields);

// Write
Task<int>  CreateAsync(string model, object payload);   // returns new record ID
Task<bool> WriteAsync(string model, int[] ids, object payload);
Task<bool> UnlinkAsync(string model, int[] ids);
```

### Domain filter syntax

Each condition in the `domain` array is `[field, operator, value]`:

```csharp
// "time >= '2026-01-02' AND time <= '2026-01-02'"
object[][] domain =
[
    [["time", ">=", "2026-01-02 00:00:00"],
     ["time", "<=", "2026-01-02 23:59:59"]]
];
```

Common operators: `=`, `!=`, `>`, `>=`, `<`, `<=`, `like`, `ilike`, `in`, `not in`.

### Step 3 — Register in `OdooImplementationDI`

```csharp
// Integration.Odoo/Implementations/OdooImplementationDI.cs
public static IServiceCollection AddOdooImplementations(this IServiceCollection services)
{
    services.TryAddTransient<ITimecardOdooIntegration, TimecardIntegration>();
    return services;
}
```

Always `TryAddTransient` — matches the lifetime of `IOdooActions`.

### Step 4 — Register the interface in `AppUseCasesDI`

```csharp
// Application.UseCases/AppUseCasesDI.cs
services.AddTransient<ITimecardOdooIntegration, TimecardIntegration>();
// or let OdooServicesDI handle it via OdooImplementationDI
```

---

## 3 — Hooking Up to Application.UseCases

Once the integration class is registered, wire it into the MediatR pipeline exactly like any other dependency.

### The full pipeline for an Odoo feature

```
Blazor Component
  → @inject ITimecardHandler
    → TimecardHandler.GetTimecardsAsync()
      → IMediator.Send(new GetTimecardsQry(...))
        → GetTimecardsQryHandler
          → ITimecardOdooIntegration.GetTimecardsAsync()
            → TimecardIntegration (Integration.Odoo)
              → IOdooActions.SearchReadAsync<TimecardDTO>(...)
                → POST to Odoo JSON-RPC endpoint
```

### Step 1 — Add the MediatR query in `Application.UseCases`

```csharp
// Application.UseCases/Queries/Transaction/Timecard/GetTimecardsQry.cs
public record GetTimecardsQry(string DateFrom, string DateTo)
    : IRequest<IEnumerable<TimecardDTO>>;

public class GetTimecardsQryHandler(ITimecardOdooIntegration timecard)
    : IRequestHandler<GetTimecardsQry, IEnumerable<TimecardDTO>>
{
    public Task<IEnumerable<TimecardDTO>> Handle(GetTimecardsQry request, CancellationToken ct)
        => timecard.GetTimecardsAsync(request.DateFrom, request.DateTo);
}
```

> ✅ Always `IRequest` — never `ITransactionalRequest`. Odoo queries don't participate in a local database transaction.

### Step 2 — Add the Web Handler interface

```csharp
// Web.BlazorServer/Handlers/Repositories/Timecard/ITimecardHandler.cs
public interface ITimecardHandler
{
    Task<IEnumerable<TimecardDTO>> GetTimecardsAsync(string dateFrom, string dateTo);
    Task<int> PostTimecardAsync(TimecardDTO payload);
}
```

### Step 3 — Add the Web Handler implementation

```csharp
// Web.BlazorServer/Handlers/Implementations/Timecard/TimecardHandler.cs
public class TimecardHandler(IMediator mediator) : ITimecardHandler
{
    public Task<IEnumerable<TimecardDTO>> GetTimecardsAsync(string dateFrom, string dateTo)
        => mediator.Send(new GetTimecardsQry(dateFrom, dateTo));

    public Task<int> PostTimecardAsync(TimecardDTO payload)
        => mediator.Send(new PostTimecardCmd(payload));
}
```

### Step 4 — Use in the Blazor component

```csharp
@inject ITimecardHandler TimecardHandler

readonly string ActionLoadTimecards = EnumHelper.GetEnumDescription(AppActions.GetTimecards);

async Task LoadTimecardsAsync()
{
    var action = await AppActionFactory.RunAsync(
        async () =>
        {
            AppBusyService.SetBusy(ActionLoadTimecards, true);
            return await TimecardHandler.GetTimecardsAsync("2026-01-01 00:00:00", "2026-01-31 23:59:59");
        },
        AppActionOptionPresets.Loading(ActionLoadTimecards));

    AppBusyService.SetBusy(ActionLoadTimecards, false);

    action.OnSuccess(results => { /* bind to grid */ return Task.CompletedTask; });
}
```

---

## What NOT to Do

```csharp
// ❌ WRONG — Injecting IOdooActions into an Application Handler
public class GetTimecardsHandler(IOdooActions odoo) : IRequestHandler<...>
{
    // Application layer must not know about IOdooActions
}

// ❌ WRONG — Injecting IOdooActions into a Blazor component
@inject IOdooActions OdooActions

// ❌ WRONG — Calling Odoo directly from outside Integration.Odoo
var result = await httpClient.PostAsync("https://odoo.company.com/jsonrpc", ...);

// ❌ WRONG — Making IXxxOdooIntegration implementation public
public class TimecardIntegration : ITimecardOdooIntegration  // should be internal
```

---

## Next Step

➡️ Read [99 — Architectural Debts](.docs/99-architectural-debts.md) for known issues and constraints to be aware of before working on any feature.
