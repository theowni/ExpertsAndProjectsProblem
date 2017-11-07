using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;
using taio_project.Controllers;
using taio_project.Models;
using System.IO;

namespace taio_project
{
    class Program
    {
        public static List<Expert> Experts;
        public static List<Project> Projects;
        public static int ExpertsCount = 0;
        public static int ProjectsCount = 0;
        static void Main(string[] args)
        {
            Experts = new List<Expert>();
            Projects = new List<Project>();
            if(args.Length == 0)
            {
                Console.WriteLine("Nie podano argumentu");
                System.Environment.Exit(1);
            }
            FillCollections(args[0]);
            Console.WriteLine("Program wczytał dane z pliku");

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var solver = new ProblemSolver(Experts, Projects);
            solver.Solve();
            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine("Czas wykonania:");
            Console.WriteLine(stopwatch.Elapsed.TotalSeconds + "s");
            Console.WriteLine();
        }
        static void FillCollections(string path)
        {
            try
            {
                //path = "tests.txt";
                if (File.Exists(path))
                {
                    string[] readText = File.ReadAllLines(path);
                    int entryCount = (int)Char.GetNumericValue(readText[0][0]);
                    int tmpExpertsCount = (int)Char.GetNumericValue(readText[1][0]);
                    int tmpProjectsCount = (int)Char.GetNumericValue(readText[2][0]);

                    for (int i = 3; i < tmpExpertsCount + 3; i++)
                    {
                        Expert currentExpert = new Expert(ExpertsCount++, new List<bool>());
                        for (int j = 0; j < entryCount; j += 2)
                        {
                            if (readText[i][j] == '1')
                                currentExpert.Specializations.Add(true);
                            else if (readText[i][j] == '0')
                                currentExpert.Specializations.Add(false);
                            else
                                throw new ArgumentNullException();
                        }
                        Experts.Add(currentExpert);
                    }
                    for (int i = 3 + tmpExpertsCount; i < tmpExpertsCount + tmpProjectsCount + 3; i++)
                    {
                        Project currentProject = new Project(ProjectsCount++, new List<int>());
                        for (int j = 0; j < entryCount; j += 2)
                        {
                            int a = (int)Char.GetNumericValue(readText[i][j]);
                            if (a < 0)
                                throw new ArgumentNullException();
                            currentProject.Requirements.Add(a);
                        }
                        Projects.Add(currentProject);
                    }
                }
            }
            catch (ArgumentNullException a)
            {
                Console.WriteLine("Podane dane są niepoprawne!");
                Console.ReadKey();
            }

            //wariant z prefiksami w i p

            //foreach (string s in readText)
            //{
            //if (s[0]=='w') //wykonawca
            //{
            //    Expert currentExpert = new Expert(ExpertCount++,new List<bool>());
            //    for (int i=3;i<s.Length;i+=3)
            //    {
            //        if (s[i] == '1')
            //            currentExpert.Specializations.Add(true);
            //        else
            //            currentExpert.Specializations.Add(false);
            //    }
            //    Experts.Add(currentExpert);
            //}
            //else //projekt
            //{
            //    Project currentProject = new Project(ProjectsCount++, new List<int>());
            //    for (int i = 3; i < s.Length; i += 3)
            //    {
            //        currentProject.Requirements.Add((int)Char.GetNumericValue(s[i]));
            //    }
            //    Projects.Add(currentProject);
            //}

            // }
        }
    }
}
