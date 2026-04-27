using WindowIdentifier = Windowing.WindowManager.WindowIdentifier;

namespace Windowing;

/// <summary>
/// Renders the active scene to the target window, or on request to a different render context
/// </summary>
public class Camera
{
    public Camera(WindowIdentifier target)
    {
        Target = target;
        UpdateAutoRendering(true);
    }

    private bool _enableAutoRendering;

    /// <summary>
    /// The target window of this camera
    /// </summary>
    public WindowIdentifier Target;

    public bool EnableAutoRendering
    {
        get
        {
            return _enableAutoRendering;
        }
        set
        {
            UpdateAutoRendering(value);
        }
    }
    
    private void UpdateAutoRendering(bool value)
    {
        if (!_enableAutoRendering && value) // if the value changed to true
            Application.RenderPipelineManager.RegisterCamera(this);
        else if (_enableAutoRendering && !value) // if the value changed to false
            Application.RenderPipelineManager.UnregisterCamera(this);

        _enableAutoRendering = value;
    }

    /// <summary>
    /// Renders the POV of the camera to its target
    /// </summary>
    public void Render()
    {
        Application.WindowManager.RegisterWindowDrawRequest(Target);
        // if rendering will be implemented further: check for return value ^^ to determine if the render request is permitted to continue
    }

    /// <summary>
    /// Renders the POV of the camera to a given render context
    /// </summary>
    /// <exception cref="NotImplementedException">This entire function has not been implemented yet, but is in the todo list for the future</exception>
    public void RenderTo()
    {
        throw new NotImplementedException();
    }
}