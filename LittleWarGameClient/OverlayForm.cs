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
        private DateTime lastChanged;
        private static string? overlayMessage;
        internal static string? OverlayMessage
        {
            get => overlayMessage;

            set
            {
                if (overlayMessage != value)
                {
                    overlayMessage = value;
                    OverlayForm.OverlayMessageChanged(null, EventArgs.Empty);
                }
            }
        }
        internal static bool IsActivated { get; private set; }

        public static event EventHandler? OverlayMessageChanged;

        public OverlayForm()
        {
            OverlayMessageChanged += MessageChanged;
            thisForm = this;
            overlayMessage = "";
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

        private void MessageChanged(object? sender, EventArgs e)
        {
            lastChanged = DateTime.Now;
        }

        protected override void OnRender(D2DGraphics g)
        {
            if (overlayMessage != null && overlayMessage != "")
            {
                g.DrawText($" >{overlayMessage}", D2DColor.Yellow, Font, 0, 25);
            }
        }

        private void textTimer_Tick(object sender, EventArgs e)
        {
             if (lastChanged.AddSeconds(3) < DateTime.Now)
            {
                overlayMessage = "";
            }
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
