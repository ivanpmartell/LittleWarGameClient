using Loyc.Collections;

namespace LittleWarGameClient.Helpers
{
    internal class OverlayHelper
    {
        private static OverlayHelper? messagesInstance;
        internal static OverlayHelper Instance
        {
            get
            {
                if (messagesInstance == null)
                    messagesInstance = new OverlayHelper();
                return messagesInstance;
            }
        }

        private Task? timerTask;
        private PeriodicTimer messageTimer;
        private readonly CancellationTokenSource cts = new();
        private readonly BDictionary<string, Notification> overlayMessages;
        
        public OverlayHelper()
        { 
            messageTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            overlayMessages = new BDictionary<string, Notification>();
            timerTask = RunTaskAsync();
        }

        internal void AddOverlayMessage(string name, Notification notification)
        {
            overlayMessages[name] = new Notification(notification.Message);
        }

        internal BDictionary<string, Notification> getOverlayMessages()
        {
            return overlayMessages;
        }

        public void Start()
        {
            timerTask = RunTaskAsync();
            Console.WriteLine("WeatherForecastBackgroundService just started");
        }

        private async Task RunTaskAsync()
        {
            try
            {
                while(await messageTimer.WaitForNextTickAsync(cts.Token))
                {
                    for (int i = 0; i < overlayMessages.Count; i++)
                    {
                        if (overlayMessages[i].Value.PostedTime.AddSeconds(6) < DateTime.Now)
                            overlayMessages.RemoveAt(i);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                AddOverlayMessage("overlayExit", new Notification("Exiting overlay..."));
                await Task.Delay(5000);
                overlayMessages.Clear();
            }
        }

        public async Task StopAsync()
        {
            if(timerTask == null)
                return;

            cts.Cancel();
            await timerTask;
            cts.Dispose();
        }
    }

    internal enum OverlayType
    {
        None,
        Direct2D,
        OpenGL
    }
}
