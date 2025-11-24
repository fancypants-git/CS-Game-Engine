using Engine.Components;
using Engine.Rendering;

namespace Engine.Interfaces;

/// <summary>
/// Interface for Drawable Components and Classes.<br/><br/>
/// Contains a Draw(Camera) method that is automatically called when the Parent Scene is Called for Rendering
/// Or this can be called Manually.<br/><br/>
/// Contains a Material to be used when Draw is called.
/// </summary>
public interface IDrawable : IDisposable
{
    public Material[] Materials { get; set; }
    
    public void Draw(Camera camera);
}