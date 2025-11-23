using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using LittleWarGameClient.Helpers;
using LittleWarGameClient.UI;

namespace LittleWarGameClient.Handlers
{
	internal class WindowHandler
	{
		[DllImport("user32.dll")]
		private static extern void SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		private static extern void EnumWindows(CallBackPtr lpEnumFunc, IntPtr lParam);

		private delegate bool CallBackPtr(IntPtr hwnd, int lParam);
		private readonly CallBackPtr callBackPtr;

		private List<ProcessInfo> _WindowStructList = new();

		private struct ProcessInfo
		{
			internal string? WindowTitle;
			internal IntPtr MainWindowHandle;
		}

		public WindowHandler()
		{
			callBackPtr = AddProcessWindowInfoToList;
		}

		internal void RunApplication(Action<CancellationTokenSource> action)
		{
			var cts = new CancellationTokenSource();
			var ct = cts.Token;
			ShowSplashScreen(ct);
			using (Mutex mutex = new(true, $"Global\\LittleWarGameClient_{ProcessHelper.Instance.Profile}"))
			{
				var hasHandle = false;
				try
				{
					try
					{
						hasHandle = mutex.WaitOne(1000, false);
						if (hasHandle == false)
						{
							ShowOriginalMainWindow();
							cts.Cancel();
						}
						else
						{
							action.Invoke(cts);
						}
					}
					catch (AbandonedMutexException)
					{
						hasHandle = true;
					}
				}
				finally
				{
					if (hasHandle)
					{
						mutex.ReleaseMutex();
					}
				}
			}
		}

		private void ShowSplashScreen(CancellationToken ct)
		{
			Thread splashScreenThread = new(() => new SplashScreen(ct).ShowDialog());
			splashScreenThread.IsBackground = true;
			splashScreenThread.SetApartmentState(ApartmentState.STA);
			splashScreenThread.Start();
		}

		private void ShowOriginalMainWindow()
		{
			Process current = Process.GetCurrentProcess();
			foreach (Process process in Process.GetProcessesByName(current.ProcessName))
			{
				if (process.Id == current.Id)
					continue;

				var clientWindows = GetProcessMainWindow(process.Handle).Where(window => window.WindowTitle == ProcessHelper.Instance.MainWindowTitle);
				if (clientWindows.Any())
				{
					var clientMainWindow = clientWindows.First();
					SetForegroundWindow(clientMainWindow.MainWindowHandle);
					break;
				}
			}
		}

		private List<ProcessInfo> GetProcessMainWindow(IntPtr processHandle)
		{
			_WindowStructList = new List<ProcessInfo>();
			EnumWindows(callBackPtr, processHandle);
			return _WindowStructList;
		}

		private bool AddProcessWindowInfoToList(IntPtr hWnd, int lparam)
		{
			StringBuilder sb = new(256);
			GetWindowText(hWnd, sb, 256);
			if (sb.Length > 0)
				_WindowStructList.Add(new ProcessInfo { MainWindowHandle = hWnd, WindowTitle = sb.ToString() });
			return true;
		}
	}
}
