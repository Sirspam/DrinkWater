using DrinkWater.UI.ViewControllers;
using HMUI;
using Zenject;

namespace DrinkWater.UI.FlowCoordinators
{
    internal sealed class DrinkWaterFlowCoordinator : FlowCoordinator
    {
        private DrinkWaterPanelController _drinkWaterPanelController = null!;

        [Inject]
        public void Construct(DrinkWaterPanelController drinkWaterPanelController)
        {
            _drinkWaterPanelController = drinkWaterPanelController;
        }
        
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!addedToHierarchy)
                return;
            ProvideInitialViewControllers(_drinkWaterPanelController);
        }
    }
}
