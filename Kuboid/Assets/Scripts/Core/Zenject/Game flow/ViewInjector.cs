using UnityEngine;
using Zenject;

public class ViewInjector : MonoInstaller
{
    #region Fields

    [SerializeField] private Transform _player;
    [SerializeField] private Transform _camera;
    
    #endregion

    #region Base methods

    public override void InstallBindings()
    {
        Container
            .Bind<Transform>()
            .WithId("Player body")
            .FromInstance(_player);

        Container
            .Bind<Transform>()
            .WithId("Camera")
            .FromInstance(_camera);
    }

    #endregion
}