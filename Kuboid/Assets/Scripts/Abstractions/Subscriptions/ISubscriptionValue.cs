public interface ISubscriptionValue<T> : ISubscriptionProperty<T>
{
    #region Properties

    new T Value { get; set; }

    #endregion
}