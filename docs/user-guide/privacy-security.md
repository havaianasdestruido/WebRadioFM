---
title: Privacy and security
description: Understand what WebRadioFM reads, sends, stores, and exposes.
---

# Privacy and security

WebRadioFM handles local media metadata and Last.fm credentials. This page documents behavior visible in the current source; it is not a claim about the security of the retired platform.

## Data read from the phone

The app requests media-library access and reads:

- song title;
- artist;
- album;
- duration; and
- the underlying XNA song object for playback.

There is reflection-based code that attempts to retrieve album art and build a `BitmapImage`, but the image is not assigned to a model or control. Audio content is played locally through XNA and is not uploaded by this codebase.

## Data sent to Last.fm

Depending on enabled features, requests can include:

- artist, title, album, and duration;
- playback start timestamp;
- Last.fm username;
- API key;
- session key;
- signed authentication token; and
- an MD5 API signature.

Statistics requests send the username, API key, limit, and period.

## Data stored locally

`IsolatedStorageSettings.ApplicationSettings` stores:

- Last.fm session key and username;
- scrobble switches and thresholds;
- theme and dark-mode settings;
- album-art and auto-love flags;
- metadata edit rules;
- blocked metadata strings; and
- settings schema version.

The API key and shared secret are **not persisted** by the current code. They remain in process memory after entry.

## Plain HTTP endpoints

`LastFMService` currently defines:

```csharp
private const string ApiBaseUrl = "http://ws.audioscrobbler.com/2.0/";
private const string AuthBaseUrl = "http://www.last.fm/api/auth/";
```

This provides no application-layer transport encryption. Network observers may be able to inspect or modify traffic, including session-bearing requests.

:::danger Recommendation
Do not use valuable credentials over an untrusted network. Before active use, verify Last.fm's current endpoint requirements and migrate the service constants and request behavior to HTTPS in a test environment.
:::

## Secrets and logging

The app does not intentionally log credentials. However:

- GET requests place request parameters in the URL;
- POST methods currently include signed parameters in both the URL query and request body;
- proxies and debugging tools may retain URLs; and
- exception text can be shown in message boxes during authentication.

Never commit personal API credentials, session keys, or captured response payloads.

## API signature limitations

MD5 here is used by the Last.fm protocol for request signing, not password hashing. `ApiSignatureHelper` sorts parameters, excludes keys containing `format`, concatenates key/value pairs, appends the shared secret, and computes lowercase MD5.

A valid signature proves knowledge of the shared secret under the API protocol; it does not encrypt the request.

## Session revocation

The app's **disconnect** button only deletes the local session. Use Last.fm's account/application settings to revoke server-side access where available.

## Uninstall and data removal

Uninstalling should remove application isolated storage. Updating a package may retain it. There is no in-app “clear all preferences” command beyond disconnecting the Last.fm session and manually changing individual settings.
