using UnityEngine;

public class GameState : SubscriptionValue<EGameState>
{
    #region Interfaces properties

    public new EGameState Value
    {
        get => _value;
        set
        {
            SetValue(value);

            if (_value == EGameState.Quit)
            {
                QuitGame();
            };
        }
    }

    #endregion

    #region Constructors

    public GameState(EGameState defaultGameState = EGameState.Gameplay)
    {
        _value = defaultGameState;
    }

    #endregion

    #region Methods

    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    #endregion
}