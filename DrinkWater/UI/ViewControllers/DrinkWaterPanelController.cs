using System.Collections;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using DrinkWater.Configuration;
using DrinkWater.UI.FlowCoordinators;
using DrinkWater.Utils;
using HMUI;
using SiraUtil.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DrinkWater.UI.ViewControllers
{
    [ViewDefinition("DrinkWater.UI.Views.DrinkWaterPanelView.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\DrinkWaterPanelView")]
    internal class DrinkWaterPanelController : BSMLAutomaticViewController
    {
        public bool displayPanelNeeded;
        private PanelMode _panelMode;
        private FlowCoordinator? _previousFlowCoordinator;

        private static readonly BeatSaberUI.ScaleOptions ScaleOptions = new BeatSaberUI.ScaleOptions
        {
            ShouldScale = true,
            MaintainRatio = true,
            Height = 512,
            Width = 512
        };

        private SiraLog _siraLog = null!;
        private PluginConfig _pluginConfig = null!;
        private ImageSources _imageSources = null!;
        private MainFlowCoordinator _mainFlowCoordinator = null!;
        private ResultsViewController _resultsViewController = null!;
        private FlowCoordinator _drinkWaterFlowCoordinator = null!;
        
        public enum PanelMode
        {
            Continue,
            Restart
        }
        
        [Inject]
        public void Construct(SiraLog siraLog, PluginConfig pluginConfig, ImageSources imageSources, MainFlowCoordinator mainFlowCoordinator, ResultsViewController resultsViewController, DrinkWaterFlowCoordinator drinkWaterFlowCoordinator)
        {
            _siraLog = siraLog;
            _pluginConfig = pluginConfig;
            _imageSources = imageSources;
            _mainFlowCoordinator = mainFlowCoordinator;
            _resultsViewController = resultsViewController;
            _drinkWaterFlowCoordinator = drinkWaterFlowCoordinator;
        }

        [UIComponent("header-content")] 
        internal readonly TextMeshProUGUI HeaderContent = null!;
        
        [UIComponent("text-content")] 
        internal readonly TextMeshProUGUI TextContent = null!;

        [UIComponent("drink-image")] 
        internal readonly ImageView DrinkImage = null!;

        [UIComponent("continue-button")] 
        internal readonly Button ContinueButton = null!;
        
        [UIComponent("continue-button")]
        internal readonly TextMeshProUGUI ContinueButtonText = null!;
        
        public void ShowDrinkWaterPanel(PanelMode mode)
        {
            _previousFlowCoordinator = _mainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();
            _previousFlowCoordinator.PresentFlowCoordinator(_drinkWaterFlowCoordinator, animationDirection: AnimationDirection.Horizontal);
            displayPanelNeeded = false;
            _panelMode = mode;
        }

        private IEnumerator MakeButtonInteractableDelay(Button button, float duration, float delayStep = 1f, string format = "0", bool showInButton = true)
        {
            var buttonTextContent = ContinueButtonText.text;
            if (showInButton)
                button.SetButtonText(buttonTextContent + (duration > 0 ? " (" + duration.ToString(format) + ")" : ""));
            button.interactable = false;
            while (duration > 0)
            {
                yield return new WaitForSeconds(delayStep);
                duration -= delayStep;
                if (duration < 0) duration = 0;
                if (showInButton)
                    button.SetButtonText(buttonTextContent + (duration > 0 ? " (" + duration.ToString(format) + ")" : ""));
            }
            button.interactable = true;
        }

        protected async override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

            if (_pluginConfig.ImageSource == ImageSources.Sources.Nya && Random.Range(0, 4) == 3)
            {
                HeaderContent.text = "Dwynk sum watew! 💦";
                TextContent.text = (_panelMode == PanelMode.Restart ? "Beyfow weestawting this song" : "Beyfow bwowsying sum noow songes") + ", dwynk sum watew! t-t-that ish iympowtant fow yow bodee!! (>ω< )";
                ContinueButtonText.text = "I undewstwand!! x3";
            }
            else
            {
                HeaderContent.text = "Drink some water! 💦";
                TextContent.text = (_panelMode == PanelMode.Restart ? "Before restarting this song" : "Before browsing some new songs") + ", drink some water, that's important for your body!";
                ContinueButtonText.text = "I understand!";
            }
            
            StartCoroutine(MakeButtonInteractableDelay(ContinueButton, _pluginConfig.WaitDuration, 0.1f, "0.0"));

            if (_pluginConfig.ShowImages)
            {
                DrinkImage.SetImage(await _imageSources.GetImagePath(_pluginConfig.ImageSource), true, ScaleOptions);
            }
        }

        [UIAction("continue-clicked")]
        private void ContinueClicked()
        {
            //TODO: Improve transitions
            _previousFlowCoordinator.DismissFlowCoordinator(_mainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf(), immediately: true);
            DrinkImage.sprite = Utilities.ImageResources.BlankSprite;
            
            switch (_panelMode)
            {
                case PanelMode.Continue:
                    _resultsViewController.ContinueButtonPressed();
                    break;
                case PanelMode.Restart:
                    _resultsViewController.RestartButtonPressed();
                    break;
            }
        }
    }
}
