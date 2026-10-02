---
title: First run
description: Verify local playback, connect Last.fm, and choose initial preferences.
---

# First run

Use this checklist after installing or deploying WebRadioFM.

## 1. Verify library access

At launch, `AudioPlayerService.LoadSongsFromMusicLibrary()` reads the XNA media library. Songs are displayed by artist and title. Tap a row to begin playback.

If no tracks appear:

- confirm the phone/emulator has local songs;
- confirm `ID_CAP_MEDIALIB_AUDIO` remains in the manifest;
- relaunch after adding music; the app does not monitor library changes continuously; and
- remember that cloud-only catalog entries may not be exposed as local `MediaLibrary.Songs` items.

## 2. Verify transport controls

Use the application bar:

- **play/pause** starts track zero when nothing has been selected, then toggles playback;
- **next** advances and wraps to the first track;
- **previous** restarts the current song after three seconds, otherwise selects the preceding song and wraps;
- **shuffle** makes each Next action choose a random playlist index.

The progress indicator is display-only; dragging it does not seek.

## 3. Connect Last.fm

Open the application-bar menu and choose **settings**. In **scrobbling → Accounts**:

1. Enter your Last.fm API key.
2. Enter the matching shared secret.
3. Tap **get token**.
4. Tap **authorize** and approve the application in the browser.
5. Return to WebRadioFM. Ensure the generated token is still in the token field.
6. Tap **connect**.
7. Confirm the status says **Connected as _username_**.

See [Last.fm setup](../user-guide/lastfm-setup.md) for the protocol and troubleshooting details.

:::caution Credentials after relaunch
Only the session key and username are persisted. The API key and shared secret remain in memory and are lost when the app process is recreated. Enter them again before making signed Last.fm calls after a cold launch.
:::

## 4. Review scrobble defaults

| Setting | Default | Effect |
|---|---:|---|
| Scrobble enabled | On | Allows eligible plays to be submitted |
| Submit Now Playing | On | Sends a status as soon as a track changes |
| Love on startup | Off | Automatically loves each newly active track when enabled |
| Delay | 180 seconds | One of the timing thresholds |
| Delay percentage | 50% | One of the timing thresholds |
| Minimum duration | 30 seconds | Shorter tracks never scrobble |
| Fetch album art | Off | Requests Last.fm track info; current UI does not display the result |

The actual timing condition has more nuance than the labels imply. Review [Scrobbling behavior](../user-guide/scrobbling.md) before tuning it.

## 5. Choose appearance

Open the **appearance** pivot in Settings to select one of ten accent names and dark/light mode. Theme changes update application resources immediately. Existing controls may not all re-render identically until their visual state changes or the page is revisited.

## 6. Check statistics

Open **scrobble stats** from the main-page menu or **visit stats page** from Settings. An authenticated account should show up to 20 recent tracks, top artists, and top tracks.

You're ready to use the app. For every player control, continue to [Playback](../user-guide/playback.md).
