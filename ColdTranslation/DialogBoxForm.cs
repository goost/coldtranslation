using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ColdTranslation
{
    public partial class DialogBoxForm : Form
    {
        public bool ShowSpeaker { get; set; }
        public bool Advance { get; private set; }

        private TranslationReader TranslationReader { get; }
        private EventHandler CurrentTicker { get; set; } = (s, e) => { };
        private Settings Settings { get; }
        private Translation CurrentTranslation { get; set; }
        private bool Hide { get; set; }
       
        public DialogBoxForm()
        {
            InitializeComponent();
            Settings = File.Exists(Settings.SettingsPath)
                ? Settings.Deserialize(Settings.SettingsPath)
                : new Settings();

            ControlBox = false;
            Text = string.Empty;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.MintCream;
            TransparencyKey = Color.MintCream;

            var textBackColor = Color.FromArgb(25, 215, 215, 215);
            label_speech.BackColor = textBackColor;
            label_speaker.BackColor = textBackColor;
            label_extra.BackColor = textBackColor;
            label_speaker.Visible = !Settings.HideSpeaker;

            KeyPreview = true;
            KeyDown += KeyDownHandler;
            KeyUp += KeyUpHandler;
            TranslationReader = new TranslationReader(Settings);

            if (string.IsNullOrEmpty(Settings.LastTranslationSheet))
            {
                button_last.Enabled = false;
            }


            Closed += (s, e) =>
            {
                var hwnd = FindWindow("Shell_TrayWnd", "");
                ShowWindow(hwnd, SW_SHOW);
                TranslationReader.Dispose();
                
            };
            Closing += (s, e) =>
            {
                Settings.Location = Location;
                Settings.Serialize(Settings.SettingsPath, Settings);
            };

            Load += (s, e) => { Location = Settings.Location; };

        }

        public void Start()
        {
            var hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_HIDE);


            Location = Settings.Location;
            Console.WriteLine($"Script started.");
        }

        public void OnStopped()
        {
            Settings.Location = Location;
            Settings.Serialize(Settings.SettingsPath, Settings);

            TranslationReader.Dispose();
            button_last.Show();
            button_load.Show();
            Hide();
            

            var hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_SHOW);
            Console.WriteLine("Stopped!");
            
        }


        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Alt || e.Shift || e.Control) return;
            switch (e.KeyCode)
            {
                case Keys.Down:
                    Advance = true;
                    break;
            }
            
        }

        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.Alt || e.Shift || e.Control) return;
            switch (e.KeyCode)
            {
                case Keys.PageDown:
                    Hide = !Hide;
                    label_speech.Visible = !Hide;
                    label_extra.Visible = !Hide;
                    label_speaker.Visible = !Hide;
                    if (!Hide) label_speaker.Visible = !Settings.HideSpeaker;
                    break;
                case Keys.End:
                    Settings.HideSpeaker = !Settings.HideSpeaker;
                    label_speaker.Visible = !Settings.HideSpeaker;
                    Settings.Serialize(Settings.SettingsPath, Settings);
                    break;
                case Keys.Left:
                case Keys.Up:
                    SetTranslation(TranslationReader.Previous());
                    break;
                case Keys.Right:
                    SetTranslation(TranslationReader.Next());
                    break;
                case Keys.Down:
                    Advance = false;
                    if (timer.Enabled)
                    {
                        timer.Enabled = false;
                        label_speech.Text = CurrentTranslation.Speech;
                        break;

                    }
                    SetTranslation(TranslationReader.Next());
                    break;
            }

        }


        private void button_load_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"",
                Filter = "Excel 2007+ Files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                InitTranslation(openFileDialog.FileName);
            }

        }

        private void button_last_Click(object sender, EventArgs e)
        {
            InitTranslation(Settings.LastTranslationSheet);
           
        }

        private void InitTranslation(string fileName)
        {
            Cursor.Current = Cursors.WaitCursor;
            button_last.Enabled = false;
            button_load.Enabled = false;
            var row = TranslationReader.ReadXlsx(fileName, this);
            if (row.HasValue)
            {
                button_last.Hide();
                button_load.Hide();
                //Controls.RemoveByKey("button_load");
                //Controls.RemoveByKey("button_last");
                SetTranslation(row.Value);
            }
           
            button_last.Enabled = true;
            button_load.Enabled = true;
            Cursor.Current = Cursors.Default;
        }

        private void SetTranslation(Translation translation)
        {
            label_speaker.Text = translation.Speaker;
            var argb = 0;
            if (!string.IsNullOrEmpty(translation.Color))
            {
                argb = int.Parse(translation.Color, NumberStyles.HexNumber);
            }
            label_speech.ForeColor = Color.FromArgb(argb);
            timer.Tick -= CurrentTicker;
            if (!string.IsNullOrEmpty(translation.Speech))
            {
                CurrentTicker = TimerTick(translation.Speech);
                timer.Tick += CurrentTicker;
                timer.Interval = 2;
                timer.Enabled = true;
            }
            else
            {
                label_speech.Text = string.Empty;
            }

            label_extra.Text = translation.Extra;

            CurrentTranslation = translation;
        }

        private EventHandler TimerTick(string speech)
        {
            var currentIndex = 0;
            var array = speech.ToCharArray();
            label_speech.Text = "";
            var delay = 40 / timer.Interval;
            return (sender, e) =>
            {
                if (delay-- >= 0) return;
                label_speech.Text = $"{label_speech.Text}{array[currentIndex++]}";
                if (currentIndex >= array.Length)
                {
                    timer.Enabled = false;

                }
            };
        }

        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/e231f5be-5233-4eee-b142-7aef50f37287/disabling-andor-hiding-windows-taskbar?forum=csharpgeneral
        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        private const int WM_NCHITTEST = 0x84;
        private const int HTCAPTION = 0x2;
        private const int HTCLIENT = 0x1;

        ///
        /// Handling the window messages
        /// https://stackoverflow.com/questions/7482922/remove-the-title-bar-in-windows-forms
        ///
        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == WM_NCHITTEST && (int)message.Result == HTCLIENT)
                message.Result = (IntPtr)HTCAPTION;
        }
    }
}
