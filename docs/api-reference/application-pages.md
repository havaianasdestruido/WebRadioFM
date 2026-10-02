---
title: Application and page reference
description: Members and event handlers in App, MainPage, SettingsPage, StatsPage, and LocalizedStrings.
---

# Application and page reference

Source namespace: `WebRadioFM`.

## `App : Application`

Global Silverlight entry point and owner of shared application objects.

### Static properties

| Property | Type | Access | Description |
|---|---|---|---|
| `LastFm` | `LastFMService` | public get, private set | Process-wide Last.fm client; constructed before frame initialization |
| `RootFrame` | `PhoneApplicationFrame` | public get, private set | Root navigation frame |

### Constructor

`App()` initializes resources, upgrades settings, creates/loads Last.fm state, applies theme, creates frame/language, and enables debug-only diagnostics.

### Lifecycle and navigation handlers

| Member | Description |
|---|---|
| `Application_Launching` | Connected lifecycle hook; empty |
| `Application_Activated` | Connected lifecycle hook; empty |
| `Application_Deactivated` | Connected lifecycle hook; empty |
| `Application_Closing` | Connected lifecycle hook; empty |
| `RootFrame_NavigationFailed` | Breaks only with debugger attached |
| `Application_UnhandledException` | Breaks only with debugger attached |
| `InitializePhoneApplication` | Idempotently creates/configures root frame |
| `CompleteInitializePhoneApplication` | Assigns frame to `RootVisual` after first navigation |
| `CheckForResetNavigation` | Schedules back-stack clearing after reset |
| `ClearBackStackAfterReset` | Removes all back entries after qualifying navigation |
| `InitializeLanguage` | Applies RESX language and flow direction; rethrows errors |

## `MainPage : PhoneApplicationPage`

Owns local player state and scrobble orchestration.

### Private state

| Field | Type | Purpose |
|---|---|---|
| `_player` | `AudioPlayerService` | Playlist and transport |
| `_progressTimer` | `DispatcherTimer` | One-second progress/scrobble polling |
| `_currentScrobbledTrack` | `Track` | Reference marker preventing duplicates |
| `_playStartTime` | `DateTime` | Wall-clock start for Unix scrobble timestamp |
| `_scrobblePending` | `bool` | In-flight/blocked guard |
| `_isLoved` | `bool` | Optimistic heart state for active track |

### Constructor and loading

| Member | Description |
|---|---|
| `MainPage()` | Initializes controls/timer/player events, loads songs, sets status |
| `LoadPlaylist()` | Loads library, binds list/count, or shows empty-library message |

### Player UI handlers

| Member | Trigger | Behavior |
|---|---|---|
| `PlaylistSelectionChanged` | List selection | Plays selected index and clears selection |
| `PlayPause_Click` | App bar | Starts index 0 or toggles pause/resume |
| `Next_Click` | App bar | Calls `PlayNext()` |
| `Previous_Click` | App bar | Calls `PlayPrevious()` |
| `Shuffle_Click` | App bar | Toggles shuffle and icon |
| `TrackInfo_Click` | Menu | Displays original metadata/duration |
| `LoveButton_Click` | Heart | Optimistically toggles and calls love/unlove |
| `Settings_Click` | Menu | Navigates to Settings |
| `Stats_Click` | Menu | Navigates to Stats |

### Playback/scrobble methods

| Member | Description |
|---|---|
| `ProgressTimerTick` | Publishes progress, updates slider/time, calls `CheckScrobble` while playing |
| `OnTrackChanged(Track)` | Transforms/displays metadata; resets play state; auto-loves; starts timer; sends Now Playing/info |
| `OnPlayStateChanged(bool)` | Updates play/pause icon; starts timer when playing |
| `SendNowPlaying(Track)` | Applies edits and calls `UpdateNowPlayingAsync` if all switches permit |
| `CheckScrobble()` | Evaluates auth, duplicate, duration, timing, transforms, block rules, and submission |
| `UpdateLoveButton()` | Chooses accent/subtle brush from optimistic love state |
| `UpdateScrobbleStatus()` | Maps authentication/settings to base label |
| `OnNavigatedTo` | Calls base and refreshes status |

## `SettingsPage : PhoneApplicationPage`

Maps settings controls to `SettingsManager` and Last.fm auth methods.

### State and setup

| Member | Description |
|---|---|
| `_currentToken` | Token returned during current page lifetime; required by Authorize |
| `SettingsPage()` | Initializes controls, binds theme names, loads settings |
| `LoadSettings()` | Copies service/settings/rules into controls and status |
| `SaveApiCredentials()` | Trims text fields and calls `LastFMService.Configure` |
| `UpdateAuthStatus()` | Sets connected text and logout enabled state |

### Authentication handlers

| Member | Description |
|---|---|
| `GetToken_Click` | Validates credentials, configures service, calls `GetTokenAsync`, populates token/buttons |
| `Authorize_Click` | Opens auth URL with `WebBrowserTask`; requires `_currentToken` |
| `Connect_Click` | Validates token, calls `GetSessionAsync`, updates status/migration |
| `Logout_Click` | Clears session and auth controls |

### Immediate-setting handlers

| Member | Saved property |
|---|---|
| `ScrobbleEnabledChanged` | `ScrobblerEnabled` |
| `NowPlayingChanged` | `NowPlayingEnabled` |
| `LoveOnStartupChanged` | `LoveOnStartup` |
| `DelaySecsChanged` | integer `ScrobbleDelaySecs` plus label |
| `DelayPercentChanged` | integer `ScrobbleDelayPercent` plus label |
| `MinDurationChanged` | integer `MinTrackDuration` plus label |
| `FetchAlbumArtChanged` | `FetchAlbumArt` |
| `ThemeSelectionChanged` | `ThemeName`, then applies theme |
| `DarkModeChanged` | `ThemeIsDark`, then applies theme |

Value-changed events can run during control initialization. Setters save values each time they run.

### Multiline-setting handlers

| Member | Parsing |
|---|---|
| `SaveBlocked_Click` | Trim non-empty lines into `List<string>` |
| `SaveEdits_Click` | Parse `field:pattern:replacement`, discard empty patterns |
| `SaveRegex_Click` | Same parser; regex validity is not checked here |

### Navigation

`Stats_Click` navigates to Stats. `OnNavigatedTo` reloads settings/status and enables auth/connect when token text remains.

## `StatsPage : PhoneApplicationPage`

### State

`_artistsPeriod` and `_tracksPeriod` both default to `"7day"` and retain last button selection.

### Members

| Member | Description |
|---|---|
| `StatsPage()` | Initializes and starts all three loads |
| `LoadRecentTracks()` | Clears if unauthenticated; otherwise requests 20 for session username |
| `LoadTopArtists(string)` | Stores period and requests 20 |
| `LoadTopTracks(string)` | Stores period and requests 20 |
| `Refresh_Click` | Reloads all with current periods |
| `PeriodArtists_Click` | Reads `Button.Tag` as period |
| `PeriodTracks_Click` | Reads `Button.Tag` as period |
| `OnNavigatedTo` | Reloads recent tracks only |

Every Stats error callback is empty.

## `LocalizedStrings`

Resource bridge with one property:

| Property | Type | Description |
|---|---|---|
| `LocalizedResources` | `AppResources` | Returns a static generated resource accessor instance |
