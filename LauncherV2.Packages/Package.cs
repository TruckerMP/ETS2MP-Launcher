using System;

namespace LauncherV2.Packages
{
	public class Package
	{
		public string Name
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public bool ReqAts
		{
			get;
			set;
		}

		public bool ReqEts
		{
			get;
			set;
		}

		public bool Optional
		{
			get;
			set;
		}

		public int FileCount
		{
			get;
			set;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public Package()
		{
			this.<FileCount>k__BackingField = 0;
			base..ctor();
		}
	}
}
