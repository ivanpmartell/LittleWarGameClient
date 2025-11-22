namespace LittleWarGameClient.UI
{
    partial class SplashScreen
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
			pictureBox1 = new PictureBox();
			splashText = new TextBox();
			panel1 = new Panel();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// pictureBox1
			// 
			pictureBox1.BackColor = Color.Black;
			pictureBox1.Location = new Point(12, 25);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(100, 100);
			pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox1.TabIndex = 1;
			pictureBox1.TabStop = false;
			// 
			// splashText
			// 
			splashText.BackColor = Color.Black;
			splashText.BorderStyle = BorderStyle.None;
			splashText.Enabled = false;
			splashText.ForeColor = Color.White;
			splashText.Location = new Point(118, 62);
			splashText.Name = "splashText";
			splashText.Size = new Size(270, 16);
			splashText.TabIndex = 0;
			splashText.Text = "Littlewargame";
			splashText.TextAlign = HorizontalAlignment.Center;
			// 
			// panel1
			// 
			panel1.BackColor = Color.Black;
			panel1.ForeColor = Color.Black;
			panel1.Controls.Add(pictureBox1);
			panel1.Controls.Add(splashText);
			panel1.Location = new Point(0, 0);
			panel1.Name = "panel1";
			panel1.Size = new Size(400, 150);
			panel1.TabIndex = 2;
			// 
			// SplashScreen
			// 
			AutoScaleMode = AutoScaleMode.None;
			BackColor = Color.Black;
			ClientSize = new Size(400, 150);
			Controls.Add(panel1);
			ForeColor = Color.Black;
			FormBorderStyle = FormBorderStyle.None;
			Icon = (Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			Name = "SplashScreen";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Littlewargame";
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			ResumeLayout(false);
		}

		#endregion
		private PictureBox pictureBox1;
        private TextBox splashText;
		private Panel panel1;
	}
}