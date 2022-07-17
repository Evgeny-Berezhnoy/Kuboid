using UnityEngine;

public class InputRotationController : IUpdate
{
    #region Fields

    private string _mouseXAxis;
    private string _mouseYAxis;
    private float _sensitivity;

    #endregion

    #region Observers

    private ISubscriptionProperty<float> _mouseXShiftSubscription;
    private ISubscriptionProperty<float> _mouseYShiftSubscription;
    
    #endregion

    #region Contructors

    public InputRotationController(
        ISubscriptionProperty<float> mouseXShiftSubscription,
        ISubscriptionProperty<float> mouseYShiftSubscription,
        string mouseXAxis,
        string mouseYAxis,
        float sensitivity)
    {
        _mouseXShiftSubscription    = mouseXShiftSubscription;
        _mouseYShiftSubscription    = mouseYShiftSubscription;

        _mouseXAxis                 = mouseXAxis;
        _mouseYAxis                 = mouseYAxis;
        _sensitivity                = sensitivity;
    }

    #endregion

    #region Interface methods
    
    public void OnUpdate(float deltaTime)
    {
        var mouseX = Input.GetAxis(_mouseXAxis) * _sensitivity * deltaTime;
        var mouseY = Input.GetAxis(_mouseYAxis) * _sensitivity * deltaTime;

        _mouseXShiftSubscription.Value = mouseX;
        _mouseYShiftSubscription.Value = mouseY;
    }

    #endregion
}