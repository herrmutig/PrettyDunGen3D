using Godot;

// Instantiates a Scene on Chunks
// TODO Add Constants for the meta strings

namespace PrettyDunGen3D;

[GlobalClass]
[Tool]
public partial class PackedSceneInstantiateRule : PrettyDunGen3DRule
{
    [Export]
    public PackedScene SceneToInstantiate { get; set; }

    public override string OnGenerate(PrettyDunGen3DGenerator generator)
    {
        foreach (var chunk in generator.Graph.GetNodes())
        {
            Node instance = SceneToInstantiate.Instantiate();
            instance.SetMeta("pd3d_chunk", chunk);
            chunk.AddChild(instance);
        }

        return null;
    }
}
