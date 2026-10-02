---
title: Build from source
description: Compile WebRadioFM with the Windows Phone 8.1 Silverlight toolchain.
---

# Build from source

The phone application can only be built where Windows Phone 8.1 Silverlight MSBuild targets and references are available. The Docusaurus site has a separate Node.js build described in [Documentation development](./documentation.md).

## Required application toolchain

- Windows
- Visual Studio/MSBuild with Windows Phone 8.1 Silverlight targets
- Windows Phone 8.1 SDK/reference assemblies
- Silverlight development components
- PowerShell (for repository scripts)

:::warning Preserve a working environment
These installers are retired. Use legally obtained, integrity-checked media and isolate legacy development environments. Files in `required_stuff_to_install_b4_building/` are historical shortcuts/links, not trusted bundled installers.
:::

:::caution Compiler version
Although the project declares Visual Studio 2013 (`12.0`) as its minimum, the checked-in C# uses later syntax such as null-conditional access, auto-property initializers, and expression-bodied methods. A stock C# 5 compiler can reject it. Use a compatible newer compiler that can still load the phone targets, or backport those expressions before attempting a strict VS2013 build.
:::

## Visual Studio build

1. Open `WebRadioFM.sln`.
2. Select `Debug` or `Release`.
3. Select a platform (`x86`, `x64`, or `ARM`; `AnyCPU` also exists in the project).
4. Choose **Build → Build Solution**.
5. Deploy with **F5** if a compatible destination is configured.

The project imports:

```text
Microsoft.WindowsPhone.v8.1.Overrides.targets
Microsoft.WindowsPhone.v8.1.CSharp.targets
```

The precise paths are resolved under `$(MSBuildExtensionsPath)`. “Imported project not found” means the required phone targets are absent or not registered.

## Standard build script

`build.ps1` locates MSBuild in known Visual Studio 2022/18 and MSBuild 12 paths, falling back to `Get-Command msbuild.exe`.

```powershell
# Defaults: Release, x86 and x64
.\build.ps1

# Physical-device package
.\build.ps1 -Configuration Release -Platforms ARM

# Multiple explicit targets
.\build.ps1 -Configuration Debug -Platforms x86,ARM

# Remove BuildOutput and invoke MSBuild Clean
.\build.ps1 -Clean -Platforms x86,ARM
```

Unlike older README wording, the script default is currently **x86 and x64**, not ARM. Pass `-Platforms ARM` explicitly for a physical-device package.

The script:

1. locates MSBuild;
2. optionally restores with `.nuget/NuGet.exe` if that file exists;
3. builds each requested platform;
4. looks for the generated XAP under `WebRadioFM/Bin/...`;
5. copies found packages into `BuildOutput/<platform>/`; and
6. exits nonzero if any MSBuild invocation failed.

A “XAP not found” line is only a warning in the copy phase.

## Batch launcher

`simple_build_all.bat` is a legacy convenience wrapper around PowerShell. Inspect it before use and prefer direct `build.ps1` invocation when you need explicit configuration/platform control.

## Advanced compatibility script

`build_no_silverlight.ps1` attempts to repair an incomplete modern Windows environment by:

- creating a Windows Phone v8.1 reference-assembly junction to v8.0;
- writing 64-bit Silverlight and Windows Phone SDK registry keys;
- invoking MSBuild; and
- patching generated XAP manifests to claim Windows Phone 8.0/runtime compatibility.

It supports:

```powershell
.\build_no_silverlight.ps1 -SetupOnly
.\build_no_silverlight.ps1 -Configuration Release -Platforms x86
```

:::danger Experimental and invasive
The script elevates, writes `HKLM`, creates a system directory junction, and rewrites package manifests. A package produced this way is not equivalent to a true Windows Phone 8.1 SDK build and can fail at compile, install, or runtime. Use only in a disposable environment after reviewing the script.
:::

## Build outputs

The project property is:

```xml
<XapFilename>WebRadioFM_$(Configuration)_$(Platform).xap</XapFilename>
```

Expected copied paths therefore follow:

```text
BuildOutput/<platform>/WebRadioFM_<Configuration>_<platform>.xap
```

Examples:

```text
BuildOutput/ARM/WebRadioFM_Release_ARM.xap
BuildOutput/x86/WebRadioFM_Debug_x86.xap
```

## Common failures

| Failure | Meaning / next action |
|---|---|
| `MSBuild not found` | Install/register compatible build tools or expose `msbuild.exe` on PATH. |
| Imported WP targets missing | The Windows Phone SDK targets are not installed/resolvable. |
| Reference `Microsoft.Xna.Framework` unresolved | XNA/phone reference assemblies are missing. |
| XAML validation errors | Check target SDK resources, names, and event handler signatures. |
| Build succeeds, XAP copy warns | Locate `Bin` output and compare actual path/name with script assumptions. |
| Package rejected on device | Check architecture, manifest platform version, signing, and developer unlock. |

## What does not work

These commands do not supply the legacy target framework:

```bash
dotnet build WebRadioFM.sln
```

Nor can a UWP workload emit this project's XAP unchanged. Producing APPX/MSIX requires a migration to a different application model, not a build flag.
