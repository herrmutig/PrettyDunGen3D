using Godot;

namespace PrettyDunGen3D;

[GlobalClass]
[Tool]
public partial class PrettyPlannerTransformer : PrettyPlannerNode
{
    public virtual Transform3D[] GetTransformations()
    {
        GD.PushWarning(
            "The default transformer does nothing, please use an inherited version or create your own transformer by inheriting from PrettyPlannerTransformer",
            this
        );
        // https://docs.godotengine.org/de/4.x/tutorials/3d/using_transforms.html
        // Basis identityBasis = Basis.Identity;
        return [];
    }
}
