using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace LauncherV2
{
	public class Debug : Window, IComponentConnector
	{
		private const string EtsId = "227300";

		private const string AtsId = "270880";

		internal Debug DebugWindow;

		internal TextBox txtResults;

		private bool _contentLoaded;

		public Debug()
		{
			this.InitializeComponent();
			this.CheckSteamKeys("227300", "ETS");
			this.CheckSteamKeys("270880", "ATS");
		}

		private void DebugWindow_OnKeyDown(object sender, KeyEventArgs e)
		{
			bool flag = e.Key == Key.Escape;
			if (flag)
			{
				base.Close();
			}
		}

		private void CheckSteamKeys(string id, string name)
		{
			this.AddResultLine("Steam Registry Key Information for " + name + ":");
			this.AddResultLine("");
			this.AddResultLine("Software\\Valve\\Steam\\Apps\\" + id);
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam\\Apps\\" + id))
			{
				bool flag = registryKey != null;
				if (flag)
				{
					this.AddResultLine(name + " Registry Key Found");
					bool flag2 = (registryKey.GetValue("Installed") ?? "").ToString() == "1";
					if (flag2)
					{
						this.AddResultLine(name + " Installed");
					}
					else
					{
						this.AddResultLine(name + " Not Installed");
					}
				}
				else
				{
					this.AddResultLine(name + " Registry Key Not Found");
				}
			}
			this.AddResultLine("");
			this.AddResultLine("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App " + id);
			RegistryKey registryKey2 = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
			using (RegistryKey registryKey3 = registryKey2.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App " + id))
			{
				bool flag3 = registryKey3 != null;
				if (flag3)
				{
					this.AddResultLine(name + " Uninstall Key Found");
					bool flag4 = (registryKey3.GetValue("InstallLocation") ?? "").ToString() != "";
					if (flag4)
					{
						string text = registryKey3.GetValue("InstallLocation").ToString();
						this.AddResultLine("Install Location: " + text);
						this.AddResultLine(Directory.Exists(text) ? "Install Directory Exists" : "Install Directory Doesn't Exist");
					}
					else
					{
						this.AddResultLine("No Install Location Found");
					}
				}
				else
				{
					this.AddResultLine(name + " Uninstall Key Not Found");
				}
			}
			this.AddResultLine("");
		}

		private void AddResultLine(string str)
		{
			TextBox textBox = this.txtResults;
			textBox.Text = textBox.Text + str + "\r\n";
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
		public void InitializeComponent()
		{
			bool contentLoaded = this._contentLoaded;
			if (!contentLoaded)
			{
				this._contentLoaded = true;
				Uri resourceLocator = new Uri("/Launcher;component/debug.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId != 1)
			{
				if (connectionId != 2)
				{
					this._contentLoaded = true;
				}
				else
				{
					this.txtResults = (TextBox)target;
				}
			}
			else
			{
				this.DebugWindow = (Debug)target;
				this.DebugWindow.KeyDown += new KeyEventHandler(this.DebugWindow_OnKeyDown);
			}
		}
	}
}
