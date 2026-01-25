using System.Linq;
using Godot;
using PrettyDunGen3D;

// TODO Consider Connectors to also apply meta data... Not sure though if needed...
[Tool]
[GlobalClass]
public partial class MetaDataRule : PrettyDunGen3DRule
{
    public enum MetaDataApplyStrategy
    {
        Sequencial,
        Random,
    }

    [Export]
    public string CategoryToApplyMeta { get; set; } = "";

    [Export]
    public int MaxChunksToApplyMeta { get; set; } = -1;

    [Export]
    public Vector2I IndexRangeToApplyMeta { get; set; } = new Vector2I(-1, -1);

    [Export]
    public MetaDataApplyStrategy Strategy { get; set; } = MetaDataApplyStrategy.Sequencial;

    public override string OnGenerate(PrettyDunGen3DGenerator generator)
    {
        if (GetMetaList().Count < 1)
            return $"No Metadata defined. Please add MetaData to MetaDataRule: {Name}";

        if (CategoryToApplyMeta == null || CategoryToApplyMeta == "")
            return null;

        var nodes = generator
            .Graph.GetNodes()
            .Where(chunk => chunk.ContainsCategory(CategoryToApplyMeta))
            .ToList();

        if (Strategy == MetaDataApplyStrategy.Random)
        {
            RandomNumberGenerator numberGenerator = new();
            numberGenerator.Seed = generator.Seed;

            // Fisher–Yates Shuffle
            for (int i = nodes.Count - 1; i > 0; i--)
            {
                int j = numberGenerator.RandiRange(0, i);
                (nodes[i], nodes[j]) = (nodes[j], nodes[i]);
            }
        }

        int appliedCounter = 0;

        for (int i = 0; i < nodes.Count; i++)
        {
            if (appliedCounter >= MaxChunksToApplyMeta)
                return null;

            if (!IsInIndexRange(i))
                continue;

            foreach (string metaName in GetMetaList())
            {
                // Do not override script data...
                if (metaName == "_custom_type_script")
                {
                    continue;
                }

                nodes[i].SetMeta(metaName, GetMeta(metaName));
            }

            appliedCounter++;
        }

        return null;
    }

    private bool IsInIndexRange(int index)
    {
        return index >= IndexRangeToApplyMeta.X
            && (index <= IndexRangeToApplyMeta.Y || IndexRangeToApplyMeta.Y < 0);
    }
}
