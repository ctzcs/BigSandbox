namespace ScriptsBox.DI_VContainer
{
    public class BattleFactory
    {
        private readonly HelloWorldService _helloWorldService;

        public BattleFactory(HelloWorldService helloWorldService)
        { 
            _helloWorldService = helloWorldService;
        }

        public Battle Create() => new Battle(_helloWorldService);
    }
}