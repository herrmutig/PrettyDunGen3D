using Godot;
using PrettyDunGen3D;

[GlobalClass]
[Tool]
public partial class PrettyPlannerAction : PrettyPlannerNode
{
    /// <summary>
    /// Searches the executer chain backwards and returns the most recent
    /// <see cref="PrettyPlannerTransformer"/> in the chain.
    /// </summary>
    /// <returns>
    /// The last <see cref="PrettyPlannerTransformer"/> found in the chain,
    /// or <c>null</c> if no transformer exists.
    /// </returns>
    protected PrettyPlannerTransformer FindLastPlannerTransformer()
    {
        PrettyPlannerNode current = PreviousExecuter;
        while (current != null)
        {
            if (current is PrettyPlannerTransformer)
                return (PrettyPlannerTransformer)current;

            current = current.PreviousExecuter;
        }

        return null;
    }
}
