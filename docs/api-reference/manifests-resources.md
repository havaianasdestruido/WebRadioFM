---
title: Manifests, project, and resources
description: Build properties, package identity, capabilities, assets, assembly metadata, and resource keys.
---

# Manifests, project, and resources

This page covers non-class inputs that define how WebRadioFM builds, launches, and accesses phone features.

## `WebRadioFM.csproj`

### Identity and target

| Property | Value |
|---|---|
| Tools version | `12.0` |
| Project type | Windows Phone + C# GUIDs |
| Output type | `Library` (Silverlight application assembly) |
| Root namespace / assembly | `WebRadioFM` |
| Target framework | `WindowsPhone v8.1` |
| Silverlight application | `true` |
| XAP output | `true` |
| Entry point | `WebRadioFM.App` |
| Minimum Visual Studio | `12.0` |
| XAML validation | enabled, errors thrown |

XAP name is `WebRadioFM_$(Configuration)_$(Platform).xap`.

### Configurations

Debug enables full symbols, disables optimization, and defines `DEBUG;TRACE;SILVERLIGHT;WINDOWS_PHONE`. Release uses PDB-only debug type, optimization, and `TRACE;SILVERLIGHT;WINDOWS_PHONE`.

Both exist for AnyCPU, x86, ARM, and x64 with platform-specific output paths.

### Items

The old-style project explicitly includes:

- 17 C# compile items including the generated RESX designer;
- application definition and three XAML pages;
- two manifest files;
- 12 PNG content assets;
- `Microsoft.Xna.Framework` reference; and
- embedded `AppResources.resx`.

Two empty compatibility targets are declared at the end:

- `_GetRecursiveResolvedSDKReferences`
- `ValidateWMAppManifest` depending on `WMAppManifestWinMDRegistration`

They do not supply missing platform binaries/targets.

## `WMAppManifest.xml`

### App identity

| Attribute | Value |
|---|---|
| Platform | `8.1` |
| Default language | `en-US` |
| Title | `WebRadio.FM` |
| Runtime | `Silverlight` |
| Version | `1.0.0.0` |
| Genre | `apps.normal` |
| Author/Publisher | `WebRadioFM` |
| Description | Windows Phone 8.1 music player with Last.fm scrobbling |

Product/publisher IDs are GUID values in the manifest. Release environments may require registered identities.

### Capabilities

- `ID_CAP_NETWORKING`
- `ID_CAP_MEDIALIB_AUDIO`
- `ID_CAP_MEDIALIB_PLAYBACK`
- `ID_CAP_WEBBROWSERCOMPONENT`
- `ID_CAP_ISOLATED_STORAGE`

### Tasks and tiles

The default task navigates to `MainPage.xaml`. A primary flip tile references 71×71, 150×150, and 310×150 images and enables the large tile.

Supported screen resolutions are WVGA, WXGA, and HD720P.

## `AppManifest.xml`

The checked-in Silverlight deployment manifest template contains the `Deployment` root and an empty `Deployment.Parts` collection. The Windows Phone build targets generate/populate package deployment metadata during the XAP build; do not infer the final runtime manifest solely from this minimal template.

## `AssemblyInfo.cs`

Assembly/product title is `WebRadioFM`; description states its Windows Phone/Last.fm purpose. COM visibility is false. Assembly and file versions are `1.0.0.0`, and neutral resource language is `en-US`.

## Application resources

`AppResources.resx` currently contains:

| Group | Keys |
|---|---|
| Culture | `ResourceFlowDirection`, `ResourceLanguage` |
| Titles | `ApplicationTitle`, `MainTitle`, `SettingsTitle` |
| App bar | `AppBarPlay`, `AppBarPause`, `AppBarNext`, `AppBarPrev`, `AppBarShuffle`, `AppBarSettings` |
| Library | `NoTracksMessage` |
| Last.fm | header, API labels, login/logout, auth status, enable label |

Not every hard-coded page string has a resource key, and not every existing key is used by current XAML.

`AppResources.Designer.cs` is generated with `ResourceManager` and culture support. `LocalizedStrings` makes an accessor available as `StaticResource LocalizedStrings`.

## XAML application resources

`App.xaml` defines:

- `LocalizedStrings`
- `BoolToVisibilityConverter`
- `InverseBoolToVisibilityConverter`

It also wires `PhoneApplicationService` lifecycle events to `App.xaml.cs`.

## Packaged PNG assets

| Asset | Use |
|---|---|
| `ApplicationIcon.png` | App list icon |
| `SplashScreen.png` | Launch splash |
| `TileSquare71x71.png` | Small tile |
| `TileSquare150x150.png` | Medium tile |
| `TileWide310x150.png` | Wide tile |
| `appbar.transport.play.png` | Play state |
| `appbar.transport.pause.png` | Pause state |
| `appbar.transport.ff.png` | Next |
| `appbar.transport.rew.png` | Previous |
| `appbar.shuffle.png` | Shuffle off |
| `appbar.shuffle.active.png` | Shuffle on |
| `appbar.refresh.png` | Stats refresh |

Paths are package-relative and use `/Assets/...` in XAML/code.

## Change checklist

When adding source/page/assets:

1. add the file to the correct `.csproj` `ItemGroup`;
2. configure `DependentUpon` for code-behind where appropriate;
3. mark packaged images as `Content` and copy when needed;
4. add required capabilities to the phone manifest;
5. update navigation/task metadata if entry points change; and
6. test a clean XAP, not only incremental Visual Studio output.
