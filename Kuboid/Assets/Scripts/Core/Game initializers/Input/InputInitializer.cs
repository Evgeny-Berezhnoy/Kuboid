using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InputInitializer
{
    #region Injected methods

    [Inject]
    private void Initialize(
        // GameContextInjector
        [Inject] ControllersManager controllersManager,
        // ControlsInjector
        [Inject(Id = "Player movement")] ISubscriptionProperty<Vector2> playerMovementSubscription,
        [Inject(Id = "Mouse X shift")] ISubscriptionProperty<float> mouseXShiftSubscription,
        [Inject(Id = "Mouse Y shift")] ISubscriptionProperty<float> mouseYShiftSubscription,
        [Inject(Id = "Horizontal axis")] string horizontalAxis,
        [Inject(Id = "Vertical axis")] string verticalAxis,
        [Inject(Id = "Mouse X axis")] string mouseXAxis,
        [Inject(Id = "Mouse Y axis")] string mouseYAxis,
        [Inject(Id = "Mouse sensitivity")] float sensitivity)
    {
        var inputRotationController =
            new InputRotationController(
                mouseXShiftSubscription,
                mouseYShiftSubscription,
                mouseXAxis,
                mouseYAxis,
                sensitivity);

        var inputMotionController =
            new InputMotionController(
                playerMovementSubscription,
                horizontalAxis,
                verticalAxis);

        controllersManager.AddController(inputRotationController, EGameState.Gameplay);
        controllersManager.AddController(inputMotionController, EGameState.Gameplay);
    }

    #endregion
}