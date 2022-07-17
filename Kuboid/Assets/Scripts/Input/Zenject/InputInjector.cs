using UnityEngine;
using Zenject;

public class InputInjector : MonoInstaller
{
    #region Fields

    [SerializeField] private string _horizontalAxis = "Horizontal";
    [SerializeField] private string _verticalAxis  = "Vertical";
    [SerializeField] private string _mouseXAxis = "Mouse X";
    [SerializeField] private string _mouseYAxis = "Mouse Y";
    [SerializeField][Range(0f, 100f)] private float _sensetivity = 100f;

    #endregion

    #region Base methods

    public override void InstallBindings()
    {
        Container.BindSubscriptionProperty<Vector2>("Player movement");

        Container
            .Bind<string>()
            .WithId("Horizontal axis")
            .FromInstance(_horizontalAxis);

        Container
            .Bind<string>()
            .WithId("Vertical axis")
            .FromInstance(_verticalAxis);

        Container.BindSubscriptionProperty<float>("Mouse X shift");
        Container.BindSubscriptionProperty<float>("Mouse Y shift");
        
        Container
            .Bind<string>()
            .WithId("Mouse X axis")
            .FromInstance(_mouseXAxis);

        Container
            .Bind<string>()
            .WithId("Mouse Y axis")
            .FromInstance(_mouseYAxis);

        Container
            .Bind<float>()
            .WithId("Mouse sensitivity")
            .FromInstance(_sensetivity);
    }

    #endregion
}