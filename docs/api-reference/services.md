---
title: Service reference
description: Complete member reference for AudioPlayerService and LastFMService.
---

# Service reference

Namespace: `WebRadioFM.Services`.

## `AudioPlayerService : IDisposable`

Adapts XNA local music APIs to an event-based page service.

### Events

| Event | Delegate | Payload |
|---|---|---|
| `TrackChanged` | `Action<Track>` | Current copied track metadata |
| `PlayStateChanged` | `Action<bool>` | Whether service considers player active |
| `ProgressChanged` | `Action<TimeSpan>` | Current XNA play position |
| `PlaylistEnded` | `Action` | Declared but never raised |

### Properties

| Property | Type | Access | Description |
|---|---|---|---|
| `CurrentTrack` | `Track` | get | Valid indexed item or null |
| `IsPlaying` | `bool` | get | Mirrored play state |
| `IsShuffled` | `bool` | get/set | Changes Next selection algorithm |
| `Playlist` | `List<Track>` | get | Exposes mutable internal list |
| `Position` | `TimeSpan` | get | `MediaPlayer.PlayPosition` |
| `Duration` | `TimeSpan` | get | Current copied track duration or zero |

### Methods

| Signature | Behavior |
|---|---|
| `AudioPlayerService()` | Initializes fields/library and subscribes static XNA events |
| `void LoadSongsFromMusicLibrary()` | Rebuilds the sorted copied playlist and aligned XNA song map; sets index 0 when non-empty |
| `void Play(int index = -1)` | Optionally selects a valid mapped index; dispatches XNA play and service events |
| `void Pause()` | Pauses XNA and publishes false |
| `void Resume()` | Resumes XNA and publishes true |
| `void Stop()` | Stops XNA and publishes false |
| `void PlayNext()` | Random index in shuffle mode, otherwise increment/wrap; then `Play()` |
| `void PlayPrevious()` | Restarts after 3 seconds, otherwise decrement/wrap; then `Play()` |
| `void UpdateProgress()` | Publishes current position only when `_isPlaying` |
| `List<Track> GetTracksByArtist(string artist)` | Case-insensitive exact match, returning a new list |
| `void Dispose()` | Unsubscribes static media events once |

### Private handlers

- `OnMediaStateChanged` mirrors `MediaPlayer.State == MediaState.Playing`, then publishes state and progress.
- `OnActiveSongChanged` searches the aligned song map by identity, sets the corresponding shared index, and publishes track/state; exceptions are ignored.

`Play` catches playback exceptions and presents `MessageBox.Show`.

## `LastFMService`

Callback-based Last.fm API client and session store.

### Properties

| Property | Type | Access | Description |
|---|---|---|---|
| `Session` | `LastFmSession` | get | Current session object |
| `IsAuthenticated` | `bool` | get | Session exists and has a key |
| `ApiKey` | `string` | get/set | In-memory API key |
| `ApiSecret` | `string` | get/set | In-memory signature secret |

### Configuration/authentication

#### `void Configure(string apiKey, string apiSecret)`

Assigns credentials without validation or persistence.

#### `string GetAuthUrl(string token)`

Formats Last.fm authorization URL from base URL, `_apiKey`, and token. Values are not explicitly escaped in this method.

#### `void GetTokenAsync(Action<string> callback, Action<string> errorCallback)`

Signs/calls `auth.gettoken`. Success payload is token. Fails on API error or parser returning empty.

#### `void GetSessionAsync(string token, Action<LastFmSession> callback, Action<string> errorCallback)`

Signs/calls `auth.getSession`. On valid session, replaces `_session`, persists it, then invokes success.

### Track mutations

All methods first reject an unauthenticated session with `Not authenticated with Last.fm`.

| Signature | API method | Optional fields |
|---|---|---|
| `UpdateNowPlayingAsync(string artist, string track, string album, int duration, Action callback, Action<string> errorCallback)` | `track.updateNowPlaying` | album when non-empty |
| `ScrobbleAsync(string artist, string track, string album, int duration, DateTime timestamp, Action callback, Action<string> errorCallback)` | `track.scrobble` | album when non-empty |
| `LoveTrackAsync(string artist, string track, Action callback, Action<string> errorCallback)` | `track.love` | none |
| `UnloveTrackAsync(string artist, string track, Action callback, Action<string> errorCallback)` | `track.unlove` | none |

All include method, API key, session key, artist/track, and signature. Scrobble adds Unix timestamp and duration; Now Playing adds duration.

### Read methods

| Signature | Return callback | API method |
|---|---|---|
| `GetRecentTracksAsync(string username, int limit, Action<List<RecentTrackItem>> callback, Action<string> errorCallback)` | list or empty list | `user.getRecentTracks` |
| `GetTopArtistsAsync(string username, int limit, string period, Action<List<TopArtistItem>> callback, Action<string> errorCallback)` | list or empty list | `user.getTopArtists` |
| `GetTopTracksAsync(string username, int limit, string period, Action<List<TopTrackItem>> callback, Action<string> errorCallback)` | list or empty list | `user.getTopTracks` |
| `GetTrackInfoAsync(string artist, string track, Action<TrackInfoData> callback, Action<string> errorCallback)` | track info | `track.getInfo` |

`GetTrackInfoAsync` adds session username when non-empty but does not require authentication.

### Session persistence

| Method | Behavior |
|---|---|
| `LoadSession()` | Reads key and username if present; suppresses errors |
| `SaveSession()` | Private; writes key and username, then saves; suppresses errors |
| `ClearSession()` | Replaces session, removes both keys, saves; suppresses errors |

### Request internals

| Method | Description |
|---|---|
| `BuildRequestUrl(Dictionary<string,string>)` | Escapes every pair into API URL and appends `format=json` |
| `MakeRequest(...)` | Enforces four-request cap, creates/configures `HttpWebRequest`, writes POST or begins response |
| `HandleResponse(IAsyncResult)` | Reads JSON, decrements count, dispatches success/error |
| `GetErrorFromJson(string)` | Returns formatted nonzero API error or null |
| `ParseToken(string)` | Returns deserialized token or null |
| `ParseSession(string)` | Maps session response into `LastFmSession` or null |
| `ToUnixTimestamp(DateTime)` | Converts UTC difference from Unix epoch, flooring seconds |

### Nested `RequestState`

Private callback state with `Request`, `Callback`, `ErrorCallback`, and `OnComplete` auto-properties.
