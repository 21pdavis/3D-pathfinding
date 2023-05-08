public class Edge
{
    public PathGraphNode node { private set; get; }
    public double weight{ private set; get; }

    public Edge(PathGraphNode node, double weight)
    {
        this.node = node;
        this.weight = weight;
    }
}
