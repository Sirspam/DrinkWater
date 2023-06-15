using System;
using DrinkWater.Configuration;
using DrinkWater.UI.ViewControllers;
using DrinkWater.Utils;
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
		private readonly DrinkWaterValues _drinkWaterValues;
		private readonly GameScenesManager _gameScenesManager;
		private readonly DrinkWaterPanelController _drinkWaterPanelController;
		private readonly StandardLevelScenesTransitionSetupDataSO _standardLevelScenesTransitionSetupData;

		public DrinkWaterManager(SiraLog siraLog, PluginConfig pluginConfig, ILevelFinisher levelFinisher, DrinkWaterValues drinkWaterValues, GameScenesManager gameScenesManager, DrinkWaterPanelController drinkWaterPanelController, StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupData)
		{
			_siraLog = siraLog;
			_pluginConfig = pluginConfig;
			_levelFinisher = levelFinisher;
			_drinkWaterValues = drinkWaterValues;
			_gameScenesManager = gameScenesManager;
			_drinkWaterPanelController = drinkWaterPanelController;
			_standardLevelScenesTransitionSetupData = standardLevelScenesTransitionSetupData;
		}
		
		private void LevelFinisherOnStandardLevelFinished(LevelCompletionResults results)
		{
			if (!_pluginConfig.Enabled) 
				return;

			var practiceSettings = _standardLevelScenesTransitionSetupData.practiceSettings;

			var songPlayDuration = practiceSettings == null
				? results.endSongTime
				: results.endSongTime - practiceSettings.startSongTime;

			var songSpeedMul = practiceSettings?.songSpeedMul ?? results.gameplayModifiers.songSpeedMul;
			
			_siraLog.Debug($"Song duration {songPlayDuration}, " +
			              $"Speed Multiplier {songSpeedMul}, " +
			              $"Actual playtime {songPlayDuration / songSpeedMul}, " +
			              $"Pause time {_drinkWaterValues._pauseTimeInSeconds}");
			
			_playTime += songPlayDuration / songSpeedMul;
			if (_pluginConfig.CountPauseTime)
			{
				_playTime += _drinkWaterValues._pauseTimeInSeconds;
			}
			
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
			else if (_pluginConfig.EnableByPlayCount && _playCount >= _pluginConfig.PlayCountBeforeWarning)
			{
				_siraLog.Debug($"PlayCount: {_playCount}, Setting: {_pluginConfig.PlayCountBeforeWarning}");
				_siraLog.Info("Required play count met");
				_drinkWaterPanelController.displayPanelNeeded = true;
				// same here
				_playCount = 0;
				_playTime = 0f;
			}

			void TransitionDidFinishEvent(ScenesTransitionSetupDataSO scenesTransitionSetupDataSo, DiContainer diContainer)
			{
				_gameScenesManager.transitionDidFinishEvent -= TransitionDidFinishEvent;
				
				_drinkWaterPanelController.ShowDrinkWaterPanel(DrinkWaterPanelController.PanelMode.None);
			}

			if (results.levelEndAction == LevelCompletionResults.LevelEndAction.Quit && _drinkWaterPanelController.displayPanelNeeded)
				_gameScenesManager.transitionDidFinishEvent += TransitionDidFinishEvent;
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