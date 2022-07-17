using UnityEngine;

public class InputMotionController : IUpdate
{
    #region Fields

    private string _horizontalAxis;
    private string _verticalAxis;

    #endregion

    #region Observers

    private ISubscriptionProperty<Vector2> _inputVectorSubscription;

    #endregion

    #region Constructors

    public InputMotionController(
        ISubscriptionProperty<Vector2> inputVectorSubscription,
        string horizontalAxis,
        string verticalAxis)
    {
        _inputVectorSubscription = inputVectorSubscription;

        _horizontalAxis = horizontalAxis;
        _verticalAxis   = verticalAxis;
    }

    #endregion

    #region Interface methods

    public void OnUpdate(float deltaTime)
    {
        var horizontalAxisShift = Input.GetAxis(_horizontalAxis);
        var verticalAxisShift   = Input.GetAxis(_verticalAxis);

        if (horizontalAxisShift == 0 && verticalAxisShift == 0) return;

        var inputVector =
            new Vector2(
                horizontalAxisShift * deltaTime,
                verticalAxisShift * deltaTime);

        _inputVectorSubscription.Value = inputVector;
    }

    #endregion
}