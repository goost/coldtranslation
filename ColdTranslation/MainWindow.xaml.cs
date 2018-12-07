using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
using ColdTranslation.Legacy;
using ColdTranslation.Translation;
using ColdTranslation.ViewModel;
using Microsoft.Win32;
using Optional;
using PS4RemotePlayInterceptor;
using Path = System.IO.Path;
using Settings = ColdTranslation.Properties.Settings;

namespace ColdTranslation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool Advance { get; set; }
        private System.Windows.Threading.DispatcherTimer Timer { get; }
        private EventHandler CurrentTicker { get; set; } = (s, e) => { };
        private Model.Translation CurrentTranslation { get; set; }
        private TranslationReader TranslationReader { get; }

        private bool LeftDPad { get; set; }
        private bool RightDPad { get; set; }
        private bool Circle { get; set; }
        private bool L3 { get; set; }
        private bool Touch1 { get; set; }
        private bool TouchButton { get; set; }
        private bool HideAll { get; set; }
        private bool ControllerMode { get; set; }
        private BrushConverter BrushConverter { get; } = new BrushConverter();

        private SolidColorBrush ControllerModeColorBrush { get; }
        private SolidColorBrush NotControllerModeColorBrush { get; }


        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            TranslationReader = ((MainWindowViewModel) DataContext).TranslationReader;

            Top = Settings.Default.Location.Y;
            Left = Settings.Default.Location.X;
            Timer = new System.Windows.Threading.DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };

            NotControllerModeColorBrush = (SolidColorBrush)BrushConverter.ConvertFrom("#3EFF0000");
            ControllerModeColorBrush = (SolidColorBrush)BrushConverter.ConvertFrom("#3E00FF00");

            Interceptor.InjectionMode = InjectionMode.Compatibility;
            Interceptor.Callback = OnReceiveData;
            Interceptor.EmulateController = false;

            #region Events
            MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            };
            Closing += (s, e) =>
            {
                Console.WriteLine("Closing");
                Timer.Stop();
                Settings.Default.Location = new Point(Left, Top);
            };
            Closed += (s, e) =>
            {
                Console.WriteLine("CLOSED");
                Interceptor.StopInjection();
                Settings.Default.Save();
            };
            #endregion

            #region Input
            //Keyboard.AddKeyUpHandler(this, KeyUpHandler);
            //Keyboard.AddKeyDownHandler(this, KeyUpHandler);
            KeyUp += KeyUpHandler;
            KeyDown += KeyDownHandler;

            #endregion

            #region Buttons

            ExitButton.Click += ExitHandler;
            PickSheetButton.Click += (s, e) =>
            {
                if (!InitInterception()) return;
                var openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"",
                    Filter = "Excel 2007+ Files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                    FilterIndex = 0,
                    RestoreDirectory = true
                };
                if (openFileDialog.ShowDialog() != true) return;
                if (!InitTranslation(openFileDialog.FileName))
                {
                    MessageBox.Show("Could not load provided XLSX. Is it a translation sheet?", "Parsing Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                };
                ControllerModeIndicator.Fill = NotControllerModeColorBrush;
                ControllerModeIndicator.Visibility = Visibility.Visible;
                Keyboard.Focus(this);
            };

            LastSheetButton.Click += (s, e) =>
            {
                if (!InitInterception()) return;
                if (!InitTranslation(Settings.Default.LastTranslationSheet))
                {
                    MessageBox.Show("Could not load last loaded XLSX. Was the file deleted?", "Load Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                };
                ControllerModeIndicator.Fill = NotControllerModeColorBrush;
                ControllerModeIndicator.Visibility = Visibility.Visible;
                Keyboard.Focus(this);
            };

            #endregion

           

        }


        private bool InitInterception()
        {
            return true;
            try
            {
                Interceptor.Inject();

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Error on injecting." +
                    "\nEither Remote Play is not started or something other hinders the injection." +
                    "\nPlease start/restart RemotePlay before loading a translation.", "Injection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

        }

        private void OnReceiveData(ref DualShockState state)
        {
            if (state.Touch1.IsTouched)
            {
                Touch1 = true;
            }
            else if (Touch1)
            {
                Touch1 = false;
                Dispatcher.Invoke(ToggleControllerMode);
            }


            if (ControllerMode)
            {
                if (state.DPad_Left)
                {
                    LeftDPad = true;
                }
                else if (LeftDPad)
                {
                    LeftDPad = false;
                    Dispatcher.Invoke(RewindTranslation);
                }

                if (state.Circle)
                {
                    Circle = true;
                }
                else if (Circle)
                {
                    Circle = false;
                    Dispatcher.Invoke(AdvanceTranslation);
                }

                if (state.DPad_Right)
                {
                    RightDPad = true;

                }
                else if (RightDPad)
                {
                    RightDPad = false;
                    Dispatcher.Invoke(AdvanceTranslation);
                }

                if (state.L3)
                {
                    L3 = true;
                }
                else if (L3)
                {
                    L3 = false;
                    Dispatcher.Invoke(ToggleVisibility);
                }
            }

            if (Advance)
            {
                state.Circle = true;
            }
        }

        private void ToggleControllerMode()
        {
            ControllerMode = !ControllerMode;
            var color = ControllerMode ? ControllerModeColorBrush : NotControllerModeColorBrush;
            ControllerModeIndicator.Fill = color;

        }

        private void AdvanceTranslation()
        {
            SetTranslation(TranslationReader.Next());
        }

        private void RewindTranslation()
        {
            SetTranslation(TranslationReader.Previous());
        }
        private void ToggleVisibility()
        {
            HideAll = !HideAll;
            var nVisibility = HideAll ? Visibility.Hidden : Visibility.Visible;
            SpeakerBox.Visibility = nVisibility;
            SpeechBox.Visibility = nVisibility;
            ExtraBox.Visibility = nVisibility;
        }


        private void ExitHandler(object sender, RoutedEventArgs e)
        {

            if (MessageBoxResult.OK == MessageBox.Show("Exit Cold Translation?", "Confirm Exit", MessageBoxButton.OKCancel,
                    MessageBoxImage.Question))
            {
                Close();
            }
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt)
                || Keyboard.IsKeyDown(Key.RightAlt)
                || Keyboard.IsKeyDown(Key.LeftCtrl)
                || Keyboard.IsKeyDown(Key.RightCtrl)
                || Keyboard.IsKeyDown(Key.LeftShift)
                || Keyboard.IsKeyDown(Key.RightShift)) return;
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                    Advance = true;
                    break;
            }
        }

        private void KeyUpHandler(object sender, KeyEventArgs e)
        {

            if (Keyboard.IsKeyDown(Key.LeftAlt)
                || Keyboard.IsKeyDown(Key.RightAlt)
                || Keyboard.IsKeyDown(Key.LeftCtrl)
                || Keyboard.IsKeyDown(Key.RightCtrl)
                || Keyboard.IsKeyDown(Key.LeftShift)
                || Keyboard.IsKeyDown(Key.RightShift)) return;
            switch (e.Key)
            {
                case Key.Escape:
                    ExitHandler(sender, e);
                    break;
                case Key.Left:
                    SetTranslation(TranslationReader.Previous());
                    break;
                case Key.Right:
                    SetTranslation(TranslationReader.Next());
                    break;
                case Key.Up:
                    Advance = false;
                    break;
                case Key.Down:
                    Advance = false;
                    SetTranslation(TranslationReader.Next());
                    break;
            }
        }

        private bool InitTranslation(string fileName)
        {
            //Mouse.OverrideCursor = Cursors.Wait;
            Cursor = Cursors.Wait;

            LastSheetButton.IsEnabled = false;
            PickSheetButton.IsEnabled = false;
            var row = TranslationReader.ReadXlsx(fileName, this);
            var returnValue = false;
            if (row.HasValue)
            {
                LastSheetButton.Visibility = Visibility.Hidden;
                PickSheetButton.Visibility = Visibility.Hidden;
                SetTranslation(row.Value);
                returnValue = true;
            }

            LastSheetButton.IsEnabled = true;
            PickSheetButton.IsEnabled = true;
            Cursor = null;
            return returnValue;
        }

        private void SetTranslation(Model.Translation translation)
        {

            SpeechBox.Foreground = (SolidColorBrush)BrushConverter.ConvertFrom($"#{translation.Color}");
            SpeechBox.Text = "";
            SpeakerBox.Text = translation.Speaker;
            ExtraBox.Text = translation.Extra;

            Timer.Stop();
            Timer.Tick -= CurrentTicker;
            CurrentTicker = TimerTick(translation);
            Timer.Tick += CurrentTicker;
            CurrentTranslation = translation;
            Timer.Start();





        }

        private EventHandler TimerTick(Model.Translation translation)
        {
            var currentIndex = 0;
            var speechLength = translation.Speech.Length;
            //TODO (BUG) Should be 6 MS, but is not?
            var delay = 6 / Timer.Interval.TotalMilliseconds;
            return (sender, e) =>
            {

                if (delay-- >= 0) return;
                SpeechBox.Text = $"{translation.Speech.Substring(0, currentIndex++)}";
                if (currentIndex > speechLength)
                {
                    Timer.Stop();

                }

            };
        }

        
    }
}
