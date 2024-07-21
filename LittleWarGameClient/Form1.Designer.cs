namespace LittleWarGameClient
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            loaderImage = new PictureBox();
            loadingPanel = new Panel();
            mainImage = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)loaderImage).BeginInit();
            loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainImage).BeginInit();
            SuspendLayout();
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = Color.Black;
            webView.Dock = DockStyle.Fill;
            webView.Location = new Point(0, 0);
            webView.Margin = new Padding(0);
            webView.Name = "webView";
            webView.Size = new Size(1264, 681);
            webView.TabIndex = 0;
            webView.ZoomFactor = 1D;
            webView.NavigationCompleted += webView_NavigationCompleted;
            webView.WebMessageReceived += webView_WebMessageReceived;
            // 
            // loaderImage
            // 
            loaderImage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            loaderImage.ErrorImage = null;
            loaderImage.Image = Properties.Resources.loader;
            loaderImage.Location = new Point(1164, 581);
            loaderImage.Name = "loaderImage";
            loaderImage.Size = new Size(100, 100);
            loaderImage.SizeMode = PictureBoxSizeMode.StretchImage;
            loaderImage.TabIndex = 1;
            loaderImage.TabStop = false;
            // 
            // loadingPanel
            // 
            loadingPanel.Controls.Add(mainImage);
            loadingPanel.Controls.Add(loaderImage);
            loadingPanel.Dock = DockStyle.Fill;
            loadingPanel.Location = new Point(0, 0);
            loadingPanel.Name = "loadingPanel";
            loadingPanel.Size = new Size(1264, 681);
            loadingPanel.TabIndex = 2;
            loadingPanel.Visible = false;
            // 
            // mainImage
            // 
            mainImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mainImage.ErrorImage = null;
            mainImage.Image = Properties.Resources.soldier;
            mainImage.Location = new Point(432, 140);
            mainImage.Name = "mainImage";
            mainImage.Size = new Size(400, 400);
            mainImage.SizeMode = PictureBoxSizeMode.StretchImage;
            mainImage.TabIndex = 2;
            mainImage.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1264, 681);
            Controls.Add(loadingPanel);
            Controls.Add(webView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1280, 720);
            Name = "Form1";
            Text = "Littlewargame";
            Activated += Form1_Activated;
            ResizeEnd += Form1_ResizeEnd;
            Resize += Form1_Resize;
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            ((System.ComponentModel.ISupportInitialize)loaderImage).EndInit();
            loadingPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainImage).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private PictureBox loaderImage;
        private Panel loadingPanel;
        private PictureBox mainImage;
    }
}
