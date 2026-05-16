using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WebRadioFM.Helpers
{
    public class ThemeInfo
    {
        public string Name { get; set; }
        public Color Accent { get; set; }
        public Color Background { get; set; }
        public Color Foreground { get; set; }
        public Color Subtle { get; set; }
    }

    public static class ThemeManager
    {
        private static readonly List<ThemeInfo> Themes = new List<ThemeInfo>
        {
            new ThemeInfo { Name = "Default",   Accent = Color.FromArgb(255, 0, 150, 136) },
            new ThemeInfo { Name = "Indigo",    Accent = Color.FromArgb(255, 63, 81, 181) },
            new ThemeInfo { Name = "Pink",      Accent = Color.FromArgb(255, 234, 30, 99) },
            new ThemeInfo { Name = "Red",       Accent = Color.FromArgb(255, 221, 67, 55) },
            new ThemeInfo { Name = "Orange",    Accent = Color.FromArgb(255, 255, 152, 0) },
            new ThemeInfo { Name = "Green",     Accent = Color.FromArgb(255, 76, 175, 80) },
            new ThemeInfo { Name = "Blue",      Accent = Color.FromArgb(255, 33, 150, 243) },
            new ThemeInfo { Name = "Purple",    Accent = Color.FromArgb(255, 156, 39, 176) },
            new ThemeInfo { Name = "Teal",      Accent = Color.FromArgb(255, 0, 150, 136) },
            new ThemeInfo { Name = "Amber",     Accent = Color.FromArgb(255, 255, 193, 7) },
        };

        public static List<string> ThemeNames
        {
            get { return Themes.Select(t => t.Name).ToList(); }
        }

        public static ThemeInfo CurrentTheme
        {
            get
            {
                string name = SettingsManager.ThemeName;
                var theme = Themes.FirstOrDefault(t => t.Name == name);
                if (theme == null)
                    theme = Themes[0];
                return theme;
            }
        }

        public static void ApplyTheme()
        {
            var theme = CurrentTheme;
            bool dark = SettingsManager.ThemeIsDark;

            Color bg = dark ? Color.FromArgb(255, 0, 0, 0) : Color.FromArgb(255, 255, 255, 255);
            Color fg = dark ? Color.FromArgb(255, 255, 255, 255) : Color.FromArgb(255, 0, 0, 0);
            Color subtle = dark ? Color.FromArgb(200, 200, 200, 200) : Color.FromArgb(150, 100, 100, 100);

            var res = Application.Current.Resources;

            res["PhoneAccentBrush"] = new SolidColorBrush(theme.Accent);
            res["PhoneAccentColor"] = theme.Accent;

            res["PhoneBackgroundBrush"] = new SolidColorBrush(bg);
            res["PhoneBackgroundColor"] = bg;

            res["PhoneForegroundBrush"] = new SolidColorBrush(fg);
            res["PhoneForegroundColor"] = fg;

            res["PhoneSubtleBrush"] = new SolidColorBrush(subtle);
            res["PhoneSubtleColor"] = subtle;

            res["PhoneChromeBrush"] = new SolidColorBrush(
                dark ? Color.FromArgb(255, 30, 30, 30) : Color.FromArgb(255, 240, 240, 240));

            if (dark)
            {
                res["PhoneTextNormalStyle"] = GetTextStyle(fg, 16);
                res["PhoneTextTitle1Style"] = GetTextStyle(fg, 28);
                res["PhoneTextTitle2Style"] = GetTextStyle(fg, 24);
                res["PhoneTextLargeStyle"] = GetTextStyle(fg, 18);
                res["PhoneTextSubtleStyle"] = GetTextStyle(subtle, 14);
            }
        }

        private static Style GetTextStyle(Color color, double size)
        {
            var style = new Style(typeof(System.Windows.Controls.TextBlock));
            style.Setters.Add(new Setter(System.Windows.Controls.TextBlock.ForegroundProperty, new SolidColorBrush(color)));
            style.Setters.Add(new Setter(System.Windows.Controls.TextBlock.FontSizeProperty, size));
            return style;
        }
    }
}
