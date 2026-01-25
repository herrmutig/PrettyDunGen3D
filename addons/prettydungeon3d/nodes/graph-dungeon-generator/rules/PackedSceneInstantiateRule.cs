using Godot;

// Instantiates a Scene on Chunks and ChunkConnectors
namespace PrettyDunGen3D;

[GlobalClass]
[Tool]
public partial class PackedSceneInstantiateRule : PrettyDunGen3DRule
{
    [Export]
    public PackedScene Node3DSceneToInstantiate { get; set; }

    [Export]
    public bool InstantiateForConnectors { get; set; } = true;

    [Export]
    public bool ApplyChunkMetaDataListToScene { get; set; } = true;

    public override string OnGenerate(PrettyDunGen3DGenerator generator)
    {
        if (Node3DSceneToInstantiate == null)
            return "Can not instantiate scenes since Node3DSceneToInstantiate is not set!";

        foreach (var chunk in generator.Graph.GetNodes())
        {
            Node3D instance = (Node3D)Node3DSceneToInstantiate.Instantiate();
            instance.SetMeta(MetaDataUtility.METADATA_CHUNK_SIZE, chunk.Size);
            instance.SetMeta(MetaDataUtility.METADATA_CHUNK_NODE_PATH, chunk.GetPath());
            ApplyMetaData(chunk, instance);

            chunk.AddChild(instance);
            instance.Owner = chunk.Owner;

            if (InstantiateForConnectors)
            {
                foreach (var chunkConnector in chunk.Connectors)
                {
                    // Avoids duplicate scene spawning when another chunk has the same connector attached.
                    if (chunkConnector.HasMeta(MetaDataUtility.METADATA_CHUNK_CONNECTED))
                        continue;

                    chunkConnector.SetMeta(MetaDataUtility.METADATA_CHUNK_CONNECTED, true);

                    // Create Scene Instance and add useful metadata for scene
                    Node3D connInstance = (Node3D)Node3DSceneToInstantiate.Instantiate();
                    connInstance.SetMeta(MetaDataUtility.METADATA_CHUNK_SIZE, chunkConnector.Size);
                    connInstance.SetMeta(
                        MetaDataUtility.METADATA_CONNECTOR_NODE_PATH,
                        chunkConnector.GetPath()
                    );
                    ApplyMetaData(chunkConnector, connInstance);

                    chunkConnector.AddChild(connInstance);
                    connInstance.Owner = chunkConnector.Owner;
                }
            }
        }

        return null;
    }

    private void ApplyMetaData(Node source, Node destination)
    {
        if (!ApplyChunkMetaDataListToScene)
            return;

        foreach (string metaName in source.GetMetaList())
        {
            // Do not override script data...
            if (metaName == "_custom_type_script")
            {
                continue;
            }

            destination.SetMeta(metaName, source.GetMeta(metaName));
        }
    }
}
