using Godot;

namespace PrettyDunGen3D;

[GlobalClass]
[Tool]
public partial class FillFloorRule : PrettyRoomPlannerRule
{
    [Export]
    public string RoomResourceCategory { get; set; } = "floor";

    [Export]
    public Vector2 FillGridSize { get; set; } = Vector2.One;

    public override void Execute(PrettyRoomPlanner roomPlanner)
    {
        PrettyRoomResource roomResource = roomPlanner.GetRandomRoomResource(RoomResourceCategory);
        if (roomResource == null)
            return;

        Vector2 iterations = new Vector2(roomPlanner.Size.X, roomPlanner.Size.Z) / FillGridSize;

        iterations = iterations.Round();
        Vector3 startPosition = Vector3.Zero;
        startPosition.X = -roomPlanner.Size.X * 0.5f + 0.5f * FillGridSize.X;
        startPosition.Z = -roomPlanner.Size.Z * 0.5f + 0.5f * FillGridSize.Y;

        for (int x = 0; x < iterations.X; x++)
        for (int z = 0; z < iterations.Y; z++)
        {
            var randomScene = roomResource.Scenes.PickRandom();
            Vector3 position = startPosition + new Vector3(x, 0, z);
            Node3D instance = (Node3D)randomScene.Instantiate();
            instance.Position = position;

            roomPlanner.AddSceneInstance(roomResource.Category, instance);
        }
    }

    public override void DrawDebug() { }
}
