namespace LittleWarGameClient.Handlers
{
	internal class ArgumentsHandler
	{
		private readonly Dictionary<string, string> _argsDictionary;

		public ArgumentsHandler()
		{
			var args = Environment.GetCommandLineArgs()[1..];
			args = Array.ConvertAll(args, d => d.ToLower());
			_argsDictionary = new();

			for (int i = 0; i < args.Length; i += 2)
			{
				if (args.Length == i + 1 || args[i + 1].StartsWith("-"))
				{
					_argsDictionary.Add(args[i][1..], string.Empty);
					i--;
				}
				if (args.Length >= i + 1 && !args[i + 1].StartsWith("-"))
					_argsDictionary.Add(args[i][1..], args[i + 1]);
			}
		}

		private string GetArgumentValue(string name)
		{
			if (!_argsDictionary.TryGetValue(name, out string? value))
				value = "";
			return value;
		}

		public string GetProfileArgumentOrDefault()
		{
			var value = GetArgumentValue("profile");
			if (String.IsNullOrEmpty(value))
				return "main";
			return value;
		}
	}
}
