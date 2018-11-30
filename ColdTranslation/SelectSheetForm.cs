using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ColdTranslation
{
    public partial class SelectSheetForm : Form
    {
        public string SelectedSheet => (string) listBox_sheets.SelectedItem;

        public SelectSheetForm(List<string> sheets)
        {
            InitializeComponent();
            Load += (s, ea) => {
                var wa = Screen.PrimaryScreen.WorkingArea;
                Location = new Point(wa.Right/2 - Width, wa.Bottom/2 - Height);
            };
            listBox_sheets.Items.AddRange(sheets.ToArray());
        }
    }
}
