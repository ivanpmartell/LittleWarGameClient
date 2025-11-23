using LittleWarGameClient.Handlers;

namespace LittleWarGameClient.UI
{
	internal sealed partial class SplashScreen : Form
	{
		private readonly CancellationToken _token;
		internal SplashScreen(CancellationToken ct)
		{
			_token = ct;

			//Load resources as early as possible before initialisation
			var gameFont = FontHandler.gameFont(24F);
			var soldierImage = Properties.Resources.soldier;
			InitializeComponent();
			splashText.Font = gameFont;
			pictureBox1.Image = soldierImage;
			ct.Register(CloseSplashScreen);
		}

		private void SplashScreen_Load(object sender, EventArgs e)
		{
			if (_token.IsCancellationRequested)
			{
				Close();
			}
		}

		private void CloseSplashScreen()
		{
			if (Application.OpenForms.OfType<SplashScreen>().Any())
			{
				if (InvokeRequired)
				{
					if (IsHandleCreated)
						BeginInvoke(Close);
				}
				else
					Close();
			}
		}
	}
}
