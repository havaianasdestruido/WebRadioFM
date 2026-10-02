---
title: Model reference
description: Local track, session, authentication, scrobble, statistics, and track-info data contracts.
---

# Model reference

Namespace: `WebRadioFM.Models`.

## Local application models

### `Track : INotifyPropertyChanged`

| Property | Type | Description |
|---|---|---|
| `Title` | `string` | Local title; setter notifies |
| `Artist` | `string` | Local artist; setter notifies |
| `Album` | `string` | Local album; setter notifies |
| `FilePath` | `string` | Declared but not populated by current loader |
| `Duration` | `TimeSpan` | Local media duration; setter notifies |
| `AlbumArt` | `string` | Declared but not populated by current loader |
| `DisplayText` | `string` (get) | `Artist + " - " + Title` |

`PropertyChanged` is raised with caller member name by protected `OnPropertyChanged`. Changing Artist/Title does not additionally notify `DisplayText`.

### `LastFmSession`

| Property | Type | Description |
|---|---|---|
| `SessionKey` | `string` | Last.fm `sk` value |
| `Username` | `string` | Authorized account name |
| `IsAuthenticated` | `bool` (get) | True when session key is non-empty |

## Authentication/error contracts

Defined in `LastFmResponses.cs`.

| Class | JSON shape / members |
|---|---|
| `TokenResponse` | `token : string` |
| `SessionResponse` | `session : SessionData` |
| `SessionData` | `name : string`, `key : string`, `subscriber : int` |
| `ErrorResponse` | `error : int`, `message : string` |

All are `[DataContract]` with `[DataMember(Name = ...)]` properties.

## Scrobble response contracts

The following types model Last.fm's detailed scrobble response but current `ScrobbleAsync` only checks generic API errors and does not deserialize them:

| Class | Members |
|---|---|
| `ScrobbleResponse` | `scrobbles : ScrobblesData` |
| `ScrobblesData` | `attr : ScrobbleAttr` mapped from `@attr`; `scrobble : ScrobbleItem[]` |
| `ScrobbleAttr` | `accepted : int`, `ignored : int` |
| `ScrobbleItem` | `artist`, `track`, `album`, `timestamp` as `ScrobbleElement`; `ignoredMessage` |
| `ScrobbleElement` | `text` mapped from `#text`; `corrected : string` |
| `IgnoredMessage` | `text` mapped from `#text`; `code : string` |

## Recent tracks

Defined in `LastFmStats.cs`.

### Wrappers

| Class | Members |
|---|---|
| `RecentTracksResponse` | `recenttracks : RecentTracksData` |
| `RecentTracksData` | `track : List<RecentTrackItem>`, `attr : PageAttr` from `@attr` |

### `RecentTrackItem`

| Property | Type / mapping |
|---|---|
| `artist` | `StatsArtist` |
| `name` | `string` |
| `album` | `StatsAlbum` |
| `image` | `List<StatsImage>` |
| `date` | `StatsDate` |
| `attr` | `NowPlayingAttr` from `@attr` |
| `loved` | `string` |
| `IsNowPlaying` | derived true when `attr.nowplaying == "true"` |

## Shared statistics values

| Class | Members |
|---|---|
| `StatsArtist` | `text` from `#text`, `mbid` |
| `StatsAlbum` | `text` from `#text`, `mbid` |
| `StatsDate` | `uts`, `text` from `#text` |
| `NowPlayingAttr` | `nowplaying` string |
| `StatsImage` | `text` from `#text`, `size` |
| `PageAttr` | `page`, `perPage`, `totalPages`, `total` — all strings |

## Top artists

| Class | Members |
|---|---|
| `TopArtistsResponse` | `topartists : TopArtistsData` |
| `TopArtistsData` | `artist : List<TopArtistItem>`, `attr : PageAttr` |
| `TopArtistItem` | `name`, `playcount`, `listeners`, `mbid`, `url`, `image : List<StatsImage>` |

Numeric API fields such as play count remain strings.

## Top tracks

| Class | Members |
|---|---|
| `TopTracksResponse` | `toptracks : TopTracksData` |
| `TopTracksData` | `track : List<TopTrackItem>`, `attr : PageAttr` |
| `TopTrackItem` | `name`, `playcount`, `listeners`, `mbid`, `url`, `artist : StatsArtist`, `image` |

## Track information

| Class | Members |
|---|---|
| `TrackInfoResponse` | `track : TrackInfoData` |
| `TrackInfoData` | `name`, `duration`, `mbid`, `url`, `artist : StatsArtist`, `album : TrackAlbum` |
| `TrackAlbum` | `artist`, `title`, `mbid`, `url`, `image : List<StatsImage>` |

The app requests `TrackInfoData` when album-art fetching is enabled but currently discards the successful callback value.

## Serialization guidance

- Keep the parameterless public types/properties available to `DataContractJsonSerializer`.
- Preserve `DataMember` names for special keys such as `#text` and `@attr`.
- Model members can be absent/null in real responses; XAML nested bindings generally tolerate null, but code consumers need guards.
- Add new response variants defensively; Last.fm sometimes varies shapes based on context/cardinality.
