using OpenTK.Windowing.Desktop;

namespace Windowing;

/// <summary>
/// Manages the CPU side of the rendering pipeline, camera rendering and render context requests
/// </summary>
public class RenderPipelineManager
{
    private List<Camera> _cameras = new(); // the camera objects themselves are managed by the GameManager


    /// <summary>
    /// Registers a camera for rendering
    /// </summary>
    /// <param name="camera"></param>
    public void RegisterCamera(Camera camera)
    {
        _cameras.Add(camera);
    }

    /// <summary>
    /// Unregisters a camera for rendering
    /// </summary>
    /// <param name="camera"></param>
    public void UnregisterCamera(Camera camera)
    {
        _cameras.Remove(camera);
    }

    /// <summary>
    /// Requests all cameras to render to their respective windows
    /// </summary>
    public void RenderCameras()
    {
        HashSet<Camera> unregisterCameras = new();
        foreach (Camera camera in _cameras)
        {
            // if this window has been closed this frame, if so unregister the camera
            // this will only occur when the window has been closed manually, as in the other scenario there will be no cameras targeting the closed window
            if (Application.WindowManager.ClosedWindows.Contains(camera.Target))
            {
                unregisterCameras.Add(camera);
                continue;
            }

            // if this window is not a valid target, create a new window
            if (!Application.WindowManager.IsValidTarget(camera.Target))
                Application.WindowManager.CreateWindow(camera.Target, NativeWindowSettings.Default);

            // render the camera's POV onto the window target
            camera.Render();
        }

        foreach (Camera camera in unregisterCameras)
        {
            UnregisterCamera(camera);
        }
    }
}