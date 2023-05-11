public class Edge
{
    public PathGraphNode From { private set; get; }
    public PathGraphNode To { private set; get; }
    public double Weight{ private set; get; }

    public Edge(PathGraphNode from, PathGraphNode to, double weight)
    {
        this.From = from;
        this.To = to;
        this.Weight = weight;
    }
}
