using VContainer;

namespace ScriptsBox.DI_VContainer
{
    public class BattleInspector:ClassInspector<Battle>
    {
        [Inject]
        public void Constructor(Battle battle)
        {
            this.instance = battle;
        }
    }
}