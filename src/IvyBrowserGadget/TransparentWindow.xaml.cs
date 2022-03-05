using Invary.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Invary.IvyBrowserGadget
{
	public partial class TransparentWindow : Window
	{

		public TransparentWindow(Window owner)
		{
			InitializeComponent();

			Owner = owner;
			WindowStartupLocation = WindowStartupLocation.Manual;
			Top = Owner.Top;
			Left = Owner.Left;
			Width = Owner.Width;
			Height = Owner.Height;


			MouseLeftButtonDown += (sender, e) =>
			{
				if (e.ButtonState != MouseButtonState.Pressed)
					return;

				//move window, only resizable mode
				if (ResizeMode != ResizeMode.NoResize)
					DragMove();
			};


			ContextMenuOpening += Menu_Opening;

			if (Setting.Current.bLockUIWhenStartApp)
			{
				ResizeMode = ResizeMode.NoResize;
			}

			Loaded += Form1_Shown;
		}




		void BuildAndStart()
		{
			Uty.SwitchResourceDictionaryByLanguage(Application.Current.Resources, Setting.Current.TwoLetterISOLanguageName);
			Uty.SwitchResourceDictionaryByLanguage(Resources, Setting.Current.TwoLetterISOLanguageName);


			//if ctrl pressed, not restore position.
			if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) == KeyStates.Down
				|| (Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) == KeyStates.Down)
			{
			}
			else
			{
				WindowStartupLocation = WindowStartupLocation.Manual;
				Left = Setting.Current.ptLocation.X;
				Top = Setting.Current.ptLocation.Y;
				Width = Setting.Current.Size.Width;
				Height = Setting.Current.Size.Height;
			}
		}



		void Destroy()
		{
		}



		private void Menu_Opening(object sender, ContextMenuEventArgs e)
		{
			foreach (object menuitem in ContextMenu.Items)
			{
				if (menuitem is not MenuItem item)
					continue;

				if (item.Name == "UILock")
					item.IsEnabled = (ResizeMode != ResizeMode.NoResize);
				if (item.Name == "UIUnlock")
					item.IsEnabled = (ResizeMode == ResizeMode.NoResize);

				if (item.Name == "LockBrowser")
					item.IsEnabled = _bUseBrowser;
				if (item.Name == "UnlockBrowser")
					item.IsEnabled = ! _bUseBrowser;
			}
		}


		private void Menu_OnLock(object sender, RoutedEventArgs e)
		{
			ResizeMode = ResizeMode.NoResize;
		}


		private void Menu_OnUnlock(object sender, RoutedEventArgs e)
		{
			ResizeMode = ResizeMode.CanResize;
		}







		private void Menu_OnSetting(object sender, RoutedEventArgs e)
		{
			{
				var wnd = new SettingWindow();
				wnd.Owner = this;
				wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				var ret = wnd.ShowDialog();
				if (ret == false)
					return;

				var setting = wnd.NewSetting;
				if (setting == null)
					return;

				Setting.Current = setting;
				{
					Setting.Current.ptLocation = new Point(Left, Top);
					Setting.Current.Size = new Size(Width, Height);
				}
			}
			{
				Uty.SwitchResourceDictionaryByLanguage(Application.Current.Resources, Setting.Current.TwoLetterISOLanguageName);
				Uty.SwitchResourceDictionaryByLanguage(Resources, Setting.Current.TwoLetterISOLanguageName);

				var wnd = Owner as MainWindow;
				if (wnd != null)
				{
					//rebuild window
					wnd.Rebuild();
				}
			}
		}


		private void Menu_OnQuit(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Menu_OnQuitWithoutSave(object sender, RoutedEventArgs e)
		{
			var wnd = Owner as MainWindow;
			if (wnd != null)
				wnd._bNoSaveSettingWhenClose = true;
			Close();
		}




		private void Menu_OnZoom100(object sender, RoutedEventArgs e)
		{
			var wnd = Owner as MainWindow;
			if (wnd != null)
				wnd.SetZoom(1.0);
		}

		private void Menu_OnZoom125(object sender, RoutedEventArgs e)
		{
			var wnd = Owner as MainWindow;
			if (wnd != null)
				wnd.SetZoom(1.25);
		}

		private void Menu_OnZoom150(object sender, RoutedEventArgs e)
		{
			var wnd = Owner as MainWindow;
			if (wnd != null)
				wnd.SetZoom(1.50);
		}

		private void Menu_OnZoom175(object sender, RoutedEventArgs e)
		{
			var wnd = Owner as MainWindow;
			if (wnd != null)
				wnd.SetZoom(1.75);
		}

		private void Menu_OnZoom200(object sender, RoutedEventArgs e)
		{
			var wnd = Owner as MainWindow;
			if (wnd != null)
				wnd.SetZoom(2.0);
		}

		private void Menu_OnZoom75(object sender, RoutedEventArgs e)
		{
			var wnd = Owner as MainWindow;
			if (wnd != null)
				wnd.SetZoom(0.75);
		}

		private void Menu_OnZoom50(object sender, RoutedEventArgs e)
		{
			var wnd = Owner as MainWindow;
			if (wnd != null)
				wnd.SetZoom(0.5);
		}

		private void Menu_OnZoom25(object sender, RoutedEventArgs e)
		{
			var wnd = Owner as MainWindow;
			if (wnd != null)
				wnd.SetZoom(0.25);
		}


		bool _bUseBrowser = false;


		private void Menu_OnBrowserLock(object sender, RoutedEventArgs e)
		{
			if (_bUseBrowser == false)
				return;

			var wnd = Owner as MainWindow;
			if (wnd == null)
				return;

			{
				_bUseBrowser = false;
				Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
				IsChanging = true;
				Top = wnd.Top;
				Height = wnd.Height;
				IsChanging = false;
			}
		}



		private void Menu_OnBrowserUnlock(object sender, RoutedEventArgs e)
		{
			if (_bUseBrowser)
				return;

			var wnd = Owner as MainWindow;
			if (wnd == null)
				return;

			{
				_bUseBrowser = true;

				Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
				IsChanging = true;
				Top = Top + Height - 50;
				Height = 50;
				IsChanging = false;
			}
		}



		private void OnOpenDownloadURL(object sender, MouseButtonEventArgs e)
		{
			Uty.OpenURL(Setting.strDownloadUrl);
		}












		private void Form1_Shown(object sender1, EventArgs e1)
		{
			BuildAndStart();

			if (Setting.Current.bCheckAutoUpdate)
			{
				TimeSpan span = DateTime.Now - Setting.Current.dtLastCheckAutoUpdate;
				if (span.TotalDays >= 0)
				{
					//Update the date, regardless of the result.
					Setting.Current.dtLastCheckAutoUpdate = DateTime.Now;

					UpdateStatus.CheckUpdate((sender, e) =>
					{
						if (e.IsNewVersiionExists)
						{
							try
							{
								imageInformation.Dispatcher.BeginInvoke(new Action(() =>
								{
									imageInformation.Visibility = Visibility.Visible;
								}));
							}
							catch (Exception)
							{
							}
						}
					}, 30, null);
				}
			}

		}












		public bool IsChanging { set; get; } = false;




		private void Window_Activated(object sender, EventArgs e)
		{

		}


		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (IsChanging)
				return;

			{
				IsChanging = true;
				Owner.Width = Width;
				Owner.Height = Height;
				IsChanging = false;
			}
		}
		private void Window_LocationChanged(object sender, EventArgs e)
		{
			if (IsChanging)
				return;

			if (_bUseBrowser)
			{
				IsChanging = true;
				Owner.Top = Top + Height - Owner.Height;
				Owner.Left = Left;
				IsChanging = false;
			}
			else
			{
				IsChanging = true;
				Owner.Top = Top;
				Owner.Left = Left;
				IsChanging = false;
			}
		}


		private void Window_Closed(object sender, EventArgs e)
		{
			try
			{
				Owner.Close();
			}
			catch(Exception)
			{
			}
		}

	}
}
