using System;

namespace Univer.Models
{
    public class UniversityIndicator
    {
        public int Id { get; set; }
        public virtual University University { get; set; }
        public virtual Indicator Indicator { get; set; }
        public int Year { get; set; }
        public int Value { get; set; }

    }
}
