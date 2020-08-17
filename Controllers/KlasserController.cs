using Microsoft.AspNet.OData;
using Microsoft.Extensions.DependencyInjection;
using StsKlassifikation.DBContext;
using StsKlassifikation.Model;
using System;
using System.Linq;

namespace StsKlassifikation.Controllers
{
    public class KlasserController : ODataController
    {
        private readonly ClassificationContext classificationContext;
        public KlasserController(IServiceProvider sp)
        {
            classificationContext = sp.GetService<ClassificationContext>();
        }

        public IQueryable<Klasse> Get()
        {
            return classificationContext.Klasse;
        }
    }
}