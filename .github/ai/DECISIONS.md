# Decisions

## Purpose

Record technical and product choices so the implementation does not drift.

## Decision Log Template

### D-001

- Date: 2026-05-09
- Topic: Initial AI workflow scaffolding
- Decision: Add project-specific agent files and AI planning documents focused on the Umbraco music events PoC.
- Rationale: The repository had empty placeholders; structured guidance is needed before implementation work begins.
- Consequence: Future implementation tasks should update these files rather than recreate planning context from scratch.

### D-002

- Date: 2026-05-09
- Topic: Local Umbraco MCP usage
- Decision: Treat the configured Umbraco MCP server as the preferred tool for inspecting or mutating local Umbraco content state, document types, media, and data types.
- Rationale: These tasks target running CMS state and are more reliable through MCP than through guessed instructions or source-only edits.
- Consequence: Agent prompts and implementation work should distinguish between source-controlled code changes and MCP-managed local CMS changes.

## Pending Decisions

- Choose the Umbraco content modeling approach for music events.
- Choose the initial frontend visual concept.
- Decide whether event detail pages are part of the first PoC slice.

### D-003

- Date: 2026-05-09
- Topic: Music events content modeling and routing
- Decision: Model the events experience as page-based Umbraco content with an `EventsLandingPage` container and child `EventPage` nodes rather than block-only event entries.
- Rationale: The current solution is a conventional single-project Umbraco site with website routing enabled, no custom controller/service layer, and only shared block partial infrastructure. Page-based content aligns with Umbraco conventions, keeps editor authoring discoverable, and gives the PoC a native list-plus-detail route structure without adding custom routing complexity.
- Consequence: Backend work should define document types and templates around an events landing page and detail page relationship. Reusable rich content inside an event can still use existing block editor patterns where helpful, but events themselves should be first-class content nodes.

### D-004

- Date: 2026-05-09
- Topic: Initial public events experience scope
- Decision: Include both an events listing page and event detail pages in the first PoC slice.
- Rationale: The task plan already separates listing and detail implementation, and the product goal is not just event authoring but a public-facing browsing experience. A detail page supports richer body content, hero media, and navigation without overloading the listing view.
- Consequence: Frontend and backend tasks should assume two rendering surfaces: a landing template for aggregated upcoming events and an event template for individual event content.

### D-005

- Date: 2026-05-09
- Topic: Initial content hierarchy and ticketing scope
- Decision: Use a simple hierarchy of `Home` -> `EventsLandingPage` -> `EventPage`, with ticket purchasing represented only as optional outbound CTA data on the event and not as an in-site purchase workflow.
- Rationale: The PRD marks ticket purchasing flows as out of scope, while the system requirements only require core publishing fields and an understandable editor workflow. A minimal hierarchy reduces editorial friction and keeps the PoC aligned with scope.
- Consequence: The event model should prioritize title, date, venue, summary, hero image, body content, and optional ticket link/label fields instead of implementing cart or checkout structures.

### D-006

- Date: 2026-05-09
- Topic: EventPage document type field schema in Umbraco
- Decision: Update existing `eventPage` document type to include required fields (`eventTitle`, `eventDate`, `venueName`, `summary`, `heroImage`, `bodyContent`) and optional fields (`artistLineup`, `ticketCtaLabel`, `ticketCtaUrl`, `secondarySupportingImage`) in a single `Content` tab.
- Rationale: These fields match the `#sym:EventPage` business requirements and align with the frontend mapper aliases used in `Features/Events/EventContentModels.cs`.
- Consequence: Backoffice editors can author complete event content without custom code changes, and frontend rendering can consume the expected aliases directly.

### D-007

- Date: 2026-05-09
- Topic: Event registration storage and operational model (EVENTS-007)
- Decision: Implement registrations as custom database records (single app database table) with an internal service layer, while keeping `EventPage` as the source of event metadata; do not model registrations as Umbraco content nodes or members.
- Rationale: Registrations are transactional user submissions (name/email/phone/consent/timestamp) that are better suited to tabular querying, duplicate checks, and CSV export than tree content. Using content nodes would add editorial noise and weak query ergonomics; using members introduces account semantics that are out of scope for this PoC.
- Consequence: Backend work should add a small registration entity/repository flow, server-side validation (required fields, email format, duplicate policy), and endpoints for create/list/export. Backoffice visibility should be read-only listing plus CSV export actions, with empty-state handling and event-based filtering.

### D-008

- Date: 2026-05-09
- Topic: Duplicate registration behavior (EVENTS-007)
- Decision: Define duplicates by `(eventKey, normalizedEmail)` and reject repeat submissions with a clear user-safe message; allow same email across different events.
- Rationale: This policy is simple for users, minimizes accidental double-booking noise, and supports deterministic export/reporting per event.
- Consequence: Registration create flow must normalize email, enforce uniqueness at service/database level, and return a non-technical duplicate response state to the frontend.

### D-009

- Date: 2026-05-09
- Topic: Access control for registration listing and CSV export
- Decision: Restrict `/events/registrations` and `/events/registrations.csv` to authenticated users only, and require authorization on the backoffice registrations dashboard controller.
- Rationale: Registration records contain personal data (name/email/phone). Anonymous access to listing/export endpoints is a critical data-exposure risk.
- Consequence: Public users can submit registrations but cannot read/export registration data; review and operational workflows must occur through authenticated backoffice sessions.

### D-010

- Date: 2026-05-09
- Topic: Native EventPage registrations tab in Umbraco backoffice
- Decision: Implement the registrations experience as a `workspaceView` extension scoped to the `eventPage` content type, backed by an authenticated JSON endpoint for live rows, rather than embedding another standalone dashboard route in the editor.
- Rationale: Umbraco 17 workspace views are the native tab mechanism for content editors. Using a workspace view keeps the registrations table inside the EventPage editor, follows the same extension pattern as the built-in document/member views, and avoids iframe-style embedding.
- Consequence: The project now has a dedicated backoffice package manifest and a custom element that consumes the current content workspace context. The standalone dashboard can remain as a full-page fallback for export workflows, but the editor integrates the live participant table directly.
