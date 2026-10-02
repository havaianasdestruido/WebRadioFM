---
title: Requirements and compatibility
description: Supported platform, tools, capabilities, and external accounts.
---

# Requirements and compatibility

WebRadioFM has two independent environments: the **phone application** and the **Docusaurus documentation site**. Their toolchains are not interchangeable.

## Running the phone app

You need:

- a Windows Phone 8.1 device provisioned for sideloading, or a compatible Windows Phone emulator;
- a WebRadioFM `.xap` matching the destination architecture;
- music in the device/emulator media library;
- network access for Last.fm features; and
- a Last.fm account plus a Last.fm API key and shared secret for scrobbling.

The player itself can list and play local music without Last.fm. Authentication, Now Playing, scrobbling, love/unlove, track information, and statistics require network access and API credentials.

## Building the phone app

The authoritative target in `WebRadioFM.csproj` is:

```xml
<TargetFrameworkIdentifier>WindowsPhone</TargetFrameworkIdentifier>
<TargetFrameworkVersion>v8.1</TargetFrameworkVersion>
<SilverlightApplication>true</SilverlightApplication>
```

A compatible setup normally includes:

- Windows;
- Visual Studio 2013-era Windows Phone tooling or equivalent MSBuild targets;
- Windows Phone 8.1 SDK/reference assemblies;
- Silverlight development targets; and
- PowerShell for the repository build scripts.

:::danger Modern SDKs are not a replacement
Visual Studio 2022, `dotnet build`, UWP workloads, and current Windows SDKs do not include the retired Windows Phone 8.1 Silverlight targets. The two empty compatibility targets at the bottom of the project file do not recreate the SDK.
:::

The project defines `AnyCPU`, `x86`, `x64`, and `ARM` configurations. Use **ARM for physical phones**. Emulator architecture support depends on the installed emulator/SDK image.

## Phone capabilities

`Properties/WMAppManifest.xml` requests these capabilities:

| Capability | Purpose |
|---|---|
| `ID_CAP_NETWORKING` | Call the Last.fm API |
| `ID_CAP_MEDIALIB_AUDIO` | Read local songs and metadata |
| `ID_CAP_MEDIALIB_PLAYBACK` | Play media-library songs |
| `ID_CAP_WEBBROWSERCOMPONENT` | Launch Last.fm authorization |
| `ID_CAP_ISOLATED_STORAGE` | Save settings and session data |

Removing a capability can produce runtime failures even when the project still compiles.

## Last.fm prerequisites

Create an API application at [last.fm/api/account/create](https://www.last.fm/api/account/create). You need:

- **API key** — sent on API requests;
- **shared secret** — used locally to create MD5 request signatures; and
- a Last.fm user account — authorizes the generated token and owns the session.

The app does not ship with project-wide credentials.

## Documentation site requirements

To work on this handbook, you need Node.js 20 or newer and npm:

```bash
node --version
npm install
npm start
```

The documentation site can be built on Windows, macOS, or Linux. It does **not** build or emulate the Windows Phone application.

## Compatibility matrix

| Task | Windows + legacy WP SDK | Current Windows only | macOS/Linux |
|---|:---:|:---:|:---:|
| Run Docusaurus | Yes | Yes | Yes |
| Build Docusaurus | Yes | Yes | Yes |
| Compile WebRadioFM | Yes | No | No |
| Run WP emulator | Yes | No | No |
| Sideload to a WP device | Environment-dependent | No | No |
| Review/edit C# and XAML | Yes | Yes | Yes |
