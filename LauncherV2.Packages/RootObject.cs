using System;
using System.Collections.Generic;

namespace LauncherV2.Packages
{
	public class RootObject
	{
		public string CurrentVersion
		{
			get;
			set;
		}

		public string SupportedEts2
		{
			get;
			set;
		}

		public string SupportedAts
		{
			get;
			set;
		}

		public string UpdaterVersion
		{
			get;
			set;
		}

		public string BackgroundUrl
		{
			get;
			set;
		}

		public string NewsTitle
		{
			get;
			set;
		}

		public string NewsLinkDesc
		{
			get;
			set;
		}

		public string NewsLinkUrl
		{
			get;
			set;
		}

		public List<Package> Packages
		{
			get;
			set;
		}
	}
}
