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

        private int GetVerticeIdFromProjectInfo(int projectId, int requirementIndex)
        {
            int requirementsCount = Experts[0].Specializations.Count;
            return Experts.Count + requirementsCount * projectId + requirementIndex;
        }

        private int GetProjectIdFromVerticeId(int verticeId)
        {
            throw new NotImplementedException();
        }

        public void GenerateResult(Graph graphFlow)
        {
            // iterujemy się po wierzchołkach ekspertów i wypisujemy
            // projekty (i dziedzinę) do jakich zostal przypisany

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
                        // o ID = project.Id i wykorzystał umiejętność
                        // o ID = j

                        // numerowanie od 0

                        Console.WriteLine($"Expert nr { expertId } został przypisany do projektu nr { project.Id }, wykorzystał specjalizację nr { j }");
                    }
                }
            }
        }

        public Graph Solve()
        {
            int sourceVertice;
            int targetVertice;
            // tworzenie grafu
            Graph graph = CreateGraph(out sourceVertice, out targetVertice);

            // TODO
            // zaimplementowac Forda-Fulkersona
            // jesli zajdzie taka potrzeba
            Graph graphFlow;
            graph.FordFulkersonDinicMaxFlow(sourceVertice, targetVertice, out graphFlow, MaxFlowGraphExtender.BFPath);

            // tworzenie wyniku na podstawie
            // grafu przepływu
            GenerateResult(graphFlow);

            return graphFlow;
        }

        public Graph CreateGraph(out int sourceVertice, out int targetVertice)
        {
            if (Experts == null || Projects == null)
                throw new Exception("Uzupelnij wartosci wejsciowe!");

            // TODO
            // uwzględnić przypadki brzegowe: 
            // 1. 0 ekspertów
            // 2. 0 projektów

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
    }
}