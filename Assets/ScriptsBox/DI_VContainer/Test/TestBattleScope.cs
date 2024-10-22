using ScriptsBox.DI_VContainer.Test.Battle;
using ScriptsBox.DI_VContainer.Test.State;
using VContainer;
using VContainer.Unity;

namespace ScriptsBox.DI_VContainer.Test
{
    public class TestBattleScope:LifetimeScope
    {
        protected override void Awake()
        {
            base.Awake();
            this.name = nameof(TestBattleScope);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Scene>(Lifetime.Scoped);
            builder.Register( (resolver) => resolver.Resolve<GameData>().PlayerData.PlayerBattleData,Lifetime.Scoped);
            
            
        }
    }
}