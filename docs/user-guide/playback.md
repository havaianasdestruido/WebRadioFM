---
title: Playback and library
description: Use the playlist and transport controls and understand their behavior.
---

# Playback and library

The main page combines a local-library playlist, active-track metadata, a playback progress display, and application-bar controls.

## Loading the library

WebRadioFM reads `MediaLibrary.Songs` once when `MainPage` is constructed. For each song, it copies:

- title, falling back to `Unknown`;
- artist, falling back to `Unknown Artist`;
- album, falling back to `Unknown Album`; and
- duration, falling back to zero.

The display list is sorted case-sensitively by the names supplied by the media library: artist first, then song name. A failed property read is caught so that one malformed library item does not abort the entire load.

:::note Refresh behavior
There is no playlist refresh button. If you add or remove songs while WebRadioFM is open, fully navigate away/relaunch the app to construct the player and load the library again.
:::

## Selecting and starting a track

Tap any playlist row. The selection handler passes its index to `AudioPlayerService.Play(index)` and immediately clears the visual selection.

If you press Play before selecting a row, the app starts index `0` when at least one track exists.

When playback begins, the main page:

1. updates the title, artist, album, and total duration;
2. resets progress and scrobble state;
3. starts a one-second UI timer;
4. optionally loves the track;
5. sends a Now Playing request when enabled; and
6. optionally requests Last.fm track information.

Metadata displayed in the Now Playing panel passes through configured edit rules. Playlist rows use the unedited `Track.DisplayText` and album values.

## Transport controls

### Play and pause

- While playing, the button calls `MediaPlayer.Pause()`.
- While paused, it calls `MediaPlayer.Resume()`.
- The application-bar icon tracks the service's `PlayStateChanged` event.

The timer keeps ticking after Pause, but scrobble checks run only when `IsPlaying` is true.

### Next

Normal mode increments the current index and wraps to zero. Shuffle mode chooses a random index from the whole playlist. Random selection can choose the current song again; there is no history or no-repeat queue.

### Previous

- If the current playback position is greater than three seconds, Previous restarts the current index.
- Otherwise it decrements the index, wrapping from zero to the final track.

Previous does not use shuffle mode.

### Shuffle

Shuffle is a Boolean mode, not a precomputed shuffled queue. Enabling it changes how **Next** selects the next index. The highlighted shuffle icon indicates the current mode.

## Progress display

Every second, `MainPage` calls `AudioPlayerService.UpdateProgress()` and then reads the service's `Position`/`Duration` properties directly. (The page does not currently subscribe to `ProgressChanged`.) It renders:

- current time in `mm:ss` format;
- total track duration; and
- `position / duration` on a 0–1 slider.

The slider has `IsHitTestVisible="False"`; it is not a seek control.

## Track information

Choose **track info** from the main application-bar menu to show artist, title, album, and duration. These values come from the original `Track` object, not the transformed Last.fm metadata.

## Love and unlove

The heart button is available only when a track exists and a Last.fm session is authenticated.

- A gray heart becomes accented immediately, then `track.love` is called.
- An accented heart becomes gray immediately, then `track.unlove` is called.

The UI is optimistic: request errors are ignored by the page, and love state is reset to `false` whenever the track changes. It does not first query Last.fm for the track's existing loved state.

## End-of-track behavior

`AudioPlayerService` follows the static XNA `MediaPlayer` and listens for `ActiveSongChanged`. The service declares a `PlaylistEnded` event but the current implementation never raises it. Queue progression therefore depends on `MediaPlayer` behavior rather than an application-level end handler.

## Playlist ordering and playback mapping

The visible `Track` list is sorted by artist and title. During that same sorted enumeration, `AudioPlayerService` stores each original XNA `Song` at the matching index in a private mapping. `Play(index)` uses the mapped song, and active-song events resolve against the same mapping, so playback, displayed metadata, and scrobbling remain aligned when the media library's original order differs. See [Audio architecture](../architecture/audio-playback.md).
