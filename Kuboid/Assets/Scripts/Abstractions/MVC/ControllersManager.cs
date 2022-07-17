using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

public class ControllersManager : IUpdate, IFixedUpdate
{
    #region Fields

    private Dictionary<EGameState, ControllersList> _controllersLists;

    #endregion

    #region Observers

    private ISubscriptionValue<EGameState> _gameState;

    #endregion

    #region Interface methods

    public void OnUpdate(float deltaTime)
    {
        _controllersLists[_gameState.Value].OnUpdate(deltaTime);
    }

    public void OnFixedUpdate(float fixedDeltaTime)
    {
        _controllersLists[_gameState.Value].OnFixedUpdate(fixedDeltaTime);
    }

    #endregion

    #region Injected methods

    [Inject]
    public void Initialize(
        // GameContextInjector
        [Inject] ISubscriptionValue<EGameState> gameState)
    {
        _controllersLists = new Dictionary<EGameState, ControllersList>();

        var gameStates =
            Enum.GetValues(typeof(EGameState))
                .Cast<EGameState>()
                .ToList();

        for (int i = 0; i < gameStates.Count; i++)
        {
            _controllersLists.Add(gameStates[i], new ControllersList());
        };

        _gameState = gameState;
    }

    #endregion

    #region Methods

    public void AddController(IController controller, EGameState firstGameState, params EGameState[] otherGameStates)
    {
        AddController(controller, firstGameState);

        for(int i = 0; i < otherGameStates.Length; i++)
        {
            AddController(controller, otherGameStates[i]);
        };
    }

    private void AddController(IController controller, EGameState gameState)
    {
        _controllersLists[gameState].AddController(controller);
    }

    #endregion
}