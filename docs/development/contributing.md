---
title: Contributing
description: Contribution workflow, design constraints, and review checklist for WebRadioFM.
slug: /development/contributing
---

# Contributing

WebRadioFM combines a retired phone stack with a modern documentation site. Changes should preserve that distinction and avoid claiming compatibility that has not been tested.

## Before changing code

1. Read [Requirements](../getting-started/requirements.md) and [Architecture](../architecture/overview.md).
2. Identify whether the change affects the C#/XAML application, Docusaurus, or both.
3. Preserve API credentials and personal XAP signing material outside the repository.
4. Open an issue for broad behavior or compatibility changes when project collaboration is available.

## Repository workflows

### Documentation-only changes

Documentation is cross-platform:

```bash
npm install
npm start
npm run build
```

The production build checks routes, links, MDX syntax, and Mermaid rendering.

### Application changes

Application compilation requires Windows and the legacy Windows Phone targets:

```powershell
.\build.ps1 -Configuration Debug -Platforms x86
.\build.ps1 -Configuration Release -Platforms ARM
```

Run the matching emulator/device deployment and test the changed path manually.

## Coding conventions

The current application uses a deliberately old-compatible C# style, but some source already uses newer syntax. For changes intended to compile with the historical toolchain:

- prefer explicit event subscription/unsubscription;
- keep UI mutations on `Deployment.Current.Dispatcher`;
- retain callback-based APIs unless changing the toolchain is an explicit project goal;
- keep JSON contracts aligned with Last.fm property names;
- preserve `IDisposable` cleanup for static XNA player events;
- avoid introducing NuGet dependencies without checking Windows Phone Silverlight support; and
- verify language-version compatibility with the actual compiler.

Do not silently catch new errors unless there is a documented recovery path. Existing empty catches are compatibility/robustness tradeoffs, not a pattern to expand.

## UI changes

For XAML/code-behind changes:

- test WVGA, WXGA, and 720p manifest resolutions;
- keep portrait-only behavior unless both XAML and manifest expectations are revised;
- check dark and light modes and all accent-dependent controls;
- use existing phone resources rather than fixed foreground/background colors;
- confirm application-bar icon files are included as `Content`; and
- put user-facing strings in resources when extending localization.

## Last.fm changes

- Never hard-code a personal API key or shared secret.
- Prefer HTTPS after validating it on the target runtime.
- Keep API signature inputs separate from transport-only parameters such as `format`.
- Inspect both HTTP errors and Last.fm JSON errors.
- Maintain the four-request accounting invariant: every increment must have exactly one completion decrement.
- Redact session keys, tokens, signatures, and secrets from logs and issue reports.

## Documentation expectations

Behavior changes should update the relevant:

- user guide;
- architecture page;
- API reference table;
- troubleshooting notes; and
- root README if onboarding commands or platform requirements change.

Write what the code actually does. Use an admonition for legacy limitations or known gaps rather than presenting intended future behavior as current behavior.

## Review checklist

- [ ] Application builds with the intended legacy environment, or the PR clearly states why it could not be compiled.
- [ ] Manual test scenario and destination architecture are recorded.
- [ ] No credentials or generated build directories are included.
- [ ] New files are represented in `WebRadioFM.csproj` when required by old MSBuild.
- [ ] Event handlers and network request counts are cleaned up on every path.
- [ ] Dark/light appearance and navigation are checked for UI changes.
- [ ] `npm run build` succeeds for documentation changes.
- [ ] Links and source paths in docs are current.

## Reporting bugs

Include reproduction steps, runtime/tool versions, architecture, and a redacted exception/API error. Do not publish account credentials or signed request URLs. See [Troubleshooting](../user-guide/troubleshooting.md) for subsystem-specific checks.
