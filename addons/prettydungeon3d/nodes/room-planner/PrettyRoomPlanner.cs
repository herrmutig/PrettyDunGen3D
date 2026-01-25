using System.Data;
using System.Linq;
using Godot;
using Godot.Collections;

namespace PrettyDunGen3D;

[GlobalClass]
[Tool]
public partial class PrettyRoomPlanner : Node3D
{
    [ExportGroup("General")]
    [Export]
    public Vector3 Size { get; set; } = Vector3.One * 2f;

    [Export]
    public Array<PrettyRoomResource> RoomResources { get; set; } = new();

    [ExportGroup("Generation")]
    [ExportToolButton("Generate!")]
    Callable GenerateButton => Callable.From(Generate);

    [ExportToolButton("Clear")]
    Callable ClearGenerationButton => Callable.From(FreeGeneration);

    [ExportGroup("Debugging")]
    [Export]
    public Dictionary<string, Array<Node>> SceneInstanceDictionary = new();

    public override void _Ready()
    {
        if (HasMeta(MetaDataUtility.METADATA_CHUNK_SIZE))
        {
            Size = (Vector3)GetMeta(MetaDataUtility.METADATA_CHUNK_SIZE);
            Generate();
        }
    }

    public void Generate()
    {
        FreeGeneration();

        if (SceneInstanceDictionary == null)
            SceneInstanceDictionary = new();

        if (Size.X < 0 || Size.Y < 0 || Size.Z < 0)
        {
            GD.PushWarning($"Cancelled Generation: Can not generate a room with Size: {Size}");
            return;
        }

        foreach (var child in GetChildren())
        {
            if (child is PrettyPlannerRule plannerRule)
                plannerRule.Execute(this);
        }
    }

    public void FreeGeneration()
    {
        if (SceneInstanceDictionary == null)
            SceneInstanceDictionary = new();

        foreach (var kvp in SceneInstanceDictionary)
        {
            foreach (var sceneInstance in kvp.Value)
            {
                if (sceneInstance != null && !sceneInstance.IsQueuedForDeletion())
                {
                    sceneInstance.QueueFree();
                }
            }
        }

        SceneInstanceDictionary.Clear();
    }

    public PrettyRoomResource GetRandomRoomResource(string roomResourceCategory = "")
    {
        if (string.IsNullOrWhiteSpace(roomResourceCategory))
            return RoomResources.PickRandom();

        Array<PrettyRoomResource> filtered =
        [
            .. RoomResources.Where((rr) => rr.Category == roomResourceCategory),
        ];

        return filtered.PickRandom();
    }

    public void AddSceneInstance(string category, Node instance)
    {
        if (!instance.IsInsideTree())
            AddChild(instance);

        if (!SceneInstanceDictionary.ContainsKey(category))
        {
            SceneInstanceDictionary.Add(category, [instance]);
            return;
        }

        var instances = SceneInstanceDictionary[category];
        if (instances.Contains(instance))
            return;

        SceneInstanceDictionary[category].Add(instance);
    }
}
