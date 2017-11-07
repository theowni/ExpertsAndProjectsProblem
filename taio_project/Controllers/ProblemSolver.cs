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
                Console.WriteLine("Program rozpoczyna tworzenie grafu dla algorytmu FF.");
                graph = CreateGraph(out sourceVertice, out targetVertice);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Nie mozna stworzyc grafu dla 0 ekspertów lub 0 projektow.");
                return null;
            }

            // aplikacja właściwego algorytmu
            // otrzymujemy graf przepływu
            Console.WriteLine("Program zakończył tworzenie grafu.");
            Console.WriteLine("Program wykonuje na grafie algorytm FF:");
            Graph graphFlow = FordFulkerson(graph, sourceVertice, targetVertice);

            // tworzenie wyniku na podstawie
            // grafu przepływu
            Console.WriteLine("Tworzenie wyniku na podstawie obliczonego grafu przepływu.");
            GenerateResult(graphFlow);

            return graphFlow;
        }


        public void GenerateResult(Graph graphFlow)
        {
            // iterujemy się po wierzcholkach ekspertow i wypisujemy
            // projekty (i dziedzine) do jakich zostal przypisany

            Console.WriteLine();
            Console.WriteLine("///////////////////////////////");
            Console.WriteLine("///////////////////////////////");
            Console.WriteLine("////////   W Y N I K  /////////");
            Console.WriteLine("///////////////////////////////");
            Console.WriteLine("///////////////////////////////");
            Console.WriteLine("   Na podstawie grafu przepływu wypisujemy połączenia ekspertów między wymaganiami poszczególnych projektów.");
            Console.WriteLine();

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
                        Console.WriteLine($"   Ekspert nr { expertId } został przypisany do projektu nr { project.Id }, wykorzystał specjalizację nr { j }");
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

            Console.WriteLine("   Tworzymy graf rezydualny z poprzednio stworzonego grafu.");
            for (int i = 0; i < graph.VerticesCount; ++i)
                foreach (var edge in graph.OutEdges(i))
                {
                    residualGraph.AddEdge(i, edge.To, edge.Weight);
                    residualGraph.AddEdge(edge.To, i, 0);
                }

            // (glowna petla algorytmu FF)
            Console.WriteLine("      Rozpoczynam główną pętlę algorytmu:");
            while (true)
            {
                // (dopoki istnieje sciezka w grafie rezydualnym)
                Console.WriteLine("         Szukam najkrótszej ścieżki w grafie G");
                var path = FindShortestPath(residualGraph, source, target);
                if (path == null)
                {
                    Console.WriteLine("         Nie znaleziono ścieżki w grafie. Wychodzę z pętli.");
                    break;
                }

                Console.WriteLine("         Znaleziono ścieżkę w grafie.");
                // (minimalna przepustowosc w sciezce)
                int cfMin = int.MaxValue;
                for (int i = 1; i < path.Count; ++i)
                    cfMin = Math.Min(cfMin, (int)residualGraph.GetEdgeWeight(path[i - 1], path[i]));

                Console.WriteLine($"         Określam minimalną przepustowość na ścieżce={cfMin}.");

                Console.WriteLine($"         Aktualizuję wagi grafu rezydualnego:");
                // (aktualizacja grafu rezydualnego)
                for (int i = 1; i < path.Count; ++i)
                {
                    Console.WriteLine($"            Zmniejszam przepływ o {cfMin} w kierunku ujścia zgodnie ze ścieżką dla krawędzi {path[i-1]}, {path[i]}");
                    var weight = (int)residualGraph.GetEdgeWeight(path[i - 1], path[i]);
                    residualGraph.ModifyEdgeWeight(path[i - 1], path[i], -cfMin);

                    Console.WriteLine($"            Zwiększam przepływ o {cfMin} w kierunku źródła zgodnie ze ścieżką dla krawędzi {path[i - 1]}, {path[i]}");
                    var weight2 = (int)residualGraph.GetEdgeWeight(path[i], path[i-1]);
                    residualGraph.ModifyEdgeWeight(path[i], path[i - 1], cfMin);
                }
            }

            // graf rezydualny => graf przepływu
            Console.WriteLine($"      Wyjściowy graf rezydualny jest grafem maksymalnego przepływu.");
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

            Console.WriteLine($"            Wstawiamy na stos wierzchołek źródła - {source}");
            while (queue.Count > 0)
            {
                bool targetFound = false;
                var vertex = queue.Dequeue();

                Console.WriteLine($"            Zdejmujemy ze stosu wierzchołek {vertex}.");
                foreach (var edge in graph.OutEdges(vertex))
                {
                    if (edge.Weight == 0)
                        continue;

                    var neighbor = edge.To;
                    if (previous.ContainsKey(neighbor))
                        continue;

                    Console.WriteLine($"            Istnieje krawędź jaką możemy przejść od wierzchołka {vertex} do {neighbor}.");
                    Console.WriteLine($"            Dodajemy wierzcholek {vertex} jako poprzednik {neighbor}.");
                    previous[neighbor] = vertex;

                    if (neighbor == target)
                    {
                        Console.WriteLine($"            Doszlismy do źródła. Kończymy.");
                        targetFound = true;
                        break;
                    }

                    Console.WriteLine($"            Dodajemy na stos wierzcholek {neighbor}.");
                    queue.Enqueue(neighbor);
                }

                if (targetFound)
                    break;
            }

            Console.WriteLine($"            Obliczamy ścieżkę na podstawie poprzedników.");
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

            if (shortestPath(target) != null)
            {
                Console.Write($"            Obliczona ścieżka to: ");
                foreach (var el in shortestPath(target))
                    Console.Write($"{el}, ");
                Console.Write("\n");
            }

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

            Console.WriteLine("   Stworzono wierzchołki: źródło i ujście.");
            Console.WriteLine("   Łączenie wierzchołków-eskpertów ze źródłem - wagi=1.");
            // 1. polaczenie zrodla z Ekspertami
            foreach(var expert in Experts)
            {
                graph.AddEdge(sourceVertice, expert.Id);
            }

            Console.WriteLine("   Dla każdego projektu tworzymy wierzchołki wymagań i łączymy z ujściem:");
            // 2. polaczenie projektow z celem
            foreach(var project in Projects)
            {
                for(int i = 0; i < project.Requirements.Count; ++i)
                {
                    // każdy projekt ma osobne wierzchołki dla 
                    // poszczególnych wymagań

                    int verticeId = GetVerticeIdFromProjectInfo(project.Id, i);
                    graph.AddEdge(verticeId, targetVertice, project.Requirements[i]);
                    Console.WriteLine($"      Wymaganie nr {i}, projektu nr {project.Id} łączymy z ujściem - waga={project.Requirements[i]}");
                }
            }

            Console.WriteLine("   Każdego eksperta łączymy z wierzchołkami wymagań projektów, ma których się zna:");
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

                        Console.WriteLine($"      Łączymy eksperta {expert.Id} z projektem nr {projectFromSpec.Id} i wymaganiem nr {i} - waga=1.");
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