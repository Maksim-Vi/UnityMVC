using System;

namespace Code.MVC
{
    public interface IView
    {
        /// <summary> Bind a model to the view. Called by the controller. </summary>
        void ApplyModel(IModel model);

        /// <summary> Close/destroy the view (calls Destroy(gameObject) by default). </summary>
        void Close();

        /// <summary> Set a reference to the controller (for event callbacks). </summary>
        void SetController(IController controller);

        /// <summary> Invoked before the actual Destroy, when Close() is initiated. </summary>
        event Action Closed;

        /// <summary> Invoked in OnDestroy(). </summary>
        event Action Destroyed;
    }
}