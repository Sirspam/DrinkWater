using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using DrinkWater.Configuration;
using UnityEngine;

namespace DrinkWater.UI.ViewControllers
{
	internal sealed class PlayCountSettingsModalController
	{
		private bool _parsed;
		
		[UIParams] private readonly BSMLParserParams _parserParams = null!;
		
		[UIValue("enable-play-count-bool")]
		private bool EnableByPlaytimeCount
		{
			get => _pluginConfig.EnableByPlayCount;
			set => _pluginConfig.EnableByPlayCount = value;
		}

		[UIValue("play-count-warning-int")]
		private int PlayCountBeforeWarningValue
		{
			get => _pluginConfig.PlayCountBeforeWarning;
			set => _pluginConfig.PlayCountBeforeWarning = value;
		}
		
		[UIValue("play-count-level-finishes-bool")]
		private bool CountLevelFinishes
		{
			get => _pluginConfig.CountLevelFinishes;
			set => _pluginConfig.CountLevelFinishes = value;
		}
		
		[UIValue("play-count-level-fails-bool")]
		private bool CountLevelFails
		{
			get => _pluginConfig.CountLevelFails;
			set => _pluginConfig.CountLevelFails = value;
		}
		
		[UIValue("play-count-level-restarts-bool")]
		private bool CountLevelRestarts
		{
			get => _pluginConfig.CountLevelRestarts;
			set => _pluginConfig.CountLevelRestarts = value;
		}
		
		[UIValue("play-count-level-quits-bool")]
		private bool CountLevelQuits
		{
			get => _pluginConfig.CountLevelQuits;
			set => _pluginConfig.CountLevelQuits = value;
		}

		private readonly PluginConfig _pluginConfig;

		public PlayCountSettingsModalController(PluginConfig pluginConfig)
		{
			_pluginConfig = pluginConfig;
		}
		
		[UIAction("format-play-count-slider")]
		private string FormatPlayCountSlider(int value)
		{
			return value == 1 ? "After every level" : value.ToString();
		}

		private void Parse(Component parentTransform)
		{
			if (!_parsed)
			{
				BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "DrinkWater.UI.Views.PlayCountSettingsView.bsml"), parentTransform.gameObject, this);
				_parsed = true;
			}
		}

		public void ShowModal(Transform parentTransform)
		{
			Parse(parentTransform);
			
			_parserParams.EmitEvent("close-modal");
			_parserParams.EmitEvent("open-modal");
		}
	}
}