using UnityEngine;
using Zenject;

public class GamePhysicsInitializer
{
    #region Injected methods

    [Inject]
    private void Initialize(
        // GameContextInjector
        [Inject] ControllersManager controllersManager,
        // ViewInjector
        [Inject(Id = "Player body")] Transform playerBody,
        [Inject(Id = "Camera")] Transform camera,
        // GamePhysicsInjector
        [Inject(Id = "Player speed")] float playerSpeed,
        // ControlsInjector
        [Inject(Id = "Player movement")] ISubscriptionProperty<Vector2> playerMovementSubscription,
        [Inject(Id = "Mouse X shift")] ISubscriptionProperty<float> mouseXShiftSubscription,
        [Inject(Id = "Mouse Y shift")] ISubscriptionProperty<float> mouseYShiftSubscription)
    {
        var playerRotationController =
            new PlayerRotationController(
                playerBody,
                camera,
                mouseXShiftSubscription,
                mouseYShiftSubscription);

        var playerMotionController =
            new PlayerMotionController(
                playerSpeed,
                playerBody.GetComponentInChildren<Rigidbody>(),
                playerMovementSubscription);

        controllersManager.AddController(playerRotationController, EGameState.Gameplay);
        controllersManager.AddController(playerMotionController, EGameState.Gameplay);
    }

    #endregion
}