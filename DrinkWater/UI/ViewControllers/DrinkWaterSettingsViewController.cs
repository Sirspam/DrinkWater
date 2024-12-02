using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using DrinkWater.Configuration;
using DrinkWater.Utils;
using HMUI;
using IPA.Loader;
using SiraUtil.Logging;
using SiraUtil.Web.SiraSync;
using SiraUtil.Zenject;
using TMPro;
using Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DrinkWater.UI.ViewControllers
{
	internal sealed class DrinkWaterSettingsViewController : IInitializable, IDisposable, INotifyPropertyChanged
	{
		internal delegate void SetSettingsButtonUnderline(bool active);
		
		private bool _updateAvailable;

		[UIComponent("update-text")] 
		private readonly TextMeshProUGUI _updateText = null!;

		[UIComponent("settings-container")]
		private readonly Transform _settingsContainerTransform = null!;
		
		[UIComponent("playtime-settings-button")]
		private readonly Button _playtimeSettingsButton = null!;
		
		[UIComponent("play-count-settings-button")]
		private readonly Button _playCountSettingsButton = null!;

		private readonly SiraLog _siraLog;
		private readonly PluginConfig _pluginConfig;
		private readonly PluginMetadata _pluginMetadata;
		private readonly ISiraSyncService _siraSyncService;
		private readonly TimeTweeningManager _timeTweeningManager;
		private PlaytimeSettingsModalController _playtimeSettingsModalController;
		private PlayCountSettingsModalController _playCountSettingsModalController;

		public DrinkWaterSettingsViewController(SiraLog siraLog, PluginConfig pluginConfig, UBinder<Plugin, PluginMetadata> pluginMetadata, ISiraSyncService siraSyncService, TimeTweeningManager timeTweeningManager, PlaytimeSettingsModalController playtimeSettingsModalController, PlayCountSettingsModalController playCountSettingsModalController)
		{
			_siraLog = siraLog;
			_pluginConfig = pluginConfig;
			_pluginMetadata = pluginMetadata.Value;
			_siraSyncService = siraSyncService;
			_timeTweeningManager = timeTweeningManager;
			_playtimeSettingsModalController = playtimeSettingsModalController;
			_playCountSettingsModalController = playCountSettingsModalController;
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		
		[UIValue("update-available")]
		private bool UpdateAvailable
		{
			get => _updateAvailable;
			set
			{
				_updateAvailable = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateAvailable)));
			}
		}
		
		[UIValue("enabled-bool")]
		private bool EnabledValue
        {
            get => _pluginConfig.Enabled;
            set => _pluginConfig.Enabled = value;
        }

        [UIValue("show-image-bool")]
        private bool ShowGifValue
        {
            get => _pluginConfig.ShowImages;
            set => _pluginConfig.ShowImages = value;
        }

        [UIValue("image-source")]
        private object ImageSource
        {
	        get => _pluginConfig.ImageSource;
	        set => _pluginConfig.ImageSource = (ImageSources.Sources) value;
        }

        [UIValue("image-sources-list")] private readonly List<object> _imageSourcesList = Enum.GetValues(typeof(ImageSources.Sources)).Cast<object>().ToList();
	        
        [UIValue("wait-duration-int")]
        private int WaitDurationValue
        {
            get => _pluginConfig.WaitDuration;
            set => _pluginConfig.WaitDuration = value;
        }

        [UIAction("format-wait-duration-slider")]
        private string FormatWaitDurationSlider(int value)
        {
	        return value switch
	        {
		        0 => "Instant",
		        1 => "1 second",
		        _ => value + " seconds"
	        };
        }

        [UIAction("playtime-settings-clicked")]
        private void PlaytimeSettingsClicked()
        {
	        _playtimeSettingsModalController.ShowModal(_settingsContainerTransform, SetPlaytimeSettingsButtonUnderline);
        }
        
        [UIAction("play-count-settings-clicked")]
        private void PlayCountSettingsClicked()
		{
	        _playCountSettingsModalController.ShowModal(_settingsContainerTransform, SetPlayCountSettingsButtonUnderline);
		}
        
        [UIAction("#post-parse")]
        private async void PostParse()
        {
	        SetPlayCountSettingsButtonUnderline(_pluginConfig.EnableByPlayCount);
	        SetPlaytimeSettingsButtonUnderline(_pluginConfig.EnableByPlaytime);
	        
	        UpdateAvailable = false;
	        
	        if (!_updateAvailable)
	        {
		        var gitVersion = await _siraSyncService.LatestVersion();
		        if (gitVersion != null && gitVersion > _pluginMetadata.HVersion)
		        {
			        _siraLog.Info($"{nameof(DrinkWater)} v{gitVersion} is available on GitHub!");
			        _updateText.text = $"{nameof(DrinkWater)} v{gitVersion} is available on GitHub!";
			        _updateText.alpha = 0f;
			        UpdateAvailable = true;
			        _timeTweeningManager.AddTween(new FloatTween(0f, 1f, val => _updateText.alpha = val, 0.4f, EaseType.InCubic), _updateText);
		        }
	        }
        }

        private void SetPlaytimeSettingsButtonUnderline(bool active)
        {
	        _playtimeSettingsButton.transform.Find("Underline").gameObject.GetComponent<ImageView>().color =
		        active ? new Color(0f, 0.7529412f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5019608f);
        }
        
        private void SetPlayCountSettingsButtonUnderline(bool active)
        {
	        _playCountSettingsButton.transform.Find("Underline").gameObject.GetComponent<ImageView>().color =
		        active ? new Color(0f, 0.7529412f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5019608f);
        }
        
		public void Initialize() => BSMLSettings.Instance.AddSettingsMenu("Drink Water", $"{nameof(DrinkWater)}.UI.Views.SettingsView.bsml", this);

		public void Dispose()
		{
			if (BSMLSettings.Instance != null)
			{
				BSMLSettings.Instance.RemoveSettingsMenu(this);
			}
		}
	}
}