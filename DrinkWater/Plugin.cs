using DrinkWater.Configuration;
using DrinkWater.Installers;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using SiraUtil.Zenject;

namespace DrinkWater
{
    [Plugin(RuntimeOptions.DynamicInit)][NoEnableDisable]
    public class Plugin
    {
        [Init]
        public Plugin(Config conf, Logger logger, Zenjector zenjector)
        {
            zenjector.UseSiraSync();
            zenjector.UseHttpService();
            zenjector.UseLogger(logger);
            zenjector.UseMetadataBinder<Plugin>();

            zenjector.Install<DrinkWaterAppInstaller>(Location.App);
            zenjector.Install<DrinkWaterMenuInstaller>(Location.Menu, conf.Generated<PluginConfig>());
            zenjector.Install<DrinkWaterGameInstaller>(Location.Singleplayer);
        }
    }
}
