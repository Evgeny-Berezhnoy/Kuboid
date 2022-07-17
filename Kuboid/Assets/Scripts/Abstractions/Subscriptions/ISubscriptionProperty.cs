using System;

public interface ISubscriptionProperty : IDisposableAdvanced
{
    #region Methods

    void Invoke();
    void Subscribe(Action action);
    void Unsubscribe(Action action);
    void UnsubscribeAll();
    
    #endregion
}

public interface ISubscriptionProperty<T> : IDisposableAdvanced
{
    #region Events

    event Action<T> OnValueChanged;

    #endregion

    #region Properties

    T Value { set; }

    #endregion

    #region Methods

    void Subscribe(Action<T> action);
    void Unsubscribe(Action<T> action);
    void UnsubscribeAll();

    #endregion
}