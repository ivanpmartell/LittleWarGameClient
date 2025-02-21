using LittleWarGameClient.Handlers;
using Loyc.Collections;
using Steamworks;
using SharpGL;
using Loyc;
using LittleWarGameClient.Helpers;
using System.Windows.Forms;

namespace LittleWarGameClient
{
    internal partial class OpenGLOverlay : Form
    {
        private static OpenGLOverlay? formInstance;
        internal static OpenGLOverlay Instance
        {
            get
            {
                if (formInstance == null || formInstance.IsDisposed)
                    formInstance = new OpenGLOverlay();
                return formInstance;
            }
        }

        internal OpenGLOverlay()
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

        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs e)
        {
            //  Get the OpenGL object, just to clean up the code.
            OpenGL gl = this.openGLControl1.OpenGL;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);	// Clear The Screen And The Depth Buffer
            gl.LoadIdentity();					// Reset The View

            var overlayMessages = OverlayHelper.Instance.getOverlayMessages();
            for (int i = 0; i < overlayMessages.Count; i++)
            {
                string notification = "";
                try {
                    var overlayMessageValue = overlayMessages.TryGet(i);
                    if (overlayMessageValue.HasValue)
                        notification = overlayMessageValue.Value.Value.Message;
                }
                catch (Exception)
                {
                    continue;
                }
                if (!String.IsNullOrEmpty(notification))
                    gl.DrawText(0, (i + 1) * 20, 1.0f, 1.0f, 0.0f, "LCD Solid", 14.0f, $" >{notification}");
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

        private void OpenGLOverlay_FormClosing(object sender, FormClosingEventArgs e)
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

        private void OpenGLOverlay_Shown(object sender, EventArgs e)
        {
            TopMost = true;
            OverlayHelper.Instance.AddOverlayMessage($"InitDone", new Notification("Overlay Initialized"));
        }

        private void OpenGLOverlay_Load(object sender, EventArgs e)
        {
            Size = GameForm.Instance.webBrowser.Size;
            var webViewBounds = new Rectangle(GameForm.Instance.webBrowser.PointToScreen(Point.Empty), Size);
            Location = webViewBounds.Location;
        }
    }
}
