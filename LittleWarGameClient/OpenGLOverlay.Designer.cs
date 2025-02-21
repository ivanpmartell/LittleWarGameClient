
namespace LittleWarGameClient
{
    partial class OpenGLOverlay
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
            openGLControl1 = new SharpGL.OpenGLControl();
            ((System.ComponentModel.ISupportInitialize)openGLControl1).BeginInit();
            SuspendLayout();
            // 
            // openGLControl1
            // 
            openGLControl1.Dock = DockStyle.Fill;
            openGLControl1.DrawFPS = false;
            openGLControl1.FrameRate = 60;
            openGLControl1.Location = new Point(0, 0);
            openGLControl1.Margin = new Padding(0);
            openGLControl1.Name = "openGLControl1";
            openGLControl1.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            openGLControl1.RenderContextType = SharpGL.RenderContextType.NativeWindow;
            openGLControl1.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            openGLControl1.Size = new Size(1445, 681);
            openGLControl1.TabIndex = 0;
            openGLControl1.OpenGLDraw += openGLControl1_OpenGLDraw;
            // 
            // OpenGLOverlay
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Black;
            ClientSize = new Size(1445, 681);
            Controls.Add(openGLControl1);
            DoubleBuffered = true;
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(1264, 681);
            Name = "OpenGLOverlay";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "OpenGLOverlay";
            TransparencyKey = Color.Black;
            FormClosing += OpenGLOverlay_FormClosing;
            Load += this.OpenGLOverlay_Load;
            Shown += OpenGLOverlay_Shown;
            ((System.ComponentModel.ISupportInitialize)openGLControl1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private SharpGL.OpenGLControl openGLControl1;
    }
}