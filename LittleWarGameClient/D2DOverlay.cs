using LittleWarGameClient.Handlers;
using Loyc.Collections;
using Steamworks;
using SharpGL;
using Loyc;
using nud2dlib.Windows.Forms;
using nud2dlib;
using LittleWarGameClient.Helpers;

namespace LittleWarGameClient
{
    internal partial class D2DOverlay : D2DForm
    {
        private static D2DOverlay? formInstance;
        internal static D2DOverlay Instance
        {
            get
            {
                if (formInstance == null || formInstance.IsDisposed)
                    formInstance = new D2DOverlay();
                return formInstance;
            }
        }

        internal D2DOverlay()
        {
            Font = FontHandler.gameFont(21.75F);
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

        protected override void OnRender(D2DGraphics g)
        {
            var overlayMessages = OverlayHelper.Instance.getOverlayMessages();
            for (int i = 0; i < overlayMessages.Count; i++)
            {
                string notification = "";
                try
                {
                    var overlayMessageValue = overlayMessages.TryGet(i);
                    if (overlayMessageValue.HasValue)
                        notification = overlayMessageValue.Value.Value.Message;
                }
                catch (Exception)
                {
                    continue;
                }
                if (!String.IsNullOrEmpty(notification))
                    g.DrawText($" >{notification}", D2DColor.Yellow, Font, 0, (i + 1) * 30);
            }
        }

        private void OnGameOverlayActivated(bool overlayActivated)
        {
            if (overlayActivated)
            {
                InvokeUI(() =>
                {
                    GameForm.Instance.isOverlayActivated = true;
                    TransparencyKey = Color.Fuchsia;
                    GameForm.Instance.ActiveControl = null;
                });
            }
            else
            {
                InvokeUI(() =>
                {
                    GameForm.Instance.isOverlayActivated = false;
                    TransparencyKey = Color.Black;
                    GameForm.Instance.ActiveControl = GameForm.Instance.webBrowser;
                });
            }
        }

        private void OnScreenShotRequested()
        {
            return;
        }

        internal void InvokeUI(Action a)
        {
            if (formInstance != null && formInstance.InvokeRequired)
            {
                if (formInstance.IsHandleCreated)
                    formInstance.BeginInvoke(new MethodInvoker(a));
            }
            else
            {
                a.Invoke();
            }
        }

        private void D2DOverlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.None:
                    e.Cancel = true;
                    break;
                default:
                    GameForm.Instance.isOverlayActivated = false;
                    break;
            }
        }

        private void D2DOverlay_Shown(object sender, EventArgs e)
        {
            TopMost = true;
            OverlayHelper.Instance.AddOverlayMessage($"InitDone", new Notification("Overlay Initialized"));
        }

        private void D2DOverlay_Load(object sender, EventArgs e)
        {
            Size = GameForm.Instance.webBrowser.Size;
            var webViewBounds = new Rectangle(GameForm.Instance.webBrowser.PointToScreen(Point.Empty), Size);
            Location = webViewBounds.Location;
        }
    }
}
