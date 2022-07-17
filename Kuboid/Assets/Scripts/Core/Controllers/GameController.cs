using UnityEngine;
using Zenject;

public class GameController : MonoBehaviour
{
    #region Injected fields

    // BasicsInjector
    [Inject] private Disposer _disposer;
    [Inject] private DiContainerWrapper _diContainer;
    // GameContextInjector
    [Inject] private ControllersManager _controllersManager;
    [Inject] private ISubscriptionValue<EGameState> _gameState;

    #endregion

    #region Unity events

    private void Start()
    {
        _diContainer.Inject(new GameInitializer());

        _gameState.Value = EGameState.Gameplay;
    }

    private void Update()
    {
        _controllersManager.OnUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _controllersManager.OnFixedUpdate(Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
        _disposer.Dispose();
    }

    #endregion
}