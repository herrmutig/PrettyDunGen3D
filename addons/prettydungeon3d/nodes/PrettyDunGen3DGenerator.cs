using System;
using System.Data;
using System.Diagnostics;
using Godot;
using Godot.Collections;

namespace Mutigsoft.PrettyDunGen3D;

[GlobalClass]
[Tool]
public partial class PrettyDunGen3DGenerator : Node3D
{
    // Mainly used by rules during generation to act on category changes.
    // TODO later we could only autogenerate when a rule has been changed or any property...

    public event Action<PrettyDunGen3DChunk> OnChunkCategoriesChanged;
    public PrettyDunGen3DGraph Graph { get; private set; }
    public Array<PrettyDunGen3DRule> Rules { get; private set; }

    [ExportGroup("General")]
    [Export]
    public bool PersistGenerated { get; set; } = false;

    [Export]
    public bool AutoGenerate { get; set; } = false;

    [Export]
    public ulong Seed { get; set; } = 0;

    [Export]
    public bool RandomizeSeedOnGeneration { get; set; } = false;

    [ExportGroup("Generation")]
    [Export]
    public float chunkSize = 10;

    [Export]
    public Vector3I chunkDimension = new Vector3I(5, 1, 5);

    [ExportToolButton("Generate!")]
    Callable GenerateButton => Callable.From(Generate);

    [ExportToolButton("Clear")]
    Callable ClearGenerationButton => Callable.From(FreeGeneration);

    [ExportGroup("Debug")]
    [Export]
    public bool ShowDebug { get; set; } = true;

    [Export]
    public Color ChunkDebugColor { get; set; } = new Color(0f, 0, 1f, 0.5f);

    [Export]
    public float AutoGenerationEditorTimeout = 4f;

    RandomNumberGenerator numberGenerator;
    Node3D generationContainer;
    Timer debugAutoGenerationTimer;

    public override void _Ready()
    {
        numberGenerator = new();

        if (Engine.IsEditorHint())
        {
            debugAutoGenerationTimer = new Timer();
            debugAutoGenerationTimer.WaitTime = AutoGenerationEditorTimeout;
            debugAutoGenerationTimer.Timeout += DebugAutoGenerateTimeout;
            AddChild(debugAutoGenerationTimer);
            debugAutoGenerationTimer.Start();
        }

        if (AutoGenerate)
            Generate();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint() && ShowDebug && Rules != null)
        {
            foreach (var rule in Rules)
            {
                rule.DrawDebug();
            }
        }
    }

    public void FreeGeneration()
    {
        foreach (var node in GetChildren())
        {
            if (node is PrettyDunGen3DChunk)
            {
                RemoveChild(node);
                node.QueueFree();
            }
        }

        if (generationContainer != null)
        {
            RemoveChild(generationContainer);
            generationContainer.QueueFree();
            generationContainer = null;
        }

        if (Graph != null)
        {
            Graph.Clear();
        }
    }

    public void Generate()
    {
        if (Graph == null)
            Graph = new();
        if (Rules == null)
            Rules = new();

        if (numberGenerator == null)
            numberGenerator = new();

        // Clean Up Phase
        FreeGeneration();

        // Validation Phase
        if (chunkDimension.X < 1 || chunkDimension.Y < 1 || chunkDimension.Z < 1)
        {
            GD.PushWarning(
                $"[WorldGenerator] Invalid chunk dimensions: {chunkDimension}. "
                    + "All chunk values must be >= 1.",
                this
            );
            return;
        }

        // Generation Phase
        if (RandomizeSeedOnGeneration)
        {
            Seed = numberGenerator.Randi();
        }

        numberGenerator.Seed = Seed;

        foreach (var chunkPosition in GetInitialChunkPositions())
        {
            PrettyDunGen3DChunk chunk = new(this);
            chunk.Name = $"Chunk_({GetChunkCoordinate(chunkPosition)})";
            Graph.AddNode(chunk);
            AddChild(chunk);
            chunk.Position = chunkPosition;
            chunk.Rotation = Vector3.Zero;
            chunk.Scale = Vector3.One;

            if (PersistGenerated)
                chunk.Owner = this;
        }

        Rules.Clear();
        var ruleNodes = FindChildren("*", nameof(PrettyDunGen3DRule), true);
        foreach (var node in ruleNodes)
            Rules.Add((PrettyDunGen3DRule)node);

        foreach (var rule in Rules)
        {
            rule.OnInitialize(this);
        }

        foreach (var rule in Rules)
            rule.OnGenerate(this);
    }

    Vector3[] GetInitialChunkPositions()
    {
        int count = chunkDimension.X * chunkDimension.Y * chunkDimension.Z;
        Vector3[] positions = new Vector3[count];
        int index = 0;

        for (int x = 0; x < chunkDimension.X; x++)
        {
            for (int y = 0; y < chunkDimension.Y; y++)
            {
                for (int z = 0; z < chunkDimension.Z; z++)
                {
                    positions[index++] = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                }
            }
        }

        return positions;
    }

    public Vector3I GetChunkCoordinate(Vector3 position)
    {
        return new Vector3I(
            Mathf.FloorToInt(position.X / chunkSize),
            Mathf.FloorToInt(position.Y / chunkSize),
            Mathf.FloorToInt(position.Z / chunkSize)
        );
    }

    public bool IsCoordinateValid(Vector3I coordinate)
    {
        return !(
            coordinate.X < 0
            || coordinate.X >= chunkDimension.X
            || coordinate.Y < 0
            || coordinate.Y >= chunkDimension.Y
            || coordinate.Z < 0
            || coordinate.Z >= chunkDimension.Z
        );
    }

    public void InformChunkCategoryChanged(PrettyDunGen3DChunk chunk)
    {
        OnChunkCategoriesChanged?.Invoke(chunk);
    }

    private void DebugAutoGenerateTimeout()
    {
        if (!Engine.IsEditorHint())
            return;

        if (AutoGenerate)
            Generate();

        debugAutoGenerationTimer.WaitTime = AutoGenerationEditorTimeout;
        debugAutoGenerationTimer.Start();
    }
}
