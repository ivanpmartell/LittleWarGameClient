using LittleWarGameClient.Handlers;

namespace LittleWarGameClient
{
    public partial class SplashScreen : Form
    {
        private static SplashScreen? formInstance;
        internal static SplashScreen Instance
        {
            get
            {
                if (formInstance == null || formInstance.IsDisposed)
                    formInstance = new SplashScreen();
                return formInstance;
            }
        }

        public SplashScreen()
        {
            InitializeComponent();
            splashText.Font = FontHandler.gameFont(24F);
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

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            if (Program.IsDoubleInstance)
            {
                Close();
            }
        }
    }
}
