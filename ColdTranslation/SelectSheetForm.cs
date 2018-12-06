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
        public bool IsSen4Mode => radioButton_sen4.Checked;

        public SelectSheetForm(List<string> sheets, bool isSen4Mode)
        {
            InitializeComponent();
            radioButton_sen3.Checked = !isSen4Mode;
            radioButton_sen4.Checked = isSen4Mode;
            toolTip_sen3.SetToolTip(radioButton_sen3, "Sen 3 Mode tries to guess the speaker from the last mentioned.");
            toolTip_sen4.SetToolTip(radioButton_sen4, "Sen 4 Mode does not guess the speaker and relies on the spreadsheet.");
            Load += (s, ea) => {
                var wa = Screen.PrimaryScreen.WorkingArea;
                Location = new Point(wa.Right/2 - Width, wa.Bottom/2 - Height);
            };
            listBox_sheets.Items.AddRange(sheets.ToArray());
        }
    }
}
