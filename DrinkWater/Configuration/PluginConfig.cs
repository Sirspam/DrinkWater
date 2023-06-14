
using System.IO;
using System.Runtime.CompilerServices;
using DrinkWater.Utils;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using IPA.Utilities;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace DrinkWater.Configuration
{
    internal class PluginConfig
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual bool ShowImages { get; set; } = true;
        [UseConverter(typeof(EnumConverter<ImageSources.Sources>))]
        [NonNullable]
        public virtual ImageSources.Sources ImageSource { get; set; } = ImageSources.Sources.Classic;
        public virtual int WaitDuration { get; set; } =  5;
        public virtual bool EnableByPlaytime { get; set; } = true;
        public virtual bool EnableByPlayCount { get; set; } = false;
        public virtual int PlaytimeBeforeWarning { get; set; } = 5;
        public virtual int PlayCountBeforeWarning { get; set; } = 2;
        
        public virtual void OnReload()
        {
            FixConfigIssues();
        }

        public virtual void Changed()
        {
            FixConfigIssues();
        }
        
        private static void FixConfigIssues()
        {
            var folderPath = Path.Combine(UnityGame.UserDataPath, nameof(DrinkWater));
            Directory.CreateDirectory(folderPath);
        }
    }
}
