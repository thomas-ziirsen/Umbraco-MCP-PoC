# Task Queue

## EVENTS-001
Status: COMPLETE

Phase:
Discovery And Structure

Goal:
Define the overall Umbraco architecture and rendering strategy for the music events PoC.

Tasks:
- Review existing Umbraco structure and conventions
- Confirm event content modeling approach
- Decide between page-based, block-based, or mixed rendering
- Confirm frontend experience scope:
  - listing only
  - or listing plus detail pages
- Define initial content hierarchy
- Record architecture decisions in DECISIONS.md

Owner:
Architect

Verification:
- Architecture aligns with current Umbraco solution structure
- Rendering strategy is clearly documented
- Content hierarchy supports future frontend implementation

---

## EVENTS-002
Status: COMPLETE

Phase:
Backoffice Implementation

Goal:
Create the required Umbraco content structure for events and ticket purchasing.

Tasks:
- Create Event document type plan
- Define required event fields
- Define ticket purchase structure
- Define media/image handling requirements
- Ensure editor authoring flow is simple and clear
- Prepare rendering integration requirements

Owner:
Backend

Verification:
- Content structure supports event rendering requirements
- Editor workflow is understandable
- Structure follows Umbraco conventions

---

## EVENTS-003
Status: COMPLETE

Phase:
Backoffice Implementation

Goal:
Wire published Umbraco content into the rendering layer.

Tasks:
- Connect event content to Razor rendering
- Create backend mapping logic if needed
- Ensure content can be rendered safely
- Handle missing or unpublished content
- Prepare frontend consumption structure

Owner:
Backend

Verification:
- Published content renders successfully
- Null/empty states handled safely
- Rendering integration works with current solution structure

---

## EVENTS-004
Status: COMPLETE

Phase:
Frontend Implementation

Goal:
Build the main event listing experience.

Tasks:
- Create Razor partial for event listing
- Build event card layout
- Add typography styling
- Add graphics and visual hierarchy
- Implement responsive layout behavior
- Add motion/animation effects

Owner:
Frontend

Verification:
- Layout renders correctly
- Responsive behavior works
- Animations load correctly
- Event cards display dynamic content

---

## EVENTS-005
Status: COMPLETE

Phase:
Frontend Implementation

Goal:
Build the event detail experience.

Tasks:
- Create event detail Razor view
- Render dynamic event information
- Add image/media presentation
- Add navigation back to listing
- Ensure responsive behavior
- Add motion polish where appropriate

Owner:
Frontend

Verification:
- Detail page renders correctly
- Dynamic content binds successfully
- Navigation works
- Responsive layout functions correctly

---

## EVENTS-006
Status: COMPLETE

Phase:
Validation

Goal:
Validate editor experience, rendering quality, and frontend stability.

Tasks:
- Test editor authoring flow
- Test empty states
- Test invalid content handling
- Test responsive behavior
- Test animation quality
- Test frontend rendering stability
- Record validation evidence
- Update STATUS.md and EVIDENCE.md

Owner:
Tester

Verification:
- Editor workflow confirmed
- Invalid content handled safely
- Responsive behavior verified
- Animations function correctly
- Validation evidence documented

---

## EVENTS-007
Status: COMPLETE

Phase:
Feature Expansion

Goal:
Allow frontend users to register/sign up for music events and manage registrations through Umbraco backoffice.

Tasks:
- Define the registration architecture and storage approach
- Decide whether registrations should use:
  - custom database records,
  - Umbraco content nodes,
  - members,
  - or a hybrid approach
- Define required registration fields:
  - name
  - email
  - phone (optional)
  - event reference
  - registration timestamp
  - consent fields if needed
- Define validation and duplicate registration handling
- Define frontend registration flow and UX
- Define success and failure states
- Create frontend registration form UI
- Add frontend validation
- Add backend registration handling
- Persist registration data safely
- Make registrations viewable from Umbraco backoffice
- Add registration overview/listing in backoffice
- Add CSV export support for registrations
- Define CSV format and export behavior
- Handle empty export states
- Verify registration flow end-to-end
- Verify CSV export functionality
- Document architecture decisions in DECISIONS.md
- Record implementation and validation evidence in EVIDENCE.md

Owner:
Architect

Verification:
- Users can register for an event from the frontend
- Validation errors display correctly
- Successful registrations are persisted
- Registrations are visible in Umbraco backoffice
- CSV export downloads correctly
- Export contains expected registration data
- Duplicate/invalid registrations are handled safely
- Responsive behavior works correctly
- Build and validation checks pass

---

## EVENTS-008
Status: COMPLETE

Phase:
Review And Sign-Off

Goal:
Perform final architecture and implementation review before sign-off.

Tasks:
- Review implementation consistency
- Review frontend maintainability
- Review backend structure
- Review Umbraco conventions
- Review technical risks
- Confirm PoC scope alignment
- Record final review notes

Owner:
Reviewer

Verification:
- No critical architectural issues remain
- Implementation aligns with agreed scope
- Risks documented
- Final review evidence recorded

---

## EVENTS-009
Status: COMPLETE

Phase:
Backoffice Extension

Goal:
Mount the event registrations table inside the EventPage editor as a native Umbraco workspace view.

Tasks:
- Add a workspace view manifest scoped to the `eventPage` content type
- Build a custom backoffice element that consumes the current EventPage workspace context
- Load registrations from the authenticated backoffice API endpoint
- Render a live participant table inside the editor
- Provide a link back to the standalone dashboard for full export workflows
- Update the task evidence and state files after validation

Owner:
Architect

Verification:
- Registrations tab appears only on EventPage editors
- The live participant table renders from the current event key
- Anonymous users cannot read the backing JSON endpoint
- Build or targeted validation passes for the touched slice

---

# Task Ownership

Architect:
- structure
- decisions
- orchestration
- scope control

Backend:
- content modeling
- Umbraco integration
- rendering integration
- backend validation

Frontend:
- Razor views
- styling
- motion
- responsive presentation

Tester:
- validation
- evidence collection
- confidence tracking

Reviewer:
- architecture review
- maintainability review
- risk checks
- sign-off validation

---

# Execution Rules

- Work one task at a time
- Prefer sequential execution over parallel execution
- Update STATUS.md when work begins
- Update EVIDENCE.md after verification
- Update DECISIONS.md when architecture changes
- Split large tasks into smaller tasks if needed
- Avoid repo-wide searches unless necessary
- Prefer targeted edits and targeted reads
- Verify work before marking tasks COMPLETE