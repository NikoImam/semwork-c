using ProgrammInterface;

namespace KernighanLinAlgorithm;

public class KernighanLin : IPartiteGraph
{
    struct SwapConfig
    {
        public Vertex FirstVertex;
        public Vertex SecondVertex;
        public int GValue;

        public SwapConfig(Vertex v1, Vertex v2, int gValue)
        {
            FirstVertex = v1;
            SecondVertex = v2;
            GValue = gValue;
        }
    }

    struct CutConfig
    {
        public List<Vertex> PartA;
        public List<Vertex> PartB;
        public int CutSize;
        
        public CutConfig(List<Vertex> pA, List<Vertex> pB, int cS)
        {
            PartA = pA;
            PartB = pB;
            CutSize = cS;
        }
    }


    public (List<Vertex>, List<Vertex>) PartiteGraph(int[,] adjacency, int iterationsCount)
    {
        var topList = new List<CutConfig>();
        Parallel.For(0, iterationsCount, (_) =>
        {
            List<Vertex> PartA = new();
            List<Vertex> PartB = new();

            List<Vertex> unlabeledVertices = new();
            SwapConfig maxSwapConfig;
            var n = adjacency.GetLength(0);

            for (var i = 0; i < n; i++)
            {
                unlabeledVertices.Add(new Vertex(i + 1));
            }

            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (adjacency[i, j] == 1)
                    {
                        unlabeledVertices[i].BindedVertices.Add(unlabeledVertices[j]);
                    }
                }
                if (i < n / 2)
                {
                    PartA.Add(unlabeledVertices[i]);
                }
                else
                {
                    PartB.Add(unlabeledVertices[i]);
                }
            }

            var bestPartA = PartA;
            var bestPartB = PartB;

            var minCutSize = GetCutSize(PartA, PartB);

            for (var c = 0; c < n / 2; c++)
            {
                maxSwapConfig = new SwapConfig();
                maxSwapConfig.GValue = int.MinValue;
                Parallel.For(0, PartA.Count, (i) =>
                {
                    Parallel.For(0, PartB.Count, (j) =>
                    {
                        var gValue = 0;
                        if (unlabeledVertices.Contains(PartA[i]) && unlabeledVertices.Contains(PartB[j]))
                        {
                            gValue = GetGValue(PartA[i], PartB[j]);
                        }
                        else
                        {
                            return;
                        }
                        if (gValue > maxSwapConfig.GValue)
                        {
                            maxSwapConfig = new SwapConfig(PartA[i], PartB[j], gValue);
                        }
                    });
                });

                SwapVerticies(maxSwapConfig.FirstVertex!, maxSwapConfig.SecondVertex!);

                unlabeledVertices.Remove(maxSwapConfig.FirstVertex!);
                unlabeledVertices.Remove(maxSwapConfig.SecondVertex!);

                var currentCutSize = GetCutSize(PartA, PartB);

                if (currentCutSize < minCutSize)
                {
                    minCutSize = currentCutSize;
                    bestPartA = new List<Vertex>(PartA);
                    bestPartB = new List<Vertex>(PartB);
                }
            }

            topList.Add(new CutConfig(bestPartA, bestPartB, minCutSize));

            int GetDCost(Vertex vertex)
            {
                var cost = 0;
                Parallel.ForEach(vertex.BindedVertices, (v) =>
                {
                    if (!PartA.Contains(v) && PartA.Contains(vertex) || !PartB.Contains(v) && PartB.Contains(vertex))
                    {
                        Interlocked.Increment(ref cost);
                    }
                    else if (PartA.Contains(v) && PartA.Contains(vertex) || PartB.Contains(v) && PartB.Contains(vertex))
                    {
                        Interlocked.Decrement(ref cost);
                    }
                });

                return cost;
            }
            int GetGValue(Vertex v1, Vertex v2)
            {
                var gValue = GetDCost(v1) + GetDCost(v2);
                if (v1.BindedVertices.Contains(v2))
                {
                    gValue -= 2;
                }

                return gValue;
            }
            void SwapVerticies(Vertex v1, Vertex v2)
            {
                PartA.Remove(v1);
                PartA.Add(v2);
                PartB.Remove(v2);
                PartB.Add(v1);
            }

        });

        var bestCut = GetBestCutConfig(topList);

        return (bestCut.PartA, bestCut.PartB);
    }

    CutConfig GetBestCutConfig(List<CutConfig> cutConfigs)
    {
        var bestConfig = new CutConfig();
        var minCut = int.MaxValue;
        foreach (var config in cutConfigs)
        {
            if (config.CutSize < minCut)
            {
                bestConfig = config;
                minCut = config.CutSize;
            }
        }
        return bestConfig;
    }

    int GetCutSize(List<Vertex> PartA, List<Vertex> PartB)
    {
        var cutSize = 0;
        
        Parallel.ForEach(PartA, (vertex) =>
        {
            Parallel.ForEach(vertex.BindedVertices, (v) =>
            {
                if (!PartA.Contains(v))
                {
                    Interlocked.Increment(ref cutSize);
                }
            });
        });

        return cutSize;
    }
}