# Building WebRadioFM

> Full build, compatibility, script, output, and troubleshooting documentation: [docs/development/building.md](docs/development/building.md)

## Prerequisites

- **Visual Studio 2013** (or later) with the **Windows Phone 8.1 SDK**
  - The Windows Phone 8.1 SDK is available via the Visual Studio installer
- **Windows Phone 8.1 Silverlight** development tools
- Alternatively, any edition of **MSBuild** that supports the Windows Phone 8.1 targets

> **Note:** This is a legacy Windows Phone 8.1 Silverlight project. Modern UWP tools cannot produce `.XAP` output. You need the Windows Phone 8.1 SDK installed.

## Build

### Using the build script

```powershell
# Build the script defaults (x86 and x64) in Release
.\build.ps1

# Build ARM explicitly for a physical phone
.\build.ps1 -Configuration Release -Platforms ARM

# Build specific configuration and platforms
.\build.ps1 -Configuration Debug -Platforms x86,ARM

# Clean build artifacts
.\build.ps1 -Clean
```

### Using the batch launcher

```
simple_build_all.bat
```

### Using Visual Studio

1. Open `WebRadioFM.sln` in Visual Studio 2013+
2. Select **Build > Build Solution** (or `Ctrl+Shift+B`)
3. Choose the desired platform from the toolbar (x86, x64, or ARM)

## Output

The build produces `.XAP` files — the deployment package format for Windows Phone 8.1:

| Platform | Output path |
|----------|-------------|
| x86      | `BuildOutput\x86\WebRadioFM_Release_x86.xap` |
| x64      | `BuildOutput\x64\WebRadioFM_Release_x64.xap` |
| ARM      | `BuildOutput\ARM\WebRadioFM_Release_ARM.xap` |

> **ARM is the only target that runs on real Windows Phone devices.** x86 and x64 are for the Windows Phone emulator only.

## Notes

- The project has no NuGet package dependencies
- MD5 API signatures are generated inline for Last.fm authentication
- The solution file uses placeholder GUIDs — this does not affect building
