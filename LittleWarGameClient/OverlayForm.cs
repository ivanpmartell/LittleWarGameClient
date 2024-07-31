using Loyc.Collections;
using nud2dlib;
using nud2dlib.Windows.Forms;
using Steamworks;

namespace LittleWarGameClient
{
    internal struct Notification
    {
        internal string message { get; }
        internal DateTime postedTime { get; }

        internal Notification(string msg)
        {
            this.message = msg;
            this.postedTime = DateTime.Now;
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
        private BDictionary<string, Notification> overlayMessages;
        internal void AddOverlayMessage(string name, Notification notification)
        {
            overlayMessages[name] = new Notification(notification.message);
        }

        internal bool IsActivated { get; private set; }

        internal OverlayForm()
        {
            overlayMessages = new BDictionary<string, Notification>();
            IsActivated = false;
            InitializeComponent();
            if (Program.LWG_FONT != null)
                Font = new Font(Program.LWG_FONT, 21.75F, FontStyle.Regular, GraphicsUnit.Point);

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
                var notification = overlayMessages[i].Value.message;
                g.DrawText($" >{notification}", D2DColor.Yellow, Font, 0, (i + 1) * 30);
            }
        }

        private void textTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < overlayMessages.Count; i++)
            {
                if (overlayMessages[i].Value.postedTime.AddSeconds(6) < DateTime.Now)
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
                    GameForm.Instance.Visible = false;
                    TransparencyKey = Color.Fuchsia;
                });
            }
            else
            {
                InvokeUI(() =>
                {
                    IsActivated = false;
                    GameForm.Instance.Visible = true;
                    TransparencyKey = Color.Black;
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
    }
}
