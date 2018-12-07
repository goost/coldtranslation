using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ColdTranslation.Properties;
using Optional;

namespace ColdTranslation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, o) =>{ Settings.Default.Location = PointToScreen(new Point(0, 0)); };
            Closed += (s, o) =>{ Settings.Default.Save(); };
            Loaded += (s, o) =>
            {
                
                Topmost = true;
                //ShowInTaskbar = false;

            };
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
            }
            MigrateSettings(OldSettings);
            Top = Settings.Default.Location.Y;
            Left = Settings.Default.Location.X;
        }

        private static void MigrateSettings(Option<Legacy.Settings> settingsO)
        {
            settingsO.MatchSome(settings =>
            {
                Settings.Default.LastRows = settings.LastRows;
                Settings.Default.HideSpeaker = settings.HideSpeaker;
                Settings.Default.LastTranslationSheet = settings.LastTranslationSheet;
                Settings.Default.Location = new Point(settings.Location.X, settings.Location.Y);
                Settings.Default.Sen4Mode = settings.Sen4Mode;
            });
            
        }

        private static Option<Legacy.Settings> OldSettings
        {
            get
            {
                try
                {
                    if (!File.Exists(Legacy.Settings.SettingsPath)) return Option.None<Legacy.Settings>();
                    var settings = Option.Some(Legacy.Settings.Deserialize(Legacy.Settings.SettingsPath));
                    File.Move(Legacy.Settings.SettingsPath, $"{Legacy.Settings.SettingsPath}.old");
                    return settings;
                }
                catch (Exception)
                {
                    return Option.None<Legacy.Settings>();
                }



            }
        }
    }
}
