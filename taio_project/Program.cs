using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;
using taio_project.Controllers;
using taio_project.Models;

namespace taio_project
{
    class Program
    {
        static void Main(string[] args)
        {
            // Przykład
            // Wprowadzamy listę obiektów Expert
            // Każdy ekspert w konstruktorze ma:
            //   1. Id numerowane od 0
            //   2. Lista umiejętności (true/false)
            var experts = new List<Expert>()
            {
                new Expert(0, new List<bool>{ false, false, true }),
                new Expert(1, new List<bool>{ false, false, true }),
                new Expert(2, new List<bool>{ false, false, true }),
                new Expert(3, new List<bool>{ false, false, true }),
                new Expert(4, new List<bool>{ true, false, true }),
                new Expert(5, new List<bool>{ false, false, true }),
                new Expert(6, new List<bool>{ false, true, true }),
                new Expert(7, new List<bool>{ false, false, true }),
                new Expert(8, new List<bool>{ false, false, false }),
                new Expert(9, new List<bool>{ false, false, true }),
            };

            // Wprowadzamy listę obiektów Project
            // Każdy projekt w konstruktorze ma:
            //   1. Id numerowane od 0
            //   2. Lista wymagań danego projektu
            var projects = new List<Project>()
            {
                new Project(0, new List<int>{ 1, 0, 1 }),
                new Project(1, new List<int>{ 1, 0, 2 }),
                new Project(2, new List<int>{ 1, 0, 1 }),
                new Project(3, new List<int>{ 1, 0, 3 }),
            };

            // Aby zaimplementować wczytywanie z pliku 
            // wystarczy uzupełnić te dwie powyższe kolekcje
            // w taki sposób jak pokazano w przykładzie.

            var solver = new ProblemSolver(experts, projects);
            solver.Solve();
        }
    }
}
