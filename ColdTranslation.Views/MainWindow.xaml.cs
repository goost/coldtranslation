using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ColdTranslation.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EventHandler CurrentTicker { get; set; } = (s, e) => { };

        public MainWindow()
        {
            InitializeComponent();

            

            SpeechBoxHidden.TargetUpdated += (s, e) =>
            {
                var speech = e.TargetObject.GetValue(TextBlock.TextProperty) as string;
                SpeechBox.Text = "";
                var sa = new StringAnimationUsingKeyFrames();
                var currentTime = 125;
                sa.KeyFrames.Add(
                    new DiscreteStringKeyFrame(
                        "",
                        KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))
                    ));
                if (!string.IsNullOrEmpty(speech))
                {
                    for (var i = 1; i < speech.Length + 1; i++)
                    {
                        var k = new DiscreteStringKeyFrame(
                            SpeechBoxHidden.Text.Substring(0, i),
                            KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(currentTime))
                        );
                        sa.KeyFrames.Add(k);
                        currentTime += 9;
                    }
                }

                SpeechBox.BeginAnimation(TextBlock.TextProperty, sa);
            };


//            var dp = DependencyPropertyDescriptor.FromProperty(
//                TextBlock.TextProperty,
//                typeof(TextBlock));
//            dp.AddValueChanged(SpeechBoxHidden, (sender, args) =>
//            {
//                
//            });
        }
    }
}
