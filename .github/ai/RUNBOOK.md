# Autonomous Project Execution Runbook

## Purpose

This document defines the operational workflow for AI agents working on this project.

The goal is reliable, incremental implementation progress with minimal unnecessary tool usage.

Agents should prioritize:
- small execution cycles,
- minimal file reads,
- verified work,
- stable tooling behavior,
- clear project state updates.

---

# Core Principles

- Prefer reliable progress over aggressive autonomy.
- Work in small, atomic tasks.
- Avoid unnecessary workspace-wide searches.
- Read only the files required for the current task.
- Verify work before marking tasks complete.
- Keep project state files lightweight and current.
- Do not fabricate file contents or project state.
- If tooling becomes unstable, stop safely and document the blocker.

---

# Active Project Files

These files should stay small and actively maintained.

## Required State Files

```text
.github/ai/TASKS.md
.github/ai/STATUS.md
.github/ai/EVIDENCE.md
.github/ai/DECISIONS.md
.github/ai/RUNBOOK.md
```

---

# Startup Sequence

When a session starts, read state with exact file paths and minimal scope.

Read in this order:

1. `.github/ai/TASKS.md`
2. `.github/ai/STATUS.md` if present
3. `.github/ai/RUNBOOK.md` only if execution rules are needed
4. `.github/ai/PRD.md` only if product scope is unclear
5. `.github/ai/SYSTEM_REQUIREMENTS.md` only if technical constraints are unclear
6. `.github/ai/DECISIONS.md` only if architecture context is required
7. `.github/ai/EVIDENCE.md` only when validating previous work

Do not load all markdown files by default.

---

# Execution Loop

Process one task at a time.

1. Read `.github/ai/TASKS.md`.
2. Select the next incomplete task.
3. Read only files needed for that task.
4. Complete the smallest useful step.
5. Verify the step.
6. Update `.github/ai/STATUS.md`.
7. Update `.github/ai/EVIDENCE.md`.
8. Update `.github/ai/DECISIONS.md` only for real architecture decisions.

Do not mark a task complete without evidence.

---

# Tool Stability Rules

If search/subagent tooling fails:

1. Switch to direct file reads with exact paths.
2. Retry direct read once if needed.
3. Use a targeted terminal read command only if direct reads fail.
4. Declare `BLOCKED` only after direct reads and terminal fallback both fail.

Never guess file contents.

---

# MCP Safe Call Guard

Use the local preflight validator before any MCP search call.

Script:

```text
scripts/Test-UmbracoMcpPayload.ps1
```

Example:

```powershell
.\scripts\Test-UmbracoMcpPayload.ps1 `
	-ToolName "mcp_umbraco-mcp_get-item-member-search" `
	-PayloadJson '{"query":"events"}'
```

If the payload is `{}` or missing `query` for known search endpoints,
the script fails fast and blocks the call locally.

## MCP Safe-Call Checklist

1. Confirm `UMBRACO_BASE_URL` points to the running local site.
2. Validate payload JSON before sending any MCP request.
3. For search endpoints, always send a non-empty `query`.
4. If exploring unknown content, use a broad query like `"a"`.
5. Treat any 400 response as payload shape mismatch first, auth second.
6. Log the exact tool name and payload used in troubleshooting notes.

---

# Verification Rules

Choose the lightest relevant verification:

- `dotnet build` for backend or Razor changes.
- Tests when test projects exist.
- Manual code inspection when tests are unavailable.
- Umbraco MCP checks only when task requires live content-model or content-state validation.

If verification is skipped, record the reason in `.github/ai/EVIDENCE.md`.

---

# Status And Evidence Rules

Keep state files concise and current.

`STATUS.md` should include:

- current task
- state (`PENDING`, `IN_PROGRESS`, `BLOCKED`, `COMPLETE`, `VERIFIED`)
- last action
- verification state
- blockers

`EVIDENCE.md` should include per completed task:

- task ID
- files changed
- commands executed
- verification result
- blockers/notes

---

# Blocker Protocol

When blocked:

1. Write the blocker clearly.
2. Record attempted reads/commands.
3. Update `STATUS.md` and `EVIDENCE.md` if possible.
4. Suggest the smallest safe next step.

Do not fabricate progress.