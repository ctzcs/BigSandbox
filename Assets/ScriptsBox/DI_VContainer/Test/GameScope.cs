using ScriptsBox.DI_VContainer.Test.State;
using VContainer;
using VContainer.Unity;

namespace ScriptsBox.DI_VContainer.Test
{
    public class GameScope:LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            
            builder.Register<GameData>(Lifetime.Singleton);
            
        }
    }
}