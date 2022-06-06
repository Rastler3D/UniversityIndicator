using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Numpy;
using Python.Runtime;
using Univer.Data;
using Univer.Models;
using Univer.ViewModels.University;


namespace Univer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db, Py.GILState state)
        {
            _db = db;
        }
        [HttpPost("chart")]
        public double[][] Index(PolynomSelection polynom)
        {
            PyDict dict = polynom.Polynom.ToPython();
            var indicators = _db.UniversityIndicators.Where(x=>x.University.Id == polynom.UniversityId).AsEnumerable().GroupBy(x=>x.Year).Last().ToList();                                   
            PyList list = new PyList(indicators.Select(x => ((double)x.Value/100).ToPython()).ToArray());
            dynamic odeSolver = Py.Import("odesolver");
            NDarray array = new(odeSolver.solve(list,dict).transpose());
            double[][] arr = array.GetData<double>().Chunk(array.shape.Dimensions[1]).ToArray();       
            return arr;
        }

    }
}
