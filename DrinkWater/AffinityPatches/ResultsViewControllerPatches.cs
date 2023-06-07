using DrinkWater.Configuration;
using DrinkWater.UI.ViewControllers;
using SiraUtil.Affinity;

namespace DrinkWater.AffinityPatches
{
	internal sealed class ResultsViewControllerPatches : IAffinity
	{
		private readonly PluginConfig _pluginConfig;
		private readonly DrinkWaterPanelController _drinkWaterPanelController;

		public ResultsViewControllerPatches(PluginConfig pluginConfig, DrinkWaterPanelController panelController)
		{
			_pluginConfig = pluginConfig;
			_drinkWaterPanelController = panelController;
		}
		
		[AffinityPrefix]
		[AffinityPatch(typeof(ResultsViewController), nameof(ResultsViewController.ContinueButtonPressed))]
		private bool ContinueButtonPressedPatch()
		{
			if (!_pluginConfig.EnablePlugin || !_drinkWaterPanelController.displayPanelNeeded) 
				return true;

			_drinkWaterPanelController.ShowDrinkWaterPanel(DrinkWaterPanelController.PanelMode.Continue);
			return false;

		}
		
		[AffinityPrefix]
		[AffinityPatch(typeof(ResultsViewController), nameof(ResultsViewController.RestartButtonPressed))]
		private bool RestartButtonPressedPatch()
		{
			if (!_pluginConfig.EnablePlugin || !_drinkWaterPanelController.displayPanelNeeded) 
				return true;

			_drinkWaterPanelController.ShowDrinkWaterPanel(DrinkWaterPanelController.PanelMode.Restart);
			return false;

		}
	}
}