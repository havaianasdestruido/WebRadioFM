---
title: Install the application
description: Deploy a WebRadioFM XAP to a Windows Phone device or emulator.
---

# Install the application

WebRadioFM is distributed as a **XAP**, the package format used by Windows Phone Silverlight applications. It is not an APPX/MSIX application and cannot be installed by modern Windows app deployment tools.

## 1. Choose the package

Prebuilt packages are under `BuildOutput/` when available:

```text
BuildOutput/
├── ARM/WebRadioFM_Release_ARM.xap
└── x86/WebRadioFM_Release_x86.xap
```

- Choose **ARM** for a physical Windows Phone.
- Choose the architecture expected by the installed Windows Phone emulator image.
- If the required package is missing or you need to trust your own build, follow [Build from source](../development/building.md).

## 2. Prepare the destination

1. Charge and unlock the phone, or start the target emulator.
2. If using hardware, connect it over USB.
3. Ensure the device has been developer-unlocked/provisioned for sideloading using the tooling appropriate to its OS version.
4. Add at least one song to the media library if you want to verify playback immediately.

:::note Provisioning is environment-specific
Microsoft's original developer-unlock and store infrastructure has been retired or changed. Existing registered devices and preserved SDK environments may behave differently. The repository cannot provision a phone by itself.
:::

## 3. Deploy the XAP

### Windows Phone Application Deployment

1. Open **Application Deployment** from the Windows Phone SDK tools.
2. Select **Device** or the running emulator.
3. Browse to the matching `.xap`.
4. Select **Deploy**.
5. Wait for the success message, then launch **WebRadio.FM** from the application list.

### Visual Studio

When building from source:

1. Open `WebRadioFM.sln` in a compatible Visual Studio installation.
2. Select the destination architecture and `Release` or `Debug`.
3. Select the connected device/emulator target.
4. Press **F5** to deploy with debugging, or **Ctrl+F5** to deploy without it.

### Preserved third-party tooling

Legacy tools such as Windows Phone Power Tools may also deploy a XAP. They are not maintained by this project. Verify old downloads before running them and prefer SDK-provided deployment tools where possible.

## 4. Verify the installation

On first launch, the main page should:

1. display the `WEBRADIO.FM` header;
2. read the local media library;
3. show a sorted list of tracks; and
4. report the track count at the bottom.

If the library is empty, WebRadioFM displays **No music found**. This does not indicate an installation failure.

## Updating

Deploying a newer XAP with the same product identity should update the application while retaining isolated settings, depending on the deployment tool. Uninstalling the app normally removes its isolated storage, including its Last.fm session and preferences.

## Uninstalling

Long-press WebRadio.FM in the phone's app list and choose **uninstall**. This removes application data stored in `IsolatedStorageSettings`.

## Installation troubleshooting

| Symptom | Check |
|---|---|
| Package architecture error | Use ARM for hardware; use the emulator's expected architecture for virtual devices. |
| Deployment cannot find device | Unlock it, reconnect USB, verify drivers, and confirm SDK tools can see it. |
| Deployment denied | Confirm developer unlock/provisioning and package signing expectations. |
| App opens with no songs | Add supported music to the local media library and relaunch. |
| App exits immediately | Deploy a Debug build and inspect Visual Studio output for manifest or missing-runtime errors. |

Continue with [First run](./first-run.md).
