---
title: API reference overview
description: Entry point for the WebRadioFM code-level reference.
---

# API reference overview

This reference documents the classes and declared members in the Windows Phone application. It is hand-maintained from source because the legacy project has no XML documentation generation or API-documentation build stage.

## Namespaces

| Namespace | Contents |
|---|---|
| `WebRadioFM` | Application, pages, resource bridge |
| `WebRadioFM.Services` | Audio and Last.fm service boundaries |
| `WebRadioFM.Helpers` | Signatures, settings, themes, converters |
| `WebRadioFM.Models` | Local and Last.fm response models |
| `WebRadioFM.Resources` | Generated resource accessor |

## Reference sections

- [Application and pages](./application-pages.md) — `App`, `MainPage`, `SettingsPage`, `StatsPage`, `LocalizedStrings`
- [Services](./services.md) — `AudioPlayerService`, `LastFMService`
- [Helpers](./helpers.md) — settings, metadata rules, signatures, themes, converters
- [Models](./models.md) — local `Track`, session, authentication, submission, statistics, track-info contracts
- [Manifests and resources](./manifests-resources.md) — project settings, capabilities, package identity, RESX and assets

## Calling conventions

### Callbacks

Last.fm operations do not return `Task`. They accept success and failure delegates:

```csharp
App.LastFm.GetTopTracksAsync(
    username,
    20,
    "7day",
    tracks => topTracksList.ItemsSource = tracks,
    error => MessageBox.Show(error));
```

Service callbacks are dispatched to the UI thread by `LastFMService` after HTTP completion. Immediate validation errors (such as unauthenticated track mutation) invoke the provided error callback synchronously in the public method.

### Nullability

The code predates nullable reference annotations. Treat reference return values, event delegates, callback arguments, model members, and settings values as potentially null unless a method guards them.

### Serialization names

Many Last.fm model properties intentionally use API-style lower case (`track`, `artist`, `name`) and are decorated with `DataMember(Name = ...)`. Renaming them requires preserving the data-member mapping and updating XAML bindings.

### Thread affinity

Page/control work belongs on the UI thread. The HTTP service dispatches completed callbacks. `AudioPlayerService` dispatches `Play`, but not every transport method, so callers should originate from the UI thread.

### Error strategy

The codebase mixes:

- error callbacks for HTTP/API operations;
- `MessageBox` for selected user-facing failures;
- empty catches for optional metadata/persistence/deserialization;
- debugger breaks for fatal app/navigation exceptions; and
- silent page callbacks in statistics and love/unlove.

The reference notes important per-member behavior but does not imply that caught errors are harmless.

## Public surface versus event handlers

XAML event handlers are private implementation methods but are documented because they define most application behavior. Public model properties are primarily serialization/binding surfaces, not a stable reusable library API.

## Source of truth

When reference text and code differ, code at the checked-out commit is authoritative. Update this section in the same change as member behavior or signatures.
