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
            this.components = new System.ComponentModel.Container();
            this.button_ok = new System.Windows.Forms.Button();
            this.listBox_sheets = new System.Windows.Forms.ListBox();
            this.radioButton_sen3 = new System.Windows.Forms.RadioButton();
            this.radioButton_sen4 = new System.Windows.Forms.RadioButton();
            this.toolTip_sen3 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_sen4 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_ok
            // 
            this.button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_ok.Location = new System.Drawing.Point(36, 220);
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
            // radioButton_sen3
            // 
            this.radioButton_sen3.AutoSize = true;
            this.radioButton_sen3.Location = new System.Drawing.Point(30, 19);
            this.radioButton_sen3.Name = "radioButton_sen3";
            this.radioButton_sen3.Size = new System.Drawing.Size(53, 17);
            this.radioButton_sen3.TabIndex = 8;
            this.radioButton_sen3.TabStop = true;
            this.radioButton_sen3.Text = "Sen 3";
            this.radioButton_sen3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_sen3.UseVisualStyleBackColor = true;
            // 
            // radioButton_sen4
            // 
            this.radioButton_sen4.AutoSize = true;
            this.radioButton_sen4.Location = new System.Drawing.Point(115, 19);
            this.radioButton_sen4.Name = "radioButton_sen4";
            this.radioButton_sen4.Size = new System.Drawing.Size(53, 17);
            this.radioButton_sen4.TabIndex = 9;
            this.radioButton_sen4.TabStop = true;
            this.radioButton_sen4.Text = "Sen 4";
            this.radioButton_sen4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_sen4.UseVisualStyleBackColor = true;
            // 
            // toolTip_sen3
            // 
            this.toolTip_sen3.AutomaticDelay = 250;
            // 
            // toolTip_sen4
            // 
            this.toolTip_sen4.AutomaticDelay = 250;
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.radioButton_sen3);
            this.groupBox.Controls.Add(this.radioButton_sen4);
            this.groupBox.Location = new System.Drawing.Point(36, 164);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(200, 50);
            this.groupBox.TabIndex = 10;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Speaker Mode";
            // 
            // SelectSheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 267);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.listBox_sheets);
            this.Controls.Add(this.button_ok);
            this.Name = "SelectSheetForm";
            this.Text = "SelectSheetForm";
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.ListBox listBox_sheets;
        private System.Windows.Forms.RadioButton radioButton_sen3;
        private System.Windows.Forms.RadioButton radioButton_sen4;
        private System.Windows.Forms.ToolTip toolTip_sen3;
        private System.Windows.Forms.ToolTip toolTip_sen4;
        private System.Windows.Forms.GroupBox groupBox;
    }
}