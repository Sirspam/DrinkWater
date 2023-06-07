using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IPA.Utilities;
using Newtonsoft.Json;
using SiraUtil.Logging;
using SiraUtil.Web;
using Random = UnityEngine.Random;

namespace DrinkWater.Utils
{
	public class ImageSources
	{
		private List<string>? _localFiles;
		private const string WaifuPicsEndpoint = "https://api.waifu.pics/sfw/neko";
		private readonly string _drinkWaterPath = Path.Combine(UnityGame.UserDataPath, nameof(DrinkWater));

		private readonly SiraLog _siraLog;
		private readonly IHttpService _httpService;
		
		public enum Sources
		{
			Classic,
			Nya,
			CatBoys,
			Local
		}

		public ImageSources(SiraLog siraLog, IHttpService httpService)
		{
			_siraLog = siraLog;
			_httpService = httpService;
		}

		public async Task<string> GetImagePath(Sources source)
		{
			return source switch
			{
				Sources.Classic => GetClassicImageUrl(),
				Sources.Nya => await Task.Run(() => GetApiImageUrl("https://api.waifu.pics/sfw/neko")),
				Sources.CatBoys => await Task.Run(() => GetApiImageUrl("https://api.catboys.com/img")),
				Sources.Local => GetLocalImagePath(),
				_ => GetClassicImageUrl()
			};
		}

		private string GetClassicImageUrl()
		{
			var classicSources = new[]
			{
				"https://media1.tenor.com/images/013d560bab2b0fc56a2bc43b8262b4ed/tenor.gif",
				"https://i.giphy.com/zWOnltJgKVlsc.gif", 
				"https://i.giphy.com/3ohhwF34cGDoFFhRfy.gif",
				"https://i.giphy.com/eRBa4tzlbNwE8.gif"
			};
			return classicSources[Random.Range(0, classicSources.Length)];
		}

		private async Task<string> GetApiImageUrl(string apiUrl)
		{
			try
			{
				_siraLog.Info($@"Attempting to get image url from {apiUrl}");
				var response = await _httpService.GetAsync(apiUrl);
				if (!response.Successful)
				{
					throw new HttpRequestException();
				}
				
				var result = JsonConvert.DeserializeObject<WebAPIEntries>(Encoding.UTF8.GetString(await response.ReadAsByteArrayAsync()));
				if (result?.Url != null)
				{
					_siraLog.Info("Loading image from " + result.Url);
					return result.Url;
				}
				_siraLog.Info("url is null. Loading Classic image instead");
				return GetClassicImageUrl();
			}
			catch (Exception exception)
			{
				_siraLog.Error("Failed to get image url " + exception);
				return GetClassicImageUrl();
			}
		}

		private string GetLocalImagePath()
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