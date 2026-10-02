---
title: UI, theming, and localization
description: XAML page composition, application resources, converters, themes, and RESX language flow.
---

# UI, theming, and localization

The UI follows the Windows Phone Silverlight model: XAML declares controls/bindings, while page code-behind handles events and assigns item sources/text directly.

## Page composition

### Main page

- application bar: play/pause, next, previous, shuffle;
- menu: settings, stats, track info;
- track `ListBox` bound to `Track` objects;
- active title/artist/album and heart button;
- display-only progress slider and times; and
- track count/scrobble status.

### Settings page

A Pivot contains:

- **scrobbling** — account, switches, thresholds, metadata/block rules;
- **appearance** — accent list and dark mode; and
- **about** — version/help and stats navigation.

### Stats page

A Pivot contains recent tracks, top artists, and top tracks, plus an application-bar refresh command.

All pages support portrait orientation only and leave the system tray visible.

## Binding model

The application uses a mixture of binding and direct assignments:

- list templates bind to model properties;
- page code assigns `ItemsSource` and text/control properties;
- `Track` implements `INotifyPropertyChanged`;
- Last.fm response models do not notify because they are immutable for page use; and
- `BoolToVisibilityConverter` drives the recent-track **now playing** label.

`Track.DisplayText` is computed but does not raise its own notification when Title/Artist changes. In current use, tracks are populated before binding and not edited afterward.

## Global resources

`App.xaml` registers:

```xml
<local:LocalizedStrings x:Key="LocalizedStrings"/>
<helpers:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter"/>
<helpers:InverseBoolToVisibilityConverter x:Key="InverseBoolToVisibilityConverter"/>
```

Platform phone resources provide brushes, colors, fonts, and text styles referenced throughout page XAML.

## Theme application

`ThemeManager.ApplyTheme()` reads saved theme name/dark mode and replaces entries in `Application.Current.Resources`:

- `PhoneAccentBrush` / `PhoneAccentColor`
- `PhoneBackgroundBrush` / `PhoneBackgroundColor`
- `PhoneForegroundBrush` / `PhoneForegroundColor`
- `PhoneSubtleBrush` / `PhoneSubtleColor`
- `PhoneChromeBrush`

In dark mode it additionally replaces `PhoneTextNormalStyle`, `PhoneTextTitle1Style`, `PhoneTextTitle2Style`, `PhoneTextLargeStyle`, and `PhoneTextSubtleStyle` with minimal `TextBlock` styles containing foreground and font size.

Changing a theme is global and immediate; there is no theme object passed into individual pages.

## Localization resources

`Resources/AppResources.resx` defines English values and the generated `AppResources.Designer.cs` exposes them. `LocalizedStrings` supplies one static `AppResources` instance for XAML access.

The startup frame reads:

- `ResourceLanguage` (`en-US`); and
- `ResourceFlowDirection` (`LeftToRight`).

However, many current user-facing strings are hard-coded directly in XAML/code-behind rather than using `AppResources`. The app is therefore resource-capable but not fully localized.

## Adding a translation

A complete localization effort should:

1. move every user-visible XAML/code-behind string into `AppResources.resx`;
2. replace hard-coded XAML with resource bindings;
3. add culture-specific `.resx` files, such as `AppResources.pt-BR.resx`;
4. list supported cultures in `WebRadioFM.csproj`;
5. verify generated designer accessibility;
6. test long labels and right-to-left flow where relevant; and
7. localize message boxes, application-bar labels, status text, and plural forms.

Do not hand-edit generated `AppResources.Designer.cs`; regenerate it through the resource tool after changing the base RESX.

## Converters

`BoolToVisibilityConverter` maps `true` to `Visible` and `false` to `Collapsed`. The inverse converter does the opposite. Both `ConvertBack` methods infer a Boolean from `Visibility`; they cast directly and will throw for unexpected types.

## Accessibility considerations

Current XAML relies on native control semantics but lacks explicit automation names/help text in source. When extending the UI:

- preserve high contrast with custom accents;
- do not encode state only by heart/icon color;
- provide accessible labels for icon-only controls;
- verify focus and keyboard order in the emulator;
- respect text scaling and long localized strings; and
- announce loading/error states for asynchronous statistics/authentication.
