using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.OpenExe.VM
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using ControlzEx.Theming;
    using Tools.OpenExe.Exts;

    public class ColorViewModel : NotifyBase
    {
        private Color standardColor;
        private Color highlightColor;

        public ColorViewModel()
        {
            this.StandardColor = Colors.Black;
            this.HighlightColor = Colors.Yellow;

            CollectionViewSource.GetDefaultView(ThemeManager.Current.Themes).GroupDescriptions.Add(new PropertyGroupDescription(nameof(Theme.BaseColorScheme)));
        }

        public Color StandardColor
        {
            get { return this.standardColor; }

            set
            {
                if (value == this.standardColor)
                {
                    return;
                }

                this.standardColor = value;
                this.OnPropertyChanged();
            }
        }

        public Color HighlightColor
        {
            get { return this.highlightColor; }

            set
            {
                if (value == this.highlightColor)
                {
                    return;
                }

                this.highlightColor = value;
                this.OnPropertyChanged();
            }
        }

        public ReadOnlyObservableCollection<Color> ThemeColors { get; } = new ReadOnlyObservableCollection<Color>(new ObservableCollection<Color> { Colors.Red, Colors.Green, Colors.Blue, Colors.White, Colors.Black, Colors.Purple });

#pragma warning disable INPC010 // The property sets a different field than it returns.
        public Color ThemeColor
        {
            get => ((SolidColorBrush)Application.Current.Resources["Fluent.Ribbon.Brushes.AccentBaseColorBrush"])?.Color ?? Colors.Pink;

            set
            {
                var solidColorBrush = new SolidColorBrush(value);
                solidColorBrush.Freeze();
                Application.Current.Resources["Fluent.Ribbon.Brushes.AccentBaseColorBrush"] = solidColorBrush;
                this.OnPropertyChanged();
            }
        }
#pragma warning restore INPC010 // The property sets a different field than it returns.

        public string CurrentBaseColor
        {
            get => this.CurrentTheme.BaseColorScheme;

            set
            {
                if (value is null)
                {
                    return;
                }

                ThemeManager.Current.ChangeThemeBaseColor(Application.Current, value);
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.CurrentTheme));
            }
        }

        public Theme CurrentTheme
        {
            get => ThemeManager.Current.DetectTheme(Application.Current);

            set
            {
                if (value is null)
                {
                    return;
                }

                ThemeManager.Current.ChangeTheme(Application.Current, value);
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.CurrentBaseColor));
            }
        }
    }
}
