using LittleWarGameClient.Handlers;
using LittleWarGameClient.UI;

namespace LittleWarGameClient
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
			WindowHandler.Instance.ShowSplashScreen();
			if (!WindowHandler.Instance.IsDoubleInstance)
			{
				ApplicationConfiguration.Initialize();
				Application.Run(GameForm.Instance);
			}
			else
			{
				WindowHandler.Instance.CloseSplashScreen();
				WindowHandler.Instance.ShowOriginalMainWindow();
			}
		}
    }
}