using LittleWarGameClient.Helpers;
using nud2dlib;
using nud2dlib.Windows.Forms;

namespace LittleWarGameClient.UI
{
    internal sealed partial class D2DOverlay : D2DForm, IGraphicsOverlay
    {
		internal D2DOverlay()
        {
            Font = new Font("LCD Solid", 14.0f);
            InitializeComponent();
        }

        protected override void OnRender(D2DGraphics g)
        {
            var overlayMessages = OverlayHelper.Instance.getOverlayMessages();
            for (int i = 0; i < overlayMessages.Count(); i++)
            {
                var notification = overlayMessages[i].Message;
                if (!String.IsNullOrEmpty(notification))
                    g.DrawText($" >{notification}", D2DColor.Yellow, Font, 0, (i + 1) * 25);
            }
        }

        public void InvokeUI(Action a)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated)
					BeginInvoke(new MethodInvoker(a));
            }
            else
            {
                a.Invoke();
            }
        }

		public void ChangeTransparencyKeyTo(System.Drawing.Color color)
		{
			TransparencyKey = color;
		}

		public void SetLocationTo(Point newLocation)
		{
            Location = newLocation;
		}

		public void SetSizeTo(Size newSize)
		{
            Size = newSize;
		}

		public void MakeVisible(bool option)
		{
            Visible = option;
		}

		private void D2DOverlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.None:
                    e.Cancel = true;
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
