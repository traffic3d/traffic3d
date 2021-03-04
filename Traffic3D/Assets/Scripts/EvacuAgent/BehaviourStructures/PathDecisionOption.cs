using System.Collections.Generic;

public class PathDecisionOption
{
    public PedestrianPoint PedestrianPoint { get; set; }
    public int PathDecisionRank { get; set; }
    public float WeightedSumOfPathNodes { get; set; }
    public List<PathDecisionNode> PathDecisionNodes { get; set; }

    public PathDecisionOption()
    {
        PathDecisionNodes = new List<PathDecisionNode>();
    }
}
