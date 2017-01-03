using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;

namespace LauncherV2
{
	internal class Launcher
	{
		internal struct ProcessInformation
		{
			public IntPtr hProcess;

			public IntPtr hThread;

			public int dwProcessId;

			public int dwThreadId;
		}

		public struct SecurityAttributes
		{
			public int nLength;

			public IntPtr lpSecurityDescriptor;

			public int bInheritHandle;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct Startupinfo
		{
			public int cb;

			public string lpReserved;

			public string lpDesktop;

			public string lpTitle;

			public int dwX;

			public int dwY;

			public int dwXSize;

			public int dwYSize;

			public int dwXCountChars;

			public int dwYCountChars;

			public int dwFillAttribute;

			public int dwFlags;

			public short wShowWindow;

			public short cbReserved2;

			public IntPtr lpReserved2;

			public IntPtr hStdInput;

			public IntPtr hStdOutput;

			public IntPtr hStdError;
		}

		public enum AllocationType
		{
			Commit = 4096,
			Reserve = 8192,
			Decommit = 16384,
			Release = 32768,
			Reset = 524288,
			Physical = 4194304,
			TopDown = 1048576,
			WriteWatch = 2097152,
			LargePages = 536870912
		}

		[Flags]
		public enum MemoryProtection
		{
			Execute = 16,
			ExecuteRead = 32,
			ExecuteReadWrite = 64,
			ExecuteWriteCopy = 128,
			NoAccess = 1,
			ReadOnly = 2,
			ReadWrite = 4,
			WriteCopy = 8,
			GuardModifierflag = 256,
			NoCacheModifierflag = 512,
			WriteCombineModifierflag = 1024
		}

		[Flags]
		public enum FreeType
		{
			Decommit = 16384,
			Release = 32768
		}

		private static Launcher _instance;

		private const uint CreateSuspended = 4u;

		private const uint Infinite = 4294967295u;

		private const uint WaitAbandoned = 128u;

		private const uint WaitObject0 = 0u;

		private const uint WaitTimeout = 258u;

		public static Launcher Instance
		{
			get
			{
				Launcher arg_14_0;
				if ((arg_14_0 = Launcher._instance) == null)
				{
					arg_14_0 = (Launcher._instance = new Launcher());
				}
				return arg_14_0;
			}
		}

		public string Location
		{
			get;
			private set;
		}

		public Dictionary<string, Game> Games
		{
			get;
			private set;
		}

		public void Setup()
		{
			this.Location = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\TruckersMP";
			this.Games.Add("ets", new Game("Euro Truck Simulator 2", "ETS2", "227300", "eurotrucks2.exe", "core_ets2mp.dll"));
			this.Games.Add("ats", new Game("American Truck Simulator", "ATS", "270880", "amtrucks.exe", "core_atsmp.dll"));
		}

		public bool Launch(string id, string arguments)
		{
			Game game = this.Games[id];
			this.SetEnvironmentVariables(game.Id);
			Launcher.ProcessInformation processInformation = default(Launcher.ProcessInformation);
			Launcher.Startupinfo startupinfo = default(Launcher.Startupinfo);
			Launcher.SecurityAttributes securityAttributes = default(Launcher.SecurityAttributes);
			Launcher.SecurityAttributes securityAttributes2 = default(Launcher.SecurityAttributes);
			startupinfo.cb = Marshal.SizeOf(startupinfo);
			securityAttributes.nLength = Marshal.SizeOf(securityAttributes);
			securityAttributes2.nLength = Marshal.SizeOf(securityAttributes2);
			bool flag = !Launcher.CreateProcess(game.Directory + game.ExeName, arguments, ref securityAttributes, ref securityAttributes2, false, 4u, IntPtr.Zero, game.Directory, ref startupinfo, out processInformation);
			bool result;
			if (flag)
			{
				MessageBox.Show("Can not create game process.", "TruckersMP - Error");
				result = false;
			}
			else
			{
				string text = this.Inject(processInformation.hProcess, this.Location + "\\" + game.DllName);
				bool flag2 = text != "success";
				if (flag2)
				{
					MessageBox.Show("Can not inject core (" + text + ")");
					result = false;
				}
				else
				{
					Launcher.ResumeThread(processInformation.hThread);
					result = true;
				}
			}
			return result;
		}

		private string Inject(IntPtr process, string dllPath)
		{
			bool flag = !File.Exists(dllPath);
			string result;
			if (flag)
			{
				result = "DLL file not found (" + dllPath + ")";
			}
			else
			{
				IntPtr moduleHandle = Launcher.GetModuleHandle("kernel32.dll");
				bool flag2 = moduleHandle == IntPtr.Zero;
				if (flag2)
				{
					result = "can not get module handle of kernel32.dll";
				}
				else
				{
					IntPtr procAddress = Launcher.GetProcAddress(moduleHandle, "LoadLibraryA");
					bool flag3 = procAddress == IntPtr.Zero;
					if (flag3)
					{
						result = "can not get LoadLibraryA address";
					}
					else
					{
						byte[] bytes = Encoding.ASCII.GetBytes(dllPath + "\0");
						IntPtr intPtr = Launcher.VirtualAllocEx(process, IntPtr.Zero, (IntPtr)bytes.Length, (Launcher.AllocationType)12288, Launcher.MemoryProtection.ReadWrite);
						bool flag4 = intPtr == IntPtr.Zero;
						if (flag4)
						{
							result = "can not allocate memory";
						}
						else
						{
							IntPtr zero = IntPtr.Zero;
							bool flag5 = !Launcher.WriteProcessMemory(process, intPtr, bytes, bytes.Length, out zero);
							if (flag5)
							{
								result = "can not write memory";
							}
							else
							{
								bool flag6 = (int)zero != bytes.Length;
								if (flag6)
								{
									result = "bytes written and path length does not match";
								}
								else
								{
									IntPtr intPtr3;
									IntPtr intPtr2 = Launcher.CreateRemoteThread(process, IntPtr.Zero, 0u, procAddress, intPtr, 0u, out intPtr3);
									bool flag7 = intPtr2 == IntPtr.Zero;
									if (flag7)
									{
										result = "can not create remote thread";
									}
									else
									{
										Launcher.WaitForSingleObject(intPtr2, 4294967295u);
										uint num;
										Launcher.GetExitCodeThread(intPtr2, out num);
										bool flag8 = num == 0u;
										if (flag8)
										{
											result = "initialization of client failed";
										}
										else
										{
											Launcher.CloseHandle(intPtr2);
											Launcher.FreeLibrary(moduleHandle);
											result = "success";
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public string GetGameDirectory(string game)
		{
			string text = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\TruckersMP", "InstallLocation" + game, "error");
			return (text == null || text == "error") ? "" : (text + "\\bin\\win_x64\\");
		}

		private void SetEnvironmentVariables(string gameId)
		{
			Environment.SetEnvironmentVariable("SteamGameId", gameId);
			Environment.SetEnvironmentVariable("SteamAppID", gameId);
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, ref Launcher.SecurityAttributes lpProcessAttributes, ref Launcher.SecurityAttributes lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref Launcher.Startupinfo lpStartupInfo, out Launcher.ProcessInformation lpProcessInformation);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern uint ResumeThread(IntPtr hThread);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, Launcher.AllocationType flAllocationType, Launcher.MemoryProtection flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll")]
		private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

		[DllImport("kernel32.dll")]
		private static extern bool GetExitCodeThread(IntPtr hThread, out uint lpExitCode);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, Launcher.FreeType dwFreeType);

		public Launcher()
		{
			this.<Games>k__BackingField = new Dictionary<string, Game>();
			base..ctor();
		}
	}
}
