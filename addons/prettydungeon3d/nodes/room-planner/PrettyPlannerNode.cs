using System;
using Godot;
using Godot.Collections;

namespace PrettyDunGen3D;

[Tool]
public partial class PrettyPlannerNode : Node
{
    public PrettyPlannerNode PreviousExecuter { get; private set; }
    public PrettyRoomPlanner RoomPlanner { get; private set; }

    public override void _EnterTree()
    {
        if (Owner is PrettyRoomPlanner roomPlanner)
            RoomPlanner = roomPlanner;
    }

    public void Execute(PrettyRoomPlanner roomPlanner, PrettyPlannerNode previousExecuter = null)
    {
        PreviousExecuter = previousExecuter;
        RoomPlanner = roomPlanner;

        OnExecute(roomPlanner, previousExecuter);

        if (!AllowChildrenToExecute())
            return;
        if (StopRuleExecution())
            return;

        foreach (var plannerNode in GetPlannerNodeChildren())
        {
            plannerNode.Execute(roomPlanner, this);
            if (plannerNode.StopRuleExecution())
                return;
        }
    }

    // Especially useful for Conditional Nodes...
    protected virtual bool AllowChildrenToExecute()
    {
        return true;
    }

    protected virtual bool StopRuleExecution()
    {
        return false;
    }

    protected virtual void OnExecute(
        PrettyRoomPlanner roomPlanner,
        PrettyPlannerNode previousExecuter
    ) { }

    protected Array<PrettyPlannerNode> GetPlannerNodeChildren()
    {
        Array<PrettyPlannerNode> plannerNodes = new();
        foreach (var child in GetChildren())
        {
            if (child is PrettyPlannerNode plannerNode)
            {
                plannerNodes.Add(plannerNode);
            }
        }

        return plannerNodes;
    }
}
