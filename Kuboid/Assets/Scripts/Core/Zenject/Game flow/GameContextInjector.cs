using Zenject;

public class GameContextInjector : MonoInstaller
{
    #region Base methods

    public override void InstallBindings()
    {
        Container
            .Bind<ISubscriptionValue<EGameState>>()
            .To<GameState>()
            .AsCached();

        Container
            .Bind<ControllersManager>()
            .AsCached();
    }

    #endregion
}