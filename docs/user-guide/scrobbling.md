---
title: Scrobbling behavior
description: Configure Now Playing, scrobble eligibility, timing, blocked entries, and retry behavior.
---

# Scrobbling behavior

A **Now Playing** update and a **scrobble** are separate Last.fm operations. WebRadioFM can send the former when playback begins and the latter after enough of an eligible track has played.

## Now Playing

A track change triggers `SendNowPlaying()` when all three conditions are true:

- a Last.fm session is authenticated;
- **Submit Now Playing** is enabled; and
- **Scrobble enabled** is enabled.

The request sends transformed artist, track, album, and total duration to `track.updateNowPlaying`. A success callback writes **Now Playing on Last.fm** to the status line. A failure clears that line.

Disabling scrobbling also disables Now Playing even when the dedicated Now Playing checkbox is on.

## Scrobble eligibility

The one-second progress tick exits without a submission when any of these is true:

- no authenticated session exists;
- no active track exists;
- scrobbling is disabled;
- a request for the current attempt is already pending;
- that `Track` object has already been marked scrobbled;
- duration is zero; or
- duration is less than **Minimum track duration**.

Metadata edits and blocking are evaluated only after these checks.

## Exact timing formula

The configured delay is calculated as:

```csharp
configuredDelay = Math.Min(
    ScrobbleDelaySecs,
    durationSeconds * ScrobbleDelayPercent / 100
);
```

The track becomes ready if either:

```text
progress >= 50%
OR
positionSeconds >= configuredDelay
```

Therefore, the effective earliest threshold is:

```text
min(50% of duration, configured seconds, configured percentage of duration)
```

### Examples

| Duration | Seconds | Percentage | Effective trigger |
|---:|---:|---:|---:|
| 240 s | 180 s | 50% | 120 s (50%) |
| 600 s | 180 s | 50% | 180 s |
| 300 s | 180 s | 30% | 90 s |
| 300 s | 300 s | 80% | 150 s (hard-coded 50%) |

:::note Percentage values above 50
The settings slider allows up to 95%, but the explicit `progressPercent >= 0.5` condition means the app never waits beyond half the track solely because a higher percentage was selected.
:::

## Setting ranges

`SettingsManager` clamps values even if callers bypass the sliders:

| Setting | Default | Allowed |
|---|---:|---:|
| Delay seconds | 180 | 30–360 seconds |
| Delay percentage | 50 | 30–95% |
| Minimum duration | 30 | 10–60 seconds |

## Metadata transformation

Before Now Playing or scrobble submission, WebRadioFM transforms title, artist, and album independently:

1. apply literal find/replace rules in saved order;
2. apply regex replacement rules in saved order; then
3. use the transformed artist and title for blocking.

The local media file and playlist are not edited. See [Metadata rules](./metadata-rules.md).

## Blocking

Each non-empty line in **Blocked Metadata** is treated as a case-insensitive substring. The app builds:

```text
artist - title
```

and blocks the scrobble when that combined transformed text contains any saved line. A blocked track is marked complete for the current play and the status becomes **Blocked**; it is not retried.

## Submission data

`track.scrobble` includes:

- transformed artist;
- transformed track title;
- transformed album when non-empty;
- duration in whole seconds;
- Unix timestamp derived from the wall-clock time at the track-change event;
- API key and session key; and
- MD5 API signature.

Pauses do not change `_playStartTime`, so the timestamp remains the original track-change time.

## Success and retry behavior

Before making the HTTP request, the page sets `_scrobblePending = true` and marks `_currentScrobbledTrack`.

- **Success:** status becomes **Scrobbled!**; the track remains marked and cannot submit again during that play.
- **Failure:** status becomes **Scrobble failed** and `_scrobblePending` resets to false. The same play is not retried: `CheckScrobble()` assigned `_currentScrobbledTrack` before calling `ScrobbleAsync()`, so the equality guard blocks the next eligible timer tick.

:::warning Current retry caveat
Although the error handler clears `_scrobblePending`, it does **not** clear `_currentScrobbledTrack`. The earlier equality check prevents an actual retry for that `Track` object. A new track-change event resets the marker.
:::

## Status labels

| Label | Meaning |
|---|---|
| `Last.fm scrobbling on` | Authenticated; scrobbling and Now Playing enabled |
| `Scrobbling on (no NP)` | Authenticated; scrobbling enabled; Now Playing disabled |
| `Last.fm scrobbling off` | Authenticated; scrobbling disabled |
| `Now Playing on Last.fm` | Last Now Playing request succeeded |
| `Scrobbled!` | Last scrobble callback succeeded |
| `Blocked` | A configured block substring matched |
| `Scrobble failed` | Request/API error occurred |
