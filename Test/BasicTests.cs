using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ASD.Graphs;
using taio_project.Controllers;
using taio_project.Models;
using System.Collections.Generic;

namespace Test
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void BasicTestOutGraphviz()
        {
            var experts = new List<Expert>()
            {
                new Expert(0, new List<bool>{ false, false, true }),
                new Expert(1, new List<bool>{ false, false, true }),
                new Expert(2, new List<bool>{ false, false, true }),
                new Expert(3, new List<bool>{ false, false, true }),
                new Expert(4, new List<bool>{ false, false, true }),
                new Expert(5, new List<bool>{ false, false, true }),
                new Expert(6, new List<bool>{ false, true,  true }),
                new Expert(7, new List<bool>{ false, false, true }),
                new Expert(8, new List<bool>{ false, false, false }),
                new Expert(9, new List<bool>{ false, false, true }),
            };
            var projects = new List<Project>()
            {
                new Project(0, new List<int>{ 1, 0, 1 }),
                new Project(1, new List<int>{ 1, 0, 1 }),
                new Project(2, new List<int>{ 1, 0, 1 }),
                new Project(3, new List<int>{ 1, 0, 1 })
            };

            var solver = new ProblemSolver(experts, projects);
            Graph graphFlow = solver.Solve();
            GraphExport ge = new GraphExport();
            ge.Export(graphFlow, null, "Graph");
        }
    }
}
