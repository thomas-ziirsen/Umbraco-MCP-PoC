# Evidence

## EVENTS-001

Status:
VERIFIED

Files Changed:
- `.github/ai/DECISIONS.md`
- `.github/ai/STATUS.md`
- `.github/ai/EVIDENCE.md`
- `.github/ai/TASKS.md`

Commands Run:
None

Verification:
Manual source inspection confirmed the app is a single-project Umbraco website with standard website routing in `Program.cs`, shared Razor imports in `Views/_ViewImports.cshtml`, and existing partial-based rendering infrastructure under `Views/Partials`.

Notes:
Chose page-based content modeling with an events landing page plus child event detail pages. Ticket purchasing remains an optional outbound CTA only, not an in-site workflow.

---

## EVENTS-002

Status:
VERIFIED

Files Changed:
- `.github/ai/BUSINESS_LOGIC.md`
- `.github/ai/FEATURES.md`
- `.github/ai/STATUS.md`
- `.github/ai/EVIDENCE.md`
- `.github/ai/TASKS.md`

Commands Run:
None

Verification:
Manual review confirmed the documented plan now covers the event document type shape, required and optional fields, media handling, editorial workflow, and rendering integration expectations.

Notes:
Aligned the feature scope with the PRD by treating ticketing as an optional outbound CTA only and by planning separate listing and detail experiences instead of a one-page booking flow.

---

## EVENTS-003

Status:
VERIFIED

Files Changed:
- `Features/Events/EventContentModels.cs`
- `Views/_ViewImports.cshtml`
- `Views/Shared/_Layout.cshtml`
- `Views/EventsLandingPage.cshtml`
- `Views/EventPage.cshtml`
- `Views/Partials/Events/EventCard.cshtml`
- `wwwroot/css/events-site.css`
- `.github/ai/STATUS.md`
- `.github/ai/EVIDENCE.md`
- `.github/ai/TASKS.md`

Commands Run:
- `dotnet build MyProject.sln`
- `dotnet build MyProject.sln -p:BaseOutputPath=artifacts/bin/ -p:BaseIntermediateOutputPath=artifacts/obj/`
- `Remove-Item -Path .\artifacts -Recurse -Force -ErrorAction SilentlyContinue`
- `dotnet build MyProject.sln -p:OutputPath=C:\Temp\MyProjectBuild\bin\`
- `dotnet build MyProject.sln -p:OutputPath=C:\Temp\MyProjectBuild\bin\`

Verification:
`dotnet build MyProject.sln -p:OutputPath=C:\Temp\MyProjectBuild\bin\` succeeded after cleaning generated validation artifacts. The remaining warning is an existing package vulnerability warning for `MailKit`.

Notes:
Added an alias-based content mapper so the templates can render event content safely without generated models. The landing page filters to upcoming child `eventPage` nodes and detail rendering fails closed when required content is missing.

---

## EVENTS-004

Status:
VERIFIED

Files Changed:
- `Views/EventsLandingPage.cshtml`
- `Views/Partials/Events/EventListingSection.cshtml`
- `Views/Partials/Events/EventCard.cshtml`
- `Views/Shared/_Layout.cshtml`
- `wwwroot/css/events-site.css`
- `wwwroot/js/events-site.js`
- `.github/ai/STATUS.md`
- `.github/ai/EVIDENCE.md`
- `.github/ai/TASKS.md`

Commands Run:
- `dotnet build MyProject.sln -p:OutputPath=C:\Temp\MyProjectBuild\bin\`

Verification:
Build passed after the listing UI was refactored into a dedicated partial and the motion layer was added. Live browser validation remains pending because the required Umbraco content types and content instances are not available in this session.

Notes:
Implemented a dedicated event listing partial, stronger card hierarchy, responsive layout behavior, and a reduced-motion-safe reveal animation system.

---

## EVENTS-005

Status:
VERIFIED

Files Changed:
- `Views/EventPage.cshtml`
- `Views/Shared/_Layout.cshtml`
- `wwwroot/css/events-site.css`
- `wwwroot/js/events-site.js`
- `.github/ai/STATUS.md`
- `.github/ai/EVIDENCE.md`
- `.github/ai/TASKS.md`

Commands Run:
- `dotnet build MyProject.sln -p:OutputPath=C:\Temp\MyProjectBuild\bin\`

Verification:
Build passed after the event detail template polish. Live runtime validation of navigation and dynamic content binding remains pending because the required Umbraco content types and sample content are not available in this session.

Notes:
Strengthened the event detail presentation with a richer hero composition, back navigation, supporting media treatment, and motion-aware reveal sequencing shared with the listing experience.

---

## EVENTS-006

Status:
VERIFIED

Files Changed:
- `Features/Events/EventContentModels.cs`
- `.github/ai/STATUS.md`
- `.github/ai/EVIDENCE.md`

Commands Run:
- `./scripts/Test-UmbracoMcpPayload.ps1 -ToolName "mcp_umbraco-mcp_get-item-member-search" -PayloadJson "{}"`
- `./scripts/Test-UmbracoMcpPayload.ps1 -ToolName "mcp_umbraco-mcp_get-item-member-search" -PayloadJson '{"query":"events"}'`
- `./scripts/Test-UmbracoMcpPayload.ps1 -ToolName "mcp_umbraco-mcp_get-template-search" -PayloadJson '{"query":"Event"}'`
- `./scripts/Test-UmbracoMcpPayload.ps1 -ToolName "mcp_umbraco-mcp_search-document" -PayloadJson '{"query":"a"}'`
- `Get-Process iisexpress -ErrorAction SilentlyContinue | Stop-Process -Force`
- `dotnet build MyProject.sln`
- `dotnet run --project MyProject.csproj --launch-profile "Umbraco.Web.UI"`
- `Invoke-WebRequest -Uri 'https://localhost:44304/' -UseBasicParsing`
- `Invoke-WebRequest -Uri 'https://localhost:44304/neon-harbor-live/' -UseBasicParsing`
- `Invoke-WebRequest -Uri 'https://localhost:44304/midnight-brass-collective/' -UseBasicParsing`

Verification:
Verified with live runtime checks after reseeding and mapper hardening. Landing page renders published upcoming event cards on `/` (empty-state no longer shown), and both event detail URLs now render full event content (`event-detail`) instead of the fallback unavailable message.

Notes:
Adjusted detail mapping to resolve rich text body content even when returned as string-backed payloads; this removed false negatives in required-field validation on event detail pages.

---

## EVENTS-007

Status:
VERIFIED

Files Changed:
- `.github/ai/TASKS.md`
- `.github/ai/STATUS.md`
- `.github/ai/DECISIONS.md`
- `Program.cs`
- `Features/Events/EventContentModels.cs`
- `Features/Events/EventRegistrationModels.cs`
- `Features/Events/EventRegistrationService.cs`
- `Views/EventPage.cshtml`
- `Views/Backoffice/EventRegistrations.cshtml`
- `wwwroot/css/events-site.css`

Commands Run:
- `dotnet build MyProject.sln`
- `Invoke-WebRequest -Uri 'https://localhost:44304/midnight-brass-collective/' -UseBasicParsing`
- `Invoke-WebRequest -Uri 'https://localhost:44304/events/register' -Method Post -Body <formData> -UseBasicParsing -MaximumRedirection 0`
- `Invoke-WebRequest -Uri 'https://localhost:44304/events/registrations?eventKey=<eventKey>' -UseBasicParsing`
- `Invoke-WebRequest -Uri 'https://localhost:44304/events/registrations.csv?eventKey=<eventKey>' -UseBasicParsing`
- `Invoke-WebRequest -Uri 'https://localhost:44304/umbraco/backoffice/registrations' -UseBasicParsing`
- `Invoke-WebRequest -Uri 'https://localhost:44304/umbraco/backoffice/registrations?eventKey=00000000-0000-0000-0000-000000000001' -UseBasicParsing`
- `Invoke-WebRequest -Uri 'https://localhost:44304/events/register' -Method Post -Body <noConsentFormData> -UseBasicParsing -MaximumRedirection 0`
- `Invoke-WebRequest -Uri 'https://localhost:44304/events/registrations.csv?eventKey=00000000-0000-0000-0000-000000000001' -UseBasicParsing`

Verification:
Verified on live local runtime. Registration form renders on event detail page; submit success/duplicate/invalid states redirect with expected query flags; registration rows persist and list via endpoint; dedicated backoffice dashboard renders registrations with event-key filter and empty-state messaging; CSV export returns expected header+rows and header-only output when no data matches.

Notes:
Selected custom database record storage (not content nodes/members) with uniqueness on `(eventKey, normalizedEmail)`. Backoffice visibility is provided at `/umbraco/backoffice/registrations` with CSV export link and filter support.

---

## EVENTS-008

Status:
VERIFIED

Files Changed:
- `Program.cs`
- `Features/Events/EventRegistrationsBackofficeController.cs`
- `.github/ai/TASKS.md`
- `.github/ai/STATUS.md`
- `.github/ai/DECISIONS.md`
- `.github/ai/EVIDENCE.md`

Commands Run:
- `dotnet build MyProject.sln`
- `Invoke-WebRequest -Uri 'https://localhost:44304/events/registrations' -UseBasicParsing`
- `Invoke-WebRequest -Uri 'https://localhost:44304/events/registrations.csv' -UseBasicParsing`

Verification:
Final review completed. Access-control hardening verified: anonymous requests to registration listing and CSV export return `401`. Build succeeds after review changes. No remaining critical architecture issues identified for current PoC scope.

Notes:
Residual non-blocking risks documented: simplistic email validation and no anti-automation controls on public registration submissions. Both are acceptable for current PoC but should be revisited before production hardening.

---

## EVENTS-009

Status:
VERIFIED

Files Changed:
- `Features/Events/EventRegistrationsBackofficeController.cs`
- `wwwroot/App_Plugins/events-registrations/umbraco-package.json`
- `wwwroot/App_Plugins/events-registrations/event-registrations-workspace-view.element.js`
- `.github/ai/TASKS.md`
- `.github/ai/STATUS.md`
- `.github/ai/DECISIONS.md`
- `.github/ai/EVIDENCE.md`

Commands Run:
- `dotnet build MyProject.sln`
- `Invoke-WebRequest -Uri 'https://localhost:44304/umbraco/backoffice/registrations/api' -UseBasicParsing`
- `Invoke-WebRequest -Uri 'https://localhost:44304/App_Plugins/events-registrations/umbraco-package.json' -UseBasicParsing`
- `Invoke-WebRequest -Uri 'https://localhost:44304/App_Plugins/events-registrations/event-registrations-workspace-view.element.js' -UseBasicParsing`

Verification:
Build succeeded after clearing the app-host lock. The new authenticated JSON endpoint returns `401` anonymously. The package manifest and workspace-view module are both served successfully from the running site.

Notes:
Implemented a native `workspaceView` extension for `eventPage` that consumes the current content workspace context, polls the registrations API, renders a live participant table, and links out to the standalone dashboard for export workflows.

Follow-up fix:
Removed TypeScript-only syntax (`override`, `declare global`) from `wwwroot/App_Plugins/events-registrations/event-registrations-workspace-view.element.js` after confirming it caused the registrations workspace tab to render blank in backoffice. Also normalized manifest values to an absolute element path and `"/registrations"` pathname.

Follow-up fix 2:
Replaced generic `User.Identity.IsAuthenticated` checks in `Features/Events/EventRegistrationsBackofficeController.cs` with `HttpContext.AuthenticateBackOfficeAsync()` so backoffice requests authenticate against the correct Umbraco scheme. Updated the workspace view dashboard action to open with `window.open(..., '_blank')` to avoid replacing the current editor panel.

Follow-up fix 3:
Removed the standalone dashboard implementation by deleting `Index` and `Event` actions from `Features/Events/EventRegistrationsBackofficeController.cs`, deleting `Views/Backoffice/EventRegistrations.cshtml`, deleting `Features/Events/EventRegistrationsDashboardModel.cs`, and removing the `Open dashboard` UI action from the EventPage workspace view.

Route verification:
- `GET /umbraco/backoffice/registrations?eventKey=...` returns `404`
- `GET /umbraco/backoffice/registrations/api?eventKey=...` remains available and returns `401` for anonymous requests

---

# Evidence Rules

For each completed task record:

- task ID
- files changed
- commands executed
- verification performed
- blockers encountered
- important implementation notes

Keep entries short and factual.