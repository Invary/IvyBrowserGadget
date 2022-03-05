using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Invary.Utility
{

	/// <summary>
	/// 
	/// </summary>
	///
	/// <example>
	///DWM dwm = new DWM();
	///Loaded += delegate
	///{
	///	if (DWM.IsEnable)
	///	{
	///		var wnd = Window.GetWindow(wndDummy);
	///		var helper = new WindowInteropHelper(wnd);
	///		var hwndView = helper.Handle;
	///
	///		IntPtr hWndTarget = (IntPtr)0x00110A82;       //Window handle to get screen-shot
	///
	///		dwm.Register(hwndView, hWndTarget);
	///		dwm.SetProperty(true, DWM.OPAQUE, new System.Drawing.Rectangle(0, 0, 800, 600));
	///		dwm.Update();
	///	}
	///};
	///
	///Closing += delegate
	///{
	///	dwm.Dispose();
	///};
	///</example>
	internal class DWM : IDisposable
	{
		IntPtr _hThumbnailId = IntPtr.Zero;

		public static bool IsEnable
		{
			get
			{
				bool bEnable;

				var ret = Win32.DwmIsCompositionEnabled(out bEnable);
				if (ret != Win32.S_OK)
					return false;

				return bEnable;
			}
		}


		public bool Register(IntPtr hWndDestination, IntPtr hWndSource)
		{
			if (_hThumbnailId != IntPtr.Zero)
				return false;

			IntPtr hThumbnailId;

			var ret = Win32.DwmRegisterThumbnail(hWndDestination, hWndSource, out hThumbnailId);
			if (ret != Win32.S_OK)
				return false;

			_hThumbnailId = hThumbnailId;

			return true;
		}



		public bool Unregister()
		{
			if (_hThumbnailId == IntPtr.Zero)
				return true;

			var ret = Win32.DwmUnregisterThumbnail(_hThumbnailId);
			if (ret != Win32.S_OK)
				return false;

			_hThumbnailId = IntPtr.Zero;
			return true;
		}



		public void Dispose()
		{
			Unregister();
		}







		public bool GetThumbnailSourceSize(out Size size)
		{
			size = new Size();

			if (_hThumbnailId == IntPtr.Zero)
				return false;

			Win32.SIZE pSize;

			var ret = Win32.DwmQueryThumbnailSourceSize(_hThumbnailId, out pSize);
			if (ret != Win32.S_OK)
				return false;

			size.Width = pSize.Width;
			size.Height = pSize.Height;

			return true;
		}



		/// <summary>
		/// Opacity, transparent
		/// </summary>
		public const byte TRANSPARENT = 0;

		/// <summary>
		/// Opacity, not transparent
		/// </summary>
		public const byte OPAQUE = 255;





		const int DWM_TNP_RECTDESTINATION = 0x00000001;
		const int DWM_TNP_RECTSOURCE = 0x00000002;
		const int DWM_TNP_OPACITY = 0x00000004;
		const int DWM_TNP_VISIBLE = 0x00000008;
		const int DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010;


		Win32.DWM_THUMBNAIL_PROPERTIES _ptnProperties = new Win32.DWM_THUMBNAIL_PROPERTIES();


		public void SetProperty(bool bClientAreaOnly, byte nOpacity, Rectangle rctDestination)
		{
			_ptnProperties.dwFlags = 0;

			_ptnProperties.fSourceClientAreaOnly = bClientAreaOnly;
			_ptnProperties.dwFlags |= DWM_TNP_SOURCECLIENTAREAONLY;

			_ptnProperties.fVisible = true;
			_ptnProperties.dwFlags |= DWM_TNP_VISIBLE;

			_ptnProperties.opacity = nOpacity;
			_ptnProperties.dwFlags |= DWM_TNP_OPACITY;

			_ptnProperties.rcDestination = new Win32.RECT(rctDestination);
			_ptnProperties.dwFlags |= DWM_TNP_RECTDESTINATION;
		}


		public void SetProperty(bool bClientAreaOnly, byte nOpacity, Rectangle rctDestination, Rectangle rctSource)
		{
			SetProperty(bClientAreaOnly, nOpacity, rctDestination);

			_ptnProperties.rcSource = new Win32.RECT(rctSource);
			_ptnProperties.dwFlags |= DWM_TNP_RECTSOURCE;
		}


		const int DWMWA_EXTENDED_FRAME_BOUNDS = 0x00000009;




		/// <summary>
		/// When window is minimized, retreived rect will be invalid!
		/// </summary>
		public static Win32.RECT? GetExtendedFrameBounds(IntPtr hWnd)
		{
			Win32.RECT rect;

			var ret = Win32.DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, out rect, Marshal.SizeOf(typeof(Win32.RECT)));
			if (ret != Win32.S_OK)
				return null;

			return rect;
		}




		public bool Update()
		{
			if (_hThumbnailId == IntPtr.Zero)
				return false;

			var ret = Win32.DwmUpdateThumbnailProperties(_hThumbnailId, ref _ptnProperties);
			if (ret != Win32.S_OK)
				return false;
			return true;
		}

	}

}
