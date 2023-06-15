using DrinkWater.Utils;
using Zenject;

namespace DrinkWater.Installers
{
	public class DrinkWaterAppInstaller : Installer
	{
		public override void InstallBindings()
		{
			Container.Bind<DrinkWaterValues>().AsSingle();
		}
	}
}