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
            SuspendLayout();
            // 
            // OverlayForm
            // 
            AnimationDraw = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(800, 450);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Location = new Point(0, 0);
            Name = "OverlayForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "OverlayForm";
            TopMost = true;
            TransparencyKey = Color.Black;
            Load += OverlayForm_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}