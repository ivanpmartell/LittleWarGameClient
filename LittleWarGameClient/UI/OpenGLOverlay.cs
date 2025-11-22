using LittleWarGameClient.Handlers;
using SharpGL;
using LittleWarGameClient.Helpers;

namespace LittleWarGameClient.UI
{
    internal sealed partial class OpenGLOverlay : Form, IGraphicsOverlay
    {
		internal OpenGLOverlay()
        {
            Font = FontHandler.gameFont(21.75F);
            InitializeComponent();
        }

        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs e)
        {
            OpenGL gl = this.openGLControl1.OpenGL;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            var overlayMessages = OverlayHelper.Instance.getOverlayMessages();
			for (int i = 0; i < overlayMessages.Count(); i++)
			{
				var notification = overlayMessages[i].Message;
				if (!String.IsNullOrEmpty(notification))
					gl.DrawText(0, (i + 1) * 25, 1.0f, 1.0f, 0.0f, "LCD Solid", 14.0f, $" >{notification}");
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

		private void OpenGLOverlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.None:
                    e.Cancel = true;
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
