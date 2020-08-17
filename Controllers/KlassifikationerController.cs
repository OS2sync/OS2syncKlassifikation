using Microsoft.AspNet.OData;
using Microsoft.Extensions.DependencyInjection;
using StsKlassifikation.DBContext;
using StsKlassifikation.Model;
using System;
using System.Linq;

namespace StsKlassifikation.Controllers
{
    public class KlassifikationerController : ODataController
    {
        private readonly ClassificationContext classificationContext;
        public KlassifikationerController(IServiceProvider sp)
        {
            classificationContext = sp.GetService<ClassificationContext>();
        }

        public IQueryable<Klassifikation> Get()
        {
            return classificationContext.Klassifikation;
        }
    }
}