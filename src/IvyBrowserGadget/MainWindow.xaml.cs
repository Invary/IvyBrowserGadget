using Invary.Utility;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Invary.IvyBrowserGadget
{
	//
	// nuget
	// Microsoft.Web.WebView2
	//
	//
	public partial class MainWindow : Window
	{

		public bool _bNoSaveSettingWhenClose = false;

		TransparentWindow? _wndTransparent = null;

		Timer _timer = new Timer();

		public MainWindow()
		{
			{
				string strSettingFile = Setting.FilePath;

				if (string.IsNullOrEmpty(strSettingFile))
				{
					strSettingFile = System.IO.Path.Combine(Uty.GetExecutableFolder(), System.IO.Path.GetFileNameWithoutExtension(Uty.GetExecutablePath()));
					strSettingFile += ".xml";
				}

				bool ret = Setting.LoadSetting(strSettingFile);

				if (ret == false)
				{
					MessageBoxResult dlgret = MessageBox.Show(Uty.ResourceApp("MessageBoxInitSetting_Text"), Uty.ResourceApp("MessageBoxInitSetting_Title"), MessageBoxButton.OKCancel);
					if (dlgret != MessageBoxResult.OK)
					{
						_bNoSaveSettingWhenClose = true;
						Close();
						return;
					}
					Setting.FilePath = strSettingFile;
				}
			}



			InitializeComponent();
			Uty.SwitchResourceDictionaryByLanguage(Application.Current.Resources, Setting.Current.TwoLetterISOLanguageName);
			Uty.SwitchResourceDictionaryByLanguage(Resources, Setting.Current.TwoLetterISOLanguageName);




			Closing += delegate
			{
				//if (WindowState == FormWindowState.Normal)
				{
					Setting.Current.ptLocation = new Point(Left, Top);
					Setting.Current.Size = new Size(Width, Height);
				}

				if (_bNoSaveSettingWhenClose == false)
					Setting.Current.SaveSetting();

				if (_wndTransparent != null)
					_wndTransparent.Close();
			};



			Loaded += delegate
			{
				_wndTransparent = new TransparentWindow(this);
				_wndTransparent.Show();
			};



			//10sec timer
			_timer.Interval = 10000;
			_timer.Elapsed += delegate
			{
				if (Setting.Current.RefreshTimeMin <= 0)
					return;

				TimeSpan span = DateTime.Now - _dtLastNavigate;
				if (span.TotalMinutes < Setting.Current.RefreshTimeMin)
					return;

				webView.Reload();
			};
			_timer.Start();



			ContentRendered += Window_ContentRendered;
		}


		DateTime _dtLastNavigate = DateTime.Now;		//init with "Now"





		async void Window_ContentRendered(object? sender, EventArgs e)
		{
			var webView2Environment = await CoreWebView2Environment.CreateAsync();
			await webView.EnsureCoreWebView2Async(webView2Environment);

			_dtLastNavigate = DateTime.Now;
			webView.Source = new Uri(Setting.Current.strBrowseURL);
			webView.ZoomFactor = Setting.Current.Zoom;
		}



		public void SetZoom(double zoom)
		{
			webView.ZoomFactor = zoom;
			Setting.Current.Zoom = zoom;
		}

		public void SetURL(string url)
		{
			_dtLastNavigate = DateTime.Now;
			webView.Source = new Uri(url);
			Setting.Current.strBrowseURL = url;
		}







		public void Rebuild()
		{
			Uty.SwitchResourceDictionaryByLanguage(Application.Current.Resources, Setting.Current.TwoLetterISOLanguageName);
			Uty.SwitchResourceDictionaryByLanguage(Resources, Setting.Current.TwoLetterISOLanguageName);

			SetURL(Setting.Current.strBrowseURL);
			SetZoom(Setting.Current.Zoom);
		}






		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (_wndTransparent != null && _wndTransparent.IsChanging == false)
			{
				_wndTransparent.IsChanging = true;
				_wndTransparent.Width = Width;
				_wndTransparent.Height = Height;
				_wndTransparent.IsChanging = false;
			}
		}

		private void Window_LocationChanged(object sender, EventArgs e)
		{
			if (_wndTransparent != null && _wndTransparent.IsChanging == false)
			{
				_wndTransparent.IsChanging = true;
				_wndTransparent.Top = Top;
				_wndTransparent.Left = Left;
				_wndTransparent.IsChanging = false;
			}
		}
	}
}
