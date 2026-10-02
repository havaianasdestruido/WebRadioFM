---
title: Debugging and testing
description: Manual verification strategy and debugging entry points for the legacy application.
---

# Debugging and testing

The repository has no automated application test project. Testing is currently manual, with Docusaurus production builds providing automated validation only for documentation.

## Debug build behavior

When `Debugger.IsAttached`, `App`:

- enables the Silverlight frame-rate counter;
- disables user idle detection; and
- breaks on unhandled application exceptions and navigation failures.

Build and deploy a `Debug` configuration with Visual Studio to use those hooks.

## High-value breakpoints

| Subsystem | Breakpoints |
|---|---|
| Startup | `App.App`, `InitializePhoneApplication`, `InitializeLanguage` |
| Library | `AudioPlayerService.LoadSongsFromMusicLibrary`, `Play` |
| XNA state | `OnMediaStateChanged`, `OnActiveSongChanged` |
| Track pipeline | `MainPage.OnTrackChanged`, `ProgressTimerTick`, `CheckScrobble` |
| Authentication | `SettingsPage.GetToken_Click`, `GetSessionAsync`, `ParseSession` |
| Requests | `LastFMService.MakeRequest`, `HandleResponse`, `GetErrorFromJson` |
| Persistence | `SettingsManager.Get/Set`, `LoadSession`, `SaveSession` |
| Stats | `StatsPage.LoadRecentTracks`, `LoadTopArtists`, `LoadTopTracks` |

## Minimum playback matrix

Test with:

- an empty media library;
- one track;
- multiple artists/albums;
- missing artist/album metadata;
- a zero/very short duration if the library permits it;
- normal Next wrap;
- Previous before and after three seconds;
- shuffle with one and multiple tracks;
- pause/resume; and
- externally triggered XNA active-song changes where possible.

Compare tapped playlist metadata with the song that actually starts to expose the sorted/original-index mismatch.

## Scrobble matrix

Use a dedicated Last.fm test account and short permitted thresholds:

| Case | Expected |
|---|---|
| Not authenticated | No Now Playing/scrobble call |
| Scrobbler off | Neither Now Playing nor scrobble |
| Now Playing off | Scrobble only |
| Duration below minimum | Never scrobble |
| Literal edit | Transformed API/display value |
| Regex edit | Applied after literal rules |
| Matching block line | `Blocked`, no scrobble |
| API success | `Scrobbled!` |
| API failure | `Scrobble failed`; known retry guard remains |
| Cold launch with saved session | Re-enter API credentials before signed calls |

Do not rapidly submit real duplicate scrobbles. Use controlled metadata and remove test activity from the account if needed.

## Network inspection

Use a debugger/proxy only in a controlled environment. The current service uses HTTP and puts parameters in URLs; captured traffic contains sensitive session material.

Inspect:

- alphabetic signature input (excluding `format`);
- URL encoding;
- POST body and query duplication;
- status/exception path;
- API JSON errors; and
- `_activeRequests` returning to zero after every completion.

Redact all secrets before saving captures.

## Settings testing

Verify default, minimum, and maximum values. Because setters clamp but deserialized stored values are returned directly, test behavior when isolated settings contain wrong types or out-of-range legacy values. `Get<T>` catches cast failures and falls back to defaults.

Test the one existing migration by preparing `LastFmScrobblingEnabled` without `SettingsVersion`, then confirming it becomes `ScrobblerEnabled` and version `1`.

## Theme and layout

Check each page in dark and light mode and at manifest resolutions:

- WVGA;
- WXGA; and
- 720p.

Check long track/artist names, empty stats, keyboard overlap on multi-line settings boxes, and Pivot horizontal content.

## Documentation validation

Run:

```bash
npm run build
```

This catches invalid front matter/MDX, duplicate routes, missing linked docs, Mermaid failures, and broken internal links configured as fatal. Also inspect at mobile and desktop widths with:

```bash
npm start
```

## Recording results

A useful change report states:

- build configuration and platform;
- physical device/emulator image;
- app/toolchain version;
- cases exercised;
- pass/fail and observed status text; and
- known cases not run.
