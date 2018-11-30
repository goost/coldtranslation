namespace ColdTranslation
{
    partial class SelectSheetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_ok = new System.Windows.Forms.Button();
            this.listBox_sheets = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // button_ok
            // 
            this.button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_ok.Location = new System.Drawing.Point(36, 174);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(194, 35);
            this.button_ok.TabIndex = 6;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            // 
            // listBox_sheets
            // 
            this.listBox_sheets.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox_sheets.FormattingEnabled = true;
            this.listBox_sheets.ItemHeight = 16;
            this.listBox_sheets.Location = new System.Drawing.Point(36, 25);
            this.listBox_sheets.Name = "listBox_sheets";
            this.listBox_sheets.Size = new System.Drawing.Size(194, 132);
            this.listBox_sheets.TabIndex = 7;
            // 
            // SelectSheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 222);
            this.Controls.Add(this.listBox_sheets);
            this.Controls.Add(this.button_ok);
            this.Name = "SelectSheetForm";
            this.Text = "SelectSheetForm";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.ListBox listBox_sheets;
    }
}