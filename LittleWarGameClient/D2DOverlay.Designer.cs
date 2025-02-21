namespace LittleWarGameClient
{
    partial class D2DOverlay
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
            SuspendLayout();
            // 
            // D2DOverlay
            // 
            AnimationDraw = true;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Black;
            ClientSize = new Size(1445, 681);
            DoubleBuffered = true;
            EscapeKeyToClose = false;
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Location = new Point(0, 0);
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(1264, 681);
            Name = "D2DOverlay";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "D2DOverlay";
            TransparencyKey = Color.Black;
            FormClosing += D2DOverlay_FormClosing;
            Load += D2DOverlay_Load;
            Shown += D2DOverlay_Shown;
            ResumeLayout(false);
        }

        #endregion
    }
}