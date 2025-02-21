using Loyc.Collections;

namespace LittleWarGameClient.Helpers
{
    internal class OverlayHelper
    {
        private static OverlayHelper? instance;
        internal static OverlayHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new OverlayHelper();
                return instance;
            }
        }

        private Task? timerTask;
        private PeriodicTimer messageTimer;
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

        private async Task RunTaskAsync()
        {
            while(await messageTimer.WaitForNextTickAsync())
            {
                for (int i = 0; i < overlayMessages.Count; i++)
                {
                    if (overlayMessages[i].Value.PostedTime.AddSeconds(6) < DateTime.Now)
                        overlayMessages.RemoveAt(i);
                }
            }
        }

        public async Task StopAsync()
        {
            if(timerTask == null)
                return;

            AddOverlayMessage("overlayExit", new Notification("Exiting overlay..."));
            await Task.Delay(1000);
            overlayMessages.Clear();
        }
    }

    internal enum OverlayType
    {
        None,
        Direct2D,
        OpenGL
    }
}
