namespace CorePlayBox.HeartStone.V2
{
    public interface IKeyWord
    {
        void Execute(Player player,GameState state);
    }
    public abstract class KeyWord
    {
        public string Name = "";
    }
}