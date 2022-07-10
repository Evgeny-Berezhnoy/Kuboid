using System;
using System.Linq;

public class Disposer : ISubscriptionProperty
{
    #region Events

    private event Action _onDispose;

    #endregion

    #region Interface properties

    public bool IsDisposed { get; private set; }

    #endregion

    #region Interface methods

    public void Invoke()
    {
        _onDispose?.Invoke();
    }

    public void Subscribe(Action action)
    {
        _onDispose += action;
    }

    public void Unsubscribe(Action action)
    {
        _onDispose -= action;
    }

    public void UnsubscribeAll()
    {
        var handlers =
            _onDispose
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

        Invoke();

        UnsubscribeAll();

        GC.SuppressFinalize(this);
    }

    #endregion
}