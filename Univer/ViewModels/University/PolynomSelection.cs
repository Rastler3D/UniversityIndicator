using System.Collections.Generic;

namespace Univer.ViewModels.University
{
    public class PolynomSelection
    {
        public int UniversityId { get; set; }
        public Dictionary<int,List<int>> Polynom { get; set; }
    }
}
