namespace LittleWarGameClient
{
    partial class OverlayForm
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
            components = new System.ComponentModel.Container();
            textTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // textTimer
            // 
            textTimer.Enabled = true;
            textTimer.Interval = 1000;
            textTimer.Tick += textTimer_Tick;
            // 
            // OverlayForm
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
            Name = "OverlayForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "OverlayForm";
            TopMost = true;
            TransparencyKey = Color.Black;
            FormClosing += OverlayForm_FormClosing;
            Load += OverlayForm_Load;
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Timer textTimer;
    }
}