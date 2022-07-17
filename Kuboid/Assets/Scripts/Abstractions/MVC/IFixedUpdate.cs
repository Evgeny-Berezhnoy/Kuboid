public interface IFixedUpdate : IController
{
    #region Methods

    void OnFixedUpdate(float fixedDeltaTime);
    
    #endregion
}