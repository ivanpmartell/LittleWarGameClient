using LittleWarGameClient.Handlers;

namespace LittleWarGameClient.UI
{
    public sealed partial class SplashScreen : Form
    {
		private static readonly SplashScreen _instance = new();
		internal static SplashScreen Instance
        {
            get { return _instance; }
        }

        private SplashScreen()
        {
            InitializeComponent();
            splashText.Font = FontHandler.gameFont(24F);
        }

        internal void InvokeUI(Action action)
        {
            if (Instance.InvokeRequired)
            {
                if (Instance.IsHandleCreated)
					Instance.BeginInvoke(new MethodInvoker(action));
            }
            else
            {
                action.Invoke();
            }
        }

        internal void CloseSplashScreen()
        {
			InvokeUI(() =>
			{
				Close();
				Dispose();
			});
		}
    }
}
