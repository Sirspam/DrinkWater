using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IPA.Utilities;
using Newtonsoft.Json;
using SiraUtil.Logging;
using Random = UnityEngine.Random;

namespace DrinkWater.Utils
{
	public class ImageSources
	{
		private List<string>? _localFiles;
		private const string WaifuPicsEndpoint = "https://api.waifu.pics/sfw/neko";
		private readonly string _drinkWaterPath = Path.Combine(UnityGame.UserDataPath, nameof(DrinkWater));
		private readonly string[] _classicSources = { "https://media1.tenor.com/images/013d560bab2b0fc56a2bc43b8262b4ed/tenor.gif", "https://i.giphy.com/zWOnltJgKVlsc.gif", "https://i.giphy.com/3ohhwF34cGDoFFhRfy.gif", "https://i.giphy.com/eRBa4tzlbNwE8.gif" };

		private readonly SiraLog _siraLog;
		
		public enum Sources
		{
			Classic,
			Nya,
			Local
		}

		public ImageSources(SiraLog siraLog)
		{
			_siraLog = siraLog;
		}

		public async Task<string> GetImagePath(Sources source)
		{
			switch (source)
			{
				case Sources.Classic:
					return GetClassic();
				case Sources.Nya:
					return await Task.Run(GetNya);
				case Sources.Local:
					return GetLocal();
				default:
					return GetClassic();;
			}
		}

		private string GetClassic()
		{
			return _classicSources[Random.Range(0, _classicSources.Length)];
		}

		private async Task<string> GetNya()
		{
			try
			{
				_siraLog.Info("Attempting to get image url from Waifu.Pics");
				using var client = new HttpClient();
				var response = await client.GetAsync(WaifuPicsEndpoint);
				var result = JsonConvert.DeserializeObject<WebAPIEntries>(Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync()));
				if (result?.Url != null)
				{
					_siraLog.Info("Loading image from " + result.Url);
					return result.Url;
				}
				_siraLog.Info("url is null. Loading Classic image instead");
				return GetClassic();
			}
			catch (Exception exception)
			{
				_siraLog.Error("Failed to get url " + exception);
				return GetClassic();
			}
		}

		private string GetLocal()
		{
			if (_localFiles == null)
			{
				var files = Directory.GetFiles(_drinkWaterPath);
				_localFiles = new List<string>();
				foreach (var file in files)
				{
					if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".gif") || file.EndsWith(".apng"))
					{
						_localFiles.Add(file);
					}
				}
			}
			
			return _localFiles!.Count == 0 ? "DrinkWater.Resources.AquaDrink.png" : _localFiles[Random.Range(0, _localFiles.Count)];
		}
	}
	
	internal class WebAPIEntries
	{
		[JsonProperty("url")]
		public string? Url { get; set; }
	}
}