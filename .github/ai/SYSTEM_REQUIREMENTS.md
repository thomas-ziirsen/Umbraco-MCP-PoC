# System Requirements

## Platform Constraints

- Keep the solution as a single Umbraco web project.
- Prefer existing Umbraco conventions and Razor-based rendering.
- Avoid adding unnecessary infrastructure or external dependencies for the PoC.

## Backoffice Requirements

- Editors must be able to create music events from the Umbraco backoffice.
- The content model should support at minimum: title, date, venue, summary, hero image, and body content.
- The authoring flow should remain understandable to non-technical users.

## Frontend Requirements

- Event presentation must be responsive.
- The UI should include polished graphics and smooth transitions.
- The experience should degrade acceptably when motion is reduced or unavailable.

## Engineering Requirements

- Keep implementation compatible with the existing partial-view structure.
- Follow the current Razor and guard-clause style.
- Keep package and configuration changes documented if introduced.
- For local Umbraco content-model and backoffice-state tasks, prefer the configured Umbraco MCP server over speculative instructions, assuming the local site is running.

## Open Questions

- Should events be standalone content nodes, block-based entries, or both?
- Should the PoC include a list view only, or list plus detail page?
- What exact frontend visual direction should the first implementation target?
