using ScriptsBox.DI_VContainer.Test.Battle;

namespace ScriptsBox.DI_VContainer.Test.State
{
    public class PlayerData
    {
        public PlayerBattleData PlayerBattleData = new PlayerBattleData();
        public void Load()
        {
            //从磁盘中取，如果没有，初始化一个，并保存
        }
    }
}