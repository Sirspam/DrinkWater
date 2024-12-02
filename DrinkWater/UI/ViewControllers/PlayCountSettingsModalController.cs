using System;
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
		private DrinkWaterSettingsViewController.SetSettingsButtonUnderline _setSettingsButtonUnderlineDelegate = null!;
		
		[UIParams] private readonly BSMLParserParams _parserParams = null!;
		
		[UIValue("enable-play-count-bool")]
		private bool EnableByPlaytimeCount
		{
			get => _pluginConfig.EnableByPlayCount;
			set
			{
				_pluginConfig.EnableByPlayCount = value;
				_setSettingsButtonUnderlineDelegate(value);
			}
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
		private string FormatPlayCountSlider(int value) => value == 1 ? "After every level" : $"{value.ToString()} levels";

		private void Parse(Component parentTransform, DrinkWaterSettingsViewController.SetSettingsButtonUnderline setSettingsButtonUnderlineDelegate)
		{
			if (!_parsed)
			{
				BSMLParser.Instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "DrinkWater.UI.Views.PlayCountSettingsView.bsml"), parentTransform.gameObject, this);
				_setSettingsButtonUnderlineDelegate = setSettingsButtonUnderlineDelegate;
				_parsed = true;
			}
		}

		public void ShowModal(Transform parentTransform,  DrinkWaterSettingsViewController.SetSettingsButtonUnderline setSettingsButtonUnderlineDelegate)
		{
			Parse(parentTransform, setSettingsButtonUnderlineDelegate);
			
			_parserParams.EmitEvent("close-modal");
			_parserParams.EmitEvent("open-modal");
		}
	}
}