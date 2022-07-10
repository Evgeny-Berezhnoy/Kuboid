using System;
using System.Linq;
using Zenject;

public class SubscriptionProperty<T> : ISubscriptionProperty<T>
{
    #region Events

    public event Action<T> OnValueChanged;

    #endregion

    #region Properties

    public T Value { set => OnValueChanged?.Invoke(value); }
    public bool IsDisposed { get; private set; }

    #endregion

    #region Interfaces methods

    public void Subscribe(Action<T> action)
    {
        OnValueChanged += action;
    }

    public void Unsubscribe(Action<T> action)
    {
        OnValueChanged -= action;
    }

    public void UnsubscribeAll()
    {
        var handlers =
            OnValueChanged
                ?.GetInvocationList()
                .ToList()
                .Cast<Action<T>>()
                .ToList();

        if (handlers == null) return;

        for (int i = 0; i < handlers.Count; i++)
        {
            Unsubscribe(handlers[i]);
        };
    }

    public void Dispose()
    {
        if (IsDisposed) return;

        IsDisposed = true;

        UnsubscribeAll();

        GC.SuppressFinalize(this);
    }

    #endregion

    #region Injected methods

    [Inject]
    protected void Initialize(
        // BasicsInjector
        [Inject] Disposer disposer)
    {
        disposer.Subscribe(Dispose);
    }

    #endregion
}