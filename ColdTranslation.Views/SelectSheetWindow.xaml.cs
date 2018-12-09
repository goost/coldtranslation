using System;
using System.Collections.Generic;
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

namespace ColdTranslation.Views
{
    /// <summary>
    /// Interaction logic for SelectSheetWindow.xaml
    /// </summary>
    public partial class SelectSheetWindow : Window
    {
        public bool IsSen4Mode => Sen4Radio.IsChecked.HasValue && Sen4Radio.IsChecked.Value;
        public string SelectedSheet => (string)SheetBox.SelectedItem;

        public SelectSheetWindow(IEnumerable<string> sheets, bool isSen4Mode)
        {
            InitializeComponent();
            Sen3Radio.IsChecked = !isSen4Mode;
            Sen4Radio.IsChecked = isSen4Mode;

            SheetBox.ItemsSource = sheets;
            SheetBox.SelectedItem = SheetBox.Items[0];
            ConfirmButton.Click += (s, e) => { DialogResult = true; };
        }
    }
}
