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

        [TestMethod]
        public void ToManyExpertsTest()
        {
            var experts = new List<Expert>()
            {
                new Expert(0, new List<bool>{ true, false, false }),
                new Expert(1, new List<bool>{ false, false, true }),
                new Expert(2, new List<bool>{ true, false, false }),
                new Expert(3, new List<bool>{ false, false, true }),
                new Expert(4, new List<bool>{ false, false, true }),
                new Expert(5, new List<bool>{ false, false, true }),
                new Expert(6, new List<bool>{ false, false, true }),
            };
            var projects = new List<Project>()
            {
                new Project(0, new List<int>{ 1, 0, 1 }),
                new Project(1, new List<int>{ 1, 0, 1 }),
            };

            var solver = new ProblemSolver(experts, projects);
            Graph graphFlow = solver.Solve();
            GraphExport ge = new GraphExport();
            ge.Export(graphFlow, null, "Graph");
        }

        [TestMethod]
        public void NoExpertsTest()
        {
            var experts = new List<Expert>()
            {
                
            };
            var projects = new List<Project>()
            {
                new Project(0, new List<int>{ 1, 0, 1 }),
                new Project(1, new List<int>{ 1, 0, 1 }),
            };

            var solver = new ProblemSolver(experts, projects);
            Graph graphFlow = solver.Solve();

            if (graphFlow == null)
                return;

            GraphExport ge = new GraphExport();
            ge.Export(graphFlow, null, "Graph");
        }


        [TestMethod]
        public void ToManyProjectsTest()
        {
            var experts = new List<Expert>()
            {
                new Expert(0, new List<bool>{ true, false, true }),
                new Expert(1, new List<bool>{ false, false, true }),
                new Expert(2, new List<bool>{ false, false, true }),
                new Expert(3, new List<bool>{ false, true, false }),
                new Expert(4, new List<bool>{ false, true, false }),
            };
            var projects = new List<Project>()
            {
                new Project(0, new List<int>{ 1, 0, 1 }),
                new Project(1, new List<int>{ 0, 0, 1 }),
                new Project(2, new List<int>{ 0, 0, 1 }),
                new Project(3, new List<int>{ 0, 0, 1 }),
            };

            var solver = new ProblemSolver(experts, projects);
            Graph graphFlow = solver.Solve();
            GraphExport ge = new GraphExport();
            ge.Export(graphFlow, null, "Graph");
        }

        [TestMethod]
        public void BigDataTest()
        {
            var experts = new List<Expert>()
            {
            };

            var projects = new List<Project>()
            {
            };

            for (int i = 0; i < 200; ++i)
            {
                var specializations = new List<bool>();
                var requirements = new List<int>();

                for (int j = 0; j < 10; ++j)
                {
                    specializations.Add(true);
                    requirements.Add(j);
                }
                experts.Add(new Expert(i, specializations));
                projects.Add(new Project(i, requirements));
            }

            var solver = new ProblemSolver(experts, projects);
            Graph graphFlow = solver.Solve();
        }
    }
}