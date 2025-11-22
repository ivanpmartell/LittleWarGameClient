using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using LittleWarGameClient.UI;

namespace LittleWarGameClient.Handlers;

internal sealed class WindowHandler
{
	private static readonly Lazy<WindowHandler> _instance = new(() => new WindowHandler());
	internal static WindowHandler Instance
	{
		get { return _instance.Value; }
	}

	internal readonly string MainWindowTitle;
	internal readonly string Profile;
	internal readonly string ExeDirectory;
	internal readonly ProcessModule? SteamOverlayModule;

	private bool? _doubleInstance = null;

	internal bool IsDoubleInstance
	{	get
		{
			if (_doubleInstance == null)
			{
				_doubleInstance = DoesTwinProcessExist();
				return _doubleInstance.Value;
			}
			return _doubleInstance.Value;
		}
	}

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

	private WindowHandler()
	{
		Profile = new ArgumentsHandler().GetProfileArgumentOrDefault();
		MainWindowTitle = $"Littlewargame({Profile})";
		ExeDirectory = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath)!;
		callBackPtr = AddProcessWindowInfoToList;

		Process currentProcess = Process.GetCurrentProcess();
		var loadedModules = currentProcess.Modules;
		SteamOverlayModule = currentProcess.Modules.Cast<ProcessModule>()
			.FirstOrDefault(m => m.ModuleName.StartsWith("GameOverlayRenderer", StringComparison.OrdinalIgnoreCase));
	}

	internal void ShowSplashScreen()
	{
		Thread splashthread = new(() => SplashScreen.Instance.ShowDialog());
		splashthread.IsBackground = true;
		splashthread.SetApartmentState(ApartmentState.STA);
		splashthread.Start();
	}

	internal void CloseSplashScreen()
	{
		SplashScreen.Instance.CloseSplashScreen();
	}

	internal void ShowOriginalMainWindow()
	{
		Process current = Process.GetCurrentProcess();
		foreach (Process process in Process.GetProcessesByName(current.ProcessName))
		{
			if (process.Id == current.Id)
				continue;

			var clientWindows = GetProcessMainWindow(process.Handle).Where(window => window.WindowTitle == MainWindowTitle);
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

	private bool DoesTwinProcessExist()
	{
		using Mutex mutex = new(true, $"Global\\LittleWarGameClient_{Profile}", out bool createdNew);
		if (createdNew)
			return false;
		return true;
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
