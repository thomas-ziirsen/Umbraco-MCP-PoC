# Business Logic

## Core Domain Rules

- A music event is publishable content managed by editors in Umbraco.
- An event must have enough information to be meaningful on the frontend.
- Unpublished events must not appear publicly.

## Suggested Event Fields

- Event title
- Event date and time
- Venue name
- Short description
- Rich content or long description
- Hero or poster image
- Optional artist or lineup text

## Content Model Plan

### Content Hierarchy

- `Home`
- `EventsLandingPage`
- `EventPage`

### EventsLandingPage

- Purpose: editor-managed landing page that owns the public events listing.
- Allowed children: `EventPage` only.
- Required fields:
	- Page title
	- Intro heading
	- Intro summary
- Optional fields:
	- Empty state heading
	- Empty state body
- Rendering responsibility: query published child `EventPage` items, filter to upcoming events by default, and render cards ordered by ascending event date.

### EventPage

- Purpose: first-class published event detail page.
- Required fields:
	- Event title
	- Event date and time
	- Venue name
	- Summary
	- Hero image
	- Body content
- Optional fields:
	- Artist or lineup text
	- Ticket CTA label
	- Ticket CTA URL
	- Secondary supporting image
- Media handling:
	- Hero image should use the Umbraco media picker and be mandatory for card/detail presentation.
	- Additional imagery remains optional so editors can publish without gallery management.
- Rendering responsibility: expose card-friendly summary data for the listing and full rich content for the detail page.

## Editorial Workflow

- Editors create or open the `EventsLandingPage` from the content tree.
- Editors add a new `EventPage` beneath the landing page for each event.
- Editors fill the required summary and hero-image fields first so listing cards are always renderable.
- Editors may add optional lineup text and an outbound ticket CTA when relevant.
- Publishing an event should be enough to make it eligible for the frontend listing.

## Ticketing Rule

- The PoC does not implement in-site ticket purchasing or order tracking.
- Ticketing is represented only as an optional outbound CTA on the event detail page.

## Rendering Integration Requirements

- Listing templates should rely on child content under `EventsLandingPage` rather than global searches.
- Event cards must render safely when optional fields are missing.
- Events with missing required fields should fail closed in the UI instead of producing broken wrappers.
- Past events should be excluded from the primary listing by default.

## Rendering Rules

- If required event data is missing, the frontend should fail safely and avoid broken visual wrappers.
- If no events exist, the site should show an intentional empty state.
- Events should be ordered in a way that makes sense for visitors, likely by upcoming date.

## Editorial Assumptions

- Editors are comfortable with standard Umbraco content editing.
- The first PoC should optimize for ease of publishing over complex data normalization.

## Decisions To Confirm

- Canonical event storage location in the content tree.
- Whether expired events stay visible as archive content.
