using CefSharp;
using nud2dlib;
using nud2dlib.Windows.Forms;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LittleWarGameClient
{
    public partial class OverlayForm : D2DForm
    {
        private static OverlayForm? thisForm;
        internal static bool IsActivated { get; private set; }
        public OverlayForm()
        {
            thisForm = this;
            IsActivated = false;
            InitializeComponent();
            try
            {
                SteamClient.Init(480);
                SteamScreenshots.Hooked = true;
                SteamScreenshots.OnScreenshotRequested += OnScreenShotRequested;
                SteamFriends.OnGameOverlayActivated += OnGameOverlayActivated;
            }
            catch { }
        }

        private void OnGameOverlayActivated(bool overlayActivated)
        {
            if (overlayActivated)
            {
                InvokeUI(() =>
                {
                    TransparencyKey = Color.Fuchsia;
                    IsActivated = true;
                    KeyPreview = true;
                    Activate();
                });
            }
            else
            {
                InvokeUI(() =>
                {
                    TransparencyKey = Color.Black;
                    IsActivated = false;
                    KeyPreview = false;
                });
            }
        }

        private void OnScreenShotRequested()
        {
            return;
        }

        internal static void InvokeUI(Action a)
        {
            if (thisForm != null)
                thisForm.BeginInvoke(new MethodInvoker(a));
        }

        private void OverlayForm_Load(object sender, EventArgs e)
        {
            var gameForm = new GameForm(this);
            gameForm.Show();
        }
    }
}
