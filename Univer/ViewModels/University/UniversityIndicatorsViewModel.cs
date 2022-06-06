using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using Univer.Models;

namespace Univer.ViewModels
{
    public class UniversityIndicatorsViewModel
    {
        public int UniversityId { get; set; }
        public int Year { get; set; }

        public List<IndicatorValue> Indicators { get; set; }

       

        
    }
    public class IndicatorValue
    {
        public int UniversityIndicatorId { get; set; }
        public Indicator Indicator { get; set; }
        public int Value { get; set; }

    }
    
}
