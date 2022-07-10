using System.Collections.Generic;
using Zenject;

public class DiContainerWrapper
{
    #region Fields

    [Inject] private DiContainer _diContainer;

    #endregion

    #region Extension methods

    public T Inject<T>(T injectable)
    {
        _diContainer.Inject(injectable);

        return injectable;
    }

    public T Inject<T>(T injectable, IEnumerable<object> extraArgs)
    {
        _diContainer.Inject(injectable, extraArgs);

        return injectable;
    }

    #endregion
}