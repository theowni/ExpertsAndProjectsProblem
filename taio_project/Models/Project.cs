using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taio_project.Models
{
    public class Project
    {
        public List<int> Requirements;
        public readonly int Id;

        public int RequirementsSum
        {
            get { return Requirements.Sum(); }
        }

        public Project(int id, List<int> requirements)
        {
            Id = id;
            Requirements = requirements;
        }
    }
}
