using System.Diagnostics;

namespace LittleWarGameClient.Handlers;

internal sealed class ProcessHelper
{
	private static readonly Lazy<ProcessHelper> _instance = new(() => new ProcessHelper());
	internal static ProcessHelper Instance
	{
		get { return _instance.Value; }
	}

	internal readonly string MainWindowTitle;
	internal readonly string Profile;
	internal readonly string ExeDirectory;
	internal readonly ProcessModule? SteamOverlayModule;

	private ProcessHelper()
	{
		Profile = new ArgumentsHandler().GetProfileArgumentOrDefault();
		if (!Profile.All(Char.IsLetterOrDigit))
			throw new InvalidDataException("Profile can only contain letters or digits");

		MainWindowTitle = $"Littlewargame({Profile})";
		ExeDirectory = Path.GetDirectoryName(Application.ExecutablePath)!;

		Process currentProcess = Process.GetCurrentProcess();
		var loadedModules = currentProcess.Modules;
		SteamOverlayModule = currentProcess.Modules.Cast<ProcessModule>()
			.FirstOrDefault(m => m.ModuleName.StartsWith("GameOverlayRenderer", StringComparison.OrdinalIgnoreCase));
	}
}
