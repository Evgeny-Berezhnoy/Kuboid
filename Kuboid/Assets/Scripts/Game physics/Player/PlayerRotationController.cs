using UnityEngine;

public class PlayerRotationController : IController
{
    #region Fields

    private const float StraightAngle = 90f;

    private Transform _transform;
    private Transform _camera;

    private float _aroundXRotation;

    #endregion

    #region Constructors

    public PlayerRotationController(
        Transform transform,
        Transform camera,
        ISubscriptionProperty<float> axisXShiftSubscription,
        ISubscriptionProperty<float> axisYShiftSubscription)
    {
        _transform  = transform;
        _camera     = camera;

        axisXShiftSubscription.Subscribe(ShiftXAxis);
        axisYShiftSubscription.Subscribe(ShiftYAxis);
    }

    #endregion

    #region Methods

    private void ShiftXAxis(float axisXShift)
    {
        _transform.Rotate(Vector3.up, axisXShift);
    }

    private void ShiftYAxis(float axisYShift)
    {
        _aroundXRotation -= axisYShift;
        _aroundXRotation = Mathf.Clamp(_aroundXRotation, -StraightAngle, StraightAngle);

        _camera.localRotation = Quaternion.Euler(_aroundXRotation, 0, 0);
    }

    #endregion
}