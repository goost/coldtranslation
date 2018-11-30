namespace ColdTranslation
{
    partial class DialogBoxForm
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
            this.button_load = new System.Windows.Forms.Button();
            this.label_extra = new ColdTranslation.GrowLabel();
            this.label_speech = new ColdTranslation.GrowLabel();
            this.label_speaker = new ColdTranslation.GrowLabel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.button_last = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_load
            // 
            this.button_load.Location = new System.Drawing.Point(238, 8);
            this.button_load.Name = "button_load";
            this.button_load.Size = new System.Drawing.Size(97, 33);
            this.button_load.TabIndex = 3;
            this.button_load.Text = "Pick Sheet";
            this.button_load.UseVisualStyleBackColor = true;
            this.button_load.Click += new System.EventHandler(this.button_load_Click);
            // 
            // label_extra
            // 
            this.label_extra.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label_extra.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_extra.Location = new System.Drawing.Point(12, 111);
            this.label_extra.Name = "label_extra";
            this.label_extra.Size = new System.Drawing.Size(706, 20);
            this.label_extra.TabIndex = 2;
            this.label_extra.Text = "Extra";
            // 
            // label_speech
            // 
            this.label_speech.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label_speech.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_speech.Location = new System.Drawing.Point(26, 45);
            this.label_speech.Name = "label_speech";
            this.label_speech.Size = new System.Drawing.Size(692, 29);
            this.label_speech.TabIndex = 1;
            this.label_speech.Text = "Speech";
            // 
            // label_speaker
            // 
            this.label_speaker.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label_speaker.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_speaker.Location = new System.Drawing.Point(2, 9);
            this.label_speaker.Name = "label_speaker";
            this.label_speaker.Size = new System.Drawing.Size(221, 26);
            this.label_speaker.TabIndex = 0;
            this.label_speaker.Text = "Speaker";
            // 
            // button_last
            // 
            this.button_last.Location = new System.Drawing.Point(353, 8);
            this.button_last.Name = "button_last";
            this.button_last.Size = new System.Drawing.Size(97, 33);
            this.button_last.TabIndex = 4;
            this.button_last.Text = "Last Sheet";
            this.button_last.UseVisualStyleBackColor = true;
            this.button_last.Click += new System.EventHandler(this.button_last_Click);
            // 
            // DialogBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 161);
            this.Controls.Add(this.button_last);
            this.Controls.Add(this.button_load);
            this.Controls.Add(this.label_extra);
            this.Controls.Add(this.label_speech);
            this.Controls.Add(this.label_speaker);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DialogBoxForm";
            this.Text = "Legend Translator";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button_load;
        private GrowLabel label_speaker;
        private GrowLabel label_speech;
        private GrowLabel label_extra;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button button_last;
    }
}