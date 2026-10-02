---
title: Listening statistics
description: Browse recent tracks, top artists, and top tracks from Last.fm.
---

# Listening statistics

The **scrobble stats** page presents three Last.fm views in a Windows Phone Pivot control.

## Open and refresh

Open the page from either:

- **scrobble stats** in the main page's application-bar menu; or
- **visit stats page** in Settings → about.

The constructor immediately loads all three views. Navigating back to the page also refreshes recent tracks. Use the application-bar refresh button to reload all views while preserving selected top-list periods.

## Recent tracks

The app calls `user.getRecentTracks` for the authenticated username with `limit=20`.

Each item can show track name, artist, album, Last.fm date text, and **now playing** when the response has `@attr.nowplaying == "true"`. The models can also deserialize images, MBIDs, loved state, and paging attributes, but the page does not display them.

## Top artists and tracks

Both lists request 20 entries. The period buttons map directly to Last.fm API values:

| Label | API period |
|---|---|
| 7 days | `7day` |
| 1 month | `1month` |
| 3 months | `3month` |
| overall | `overall` |

Top-artist rows show artist and play count. Top-track rows show track, artist, and play count.

## Authentication behavior

When `App.LastFm.IsAuthenticated` is false, each list's item source is set to `null`. The page does not show a dedicated sign-in prompt.

A loaded session also needs a populated API key to form valid calls. Because API credentials are not persisted, a page opened after a cold launch can be “authenticated” locally but still fail API requests until credentials are entered again in Settings.

## Errors and loading state

Current page-level error callbacks are empty:

```csharp
(error) => { }
```

As a result:

- no loading indicator appears;
- no error message appears;
- previous data may remain after a failed refresh; and
- parse errors look like an empty or unchanged view.

For diagnosis, debug `LastFMService.MakeRequest`, `HandleResponse`, or the relevant deserialization callback.

## Request concurrency

Opening the page starts three requests nearly together, below the service limit of four active requests. Pressing Refresh before those requests complete can make later requests fail with **Too many concurrent requests**, but that error is not surfaced by `StatsPage`.
