public class Edge
{
    public PathGraphNode from { private set; get; }
    public PathGraphNode to { private set; get; }
    public double weight{ private set; get; }

    public Edge(PathGraphNode from, PathGraphNode to, double weight)
    {
        this.from = from;
        this.to = to;
        this.weight = weight;
    }
}
