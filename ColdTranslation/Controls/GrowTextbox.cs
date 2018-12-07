using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;

namespace ColdTranslation.Controls
{
   public class GrowTextBox : TextBox
    {
        //https://stackoverflow.com/questions/9509147/label-word-wrapping
        private bool mGrowing;
      
        private void Resize()
        {
            if (mGrowing) return;
            try
            {
                mGrowing = true;
                var ft = new FormattedText(Text,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                    FontSize, Foreground, null, 1);
                Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                Width = DesiredSize.Width;
                Height = DesiredSize.Height;
            }
            finally
            {
                mGrowing = false;
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            Resize();
        }


       
    }
}