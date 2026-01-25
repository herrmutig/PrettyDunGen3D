using Godot;

// Instantiates a Scene on Chunks
// TODO Add Constants (pd3d_chunk_size) for the meta strings
// TODO Add Option to spawn Scenes for Connection Points

namespace PrettyDunGen3D;

[GlobalClass]
[Tool]
public partial class PackedSceneInstantiateRule : PrettyDunGen3DRule
{
    [Export]
    public PackedScene Node3DSceneToInstantiate { get; set; }

    [Export]
    public bool InstantiateForConnections { get; set; } = true;

    public override string OnGenerate(PrettyDunGen3DGenerator generator)
    {
        if (Node3DSceneToInstantiate == null)
            return "Can not instantiate scenes since Node3DSceneToInstantiate is not set!";

        foreach (var chunk in generator.Graph.GetNodes())
        {
            Node3D instance = (Node3D)Node3DSceneToInstantiate.Instantiate();
            instance.SetMeta("pd3d_size", chunk.Size);

            chunk.AddChild(instance);
            instance.Owner = chunk.Owner;

            if (InstantiateForConnections)
            {
                foreach (var neighbour in chunk.Neighbours)
                {
                    if (neighbour.HasMeta("pd3d_chunk_connected"))
                        continue;

                    Node3D connInstance = (Node3D)Node3DSceneToInstantiate.Instantiate();
                    connInstance.Position = chunk.GetConnectorCenter(neighbour, true);

                    connInstance.SetMeta(
                        "pd3d_size",
                        chunk.GetDistanceVectorTo(neighbour).Abs() + Vector3.Right * 2f
                    );

                    chunk.AddChild(connInstance);
                    connInstance.Owner = chunk.Owner;
                }

                chunk.SetMeta("pd3d_chunk_connected", true);
            }

            // Create another scene instance to spawn
            // Position it so that it is in the middle between the two connecting chunks
            // Add info about the two connecting chunks
            // Mark pair as connected and created (maybe some kind of a <chunk, List<Chunk> thing? - basically use a temp graph here?)
            // Done here.
        }

        return null;
    }
}
