---
title: Appearance
description: Select accent colors and switch between dark and light resources.
---

# Appearance

Open **settings → appearance** to select an accent and dark/light mode.

## Accent choices

The app exposes these names through `ThemeManager.ThemeNames`:

| Name | ARGB / hex |
|---|---|
| Default | `#009688` |
| Indigo | `#3F51B5` |
| Pink | `#EA1E63` |
| Red | `#DD4337` |
| Orange | `#FF9800` |
| Green | `#4CAF50` |
| Blue | `#2196F3` |
| Purple | `#9C27B0` |
| Teal | `#009688` |
| Amber | `#FFC107` |

`Default` and `Teal` currently use the same accent value.

Changing the selection stores `ThemeName` and immediately calls `ThemeManager.ApplyTheme()`.

## Dark and light mode

`ThemeIsDark` defaults to `true`.

| Resource | Dark | Light |
|---|---|---|
| Background | Black | White |
| Foreground | White | Black |
| Subtle | Semi-transparent light gray | Semi-transparent dark gray |
| Chrome | `30,30,30` | `240,240,240` |

The manager replaces standard phone resources such as `PhoneAccentBrush`, `PhoneBackgroundBrush`, and `PhoneForegroundBrush`. In dark mode it also rebuilds several `TextBlock` styles. In light mode it does not rebuild those text styles, relying on existing resource behavior.

## Persistence

The selected name and dark-mode Boolean are saved immediately to isolated application settings. They are applied during `App` construction before the root frame is initialized.

## Adding an accent as a developer

Add a `ThemeInfo` item to the private `Themes` list in `Helpers/ThemeManager.cs`:

```csharp
new ThemeInfo
{
    Name = "Cyan",
    Accent = Color.FromArgb(255, 0, 188, 212)
},
```

`Background`, `Foreground`, and `Subtle` properties exist on `ThemeInfo`, but `ApplyTheme()` currently derives those colors exclusively from dark mode and does not read per-theme values.

See [UI, themes, and localization](../architecture/ui-theming-localization.md) for resource flow.
