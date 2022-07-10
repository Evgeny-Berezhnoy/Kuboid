using UnityEngine;
using Zenject;

public static class DiContainerExtensions
{
    #region Extension methods

    public static void BindSubscriptionProperty<T>(this DiContainer Container)
    {
        var subscriptionProperty = new SubscriptionProperty<T>();

        Container.Inject(subscriptionProperty);

        Container
            .Bind<ISubscriptionProperty<T>>()
            .To<SubscriptionProperty<T>>()
            .FromInstance(subscriptionProperty)
            .AsCached();
    }

    public static void BindSubscriptionProperty<T>(this DiContainer Container, object id)
    {
        var subscriptionProperty = new SubscriptionProperty<T>();

        Container.Inject(subscriptionProperty);

        Container
            .Bind<ISubscriptionProperty<T>>()
            .WithId(id)
            .To<SubscriptionProperty<T>>()
            .FromInstance(subscriptionProperty)
            .AsCached();
    }

    public static void BindSubscriptionValue<T>(this DiContainer Container)
    {
        var subscriptionValue = new SubscriptionValue<T>();

        Container.Inject(subscriptionValue);

        Container
            .Bind<ISubscriptionValue<T>>()
            .To<SubscriptionValue<T>>()
            .FromInstance(subscriptionValue)
            .AsCached();
    }

    public static void BindSubscriptionValue<T>(this DiContainer Container, object id)
    {
        var subscriptionValue = new SubscriptionValue<T>();

        Container.Inject(subscriptionValue);

        Container
            .Bind<ISubscriptionValue<T>>()
            .WithId(id)
            .To<SubscriptionValue<T>>()
            .FromInstance(subscriptionValue)
            .AsCached();
    }

    public static void BindMonoBehaviour<T>(this DiContainer Container, T injectable)
        where T : MonoBehaviour
    {
        Container
            .Bind<T>()
            .FromInstance(injectable);
    }

    public static void BindMonoBehaviour<T>(this DiContainer Container, T injectable, object id)
        where T : MonoBehaviour
    {
        Container
            .Bind<T>()
            .WithId(id)
            .FromInstance(injectable);
    }

    public static void BindComponent<T>(this DiContainer Container, T injectable)
        where T : Component
    {
        Container
            .Bind<T>()
            .FromInstance(injectable);
    }

    public static void BindComponent<T>(this DiContainer Container, T injectable, object id)
        where T : Component
    {
        Container
            .Bind<T>()
            .WithId(id)
            .FromInstance(injectable);
    }
    
    #endregion
}