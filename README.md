# WebRadioFM

A **Windows Phone 8.1 Silverlight** app that plays music from your phone's local library and scrobbles plays to [Last.fm](https://www.last.fm/).

## Features

- **Local music playback** — browse and play songs from your media library via XNA `MediaPlayer`
- **Last.fm scrobbling** — automatically record tracks to your Last.fm account with configurable thresholds (time or percentage)
- **Now Playing updates** — broadcast what you're listening to in real time
- **Love / Unlove tracks** — heart button integrates with Last.fm
- **Last.fm statistics** — view recent tracks, top artists, and top tracks with selectable periods
- **Metadata editing** — simple find/replace and regex rules to clean up track info before scrobbling
- **Track blocking** — skip scrobbling for specific patterns
- **Custom themes** — 10 accent colors + dark/light mode
- **Localization** — resource-based string system (English by default)

## Documentation

- [BUILDING.md](BUILDING.md) — build prerequisites and instructions
- [INSTALLING.md](INSTALLING.md) — deployment and installation guide

## Project Structure

```
WebRadioFM.sln                 # Visual Studio 2013 solution
WebRadioFM/                    # Main project (WP 8.1 Silverlight)
├── App.xaml[.cs]              # Application entry point, theme, Last.fm service
├── MainPage.xaml[.cs]         # Player UI and scrobble logic
├── SettingsPage.xaml[.cs]     # Settings: auth, scrobble rules, themes
├── StatsPage.xaml[.cs]        # Last.fm statistics viewer
├── Helpers/                   # SettingsManager, ThemeManager, converters
├── Models/                    # Track, LastFmSession, API response models
├── Services/                  # AudioPlayerService, LastFMService
├── Properties/                # Assembly info, manifests
├── Resources/                 # Localized strings
└── Assets/                    # Icons and tile images
build.ps1                      # Multi-platform build script (x86, x64, ARM)
simple_build_all.bat           # Batch launcher for build.ps1
```

## License

See `Properties/AssemblyInfo.cs` for assembly metadata.
