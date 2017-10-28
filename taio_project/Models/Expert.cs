using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taio_project.Models
{
    public class Expert
    {
        public List<bool> Specializations;
        public readonly int Id;

        public Expert(int id, List<bool> specializations)
        {
            Id = id;
            Specializations = specializations;
        }
    }
}
