namespace Code.MVC
{
    public interface IController
    {
        /// <summary> Empty initialization. </summary>
        void Initialize();

        /// <summary> General initialization with arbitrary data. </summary>
        void Initialize<TData>(TData data);

        /// <summary> Bind a View to the controller. </summary>
        void ApplyView(IView view);

        /// <summary> Close the controller and its associated resources. Idempotent. </summary>
        void Close();

        /// <summary> Indicates whether the controller has been initialized. </summary>
        bool IsInitialized { get; }

        /// <summary> Indicates whether the controller has been closed. </summary>
        bool IsClosed { get; }
    }

}