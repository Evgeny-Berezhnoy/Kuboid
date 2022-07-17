using UnityEngine;
using Zenject;

public class GamePhysicsInjector : MonoInstaller
{
    #region Fields

    [SerializeField] private float _playerSpeed;

    #endregion

    #region Base methods

    public override void InstallBindings()
    {
        Container
            .Bind<float>()
            .WithId("Player speed")
            .FromInstance(_playerSpeed);
    }

    #endregion
}