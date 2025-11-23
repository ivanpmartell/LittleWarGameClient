using LittleWarGameClient.Handlers;
using LittleWarGameClient.Helpers;
using LittleWarGameClient.UI;

namespace LittleWarGameClient
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var windowHandler = new WindowHandler();

			windowHandler.RunApplication((splashScreenCTS) =>
            {
                ApplicationConfiguration.Initialize();
                GameForm.Instance.SplashScreenCancellationTokenSource = splashScreenCTS;
				Application.Run(GameForm.Instance);
            });
		}
    }
}