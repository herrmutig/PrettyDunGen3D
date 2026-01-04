using System;
using Godot;

namespace Mutigsoft.PrettyDunGen3D;

/// <summary>
/// Base class for defining custom generation rules used by <see cref="PrettyDunGen3DGenerator"/>.
///
/// A rule represents a single step in the dungeon generation pipeline and is executed
/// sequentially during generation phase. Rules can modify chunks,
/// place geometry, validate constraints, or abort generation entirely.
///
/// To avoid editor errors it is highly recommended to add the <see cref="ToolAttribute"/>
/// to any class that inherits from <see cref="PrettyDunGen3DRule"/>
/// More Info: https://docs.godotengine.org/en/stable/tutorials/plugins/running_code_in_the_editor.html
/// </summary>
[Tool]
[GlobalClass]
public partial class PrettyDunGen3DRule : Node
{
    /// <summary>
    /// Called once before generation begins.
    /// Use this to initialize or reset internal state.
    /// </summary>
    public virtual void OnInitialize(PrettyDunGen3DGenerator generator) { }

    /// <summary>
    /// Executed during the global generation phase.
    /// Rules run sequentially in hierarchy order.
    /// Override to implement custom generation logic.
    /// Return <c>true</c> to continue generation; <c>false</c> stops it entirely.
    /// </summary>
    public virtual bool OnGenerate(PrettyDunGen3DGenerator generator)
    {
        GD.Print("Note: Override OnGenerate to create a custom rule!", this);
        return true;
    }

    /// <summary>
    /// Can be overwritten to create custom visual debugging information
    /// </summary>
    public virtual void DrawDebug() { }
}
