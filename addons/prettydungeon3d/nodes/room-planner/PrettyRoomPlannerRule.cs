using Godot;

namespace PrettyDunGen3D;

// Note: A Rule can atmorly perform three tasks to the roomplanner:
// Instantiate a scene, Destroy a scene and Transform a scene
[Tool]
[GlobalClass]
public partial class PrettyRoomPlannerRule : Resource
{
    public virtual void Execute(PrettyRoomPlanner roomPlanner)
    {
        GD.Print(
            "This rule does nothing. Inherit from PrettyRoomPlannerRule to create custom rules",
            this
        );
    }

    public virtual void DrawDebug() { }
}
