using System;
using System.Runtime.CompilerServices;

namespace LauncherV2
{
	internal class Game
	{
		public bool Exists
		{
			get;
			private set;
		}

		public string Name
		{
			[CompilerGenerated]
			get
			{
				return this.<Name>k__BackingField;
			}
		}

		public string ShortName
		{
			[CompilerGenerated]
			get
			{
				return this.<ShortName>k__BackingField;
			}
		}

		public string Id
		{
			get;
			private set;
		}

		public string Directory
		{
			get;
			private set;
		}

		public string ExeName
		{
			get;
			private set;
		}

		public string DllName
		{
			get;
			private set;
		}

		public Game(string gameName, string gameShortName, string gameId, string gameExe, string dllNameToInject)
		{
			this.<Name>k__BackingField = gameName;
			this.<ShortName>k__BackingField = gameShortName;
			this.Id = gameId;
			this.ExeName = gameExe;
			this.DllName = dllNameToInject;
			this.Directory = Launcher.Instance.GetGameDirectory(this.ShortName);
			this.Exists = (this.Directory != "");
		}
	}
}
