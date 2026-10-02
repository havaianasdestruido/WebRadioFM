---
title: Project structure
description: Repository layout and ownership of each source area.
---

# Project structure

The repository contains one Windows Phone project, build/deployment artifacts, and this documentation site.

## Root

| Path | Responsibility |
|---|---|
| `WebRadioFM.sln` | Visual Studio solution referencing the phone project |
| `WebRadioFM/` | Application source, assets, resources, and manifests |
| `BuildOutput/` | Selected generated XAP packages checked into the repository |
| `build.ps1` | Standard multi-platform build/copy script |
| `build_no_silverlight.ps1` | Experimental environment repair and XAP patch script |
| `simple_build_all.bat` | Legacy batch build launcher |
| `README.md` | Concise repository landing page |
| `BUILDING.md` / `INSTALLING.md` | Legacy standalone instructions |
| `docs/` | Docusaurus Markdown/MDX content |
| `src/` | Documentation React pages and CSS |
| `static/` | Documentation static assets |
| `docusaurus.config.js` | Site behavior, navigation, metadata, theme |
| `sidebars.js` | User/developer documentation navigation |

## Application root

### `App.xaml` and `App.xaml.cs`

Defines global resources and application lifetime. The code creates the singleton `LastFMService`, loads a session, applies the theme, initializes `PhoneApplicationFrame`, and sets resource language/flow direction.

### Page pairs

| XAML | Code-behind | Function |
|---|---|---|
| `MainPage.xaml` | `MainPage.xaml.cs` | Playlist, active track, transport controls, Now Playing, love, scrobble timing |
| `SettingsPage.xaml` | `SettingsPage.xaml.cs` | Last.fm auth, thresholds, metadata rules, themes, about information |
| `StatsPage.xaml` | `StatsPage.xaml.cs` | Recent tracks, top artists, top tracks, periods and refresh |

Every page is portrait-only and uses a `PhoneApplicationPage` root.

## Services

### `AudioPlayerService.cs`

Wraps XNA `MediaLibrary` and static `MediaPlayer`; owns the in-memory `List<Track>`, current index, shuffle mode, play state, and media event translation.

### `LastFMService.cs`

Owns API configuration/session, signs and sends Last.fm requests, parses response/error JSON, limits concurrent requests, and persists session identity.

## Helpers

| File | Role |
|---|---|
| `ApiSignatureHelper.cs` | Sorted Last.fm parameter signatures and an inline MD5 implementation |
| `SettingsManager.cs` | Typed isolated-setting facade, rule storage/transformation, one schema migration |
| `ThemeManager.cs` | Accent catalog and runtime phone-resource replacement |
| `BoolToVisibilityConverter.cs` | Boolean/visibility XAML converters, normal and inverse |

## Models

| File | Contents |
|---|---|
| `Track.cs` | Bindable local track metadata and display label |
| `LastFmSession.cs` | Username, session key, derived authentication state |
| `LastFmResponses.cs` | Token/session/error/scrobble authentication and submission contracts |
| `LastFmStats.cs` | Recent/top/track-info data contracts |

Most Last.fm response properties intentionally retain lower-case API field names to align with `DataMember` mappings.

## Properties and resources

- `WMAppManifest.xml` declares phone identity, capabilities, default task, tile assets, and resolutions.
- `AppManifest.xml` is the Silverlight deployment manifest template.
- `AssemblyInfo.cs` declares version `1.0.0.0` and assembly metadata.
- `AppResources.resx` contains the current English resource set.
- `AppResources.Designer.cs` is generated access code.
- `LocalizedStrings.cs` exposes resources for XAML binding.

## Assets

`Assets/` contains:

- application/tile/splash PNGs;
- play, pause, next, previous, shuffle, and refresh application-bar icons.

Each application asset must remain listed as `Content` in the old-style `.csproj` to be packaged.

## Project file mechanics

`WebRadioFM.csproj` explicitly lists every C# compile item, XAML page, embedded resource, manifest, and content asset. New files are not picked up through SDK-style globs. Add them to the appropriate `ItemGroup` or they will not compile/package.
