# 11 — NetSuite Integration

[← Back to Index](../README.md)

---

## Overview

NetSuite is an external ERP system that VCCA communicates with for transactional business data. All NetSuite communication is **outbound only** — we call NetSuite, NetSuite doesn't call us.

**Location:** `Integration.NetSuite/`

NetSuite exposes two distinct APIs, and this integration supports both:

| Mode | What it is | Used for |
|---|---|---|
| **REST Record API** | HTTP CRUD on NetSuite records | Creating/updating documents (e.g. item receipts) |
| **SuiteQL** | SQL-like query language POSTed to a REST endpoint | Reading data sets (e.g. pending purchase orders) |

---

## The Rule

> **All NetSuite calls must go through `Integration.NetSuite`.** No exceptions.

```
Blazor Component
  → Web Handler → IMediator.Send()
    → Application Handler
      → IXxxNSIntegration   ← contract defined in Application.UseCases
        → Integration.NetSuite  ← NetSuite calls only happen here
          → NetSuite External API
```

---

## Project Structure

```
Integration.NetSuite/
├── Entities/
│   ├── NetSuiteSession.cs          # Credential model
│   ├── NetSuiteResponse.cs         # Generic SuiteQL response wrapper
│   ├── NetsuiteToken.cs            # OAuth2 token response
│   └── NetsuiteFindIdsResponse.cs  # ID-lookup response
├── Repositories/
│   ├── INetSuiteConnection.cs      # Connection contract — URLs + GetAccessTokenAsync()
│   └── INetSuiteActions.cs         # Full action surface: REST + SuiteQL
├── Services/
│   ├── NetSuiteConnection.cs       # Singleton — owns OAuth2/JWT auth and token caching
│   └── NetSuiteActions.cs          # Transient — owns all HTTP dispatch and SuiteQL execution
├── Implementations/
│   ├── NSImplementationDI.cs       # Registers feature implementations into DI
│   └── Transactions/               # Feature integration implementations go here
│       └── ReceivingIntegration.cs # Example stub
├── NSScripts/                      # SuiteQL .sql files — copied to Scripts/ on build
└── NetSuiteServicesDI.cs           # Root DI: AddNetSuiteServicesIntegration()
```

---

## 1 — Setting Up NetSuite Integration

### Register in the startup project

In `Web.BlazorServer/Program.cs`, call:

```csharp
builder.Services.AddNetSuiteServicesIntegration();
```

This registers two services:
- `INetSuiteConnection → NetSuiteConnection` — **Singleton** (owns OAuth2/JWT, thread-safe token caching)
- `INetSuiteActions → NetSuiteActions` — **Transient** (owns HTTP dispatch and SuiteQL execution)

### Provide credentials

**Option A — `appsettings.json` section `"NetSuite"` (non-production only):**

```json
{
  "NetSuite": {
    "AccountID": "1234567",
    "CertificateId": "your-cert-id",
    "ConsumerKey": "your-consumer-key",
    "PrivateKeyPath": "C:/path/to/privatekey.pem"
  }
}
```

**Option B — environment variables (all environments):**

| Variable | Purpose |
|---|---|
| `ACCOUNT_ID` | NetSuite account identifier |
| `NETSUITE_CERTIFICATE_ID` | OAuth certificate ID |
| `NETSUITE_CONSUMER_KEY` | OAuth consumer key |
| `PRIVATEKEY_PATH` | Absolute path to the RSA private key file (PKCS#8 PEM) |

`NetSuiteConnection` tries the config section first; falls back to environment variables; throws `ArgumentNullException` if neither is present.

### Private key requirements

- Format: PKCS#8 PEM (`-----BEGIN PRIVATE KEY-----`)
- Minimum key size: **3072 bits** (NetSuite enforces this)
- Algorithm: RSA-PSS with SHA-256 (PS256)
- **Never commit the key file** — always reference it via `PRIVATEKEY_PATH`

### How authentication works

NetSuite uses **OAuth2 client credentials with a JWT assertion** — there is no username/password. `NetSuiteConnection` handles this automatically:

1. Reads the RSA private key from the file at `PrivateKeyPath`
2. Builds a signed JWT (10-minute lifetime, cached for 8 minutes)
3. Exchanges the JWT for a Bearer access token (30-minute lifetime, cached for 28 minutes)
4. `NetSuiteActions` calls `GetAccessTokenAsync()` on every request — refresh is automatic and transparent

---

## 2 — How to Create a NetSuite Integration

Adding a new feature that reads from or writes to NetSuite takes four steps.

### Step 1 — Add the SuiteQL script (if querying data)

Create `Integration.NetSuite/NSScripts/NS_{Feature}_{Action}.sql`:

```sql
-- NS_PurchaseOrder_Get_PendingReceipt.sql
SELECT
    t.tranId   AS OrderNumber,
    t.status   AS OrderStatus,
    e.fullname AS VendorName,
    TO_CHAR(t.createdDate, 'YYYY-MM-DD"T"HH24:MI:SS') AS CreatedDate
FROM
    transaction t
JOIN entity e ON t.entity = e.id
WHERE
    t.recordtype = 'purchaseorder'
    AND t.status IN ('B', 'E')
```

The file name (without `.sql`) is the key used in code. Naming convention: `NS_{Feature}_{Action}`.

The `.csproj` copies all `NSScripts/*.sql` files to `Scripts/` in the build output — no manual copy step needed.

### Step 2 — Define the interface in `Application.UseCases`

Create `Application.UseCases/Repositories/Integration/Transaction/{Feature}/IXxxNSIntegration.cs`:

```csharp
// Application.UseCases/Repositories/Integration/Transaction/Receiving/IReceivingNSIntegration.cs
namespace Application.UseCases.Repositories.Integration.Transaction.Receiving;

public interface IReceivingNSIntegration
{
    Task<IEnumerable<PurchaseOrderDTO>> GetPendingReceiptPOsAsync(int limit = 0, int offset = 0);
}
```

This interface lives in `Application.UseCases` — it is the boundary. The Application layer depends on the contract, not on anything in `Integration.NetSuite`.

### Step 3 — Implement the class in `Integration.NetSuite`

Create `Integration.NetSuite/Implementations/Transactions/{Feature}Integration.cs`:

```csharp
// Integration.NetSuite/Implementations/Transactions/ReceivingIntegration.cs
using Application.UseCases.Repositories.Integration.Transaction.Receiving;
using Integration.NS.Repositories;

namespace Integration.NS.Implementations.Transactions;

internal class ReceivingIntegration(INetSuiteActions nsActions) : IReceivingNSIntegration
{
    public async Task<IEnumerable<PurchaseOrderDTO>> GetPendingReceiptPOsAsync(int limit = 0, int offset = 0)
    {
        return await nsActions.QueryAsync<PurchaseOrderDTO>("NS_PurchaseOrder_Get_PendingReceipt");
    }
}
```

Rules:
- Always `internal` — the interface is the public surface
- Inject `INetSuiteActions` — never inject `INetSuiteConnection` directly
- One class per feature domain, one method per operation

**`INetSuiteActions` quick reference:**

```csharp
// REST Record API
Task<T>  GetAsync<T>(string resource, object id);            // GET  /record/v1/{resource}/{id}
Task<T>  PostAsync<T, U>(string resource, U payload);        // POST /record/v1/{resource}
Task<U>  PatchAsync<U>(string resource, object id, U data);  // PATCH /record/v1/{resource}/{id}

// SuiteQL — loads script from NSScripts/ by name
Task<List<T>> QueryAsync<T>(string scriptName);   // returns all rows
Task<T?>      SingleAsync<T>(string scriptName);  // returns first row or null

// SuiteQL — raw inline query string
Task<List<T>> RawQueryAsync<T>(string suiteql);
Task<T?>      RawQueryOneAsync<T>(string suiteql);
```

> ⚠️ Parameterized script variants (`QueryAsync<T, U>`, `SingleAsync<T, U>`) are not yet implemented. For parameterized queries, use `RawQueryAsync` or `RawQueryOneAsync` with an inline query string.

### Step 4 — Register in `NSImplementationDI`

```csharp
// Integration.NetSuite/Implementations/NSImplementationDI.cs
public static IServiceCollection AddNSImplementationsIntegraton(this IServiceCollection services)
{
    services.TryAddTransient<IReceivingNSIntegration, ReceivingIntegration>();
    return services;
}
```

Always `TryAddTransient` — matches the lifetime of `INetSuiteActions`.

---

## 3 — Hooking Up to Application.UseCases

Once the integration class is registered, wire it into the MediatR pipeline exactly like any other dependency.

### The full pipeline for a NetSuite feature

```
Blazor Component
  → @inject IReceivingHandler
    → ReceivingHandler.GetPendingReceiptPOsAsync()
      → IMediator.Send(new GetPendingReceiptPOsQry(...))
        → GetPendingReceiptPOsQryHandler
          → IReceivingNSIntegration.GetPendingReceiptPOsAsync()
            → ReceivingIntegration (Integration.NetSuite)
              → INetSuiteActions.QueryAsync<PurchaseOrderDTO>(...)
                → Bearer token auto-refreshed by INetSuiteConnection
                  → POST to NetSuite SuiteQL endpoint
```

### Step 1 — Add the MediatR query in `Application.UseCases`

```csharp
// Application.UseCases/Queries/Transaction/Receiving/GetPendingReceiptPOsQry.cs
public record GetPendingReceiptPOsQry(int Limit, int Offset)
    : IRequest<IEnumerable<PurchaseOrderDTO>>;

public class GetPendingReceiptPOsQryHandler(IReceivingNSIntegration receiving)
    : IRequestHandler<GetPendingReceiptPOsQry, IEnumerable<PurchaseOrderDTO>>
{
    public Task<IEnumerable<PurchaseOrderDTO>> Handle(
        GetPendingReceiptPOsQry request, CancellationToken ct)
        => receiving.GetPendingReceiptPOsAsync(request.Limit, request.Offset);
}
```

> ✅ Always `IRequest` — never `ITransactionalRequest`. NetSuite queries don't participate in a local database transaction.

### Step 2 — Add the Web Handler interface

```csharp
// Web.BlazorServer/Handlers/Repositories/Receiving/IReceivingHandler.cs
public interface IReceivingHandler
{
    Task<IEnumerable<PurchaseOrderDTO>> GetPendingReceiptPOsAsync(int limit = 0, int offset = 0);
}
```

### Step 3 — Add the Web Handler implementation

```csharp
// Web.BlazorServer/Handlers/Implementations/Receiving/ReceivingHandler.cs
public class ReceivingHandler(IMediator mediator) : IReceivingHandler
{
    public Task<IEnumerable<PurchaseOrderDTO>> GetPendingReceiptPOsAsync(int limit = 0, int offset = 0)
        => mediator.Send(new GetPendingReceiptPOsQry(limit, offset));
}
```

### Step 4 — Use in the Blazor component

```csharp
// Receiving/ReceivingListPage.razor.cs
@inject IReceivingHandler ReceivingHandler

readonly string ActionLoadPOs = EnumHelper.GetEnumDescription(AppActions.GetPendingReceiptPOs);

async Task LoadPendingPOsAsync()
{
    var action = await AppActionFactory.RunAsync(
        async () =>
        {
            AppBusyService.SetBusy(ActionLoadPOs, true);
            return await ReceivingHandler.GetPendingReceiptPOsAsync(limit: 50, offset: 0);
        },
        AppActionOptionPresets.Loading(ActionLoadPOs));

    AppBusyService.SetBusy(ActionLoadPOs, false);

    action.OnSuccess(results => { /* bind to grid */ return Task.CompletedTask; });
}
```

---

## What NOT to Do

```csharp
// ❌ WRONG — Injecting INetSuiteActions into an Application Handler
public class GetPendingPOsHandler(INetSuiteActions nsActions) : IRequestHandler<...>
{
    // Application layer must not know about INetSuiteActions
}

// ❌ WRONG — Injecting INetSuiteActions into a Blazor component
@inject INetSuiteActions NSActions

// ❌ WRONG — Calling NetSuite from outside Integration.NetSuite
var result = await httpClient.PostAsync("https://xxx.suitetalk.api.netsuite.com/...", ...);

// ❌ WRONG — Making IXxxNSIntegration implementation public
public class ReceivingIntegration : IReceivingNSIntegration  // should be internal
```

---

## Next Step

➡️ Read [99 — Architectural Debts](.docs/99-architectural-debts.md) for known issues and constraints to be aware of before working on any feature.
