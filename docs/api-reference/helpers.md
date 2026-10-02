---
title: Helper reference
description: Settings, metadata rules, API signatures, themes, and visibility converters.
---

# Helper reference

Namespace: `WebRadioFM.Helpers`.

## `ApiSignatureHelper` (static)

### `CreateSignature(IDictionary<string,string> parameters, string secret)`

Public entry point. Sorts pairs by key, excludes keys containing `format`, concatenates key/value with no delimiters, appends secret, and returns lowercase MD5 hex.

### MD5 implementation

Private `CreateMd5` UTF-8 encodes input and formats the 16-byte digest. `ComputeMd5` implements RFC-style padding, 512-bit blocks, four rounds, and little-endian output. Private `F`, `G`, `H`, `I`, `RL`, `FF`, `GG`, `HH`, and `II` implement round functions/rotation.

No null checks are performed; null `secret`, parameters, keys, or values can fail.

## `SettingsManager` (static)

Typed facade over `IsolatedStorageSettings.ApplicationSettings`.

### Scalar properties

| Property | Type | Default | Set constraint |
|---|---|---:|---|
| `ScrobblerEnabled` | `bool` | true | none |
| `NowPlayingEnabled` | `bool` | true | none |
| `ScrobbleDelaySecs` | `int` | 180 | 30–360 |
| `ScrobbleDelayPercent` | `int` | 50 | 30–95 |
| `MinTrackDuration` | `int` | 30 | 10–60 |
| `FetchAlbumArt` | `bool` | false | none |
| `ThemeName` | `string` | `Default` | none |
| `ThemeIsDark` | `bool` | true | none |
| `LoveOnStartup` | `bool` | false | none |

### Collection methods

| Method | Description |
|---|---|
| `GetRegexRules()` | Stored list or a new empty list |
| `SaveRegexRules(List<MetadataEditRule>)` | Replaces stored regex list |
| `GetSimpleEdits()` | Stored list or a new empty list |
| `SaveSimpleEdits(List<MetadataEditRule>)` | Replaces literal list |
| `GetBlockedTracks()` | Stored list or a new empty list |
| `SaveBlockedTracks(List<string>)` | Replaces block list |

### Transformation methods

#### `string ApplyEdits(string text, string field)`

Applies matching (`edit.Field == field || "all"`) literal replacements, then matching regex replacements. Regex exceptions are ignored. The method does not check `MetadataEditRule.Enabled`.

#### `bool IsTrackBlocked(string artist, string track)`

Builds lowercase `artist - track` and returns true when it contains any lowercase block string. Null arguments or null entries are not guarded.

#### `void UpgradeSettings()`

When `SettingsVersion` is absent, optionally migrates `LastFmScrobblingEnabled`, writes version 1, and saves.

Private generic `Get<T>` falls back on any exception. `Set<T>` writes/saves and suppresses any exception.

## `MetadataEditRule`

| Property | Type | Description |
|---|---|---|
| `Field` | `string` | `title`, `artist`, `album`, or `all` by convention |
| `Pattern` | `string` | Literal text or regex |
| `Replacement` | `string` | Replacement text |
| `Enabled` | `bool` | Defaults true; not consulted during transformation |
| `Description` | `string` | Optional annotation; not exposed by current UI |

Constructors:

- parameterless, for serialization/object initialization;
- `(string field, string pattern, string replacement, string desc = "")`, sets all core fields and enables rule.

## `ThemeInfo`

Plain theme definition:

| Property | Type | Current use |
|---|---|---|
| `Name` | `string` | Settings list/lookup |
| `Accent` | `Color` | Applied accent |
| `Background` | `Color` | Declared but not read by manager |
| `Foreground` | `Color` | Declared but not read |
| `Subtle` | `Color` | Declared but not read |

## `ThemeManager` (static)

| Member | Description |
|---|---|
| `ThemeNames` | Returns a new list of names from the private catalog |
| `CurrentTheme` | Exact name match; falls back to first/Default |
| `ApplyTheme()` | Writes accent and dark/light phone resources; dark mode also writes text styles |
| `GetTextStyle(Color,double)` | Private helper creating a `TextBlock` style with foreground/font size |

The private theme catalog contains ten entries.

## `BoolToVisibilityConverter : IValueConverter`

- `Convert`: casts input to `bool`; true → `Visible`, false → `Collapsed`.
- `ConvertBack`: casts input to `Visibility`; `Visible` → true.

## `InverseBoolToVisibilityConverter : IValueConverter`

- `Convert`: true → `Collapsed`, false → `Visible`.
- `ConvertBack`: every visibility other than `Visible` → true.

Both converters ignore `targetType`, `parameter`, and `culture` and perform direct casts.
