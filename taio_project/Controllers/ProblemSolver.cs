using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;
using taio_project.Models;

namespace taio_project.Controllers
{
    public class ProblemSolver
    {
        public List<Expert> Experts;
        public List<Project> Projects;

        public ProblemSolver(List<Expert> experts, List<Project> projects)
        {
            Experts = experts;
            Projects = projects;
        }

        public Graph Solve()
        {
            int sourceVertice;
            int targetVertice;

            // tworzenie grafu dla algorytmu
            // zgodnie z zalozeniami dokumentu
            Graph graph;
            try
            {
                graph = CreateGraph(out sourceVertice, out targetVertice);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Nie mozna stworzyc grafu dla 0 ekspertów lub 0 projektow.");
                return null;
            }

            // aplikacja właściwego algorytmu
            // otrzymujemy graf przepływu
            Graph graphFlow = FordFulkerson(graph, sourceVertice, targetVertice);

            // tworzenie wyniku na podstawie
            // grafu przepływu
            GenerateResult(graphFlow);

            return graphFlow;
        }


        public void GenerateResult(Graph graphFlow)
        {
            // iterujemy się po wierzcholkach ekspertow i wypisujemy
            // projekty (i dziedzine) do jakich zostal przypisany
            for (int expertId = 0; expertId < Experts.Count; ++expertId)
            {
                foreach(var project in Projects)
                {
                    for(int j = 0; j < project.Requirements.Count; ++j)
                    {
                        int projectVertice = GetVerticeIdFromProjectInfo(project.Id, j);
                        double weight = graphFlow.GetEdgeWeight(expertId, projectVertice);

                        if (double.IsNaN(weight) || weight == 0)
                            continue;

                        // Expert o ID = expertId
                        // został przypisany do projektu
                        // o ID = project.Id i wykorzystal umiejetnosc
                        // o ID = j
                        // Numerowanie od 0
                        Console.WriteLine($"Ekspert nr { expertId } został przypisany do projektu nr { project.Id }, wykorzystał specjalizację nr { j }");
                    }
                }
            }
        }

        private Graph FordFulkerson(Graph graph, int source, int target)
        {
            Graph graphFlow = new AdjacencyListsGraph<SimpleAdjacencyList>(true, graph.VerticesCount);
            Graph residualGraph = new AdjacencyListsGraph<SimpleAdjacencyList>(true, graph.VerticesCount);

            // stworzenie grafu rezydualnego
            // dodanie odwrotnych krawedzi z
            // waga 0
            for (int i = 0; i < graph.VerticesCount; ++i)
                foreach (var edge in graph.OutEdges(i))
                {
                    residualGraph.AddEdge(i, edge.To, edge.Weight);
                    residualGraph.AddEdge(edge.To, i, 0);
                }

            // (glowna petla algorytmu FF)
            while (true)
            {
                // (dopoki istnieje sciezka w grafie rezydualnym)
                var path = FindShortestPath(residualGraph, source, target);
                if (path == null)
                    break;

                // (minimalna przepustowosc w sciezce)
                int cfMin = int.MaxValue;
                for (int i = 1; i < path.Count; ++i)
                    cfMin = Math.Min(cfMin, (int)residualGraph.GetEdgeWeight(path[i - 1], path[i]));

                // (aktualizacja grafu rezydualnego)
                for (int i = 1; i < path.Count; ++i)
                {
                    var weight = (int)residualGraph.GetEdgeWeight(path[i - 1], path[i]);
                    residualGraph.ModifyEdgeWeight(path[i - 1], path[i], -cfMin);

                    var weight2 = (int)residualGraph.GetEdgeWeight(path[i], path[i-1]);
                    residualGraph.ModifyEdgeWeight(path[i], path[i - 1], cfMin);
                }
            }

            // graf rezydualny => graf przepływu
            for (int i = 0; i < graph.VerticesCount; ++i)
                foreach (var edge in graph.OutEdges(i))
                {
                    var weight = (int)residualGraph.GetEdgeWeight(edge.To, edge.From);
                    graphFlow.AddEdge(edge.From, edge.To, weight);
                }

            return graphFlow;
        }

        public List<int> FindShortestPath(Graph graph, int source, int target)
        {
            // wyszukiwanie najkrotszej 
            // sciezki na podstawie BFS
            var previous = new Dictionary<int, int>();

            var queue = new Queue<int>();
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                bool targetFound = false;
                var vertex = queue.Dequeue();
                foreach (var edge in graph.OutEdges(vertex))
                {
                    if (edge.Weight == 0)
                        continue;

                    var neighbor = edge.To;
                    if (previous.ContainsKey(neighbor))
                        continue;

                    previous[neighbor] = vertex;

                    if (neighbor == target)
                    {
                        targetFound = true;
                        break;
                    }
                    queue.Enqueue(neighbor);
                }

                if (targetFound)
                    break;
            }

            Func<int, List<int>> shortestPath = v => {
                var path = new List<int> { };

                if (!previous.ContainsKey(v))
                    return null;

                var current = v;
                while (!current.Equals(source))
                {
                    path.Add(current);
                    current = previous[current];
                };

                path.Add(source);
                path.Reverse();

                return path;
            };

            return shortestPath(target);
        }

        public Graph CreateGraph(out int sourceVertice, out int targetVertice)
        {
            if (Experts == null || Projects == null)
                throw new Exception("Uzupelnij wartosci wejsciowe!");

            if (Experts.Count == 0 || Projects.Count == 0)
                throw new ArgumentOutOfRangeException();

            // Id ekspertów i projektów numerowane są od 0
            int requirementsCount = Experts[0].Specializations.Count;
            int verticesCount = Experts.Count + Projects.Count * requirementsCount + 2;
            sourceVertice = verticesCount - 2;
            targetVertice = verticesCount - 1;
            var graph = new AdjacencyListsGraph<SimpleAdjacencyList>(true, verticesCount);

            // 1. polaczenie zrodla z Ekspertami
            foreach(var expert in Experts)
            {
                graph.AddEdge(sourceVertice, expert.Id);
            }

            // 2. polaczenie projektow z celem
            foreach(var project in Projects)
            {
                for(int i = 0; i < project.Requirements.Count; ++i)
                {
                    // każdy projekt ma osobne wierzchołki dla 
                    // poszczególnych wymagań
                    int verticeId = GetVerticeIdFromProjectInfo(project.Id, i);
                    graph.AddEdge(verticeId, targetVertice, project.Requirements[i]);
                }
            }

            // 3. polaczenie ekspertow z projektami
            foreach(var expert in Experts)
            {
                // dla każdego eksperta iterujemy się po jego
                // specjalizacjach
                for (int i = 0; i < expert.Specializations.Count; ++i)
                {
                    // jeśli nie jest specjalistą w danej
                    // dziedzinie i, to idziemy do następnej
                    // dziedziny
                    if (expert.Specializations[i] == false)
                        continue;

                    // szukamy projektów, które potrzebują
                    // ekspertów z danej dziedziny i
                    foreach(var projectFromSpec in Projects)
                    {
                        if (projectFromSpec.Requirements[i] == 0)
                            continue;

                        int verticeId = GetVerticeIdFromProjectInfo(projectFromSpec.Id, i);
                        graph.AddEdge(expert.Id, verticeId);
                    }
                }
            }

            return graph;
        }

        private int GetVerticeIdFromProjectInfo(int projectId, int requirementIndex)
        {
            int requirementsCount = Experts[0].Specializations.Count;
            return Experts.Count + requirementsCount * projectId + requirementIndex;
        }
    }
}