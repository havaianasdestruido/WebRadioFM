---
title: Troubleshooting
description: Diagnose installation, playback, Last.fm, metadata, statistics, and appearance issues.
---

# Troubleshooting

Start with the symptom, then use a Debug deployment when the suggested checks do not resolve it.

## Installation and startup

### The XAP will not deploy

- Confirm package architecture: ARM is for a physical phone.
- Confirm the destination is developer-unlocked/provisioned.
- Confirm you are using a Windows Phone XAP deployment tool, not modern APPX/MSIX tooling.
- Rebuild against the actual Windows Phone 8.1 targets rather than only current .NET tooling.
- Check that the product and publisher identities satisfy your preserved deployment environment.

### The app fails before the first page

`App` initializes settings, `LastFMService`, theme resources, the root frame, and language. Attach Visual Studio and inspect:

- `Application_UnhandledException`;
- `RootFrame_NavigationFailed`;
- missing SDK/runtime assemblies;
- malformed XAML resources; and
- `AppResources.ResourceLanguage` / `ResourceFlowDirection` values.

When a debugger is attached, these handlers call `Debugger.Break()`.

## Music and playback

### “No music found”

The XNA `MediaLibrary.Songs` collection is empty or unavailable. Add local music, verify media-library capabilities, and relaunch.

### Tapping one row plays another song

The current service stores each sorted `Track` with a parallel XNA `Song` mapping. Confirm the deployed package includes that mapping and that no caller has added, removed, or reordered entries through the mutable `Playlist` property. See [playlist ordering and playback mapping](./playback.md#playlist-ordering-and-playback-mapping).

### Progress does not move

- Confirm XNA `MediaPlayer.State` is `Playing`.
- Confirm the one-second `DispatcherTimer` started in `OnTrackChanged`.
- Confirm duration is nonzero.
- Inspect `MediaPlayer.PlayPosition` and the `ProgressChanged` callback.

### Previous does not choose the prior song

After more than three seconds, this is intentional: Previous restarts the active index.

## Last.fm authentication

### Token request fails

- Recheck the key/secret pair.
- Remove leading/trailing spaces.
- Confirm network capability/connectivity.
- Verify Last.fm still accepts the endpoint/protocol used by this legacy client.
- Inspect the API error parsed by `GetErrorFromJson`.

### It says connected, but calls fail after relaunch

The session was loaded, but the API key and secret were not persisted. Re-enter both in Settings. The status is based only on whether a session key exists.

### “Too many concurrent requests”

The service permits four in-flight requests. Wait for requests to complete. Opening or refreshing Stats creates three calls, while player activity may create Now Playing or track-info calls.

## Scrobbling

### A track never scrobbles

Check, in order:

1. authenticated session and re-entered API credentials;
2. **Scrobble enabled**;
3. positive duration;
4. duration at or above the configured minimum;
5. transformed `artist - title` against blocked strings;
6. effective timing threshold; and
7. status text for **Blocked** or **Scrobble failed**.

### A failed scrobble does not retry

Current error handling clears `_scrobblePending` but leaves `_currentScrobbledTrack` equal to the current object. The equality guard prevents another attempt until track state is reset.

### A percentage over 50 still scrobbles halfway

The code has an independent `progressPercent >= 0.5` condition. Values over 50 cannot delay submission beyond halfway.

## Metadata rules

### A rule is visible but has no effect

- Use exactly `title`, `artist`, `album`, or `all` in lower case.
- Tap the matching **save** button.
- Confirm the pattern is non-empty.
- For literal rules, remember matching is case-sensitive.
- For regex rules, validate the expression separately; invalid patterns are silently ignored.
- Select a new track; transformation occurs on track change.

### Colons break a rule

The parser uses colon delimiters and splits into at most three fields. Redesign a regex/pattern that requires a colon or modify the settings format in code.

## Statistics

### Lists are blank

- Connect a Last.fm session.
- Re-enter the API key after a cold launch.
- Wait for existing requests to finish before Refresh.
- Debug the empty `StatsPage` error callbacks; the UI does not surface failures.
- Verify the JSON response still matches `LastFmStats.cs` data contracts.

## Appearance

### Some text does not update after switching theme

Theme changes replace application resources at runtime. Restarting the app reloads the initial phone resources before applying light mode. Revisiting a page alone does not restore the text styles that dark mode overrides; fixing the live transition requires `ThemeManager.ApplyTheme()` to restore the light-mode text styles.

## Collecting useful diagnostics

When reporting an issue, include:

- destination device/emulator and architecture;
- Windows Phone SDK and Visual Studio/MSBuild version;
- Debug output and exception stack trace;
- exact action sequence;
- whether the app was cold-launched or resumed;
- relevant settings (without credentials); and
- a redacted Last.fm error number/message.

Never attach API secrets, session keys, auth tokens, or unredacted signed URLs.
