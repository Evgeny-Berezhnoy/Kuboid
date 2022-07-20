public class TaskCompletionProxy
{
    #region Fields

    private bool _isCompleted;

    #endregion

    #region Properties

    public bool IsCompleted
    {
        get => _isCompleted;
        set => _isCompleted = value;
    }

    #endregion
}