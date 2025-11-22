using System.Runtime.InteropServices;
using LittleWarGameClient.Handlers;
using LittleWarGameClient.UI;
using Loyc.Collections;

namespace LittleWarGameClient.Helpers
{
    internal sealed class OverlayHelper
    {
		private static readonly Lazy<OverlayHelper> _instance = new(() => new OverlayHelper());
		internal static OverlayHelper Instance
		{
			get { return _instance.Value; }
		}

        internal bool IsSteamOverlayActivated { get; private set; } = false;

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("Kernel32.dll")]
		private static extern IntPtr LoadLibrary(string path);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate bool IsClause();

		private Task overlayNotificationsTask;
		private PeriodicTimer messageTimer;
		private Task? steamOverlayTask;
        private PeriodicTimer? steamOverlayTimer;
        private IntPtr steamOverlayModulePtr  = IntPtr.Zero;
		private readonly BDictionary<string, Notification> overlayMessages;
        private readonly Dictionary<string, Delegate> steamOverlayFunctions;

		private OverlayHelper()
        {
            overlayMessages = new BDictionary<string, Notification>();
			steamOverlayFunctions = new Dictionary<string, Delegate>();

			messageTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
			overlayNotificationsTask = RunMessagingAsync();

            if (WindowHandler.Instance.SteamOverlayModule != null)
            {
                steamOverlayTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(250));
                steamOverlayTask = RunSteamOverlayVerificationAsync();
            }
        }

        internal void AddOverlayMessage(string name, Notification notification)
        {
            lock (overlayMessages)
            {
                overlayMessages[name] = notification;
            }
        }

        internal Notification[] getOverlayMessages()
        {
			lock (overlayMessages)
			{
				var messages = new Notification[overlayMessages.Count];
				messages = overlayMessages.Values.ToArray();
				return messages;
			}
        }

        private async Task RunMessagingAsync()
        {
            while(await messageTimer.WaitForNextTickAsync())
            {
                for (int i = 0; i < overlayMessages.Count; i++)
                {
                    if (overlayMessages[i].Value.PostedTime.AddSeconds(6) < DateTime.Now)
                    {
						overlayMessages.RemoveAt(i);
                    }
                }
			}
        }

        private async Task RunSteamOverlayVerificationAsync()
        {
            if (steamOverlayTimer == null || GameForm.Instance.GraphicsOverlay == null)
                return;

			bool prevOverlayActivationStatus = IsSteamOverlayActivated;
            while (await steamOverlayTimer.WaitForNextTickAsync())
            {
				if (WindowHandler.Instance.SteamOverlayModule != null)
				{
					var loadedSteamOverlayModule = WindowHandler.Instance.SteamOverlayModule.ModuleName;
                    if (CallSteamOverlayFunction<bool>("IsOverlayEnabled", typeof(IsClause)))
                    {

                        if (CallSteamOverlayFunction<bool>("SteamOverlayIsUsingKeyboard", typeof(IsClause)) &&
                                CallSteamOverlayFunction<bool>("SteamOverlayIsUsingMouse", typeof(IsClause)))
                            IsSteamOverlayActivated = true;
                        else
                            IsSteamOverlayActivated = false;
						if (prevOverlayActivationStatus != IsSteamOverlayActivated)
						{
							prevOverlayActivationStatus = IsSteamOverlayActivated;
							OnSteamOverlayStatusChanged();
						}
						
					}
				}
			}
		}

		private void OnSteamOverlayStatusChanged()
		{
			var graphicsOverlayInstance = GameForm.Instance.GraphicsOverlay;
			if (graphicsOverlayInstance == null)
				return;

			if (IsSteamOverlayActivated)
			{
				graphicsOverlayInstance.InvokeUI(() =>
				{
					graphicsOverlayInstance.ChangeTransparencyKeyTo(Color.Fuchsia);
					GameForm.Instance.ActiveControl = null;
				});
			}
			else
			{
				graphicsOverlayInstance.InvokeUI(() =>
				{
					graphicsOverlayInstance.ChangeTransparencyKeyTo(Color.Black);
					GameForm.Instance.ActiveControl = GameForm.Instance.webBrowser;
				});
			}
		}

		private OutType? CallSteamOverlayFunction<OutType>(string name, Type delegateType)
        {
			if (WindowHandler.Instance.SteamOverlayModule == null)
                return default;

			if (steamOverlayModulePtr == IntPtr.Zero)
				steamOverlayModulePtr = LoadLibrary(WindowHandler.Instance.SteamOverlayModule.ModuleName);
			
			if (!steamOverlayFunctions.ContainsKey(name)) {
				IntPtr funcPtr = GetProcAddress(steamOverlayModulePtr, name);
				if (funcPtr == IntPtr.Zero)
                    throw new InvalidOperationException($"Function does not exist for Steam Overlay: {name}");

				Delegate? func = Marshal.GetDelegateForFunctionPointer(funcPtr, delegateType);
                if (func == null)
					throw new InvalidOperationException($"{name} could not be converted to the desired delegate");
				steamOverlayFunctions[name] = func;
			}

            var steamOverlayFunc = steamOverlayFunctions[name];

			var returnType = steamOverlayFunc.Method.ReturnType;
			if (typeof(OutType) != returnType)
				throw new InvalidOperationException($"{name} does not have the desired return type");

            var result = (OutType?)steamOverlayFunc.DynamicInvoke();
            if (result != null)
                return result;

			return default;
		}

        public async Task StopAsync()
        {
            if(overlayNotificationsTask == null)
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

	internal readonly record struct Notification
	{
		internal string Message { get; }
		internal DateTime PostedTime { get; }

		internal Notification(string msg)
		{
			Message = msg;
			PostedTime = DateTime.Now;
		}
	}
}
