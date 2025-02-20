using LittleWarGameClient.Handlers;
using Loyc.Collections;
using Steamworks;
using SharpGL;
using Loyc;

namespace LittleWarGameClient
{
    internal readonly record struct Notification
    {
        internal string Message { get; }
        internal DateTime PostedTime { get; }

        internal Notification(string msg)
        {
            Message = msg;
            PostedTime = DateTime.Now;
        }
    }

    internal partial class OverlayForm : Form
    {
        private static OverlayForm? formInstance;
        internal static OverlayForm Instance
        {
            get
            {
                if (formInstance == null || formInstance.IsDisposed)
                    formInstance = new OverlayForm();
                return formInstance;
            }
        }

        private bool IsGameFormLoaded = false;
        private readonly BDictionary<string, Notification> overlayMessages;
        internal void AddOverlayMessage(string name, Notification notification)
        {
            overlayMessages[name] = new Notification(notification.Message);
        }

        internal bool IsActivated { get; private set; }

        internal OverlayForm()
        {
            overlayMessages = new BDictionary<string, Notification>();
            IsActivated = false;
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

        private void textTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < overlayMessages.Count; i++)
            {
                if (overlayMessages[i].Value.PostedTime.AddSeconds(6) < DateTime.Now)
                    overlayMessages.RemoveAt(i);
            }
        }

        private void OnGameOverlayActivated(bool overlayActivated)
        {
            if (overlayActivated)
            {
                InvokeUI(() =>
                {
                    IsActivated = true;
                    TransparencyKey = Color.Fuchsia;
                    GameForm.Instance.ActiveControl = null;
                });
            }
            else
            {
                InvokeUI(() =>
                {
                    IsActivated = false;
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

        private void OverlayForm_Load(object sender, EventArgs e)
        {
            if (!IsGameFormLoaded)
            {
                IsGameFormLoaded = true;
                GameForm.Instance.Show();
            }
        }

        private void OverlayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.None:
                    e.Cancel = true;
                    break;
            }
        }

        private void OverlayForm_Shown(object sender, EventArgs e)
        {
            TopMost = true;
            textTimer.Enabled = true;
            AddOverlayMessage($"InitDone", new Notification("Overlay Initialized"));
        }
    }
}
