using Zenject;

public class GameInitializer
{
    #region Injected methods

    [Inject]
    private void Initialize(
        // BasicsInjector
        DiContainerWrapper diContainer)
    {
        diContainer.Inject(new InputInitializer());
        diContainer.Inject(new GamePhysicsInitializer());
    }

    #endregion
}