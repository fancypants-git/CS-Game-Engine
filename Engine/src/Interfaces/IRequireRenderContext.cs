using Engine.Debugging;

namespace Engine;

public interface IRequireRenderContext
{
    public bool IsInitialized { get; set; }

    public abstract bool Initialize();


    public static bool RenderContextExists()
    {
        return Application.WindowManager.RenderContext.HasValue;
    }
}