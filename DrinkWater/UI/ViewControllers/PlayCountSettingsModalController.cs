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
		
		private readonly PluginConfig _pluginConfig;

		public PlayCountSettingsModalController(PluginConfig pluginConfig)
		{
			_pluginConfig = pluginConfig;
		}
		
		[UIAction("format-play-count-slider")]
		private string FormatPlayCountSlider(int value)
		{
			if (value == 0)
			{
				return "After every level";
			}

			return value.ToString();
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