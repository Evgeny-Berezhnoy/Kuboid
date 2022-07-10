using System;

public interface IDisposableAdvanced : IDisposable
{
    #region Properties

    bool IsDisposed { get; }

    #endregion
}