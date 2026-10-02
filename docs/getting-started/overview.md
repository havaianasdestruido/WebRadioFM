---
title: Welcome to WebRadioFM
description: What WebRadioFM is, what it can do, and where to begin.
slug: /getting-started/overview
---

# Welcome to WebRadioFM

WebRadioFM is a **Windows Phone 8.1 Silverlight** music player that reads the phone's local music library and reports listening activity to [Last.fm](https://www.last.fm/). This handbook documents the shipped application, its legacy build environment, and every major part of its source code.

:::warning Legacy platform
Windows Phone 8.1 and its Silverlight development stack are discontinued. WebRadioFM is most useful for preservation, learning, and already-provisioned devices or emulator environments. A current .NET SDK by itself cannot build this application.
:::

## What the app does

- Lists tracks from XNA's `MediaLibrary` and plays them through `MediaPlayer`.
- Provides play/pause, next, previous, and shuffle controls.
- publishes **Now Playing** updates to Last.fm.
- Scrobbles eligible tracks after a configurable listening threshold.
- Loves or unloves the active track.
- Displays recent tracks, top artists, and top tracks.
- Applies literal or regular-expression metadata edits before Last.fm submission.
- Blocks scrobbles matching configured artist/title fragments.
- Provides ten accent choices and dark/light appearance modes.

## Choose your path

| I want to… | Start here |
|---|---|
| Install an existing XAP | [Installation](./install.md) |
| Configure the app for the first time | [First run](./first-run.md) |
| Connect a Last.fm account | [Last.fm setup](../user-guide/lastfm-setup.md) |
| Understand scrobble timing | [Scrobbling behavior](../user-guide/scrobbling.md) |
| Build or modify the source | [Build from source](../development/building.md) |
| Understand the implementation | [Architecture overview](../architecture/overview.md) |
| Look up a class or method | [API reference](../api-reference/overview.md) |

## Platform at a glance

| Area | Technology |
|---|---|
| Language | C# |
| UI | Windows Phone Silverlight XAML + code-behind |
| Target | Windows Phone 8.1 (`WindowsPhone,Version=v8.1`) |
| Audio | `Microsoft.Xna.Framework.Media` |
| Networking | callback-based `HttpWebRequest` |
| JSON | `DataContractJsonSerializer` |
| Persistence | `IsolatedStorageSettings.ApplicationSettings` |
| Package | XAP |
| External service | Last.fm Web Services API 2.0 |

## Repository map

```text
WebRadioFM.sln                 Visual Studio solution
WebRadioFM/                    Windows Phone application project
├── App.xaml[.cs]              startup, services, frame, resources
├── MainPage.xaml[.cs]         library/player/scrobble orchestration
├── SettingsPage.xaml[.cs]     account, rules, metadata, appearance
├── StatsPage.xaml[.cs]        Last.fm listening statistics
├── Services/                  audio and Last.fm integrations
├── Helpers/                   settings, signatures, themes, converters
├── Models/                    app and Last.fm JSON models
├── Properties/                package and assembly manifests
└── Resources/                 localizable resources
BuildOutput/                   checked-in XAP artifacts
build.ps1                      standard multi-platform PowerShell build
docs/                          this Docusaurus handbook
src/                           documentation site components and styling
```

## Important implementation realities

This documentation describes current behavior rather than intended behavior:

1. The app persists the Last.fm **session key and username**, but not the API key or API secret. Signed calls need credentials to be entered again after a fresh process launch.
2. The Last.fm base URLs in source use plain `http://`, not HTTPS. Treat account tokens and API secrets accordingly; see [Privacy and security](../user-guide/privacy-security.md).
3. “Fetch album art” currently requests track information but does not bind returned art into the player UI.
4. The repository contains no automated application test project; validation is manual and requires a compatible Windows environment.

These constraints are also called out where they affect setup, maintenance, and debugging.

## Next step

Already have an XAP? Continue to [Installation](./install.md). Working from source? Go directly to [Build from source](../development/building.md).
