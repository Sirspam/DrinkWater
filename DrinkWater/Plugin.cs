﻿using DrinkWater.Configuration;
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
            zenjector.UseLogger(logger);
            zenjector.UseMetadataBinder<Plugin>();
            zenjector.UseSiraSync();
            
            zenjector.Install<DrinkWaterMenuInstaller>(Location.Menu, conf.Generated<PluginConfig>());
        }
    }
}
