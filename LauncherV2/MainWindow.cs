using LauncherV2.Packages;
using LauncherV2.UpdateInfo;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace LauncherV2
{
	public class MainWindow : Window, IComponentConnector
	{
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly MainWindow.<>c <>9 = new MainWindow.<>c();

			public static Func<CheckBox, bool> <>9__13_0;

			public static Func<CheckBox, Package> <>9__13_1;

			public static Func<Package, string> <>9__13_2;

			public static Func<CheckBox, bool> <>9__17_0;

			public static Func<CheckBox, bool> <>9__18_0;

			public static Func<Package, bool> <>9__21_0;

			public static Func<CheckBox, bool> <>9__32_0;

			public static Func<CheckBox, bool> <>9__32_1;

			internal bool <cmdInstallUpdate_Click>b__13_0(CheckBox packageCheckBox)
			{
				return packageCheckBox.IsChecked.HasValue && packageCheckBox.IsChecked.Value;
			}

			internal Package <cmdInstallUpdate_Click>b__13_1(CheckBox packageCheckBox)
			{
				return (Package)packageCheckBox.Content;
			}

			internal string <cmdInstallUpdate_Click>b__13_2(Package package)
			{
				return package.Type;
			}

			internal bool <chkBox_Checked>b__17_0(CheckBox p)
			{
				return p.IsChecked.HasValue && p.IsChecked.Value;
			}

			internal bool <chkBox_Unchecked>b__18_0(CheckBox p)
			{
				return p.IsChecked.HasValue && p.IsChecked.Value;
			}

			internal bool <CheckForUpdates>b__21_0(Package p)
			{
				return p.FileCount > 0 && (!p.ReqAts || MainWindow.HasAts) && (!p.ReqEts || MainWindow.HasEts);
			}

			internal bool <CheckLaunchAllowed>b__32_0(CheckBox c)
			{
				return !((Package)c.Content).Optional && ((Package)c.Content).Type != "ats";
			}

			internal bool <CheckLaunchAllowed>b__32_1(CheckBox c)
			{
				return !((Package)c.Content).Optional && ((Package)c.Content).Type != "ets2";
			}
		}

		private LauncherV2.UpdateInfo.RootObject _updateRoot;

		private LauncherV2.Packages.RootObject _packageRoot;

		private readonly Queue<DownloadItem> _downloadQueue = new Queue<DownloadItem>();

		private readonly List<CheckBox> _packageCheckBoxes = new List<CheckBox>();

		private int _packageCount = 0;

		private int _curPackageIndex = 0;

		private List<string> _packageList = new List<string>();

		internal MainWindow winMain;

		internal Grid gridMain;

		internal Image imgBackground;

		internal Button cmdLaunchEts2;

		internal Button cmdLaunchAts;

		internal Label lblProgress;

		internal ProgressBar prgProgress;

		internal Button cmdInstallUpdate;

		internal ListBox lstPackages;

		internal Label lblNewsTitle;

		internal Label lblNewsLink;

		internal Image cmd_close;

		internal Label lblSupportLink;

		internal Label lblFacebookLink;

		internal Label lblStatusLink;

		internal Label lblProgressOverall;

		internal Label txtVersionInfo;

		private bool _contentLoaded;

		private static bool HasAts
		{
			get
			{
				return MainWindow.CheckGameInstalled(Launcher.Instance.Games["ats"]);
			}
		}

		private static bool HasEts
		{
			get
			{
				return MainWindow.CheckGameInstalled(Launcher.Instance.Games["ets"]);
			}
		}

		public MainWindow()
		{
			this.InitializeComponent();
			Launcher.Instance.Setup();
			this.GetProjects();
			this.CheckForUpdates();
		}

		private void cmdLaunchEts2_Click(object sender, RoutedEventArgs e)
		{
			Launcher.Instance.Launch("ets", this.GetGameCommandLine());
			Environment.Exit(0);
		}

		private void cmdLaunchAts_Click(object sender, RoutedEventArgs e)
		{
			Launcher.Instance.Launch("ats", this.GetGameCommandLine());
			Environment.Exit(0);
		}

		private void LblSupportLink_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start("https://support.truckersmp.com/");
		}

		private void LblStatusLink_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start("https://truckersmpstatus.com/");
		}

		private void LblFacebookLink_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start("https://facebook.com/truckersmpofficial");
		}

		private void cmdInstallUpdate_Click(object sender, RoutedEventArgs e)
		{
			this.Visible(new UIElement[]
			{
				this.lblProgress,
				this.prgProgress,
				this.lblProgressOverall
			}, true);
			this.Visible(new UIElement[]
			{
				this.cmdInstallUpdate,
				this.lstPackages
			}, false);
			IEnumerable<CheckBox> arg_70_0 = this._packageCheckBoxes;
			Func<CheckBox, bool> arg_70_1;
			if ((arg_70_1 = MainWindow.<>c.<>9__13_0) == null)
			{
				arg_70_1 = (MainWindow.<>c.<>9__13_0 = new Func<CheckBox, bool>(MainWindow.<>c.<>9.<cmdInstallUpdate_Click>b__13_0));
			}
			IEnumerable<CheckBox> arg_94_0 = arg_70_0.Where(arg_70_1);
			Func<CheckBox, Package> arg_94_1;
			if ((arg_94_1 = MainWindow.<>c.<>9__13_1) == null)
			{
				arg_94_1 = (MainWindow.<>c.<>9__13_1 = new Func<CheckBox, Package>(MainWindow.<>c.<>9.<cmdInstallUpdate_Click>b__13_1));
			}
			IEnumerable<Package> arg_B8_0 = arg_94_0.Select(arg_94_1);
			Func<Package, string> arg_B8_1;
			if ((arg_B8_1 = MainWindow.<>c.<>9__13_2) == null)
			{
				arg_B8_1 = (MainWindow.<>c.<>9__13_2 = new Func<Package, string>(MainWindow.<>c.<>9.<cmdInstallUpdate_Click>b__13_2));
			}
			this._packageList = arg_B8_0.Select(arg_B8_1).ToList<string>();
			this._packageCount = this._downloadQueue.Count((DownloadItem d) => this._packageList.Contains(d.Type));
			this._curPackageIndex = 0;
			this.DownloadFile();
		}

		private void cmdClose_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
		{
			bool flag = e.Key == Key.Escape;
			if (flag)
			{
				Application.Current.Shutdown();
			}
			bool flag2 = e.Key == Key.F1;
			if (flag2)
			{
				this.EmptyFolder(new DirectoryInfo(Launcher.Instance.Location));
				this.CheckForUpdates();
			}
			bool flag3 = e.Key == Key.F2;
			if (flag3)
			{
				Debug debug = new Debug();
				debug.Show();
			}
		}

		private void EmptyFolder(DirectoryInfo directoryInfo)
		{
			FileInfo[] files = directoryInfo.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				FileInfo fileInfo = files[i];
				fileInfo.Delete();
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			for (int j = 0; j < directories.Length; j++)
			{
				DirectoryInfo directoryInfo2 = directories[j];
				this.EmptyFolder(directoryInfo2);
			}
		}

		private void chkBox_Checked(object sender, EventArgs e)
		{
			UIElement arg_3A_1 = this.cmdInstallUpdate;
			IEnumerable<CheckBox> arg_2D_0 = this._packageCheckBoxes;
			Func<CheckBox, bool> arg_2D_1;
			if ((arg_2D_1 = MainWindow.<>c.<>9__17_0) == null)
			{
				arg_2D_1 = (MainWindow.<>c.<>9__17_0 = new Func<CheckBox, bool>(MainWindow.<>c.<>9.<chkBox_Checked>b__17_0));
			}
			this.Visible(arg_3A_1, arg_2D_0.Select(arg_2D_1).Count<bool>() > 0);
		}

		private void chkBox_Unchecked(object sender, EventArgs e)
		{
			UIElement arg_3A_1 = this.cmdInstallUpdate;
			IEnumerable<CheckBox> arg_2D_0 = this._packageCheckBoxes;
			Func<CheckBox, bool> arg_2D_1;
			if ((arg_2D_1 = MainWindow.<>c.<>9__18_0) == null)
			{
				arg_2D_1 = (MainWindow.<>c.<>9__18_0 = new Func<CheckBox, bool>(MainWindow.<>c.<>9.<chkBox_Unchecked>b__18_0));
			}
			this.Visible(arg_3A_1, arg_2D_0.Select(arg_2D_1).Count<bool>() > 1);
		}

		private void LblNewsLink_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start(this._packageRoot.NewsLinkUrl);
		}

		private void GetProjects()
		{
			try
			{
				string text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
				this._packageRoot = JsonConvert.DeserializeObject<LauncherV2.Packages.RootObject>(new WebClient().DownloadString("http://update.ets2mp.com/packages.json"));
				this.lblNewsTitle.Content = this._packageRoot.NewsTitle;
				this.lblNewsLink.Content = this._packageRoot.NewsLinkDesc;
				this.imgBackground.Source = new BitmapImage(new Uri(this._packageRoot.BackgroundUrl));
				this.txtVersionInfo.Content = "Installed Updater Version: " + text;
				Label label = this.txtVersionInfo;
				label.Content = label.Content + "\nCurrent Updater Version: " + this._packageRoot.UpdaterVersion;
				label = this.txtVersionInfo;
				label.Content = label.Content + "\nSupported ATS Version: " + this._packageRoot.SupportedAts;
				label = this.txtVersionInfo;
				label.Content = label.Content + "\nSupported ETS2 Version: " + this._packageRoot.SupportedEts2;
				label = this.txtVersionInfo;
				label.Content = label.Content + "\nCurrent Patch Version: " + this._packageRoot.CurrentVersion.Replace("Version ", "");
				bool flag = text != this._packageRoot.UpdaterVersion;
				if (flag)
				{
					MessageBox.Show(string.Concat(new object[]
					{
						"Please download the new installer: ",
						this._packageRoot.UpdaterVersion,
						" (Installed Version - ",
						Assembly.GetExecutingAssembly().GetName().Version,
						")"
					}));
					Process.Start("https://truckersmp.com/download");
					Process.GetCurrentProcess().Kill();
				}
			}
			catch (Exception ex)
			{
				bool flag2 = this.Retry("An error occured while contacting our update servers (" + ex.ToString() + ").  Press OK to try again or cancel to visit our download page.", "Connection Error - Retry?");
				if (flag2)
				{
					this.GetProjects();
				}
				else
				{
					Process.Start("https://truckersmp.com/download");
					Process.GetCurrentProcess().Kill();
				}
			}
		}

		private void CheckForUpdates()
		{
			this.Visible(new UIElement[]
			{
				this.cmdInstallUpdate,
				this.lblProgress,
				this.prgProgress,
				this.lstPackages,
				this.lblProgressOverall
			}, false);
			this._packageCheckBoxes.Clear();
			this.lstPackages.Items.Clear();
			this._downloadQueue.Clear();
			foreach (Package current in this._packageRoot.Packages)
			{
				current.FileCount = 0;
			}
			string[] array = new string[]
			{
				"http://update.ets2mp.com/files.json"
			};
			try
			{
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string address = array2[i];
					this._updateRoot = JsonConvert.DeserializeObject<LauncherV2.UpdateInfo.RootObject>(new WebClient().DownloadString(address));
					foreach (LauncherV2.UpdateInfo.File current2 in this._updateRoot.Files)
					{
						bool flag = Md5Utils.FileNotMatched(Launcher.Instance.Location + current2.FilePath, current2.Md5);
						if (flag)
						{
							this.AddDownload(current2);
						}
					}
				}
			}
			catch (Exception ex)
			{
				bool flag2 = this.Retry("An error occured while contacting our update servers (" + ex.ToString() + ").  Press OK to try again or cancel to visit our download page.", "Connection Error - Retry?");
				if (flag2)
				{
					this.CheckForUpdates();
				}
				else
				{
					Process.Start("https://truckersmp.com/download");
					Process.GetCurrentProcess().Kill();
				}
			}
			IEnumerable<Package> arg_1D6_0 = this._packageRoot.Packages;
			Func<Package, bool> arg_1D6_1;
			if ((arg_1D6_1 = MainWindow.<>c.<>9__21_0) == null)
			{
				arg_1D6_1 = (MainWindow.<>c.<>9__21_0 = new Func<Package, bool>(MainWindow.<>c.<>9.<CheckForUpdates>b__21_0));
			}
			Package[] array3 = arg_1D6_0.Where(arg_1D6_1).ToArray<Package>();
			bool flag3 = array3.Any<Package>();
			if (flag3)
			{
				this.Visible(new UIElement[]
				{
					this.cmdInstallUpdate,
					this.lstPackages
				}, true);
				bool flag4 = true;
				Package[] array4 = array3;
				for (int j = 0; j < array4.Length; j++)
				{
					Package package = array4[j];
					CheckBox checkBox = new CheckBox
					{
						Content = package,
						IsChecked = new bool?(!package.Optional),
						IsEnabled = package.Optional
					};
					bool flag5 = !package.Optional;
					if (flag5)
					{
						flag4 = false;
					}
					checkBox.Checked += new RoutedEventHandler(this.chkBox_Checked);
					checkBox.Unchecked += new RoutedEventHandler(this.chkBox_Unchecked);
					checkBox.Foreground = Brushes.White;
					ListBoxItem newItem = new ListBoxItem
					{
						Content = checkBox
					};
					this.lstPackages.Items.Add(newItem);
					this._packageCheckBoxes.Add(checkBox);
				}
				bool flag6 = flag4;
				if (flag6)
				{
					this.Visible(this.cmdInstallUpdate, false);
				}
			}
			this.CheckLaunchAllowed();
		}

		private void AddDownload(LauncherV2.UpdateInfo.File file)
		{
			string text = Launcher.Instance.Location + file.FilePath.Replace("/", "\\");
			string path = text.Substring(0, text.LastIndexOf("\\"));
			bool flag = !Directory.Exists(path);
			if (flag)
			{
				Directory.CreateDirectory(path);
			}
			this._downloadQueue.Enqueue(new DownloadItem
			{
				Url = new Uri("http://download.ets2mp.com/files" + file.FilePath),
				Filepath = text,
				Type = file.Type
			});
			IEnumerable<Package> arg_DF_0 = this._packageRoot.Packages;
			Func<Package, bool> <>9__0;
			Func<Package, bool> arg_DF_1;
			if ((arg_DF_1 = <>9__0) == null)
			{
				arg_DF_1 = (<>9__0 = ((Package p) => p.Type == file.Type));
			}
			foreach (Package current in arg_DF_0.Where(arg_DF_1))
			{
				Package package = current;
				package.FileCount++;
			}
		}

		private void DownloadFile()
		{
			bool flag = this._downloadQueue.Any<DownloadItem>();
			if (flag)
			{
				DownloadItem downloadItem = this._downloadQueue.Dequeue();
				bool flag2 = this._packageList.Contains(downloadItem.Type);
				if (flag2)
				{
					this.StartDownload(downloadItem.Url, downloadItem.Filepath);
				}
				else
				{
					this.DownloadFile();
				}
			}
			else
			{
				this.CheckForUpdates();
			}
		}

		private void StartDownload(Uri url, string filepath)
		{
			this._curPackageIndex++;
			WebClient webClient = new WebClient();
			string file = filepath.Substring(filepath.LastIndexOf("\\") + 1);
			webClient.DownloadProgressChanged += this.DownloadProgressChanged(file);
			webClient.DownloadFileCompleted += this.DownloadFileCompleted(url, filepath, file);
			webClient.DownloadFileAsync(url, filepath);
		}

		public DownloadProgressChangedEventHandler DownloadProgressChanged(string file)
		{
			Action<object, DownloadProgressChangedEventArgs> @object = delegate(object sender, DownloadProgressChangedEventArgs e)
			{
				this.Dispatcher.BeginInvoke(new Action(delegate
				{
					double num = double.Parse(e.BytesReceived.ToString());
					double num2 = double.Parse(e.TotalBytesToReceive.ToString());
					this.lblProgress.Content = string.Concat(new string[]
					{
						"Downloading ",
						file,
						": ",
						(num / 1024.0 / 1024.0).ToString("F"),
						" mb of ",
						(num2 / 1024.0 / 1024.0).ToString("F"),
						" mb"
					});
					this.lblProgressOverall.Content = string.Concat(new object[]
					{
						"(File ",
						this._curPackageIndex,
						" of ",
						this._packageCount,
						")"
					});
					this.prgProgress.Value = num / num2 * 100.0;
				}), DispatcherPriority.Normal, new object[0]);
			};
			return new DownloadProgressChangedEventHandler(@object.Invoke);
		}

		public AsyncCompletedEventHandler DownloadFileCompleted(Uri url, string filePath, string file)
		{
			Action<object, AsyncCompletedEventArgs> @object = delegate(object sender, AsyncCompletedEventArgs e)
			{
				bool flag = e.Error != null;
				if (flag)
				{
					this._curPackageIndex--;
					bool flag2 = this.Retry(e.Error.Message + "\nPress OK to retry.", "Connection Error - Retry?");
					if (flag2)
					{
						this.StartDownload(url, filePath);
					}
				}
				else
				{
					this.lblProgress.Content = "File " + file + " completed...";
					this.lblProgressOverall.Content = string.Concat(new object[]
					{
						"(File ",
						this._curPackageIndex,
						" of ",
						this._packageCount,
						")"
					});
					this.DownloadFile();
				}
			};
			return new AsyncCompletedEventHandler(@object.Invoke);
		}

		private static bool CheckGameInstalled(Game game)
		{
			bool result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\TruckersMP\\"))
			{
				result = (((registryKey != null) ? registryKey.GetValue("InstallLocation" + game.ShortName) : null) != null);
			}
			return result;
		}

		private void CheckLaunchAllowed()
		{
			IEnumerable<CheckBox> arg_26_0 = this._packageCheckBoxes;
			Func<CheckBox, bool> arg_26_1;
			if ((arg_26_1 = MainWindow.<>c.<>9__32_0) == null)
			{
				arg_26_1 = (MainWindow.<>c.<>9__32_0 = new Func<CheckBox, bool>(MainWindow.<>c.<>9.<CheckLaunchAllowed>b__32_0));
			}
			bool flag = arg_26_0.Count(arg_26_1) == 0;
			IEnumerable<CheckBox> arg_54_0 = this._packageCheckBoxes;
			Func<CheckBox, bool> arg_54_1;
			if ((arg_54_1 = MainWindow.<>c.<>9__32_1) == null)
			{
				arg_54_1 = (MainWindow.<>c.<>9__32_1 = new Func<CheckBox, bool>(MainWindow.<>c.<>9.<CheckLaunchAllowed>b__32_1));
			}
			bool flag2 = arg_54_0.Count(arg_54_1) == 0;
			bool flag3 = flag2 && MainWindow.HasAts;
			if (flag3)
			{
				this.cmdLaunchAts.Visibility = Visibility.Visible;
			}
			else
			{
				this.cmdLaunchAts.Visibility = Visibility.Hidden;
			}
			bool flag4 = flag && MainWindow.HasEts;
			if (flag4)
			{
				this.cmdLaunchEts2.Visibility = Visibility.Visible;
			}
			else
			{
				this.cmdLaunchEts2.Visibility = Visibility.Hidden;
			}
			this.cmdLaunchAts.IsEnabled = (flag2 && MainWindow.HasAts);
			this.cmdLaunchEts2.IsEnabled = (flag && MainWindow.HasEts);
		}

		private bool Retry(string msg, string title)
		{
			MessageBoxResult messageBoxResult = MessageBox.Show(msg, title, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
			return messageBoxResult == MessageBoxResult.OK;
		}

		private void Visible(UIElement ctrl, bool visible)
		{
			ctrl.Visibility = (visible ? Visibility.Visible : Visibility.Hidden);
		}

		private void Visible(UIElement[] ctrls, bool visible)
		{
			for (int i = 0; i < ctrls.Length; i++)
			{
				UIElement ctrl = ctrls[i];
				this.Visible(ctrl, visible);
			}
		}

		private string GetGameCommandLine()
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = commandLineArgs;
			for (int i = 0; i < array.Length; i++)
			{
				string str = array[i];
				stringBuilder.Append(str + " ");
			}
			return stringBuilder.ToString();
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
		public void InitializeComponent()
		{
			bool contentLoaded = this._contentLoaded;
			if (!contentLoaded)
			{
				this._contentLoaded = true;
				Uri resourceLocator = new Uri("/Launcher;component/mainwindow.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.winMain = (MainWindow)target;
				this.winMain.KeyDown += new KeyEventHandler(this.MainWindow_OnKeyDown);
				break;
			case 2:
				this.gridMain = (Grid)target;
				break;
			case 3:
				this.imgBackground = (Image)target;
				break;
			case 4:
				this.cmdLaunchEts2 = (Button)target;
				this.cmdLaunchEts2.Click += new RoutedEventHandler(this.cmdLaunchEts2_Click);
				break;
			case 5:
				this.cmdLaunchAts = (Button)target;
				this.cmdLaunchAts.Click += new RoutedEventHandler(this.cmdLaunchAts_Click);
				break;
			case 6:
				this.lblProgress = (Label)target;
				break;
			case 7:
				this.prgProgress = (ProgressBar)target;
				break;
			case 8:
				this.cmdInstallUpdate = (Button)target;
				this.cmdInstallUpdate.Click += new RoutedEventHandler(this.cmdInstallUpdate_Click);
				break;
			case 9:
				this.lstPackages = (ListBox)target;
				break;
			case 10:
				this.lblNewsTitle = (Label)target;
				break;
			case 11:
				this.lblNewsLink = (Label)target;
				this.lblNewsLink.MouseDown += new MouseButtonEventHandler(this.LblNewsLink_OnMouseDown);
				break;
			case 12:
				this.cmd_close = (Image)target;
				this.cmd_close.MouseDown += new MouseButtonEventHandler(this.cmdClose_Click);
				break;
			case 13:
				this.lblSupportLink = (Label)target;
				this.lblSupportLink.MouseDown += new MouseButtonEventHandler(this.LblSupportLink_OnMouseDown);
				break;
			case 14:
				this.lblFacebookLink = (Label)target;
				this.lblFacebookLink.MouseDown += new MouseButtonEventHandler(this.LblFacebookLink_OnMouseDown);
				break;
			case 15:
				this.lblStatusLink = (Label)target;
				this.lblStatusLink.MouseDown += new MouseButtonEventHandler(this.LblStatusLink_OnMouseDown);
				break;
			case 16:
				this.lblProgressOverall = (Label)target;
				break;
			case 17:
				this.txtVersionInfo = (Label)target;
				break;
			default:
				this._contentLoaded = true;
				break;
			}
		}
	}
}
