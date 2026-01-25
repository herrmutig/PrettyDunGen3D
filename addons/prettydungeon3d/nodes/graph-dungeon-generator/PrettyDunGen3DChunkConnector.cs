using Godot;
using PrettyDunGen3D;

public partial class PrettyDunGen3DChunkConnector : Node
{
    PrettyDunGen3DChunk fromChunk;
    PrettyDunGen3DChunk toChunk;

    public PrettyDunGen3DChunkConnector(PrettyDunGen3DChunk from, PrettyDunGen3DChunk to)
    {
        fromChunk = from;
        toChunk = to;
        Name = "Connector_" + from.Name + "_" + to.Name;
    }

    public bool IsConnectedToChunk(PrettyDunGen3DChunk chunk)
    {
        return fromChunk == chunk || toChunk == chunk;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }
}
