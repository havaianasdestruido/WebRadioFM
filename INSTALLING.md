# Installing WebRadioFM

## Prerequisites

- A **Windows Phone 8.1** device (or the Windows Phone 8.1 emulator)
- The built `.XAP` file (see [BUILDING.md](BUILDING.md))

## Installation Methods

### 1. Application Deployment tool (recommended)

1. Connect your Windows Phone device via USB (or start the emulator)
2. Open the **Windows Phone Application Deployment** tool
   (Start > search "Application Deployment")
3. Browse to the ARM `.XAP` file (`BuildOutput\ARM\WebRadioFM_Release_ARM.xap`)
4. Select **Deploy**

### 2. Visual Studio

1. Connect your device or start the emulator
2. Open `WebRadioFM.sln` in Visual Studio
3. Select **ARM** platform and **Release** configuration
4. Press **F5** (deploy and debug) or **Ctrl+F5** (deploy without debug)

### 3. Windows Phone Power Tools (third-party)

Tools like [Windows Phone Power Tools](https://wptools.codeplex.com/) can sideload `.XAP` files to a connected device.

## First-Time Setup

1. Open WebRadioFM on your phone
2. Go to **Settings** (app bar button)
3. Under **Scrobbling > Account**:
   - Enter your **Last.fm API Key** and **API Secret**
     (create an API account at https://www.last.fm/api/account/create)
   - Tap **Get Token**, then **Authorize** — you'll be taken to last.fm in the browser
   - After authorizing, copy the returned token and paste it back in the app
   - Tap **Connect** to exchange the token for a session key
4. Adjust scrobble rules (delays, minimum duration, etc.) to your preference
5. Customize the appearance under the **Appearance** pivot

Your session is persisted, so you only need to authenticate once.

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Deployment fails | Ensure the device is unlocked for developer/sideloading (Settings > Update & Security > For developers > Developer mode) |
| Scrobbling not working | Verify your API key and secret; check that scrobbling is enabled in settings |
| No music shows up | The app reads from the phone's `MediaLibrary` — ensure you have music on the device |
