using Loyc.Syntax;
using System.Drawing.Text;

namespace LittleWarGameClient
{
    partial class GameForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            loaderImage = new PictureBox();
            loadingPanel = new Panel();
            loadingText = new TextBox();
            mainImage = new PictureBox();
            loadingTimer = new System.Windows.Forms.Timer(components);
            webBrowser = new CefSharp.WinForms.ChromiumWebBrowser();
            ((System.ComponentModel.ISupportInitialize)loaderImage).BeginInit();
            loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainImage).BeginInit();
            SuspendLayout();
            // 
            // loaderImage
            // 
            loaderImage.Anchor = AnchorStyles.None;
            loaderImage.ErrorImage = null;
            loaderImage.Image = Properties.Resources.wolfRunning;
            loaderImage.InitialImage = null;
            loaderImage.Location = new Point(390, 560);
            loaderImage.MaximumSize = new Size(480, 100);
            loaderImage.Name = "loaderImage";
            loaderImage.Size = new Size(480, 100);
            loaderImage.SizeMode = PictureBoxSizeMode.StretchImage;
            loaderImage.TabIndex = 1;
            loaderImage.TabStop = false;
            // 
            // loadingPanel
            // 
            loadingPanel.Controls.Add(loadingText);
            loadingPanel.Controls.Add(mainImage);
            loadingPanel.Controls.Add(loaderImage);
            loadingPanel.Dock = DockStyle.Fill;
            loadingPanel.ForeColor = Color.White;
            loadingPanel.Location = new Point(0, 0);
            loadingPanel.Name = "loadingPanel";
            loadingPanel.Size = new Size(1264, 681);
            loadingPanel.TabIndex = 2;
            loadingPanel.Visible = false;
            // 
            // loadingText
            // 
            loadingText.Anchor = AnchorStyles.None;
            loadingText.BackColor = Color.Black;
            loadingText.BorderStyle = BorderStyle.None;
            loadingText.ForeColor = Color.White;
            loadingText.Location = new Point(390, 474);
            loadingText.Name = "loadingText";
            loadingText.Size = new Size(480, 16);
            loadingText.TabIndex = 3;
            loadingText.Text = "Loading";
            loadingText.TextAlign = HorizontalAlignment.Center;
            // 
            // mainImage
            // 
            mainImage.Anchor = AnchorStyles.None;
            mainImage.ErrorImage = null;
            mainImage.Image = Properties.Resources.soldier;
            mainImage.InitialImage = null;
            mainImage.Location = new Point(582, 70);
            mainImage.Margin = new Padding(0);
            mainImage.Name = "mainImage";
            mainImage.Size = new Size(100, 100);
            mainImage.SizeMode = PictureBoxSizeMode.StretchImage;
            mainImage.TabIndex = 2;
            mainImage.TabStop = false;
            // 
            // loadingTimer
            // 
            loadingTimer.Interval = 1000;
            loadingTimer.Tick += loadingTimer_Tick;
            // 
            // webBrowser
            // 
            webBrowser.ActivateBrowserOnCreation = true;
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.Location = new Point(0, 0);
            webBrowser.Margin = new Padding(0);
            webBrowser.Name = "webBrowser";
            webBrowser.Size = new Size(1264, 681);
            webBrowser.TabIndex = 3;
            webBrowser.LoadError += webView_LoadError;
            webBrowser.LoadingStateChanged += webView_LoadingStateChanged;
            // 
            // GameForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1264, 681);
            Controls.Add(webBrowser);
            Controls.Add(loadingPanel);
            DoubleBuffered = true;
            ForeColor = Color.FromArgb(0, 192, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1280, 720);
            Name = "GameForm";
            Text = "Littlewargame";
            Activated += GameForm_Activated;
            Deactivate += GameForm_Deactivate;
            FormClosing += GameForm_FormClosing;
            Load += GameForm_Load;
            ResizeEnd += GameForm_ResizeEnd;
            LocationChanged += GameForm_LocationChanged;
            Resize += GameForm_Resize;
            ((System.ComponentModel.ISupportInitialize)loaderImage).EndInit();
            loadingPanel.ResumeLayout(false);
            loadingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)mainImage).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private PictureBox loaderImage;
        private Panel loadingPanel;
        private PictureBox mainImage;
        private TextBox loadingText;
        private System.Windows.Forms.Timer loadingTimer;
        internal CefSharp.WinForms.ChromiumWebBrowser webBrowser;
    }
}
