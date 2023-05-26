namespace ProgrammInterface
{
    public interface IPartiteGraph
    {
        public (List<Vertex>, List<Vertex>) PartiteGraph(int[,] adjacencyMatrix, int checksCount);
    }
}