using UnityEngine;

public class PlayerMotionController : IFixedUpdate
{
    #region Fields

    private float _speed;

    private Rigidbody _player;

    private Vector2 _movementDirection = Vector2.zero;

    #endregion

    #region Constructors

    public PlayerMotionController(
        float speed,
        Rigidbody player,
        ISubscriptionProperty<Vector2> playerMovementSubscription)
    {
        _speed  = speed;

        _player = player;

        playerMovementSubscription.Subscribe(RegisterMovement);
    }

    #endregion

    #region Interface methods

    public void OnFixedUpdate(float fixedDeltaTime)
    {
        if (_movementDirection == Vector2.zero) return;

        var movementVector =
            _player.position +
            _movementDirection.x * _player.transform.right * _speed +
            _movementDirection.y * _player.transform.forward * _speed;

        _player.MovePosition(movementVector);

        _movementDirection = Vector2.zero;
    }

    #endregion

    #region Methods

    private void RegisterMovement(Vector2 movementDirection)
    {
        _movementDirection += movementDirection;
    }

    #endregion
}