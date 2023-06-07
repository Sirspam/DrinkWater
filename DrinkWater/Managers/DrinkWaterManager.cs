using System;
using DrinkWater.Configuration;
using DrinkWater.UI.ViewControllers;
using SiraUtil.Logging;
using SiraUtil.Services;
using Zenject;

namespace DrinkWater.Managers
{
	internal sealed class DrinkWaterManager : IInitializable, IDisposable
	{
		private int _playCount;
		private float _playTime;

		private readonly SiraLog _siraLog;
		private readonly PluginConfig _pluginConfig;
		private readonly ILevelFinisher _levelFinisher;
		private readonly DrinkWaterPanelController _drinkWaterPanelController;
		private readonly StandardLevelScenesTransitionSetupDataSO _standardLevelScenesTransitionSetupData;

		public DrinkWaterManager(SiraLog siraLog, PluginConfig pluginConfig, ILevelFinisher levelFinisher, DrinkWaterPanelController drinkWaterPanelController, StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupData)
		{
			_siraLog = siraLog;
			_pluginConfig = pluginConfig;
			_levelFinisher = levelFinisher;
			_drinkWaterPanelController = drinkWaterPanelController;
			_standardLevelScenesTransitionSetupData = standardLevelScenesTransitionSetupData;
		}
		
		private void LevelFinisherOnStandardLevelFinished(LevelCompletionResults obj)
		{
			if (!_pluginConfig.EnablePlugin) return;

			var practiceSettings = _standardLevelScenesTransitionSetupData.practiceSettings;

			var songPlayDuration = practiceSettings == null
				? obj.endSongTime
				: obj.endSongTime - practiceSettings.startSongTime;

			var songSpeedMul = practiceSettings?.songSpeedMul ?? obj.gameplayModifiers.songSpeedMul;
			
			_siraLog.Info($"Song duration {songPlayDuration}, " +
			              $"Speed Multiplier {songSpeedMul}, " +
			              $"Actual playtime {songPlayDuration / songSpeedMul}");
			
			_playTime += songPlayDuration / songSpeedMul;
			_playCount += 1;
			// playtime we get from the game is in second, config playtime setting is in minute
			if (_pluginConfig.EnableByPlaytime && _playTime >= _pluginConfig.PlaytimeBeforeWarning * 60)
			{
				_siraLog.Debug($"Playtime: {_playTime}, Setting: {_pluginConfig.PlaytimeBeforeWarning * 60}");
				_siraLog.Info("Required play time met");
				_drinkWaterPanelController.displayPanelNeeded = true;
				// reset both playtime and playCount
				// otherwise we may get notification by play count right after a playtime notification
				// or vise-versa
				_playTime = 0f;
				_playCount = 0;
			}
			else if (_pluginConfig.EnableByPlaycount && _playCount >= _pluginConfig.PlaycountBeforeWarning)
			{
				_siraLog.Debug($"PlayCount: {_playCount}, Setting: {_pluginConfig.PlaycountBeforeWarning}");
				_siraLog.Info("Required play count met");
				_drinkWaterPanelController.displayPanelNeeded = true;
				// same here
				_playCount = 0;
				_playTime = 0f;
			}
		}

		public void Initialize()
		{
			_levelFinisher.StandardLevelFinished += LevelFinisherOnStandardLevelFinished;
		}

		public void Dispose()
		{
			_levelFinisher.StandardLevelFinished -= LevelFinisherOnStandardLevelFinished;
		}
	}
}