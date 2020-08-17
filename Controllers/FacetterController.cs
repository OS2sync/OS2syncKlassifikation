using Microsoft.AspNet.OData;
using Microsoft.Extensions.DependencyInjection;
using StsKlassifikation.DBContext;
using StsKlassifikation.Model;
using System;
using System.Linq;

namespace StsKlassifikation.Controllers
{
    public class FacetterController : ODataController
    {
        private readonly ClassificationContext classificationContext;
        public FacetterController(IServiceProvider sp)
        {
            classificationContext = sp.GetService<ClassificationContext>();
        }

        public IQueryable<Facet> Get()
        {
            return classificationContext.Facet;
        }
    }
}