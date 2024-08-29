using LittleWarGameClient.Handlers;
using Loyc.Collections;
using nud2dlib;
using nud2dlib.Windows.Forms;
using Steamworks;

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

    internal partial class OverlayForm : D2DForm
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
            InitializeComponent();
            Font = new Font(FontHandler.lwgFont, 21.75F, FontStyle.Regular, GraphicsUnit.Point);

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
            for (int i = 0; i < overlayMessages.Count; i++)
            {
                var overlayMessageValue = overlayMessages.TryGet(i);
                if (overlayMessageValue.HasValue)
                {
                    var notification = overlayMessageValue.Value.Value.Message;
                    g.DrawText($" >{notification}", D2DColor.Yellow, Font, 0, (i + 1) * 30);
                }
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
