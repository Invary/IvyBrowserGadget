using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Invary.IvyBrowserGadget
{



	[Serializable]
	public class Setting
	{
		[XmlIgnore]
		public static Setting Current { get; set; } = new Setting();

		[XmlIgnore]
		public static string FilePath { get; set; } = "";


		[XmlIgnore]
		public static int nVersion { get; } = 100;

		[XmlIgnore]
		public static string strVersion { get; } = $"Ver{nVersion}";


		[XmlIgnore]
		public static string strProductGuid { get; } = "{F2BE2FF1-B74D-431E-AAFC-DA2983E57129}";

		[XmlIgnore]
		public static string strUpdateCheckUrl { get; } = @"https://raw.githubusercontent.com/Invary/Status/main/status.json";


		[XmlIgnore]
		public static string strDownloadUrl { get; } = @"https://github.com/Invary/IvyBrowserGadget/Releases";








		public Setting()
		{
			TwoLetterISOLanguageName = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
		}


		public bool bCheckAutoUpdate { set; get; } = true;
		public DateTime dtLastCheckAutoUpdate { set; get; } = DateTime.MinValue;






		public Point ptLocation { set; get; } = new Point(0, 0);


		public Size Size { set; get; } = new Size(100, 100);

		public bool bLockUIWhenStartApp { set; get; } = false;


		public double Zoom { set; get; } = 1.0;
		public string strBrowseURL { set; get; } = "about:blank";

		/// <summary>
		/// zero mean no refresh
		/// </summary>
		public int RefreshTimeMin { set; get; } = 0;



		public string TwoLetterISOLanguageName { set; get; } = "";


		public Setting Clone()
		{
			var serializer = new XmlSerializer(typeof(Setting));
			using (var ms = new MemoryStream())
			{
				serializer.Serialize(ms, this);

				ms.Seek(0, SeekOrigin.Begin);
				var load = (Setting?)serializer.Deserialize(ms);
				if (load == null)
					throw new Exception();

				return load;
			}
		}


		public bool SaveSetting(string strFilePath = "")
		{
			if (string.IsNullOrEmpty(strFilePath))
				strFilePath = FilePath;

			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Setting));
				using (FileStream fs = new FileStream(strFilePath, FileMode.Create))
				{
					serializer.Serialize(fs, this);
					fs.Close();
				}
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}



		public static bool LoadSetting(string strFilePath)
		{
			if (string.IsNullOrEmpty(strFilePath))
				return false;
			if (File.Exists(strFilePath) == false)
				return false;

			FilePath = strFilePath;

			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Setting));

				using (FileStream fs = new FileStream(strFilePath, FileMode.Open))
				{
					var load = (Setting?)serializer.Deserialize(fs);
					fs.Close();

					if (load == null)
						return false;

					Current = load;
				}

				if (string.IsNullOrEmpty(Current.TwoLetterISOLanguageName) || Current.TwoLetterISOLanguageName.Length != 2)
					Current.TwoLetterISOLanguageName = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;


				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
