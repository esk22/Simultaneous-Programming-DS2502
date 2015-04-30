namespace DataManagerWindow
{
    partial class HelpWindow
    {
        //// <summary>
        //// Required designer variable.
        //// </summary>
        private System.ComponentModel.IContainer components = null;

        //// <summary>
        //// Clean up any resources being used.
        //// </summary>
        //// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        //// <summary>
        //// Required method for Designer support - do not modify
        //// the contents of this method with the code editor.
        //// </summary>
        private void InitializeComponent()
        {
            this.HelpText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            /// 
            /// HelpText
            /// 
            this.HelpText.Location = new System.Drawing.Point(12, 12);
            this.HelpText.Multiline = true;
            this.HelpText.Name = "HelpText";
            this.HelpText.Size = new System.Drawing.Size(254, 352);
            this.HelpText.TabIndex = 0;
            this.HelpText.TextChanged += new System.EventHandler(this.HelpText_TextChanged);
            /// 
            /// HelpWindow
            /// 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 376);
            this.Controls.Add(this.HelpText);
            this.Name = "HelpWindow";
            this.Text = "HelpWindow";
            this.Load += new System.EventHandler(this.HelpWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox HelpText;

    }
}