using Godot;
using Godot.Collections;

namespace PrettyDunGen3D;

[Tool]
[GlobalClass]
public partial class PrettyRoomPlannerRuleSet : Resource
{
    [Export]
    public Array<PrettyRoomPlannerRule> Rules
    {
        get => rules;
        private set
        {
            if (value == null)
                rules.Clear();
            else
                rules = value;

            EmitChanged();
        }
    }

    private Array<PrettyRoomPlannerRule> rules = new();
}
