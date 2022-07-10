using System;
using System.Linq;
using Zenject;

public class ActionSubscriptionProperty : ISubscriptionProperty
{
    #region Events

    private event Action _onInvoke;

    #endregion

    #region Interface properties

    public bool IsDisposed { get; private set; }

    #endregion

    #region Interface methods

    public void Invoke()
    {
        _onInvoke?.Invoke();
    }

    public void Subscribe(Action action)
    {
        _onInvoke += action;
    }

    public void Unsubscribe(Action action)
    {
        _onInvoke -= action;
    }

    public void UnsubscribeAll()
    {
        var handlers =
            _onInvoke
                ?.GetInvocationList()
                .ToList()
                .Cast<Action>()
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