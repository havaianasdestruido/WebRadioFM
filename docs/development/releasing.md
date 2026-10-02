---
title: Release process
description: Prepare, validate, package, document, and publish a WebRadioFM release.
---

# Release process

There is no automated phone-application release pipeline. Treat each XAP as a reproducible artifact from a named legacy environment.

## 1. Choose the version

Version appears in multiple places:

- `Properties/AssemblyInfo.cs`: `AssemblyVersion` and `AssemblyFileVersion`;
- `Properties/WMAppManifest.xml`: `App Version`;
- Settings about text: currently hard-coded `Version 1.0` in `SettingsPage.xaml`;
- `LastFMService` user agent: currently `WebRadioFM/1.0`; and
- documentation/release notes.

Update them consistently where the release policy requires it.

## 2. Validate source and package metadata

Review:

- product/publisher IDs;
- requested capabilities;
- app title/description;
- application, tile, and splash assets;
- target framework and platform configuration; and
- every new source/content item in `WebRadioFM.csproj`.

Do not insert production API credentials in source or a package.

## 3. Build clean packages

On the preserved release machine:

```powershell
.\build.ps1 -Clean -Configuration Release -Platforms x86,ARM
.\build.ps1 -Configuration Release -Platforms x86,ARM
```

Add x64 only if it is a supported/tested release target. Record the exact MSBuild and Windows Phone SDK versions.

## 4. Test

At minimum:

- install/update/uninstall behavior;
- empty and populated libraries;
- full playback transport;
- Last.fm token/session flow;
- Now Playing, scrobble, love, unlove;
- metadata edit/block rules;
- all statistics pivots and periods;
- dark/light theme; and
- app suspend/resume and cold launch.

Use the [debugging and testing matrix](./debugging-testing.md).

## 5. Hash and archive artifacts

Store each final XAP with:

- file name containing version/configuration/platform;
- SHA-256 checksum;
- source commit identifier;
- toolchain inventory; and
- test notes.

The current script's file name does not include an application version, so release storage should add that context externally.

## 6. Update documentation

Update user-visible behavior, known limitations, version references, and build/deployment requirements. Build the docs:

```bash
npm ci
npm run build
```

The GitHub Actions Documentation workflow builds pull requests without deploying them. Pushes to `main` deploy the `main` documentation, while manual runs deploy the branch or tag selected for the dispatch. Deployment requires repository Pages to be configured for GitHub Actions.

## 7. Publish

If using GitHub Releases, attach only reviewed artifacts and their checksums. Explain that:

- Windows Phone 8.1 is unsupported;
- ARM is intended for physical hardware;
- sideloading depends on an already-capable environment; and
- users provide their own Last.fm API credentials.

## Rollback

Keep prior XAPs and checksums. A package downgrade may be rejected or may interact with newer isolated settings. The current settings schema only migrates from one legacy key and has no downgrade migration, so test rollback with both retained and clean app data.
