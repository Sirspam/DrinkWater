using DrinkWater.Managers;
using Zenject;

namespace DrinkWater.Installers
{
	internal sealed class DrinkWaterGameInstaller : Installer
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<DrinkWaterPauseManager>().AsSingle();
		}
	}
}