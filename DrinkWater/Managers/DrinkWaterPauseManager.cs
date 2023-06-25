using System;
using DrinkWater.Utils;
using Zenject;

namespace DrinkWater.Managers
{
	internal sealed class DrinkWaterPauseManager : IInitializable, IDisposable
	{
		private DateTime _pauseStartTime;

		private readonly PauseController _pauseController;
		private readonly DrinkWaterValues _drinkWaterValues;

		public DrinkWaterPauseManager(PauseController pauseController, DrinkWaterValues drinkWaterValues)
		{
			_pauseController = pauseController;
			_drinkWaterValues = drinkWaterValues;
		}
		
		private void PauseControllerOndidPauseController()
		{
			_pauseStartTime = DateTime.Now;
		}

		private void PauseControllerOndidResumeEvent()
		{
			_drinkWaterValues._pauseTimeInSeconds += (int) (DateTime.Now - _pauseStartTime).TotalSeconds;
		}

		private void PauseControllerOndidReturnToMenuEvent()
		{
			_drinkWaterValues._pauseTimeInSeconds += (int) (DateTime.Now - _pauseStartTime).TotalSeconds;
		}

		public void Initialize()
		{
			_drinkWaterValues._pauseTimeInSeconds = 0;
			
			_pauseController.didPauseEvent += PauseControllerOndidPauseController;
			_pauseController.didResumeEvent += PauseControllerOndidResumeEvent;
			_pauseController.didReturnToMenuEvent += PauseControllerOndidReturnToMenuEvent;
		}

		public void Dispose()
		{
			_pauseController.didPauseEvent -= PauseControllerOndidPauseController;
			_pauseController.didResumeEvent -= PauseControllerOndidResumeEvent;
			_pauseController.didReturnToMenuEvent -= PauseControllerOndidReturnToMenuEvent;
		}
	}
}