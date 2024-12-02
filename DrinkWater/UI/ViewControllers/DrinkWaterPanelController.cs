using System;
using System.Collections;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using DrinkWater.Configuration;
using DrinkWater.UI.FlowCoordinators;
using DrinkWater.Utils;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace DrinkWater.UI.ViewControllers
{
    [ViewDefinition("DrinkWater.UI.Views.DrinkWaterPanelView.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\DrinkWaterPanelView")]
    internal sealed class DrinkWaterPanelController : BSMLAutomaticViewController
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
        
        private PluginConfig _pluginConfig = null!;
        private ImageSources _imageSources = null!;
        private MainFlowCoordinator _mainFlowCoordinator = null!;
        private ResultsViewController _resultsViewController = null!;
        private FlowCoordinator _drinkWaterFlowCoordinator = null!;
        
        public enum PanelMode
        {
            Continue,
            Restart,
            None
        }
        
        [Inject]
        public void Construct(PluginConfig pluginConfig, ImageSources imageSources, MainFlowCoordinator mainFlowCoordinator, ResultsViewController resultsViewController, DrinkWaterFlowCoordinator drinkWaterFlowCoordinator)
        {
            _pluginConfig = pluginConfig;
            _imageSources = imageSources;
            _mainFlowCoordinator = mainFlowCoordinator;
            _resultsViewController = resultsViewController;
            _drinkWaterFlowCoordinator = drinkWaterFlowCoordinator;
        }
        
        [UIComponent("header-content")]
        private readonly TextMeshProUGUI _headerContent = null!;
        
        [UIComponent("text-content")]
        private readonly TextMeshProUGUI _textContent = null!;

        [UIComponent("drink-image")]
        private readonly ImageView _drinkImage = null!;
        
        [UIObject("loading-indicator")]
        private readonly GameObject _loadingIndicator = null!;

        [UIComponent("continue-button")]
        private readonly Button _continueButton = null!;
        
        [UIComponent("continue-button")]
        private readonly TextMeshProUGUI _continueButtonText = null!;

        public void ShowDrinkWaterPanel(PanelMode mode)
        {
            _previousFlowCoordinator = _mainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();
            _previousFlowCoordinator.PresentFlowCoordinator(_drinkWaterFlowCoordinator, animationDirection: AnimationDirection.Horizontal);
            displayPanelNeeded = false;
            _panelMode = mode;
        }

        private IEnumerator MakeButtonInteractableDelay(Button button, float duration, float delayStep = 1f, string format = "0", bool showInButton = true)
        {
            var buttonTextContent = _continueButtonText.text;
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

        private void SetImageLoading(bool value)
        {
            if (value)
            {
                _loadingIndicator.SetActive(true);
                _drinkImage.gameObject.SetActive(false);
            }
            else
            {
                _loadingIndicator.SetActive(false);
                _drinkImage.gameObject.SetActive(true);
            }
        }
        protected override async void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

            // I apologise for this
            if (_pluginConfig.ImageSource == ImageSources.Sources.Nya && Random.Range(0, 4) == 3)
            {
                _headerContent.text = "Dwynk sum watew! 💦";
                _textContent.text = (_panelMode == PanelMode.Restart ? "Beyfow weestawting this song" : "Beyfow bwowsying sum noow songes") + ", dwynk sum watew! i-i-it ish iympowtant fow yow bodee!! (>ω< )";
                _continueButtonText.text = "I undewstwand!! x3";
            }
            else
            {
                _headerContent.text = "Drink some water! 💦";
                _textContent.text = (_panelMode == PanelMode.Restart ? "Before restarting this song" : "Before browsing some new songs") + ", drink some water, it's important for your body!";
                _continueButtonText.text = "I understand!";
            }
            
            StartCoroutine(MakeButtonInteractableDelay(_continueButton, _pluginConfig.WaitDuration, 0.1f, "0.0"));

            if (_pluginConfig.ShowImages)
            {
                SetImageLoading(true);
                await _drinkImage.SetImageAsync(await _imageSources.GetImagePath(_pluginConfig.ImageSource), true, ScaleOptions);
                SetImageLoading(false);
            }
            else
            {
                _loadingIndicator.SetActive(false);
                _drinkImage.gameObject.SetActive(false);
            }
        }

        [UIAction("continue-clicked")]
        private void ContinueClicked()
        {
            if (_previousFlowCoordinator is null)
            {
                return;
            }
            
            _previousFlowCoordinator.DismissFlowCoordinator(_mainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf(), immediately: _panelMode != PanelMode.None);
            _drinkImage.sprite = Utilities.ImageResources.BlankSprite;
            
            switch (_panelMode)
            {
                case PanelMode.Continue:
                    _resultsViewController.ContinueButtonPressed();
                    break;
                case PanelMode.Restart:
                    _resultsViewController.RestartButtonPressed();
                    break;
                case PanelMode.None:
                default:
                    break;
            }
        }
    }
}
