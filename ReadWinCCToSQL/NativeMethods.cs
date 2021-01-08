using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ReadWinCCToSQL
{ 
	/// <summary>
	/// win32 API
	/// </summary>
	internal static class NativeMethods
	{
		[DllImport("User32.dll", EntryPoint = "FindWindow", CharSet =  CharSet.Unicode)]
		internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", EntryPoint = "GetSystemMenu", CharSet = CharSet.Unicode)]
		internal static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

		[DllImport("user32.dll", EntryPoint = "RemoveMenu", CharSet = CharSet.Unicode)]
		internal static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);
	}
}
