using System;
using System.IO;
using System.Windows;
using ColdTranslation.Legacy;
using ColdTranslation.Translation;
using Optional;
using Settings = ColdTranslation.Properties.Settings;

namespace ColdTranslation.ViewModel
{
    public class MainWindowViewModel
    {

        public Model.TranslationModel CurrentTranslationModel { get; private set; }

        public TranslationReader TranslationReader { get; }

        public MainWindowViewModel()
        {
#if DEBUG
            ConsoleManager.Show();
#endif
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
            }
            MigrateSettings(OldSettings);

            TranslationReader = new TranslationReader();
            CurrentTranslationModel = new Model.TranslationModel()
            {
                Translation = new Model.Translation()
                {
                    Speech = "Speech",
                    Speaker = "Speaker",
                    Extra = "Extra",
                    Color = "FF000000"
                }
            };


        }


        public void ExitClick(object sender, EventArgs e)
        {
            if (MessageBoxResult.OK == MessageBox.Show("Exit Cold Translation?", "Confirm Exit", MessageBoxButton.OKCancel,
                    MessageBoxImage.Question))
            {
                Application.Current.Shutdown();
            }
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