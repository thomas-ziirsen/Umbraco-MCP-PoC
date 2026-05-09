# Copilot instructions

## Build and local commands

- Restore/build the solution with `dotnet restore MyProject.sln` and `dotnet build MyProject.sln`.
- Run the site locally with `dotnet run --project MyProject.csproj --launch-profile "Umbraco.Web.UI"`. That launch profile is defined in `Properties\launchSettings.json` and uses `https://localhost:44304` and `http://localhost:59112` in Development.
- There is currently no test project in the solution. `dotnet test MyProject.sln --no-build` will not find tests, so there is no single-test command to use until a test project is added.
- There is no lint command or lint-specific configuration in this repository.

## High-level architecture

- `Program.cs` contains the full application startup. The app is a single ASP.NET Core / Umbraco web project that registers Umbraco with `.AddBackOffice()`, `.AddWebsite()`, and `.AddComposers()`, then exposes both backoffice and website middleware/endpoints through `app.UseUmbraco()`.
- There are no custom controllers, composers, or services in the repository yet. Most behavior is currently convention-based Umbraco behavior plus Razor partials.
- `MyProject.csproj` is the only project in `MyProject.sln`. Package identities live there, while package versions are centralized in `Directory.Packages.props`.
- The current customization surface is the Razor partial structure under `Views\Partials\...`:
  - Block Grid rendering flows through `Views\Partials\blockgrid\default.cshtml` -> `items.cshtml` -> `Views\Partials\blockgrid\Components\<element-alias>.cshtml`, with `areas.cshtml` and `area.cshtml` handling nested areas.
  - Block List rendering flows through `Views\Partials\blocklist\default.cshtml` -> `Views\Partials\blocklist\Components\<element-alias>.cshtml`.
  - Single Block rendering starts in `Views\Partials\singleblock\default.cshtml`, first tries `singleBlock\Components\<element-alias>.cshtml`, and falls back to `blocklist\Components\<element-alias>.cshtml`.
- Runtime behavior is shaped by `appsettings.json` and `appsettings.Development.json`. This repo enables unattended Umbraco upgrades, allows invariant editing from non-default languages, disables concurrent logins, throws macro errors in Development, and enables Umbraco hosting debug mode in Development.
- Treat `umbraco\` data/logs and `wwwroot\media\` as local runtime state. They are ignored by git and should not be treated as source-controlled application code.

## Key conventions

- Keep NuGet version changes in `Directory.Packages.props`; `MyProject.csproj` should stay focused on project settings and package references.
- Match block editor partial names to Umbraco element aliases. Adding a new block element in Umbraco usually requires a matching partial in the appropriate `Views\Partials\...\Components\` folder or rendering will fail or fall back.
- Preserve the typed Umbraco Razor pattern already used here: `@inherits UmbracoViewPage<T>` plus the shared imports from `Views\_ViewImports.cshtml` (`Umbraco.Extensions`, published models, MVC tag helpers).
- Follow the existing guard-clause style in partials (`if (Model?.Any() != true) { return; }`, `if (Model.ContentKey == Guid.Empty) { return; }`) instead of rendering empty wrappers.
- Do not remove the Razor settings in `MyProject.csproj` unless the Umbraco ModelsBuilder mode changes. The project currently disables Razor compile on build/publish and copies generated Razor files to publish output to support the current Umbraco setup.
- If MCP tooling is used locally, `.vscode\mcp.json` already defines Chrome DevTools and an Umbraco MCP server. Keep its `UMBRACO_BASE_URL` aligned with the active local launch URL.
- Prefer the Umbraco MCP server for live/local Umbraco operations such as inspecting or managing documents, media, document types, and data types. Prefer source edits for application code, Razor partials, styling, and configuration.
- When using the Umbraco MCP server, assume the local site must be running and reachable at the configured `UMBRACO_BASE_URL` before relying on MCP-backed checks or mutations.
