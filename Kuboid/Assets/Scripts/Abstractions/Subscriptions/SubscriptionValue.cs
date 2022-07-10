using System;
using System.Linq;
using Zenject;

public class SubscriptionValue<T> : ISubscriptionValue<T>
{
    #region Events

    public event Action<T> OnValueChanged;

    #endregion

    #region Fields

    protected T _value;

    #endregion

    #region Interfaces properties

    public T Value
    {
        get => _value;
        set => SetValue(value);
    }

    public bool IsDisposed { get; private set; }

    #endregion

    #region Interfaces Methods

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

    public void SetValue(T value)
    {
        _value = value;

        OnValueChanged?.Invoke(value);
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