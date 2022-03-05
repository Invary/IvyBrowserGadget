using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Invary.Utility
{
	internal class Win32
	{



		public const int S_OK = 0;


		[DllImport("dwmapi.dll")]
		public static extern int DwmIsCompositionEnabled(out bool bEnabled);

		[DllImport("dwmapi.dll")]
		public static extern int DwmRegisterThumbnail(IntPtr hWndDestination, IntPtr hWndSource, out IntPtr hThumbnailId);


		[DllImport("dwmapi.dll")]
		public static extern int DwmUnregisterThumbnail(IntPtr hThumbnailId);

		[DllImport("dwmapi.dll")]
		public static extern int DwmQueryThumbnailSourceSize(IntPtr _hThumbnailId, out SIZE pSize);

		[DllImport("dwmapi.dll")]
		public static extern int DwmUpdateThumbnailProperties(IntPtr hThumbnailId, ref DWM_THUMBNAIL_PROPERTIES ptnProperties);


		[DllImport("dwmapi.dll")]
		public static extern int DwmGetWindowAttribute(IntPtr hwnd, Int32 dwAttribute, out RECT pvAttribute, Int32 cbAttribute);


		[DllImport("dwmapi.dll")]
		public static extern int DwmFlush();


		//[DllImport("dwmapi.dll")]
		//[return: MarshalAs(UnmanagedType.Bool)]
		//public static extern bool DwmDefWindowProc(IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam, out IntPtr lResult);



		[DllImport("User32.dll")]
		public static extern IntPtr GetParent(IntPtr hWnd);


		[DllImport("User32.dll")]
		public static extern IntPtr WindowFromPoint(POINT Point);

		public static IntPtr WindowFromPoint(int x, int y)
		{
			POINT pt;
			pt.x = x;
			pt.y = y;
			return WindowFromPoint(pt);
		}


		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsIconic(IntPtr hWnd);



		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetClientRect(IntPtr hWnd, ref RECT lprect);

		public static RECT? GetClientRect(IntPtr hWnd)
		{
			RECT rect = new RECT();
			var ret = GetClientRect(hWnd, ref rect);
			if (ret == false)
				return null;
			return rect;
		}


		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowRect(IntPtr hWnd, ref RECT lprect);

		public static RECT? GetWindowRect(IntPtr hWnd)
		{
			RECT rect = new RECT();
			var ret = GetWindowRect(hWnd, ref rect);
			if (ret == false)
				return null;
			return rect;
		}





		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

		public static WINDOWPLACEMENT? GetWindowPlacement(IntPtr hWnd)
		{
			WINDOWPLACEMENT wndpl = new WINDOWPLACEMENT();
			wndpl.length = Marshal.SizeOf(wndpl);
			var ret= GetWindowPlacement(hWnd, ref wndpl);
			if (ret == false)
				return null;
			return wndpl;
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPLACEMENT
		{
			public Int32 length;
			public Int32 flags;
			public Int32 showCmd;
			public POINT ptMinPosition;
			public POINT ptMaxPosition;
			public RECT rcNormalPosition;
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public Int32 x;
			public Int32 y;
		}



		[StructLayout(LayoutKind.Sequential)]
		internal struct SIZE
		{
			public int Width;
			public int Height;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct DWM_THUMBNAIL_PROPERTIES
		{
			public int dwFlags;
			public RECT rcDestination;
			public RECT rcSource;
			public byte opacity;
			public bool fVisible;
			public bool fSourceClientAreaOnly;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct RECT
		{
			internal RECT(int left, int top, int right, int bottom)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}
			internal RECT(Rectangle rect)
			{
				Left = rect.Left;
				Top = rect.Top;
				Right = rect.Right;
				Bottom = rect.Bottom;
			}


			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

	}
}
