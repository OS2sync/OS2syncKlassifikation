using Microsoft.Extensions.Logging;
using Quartz;
using StsKlassifikation.DBContext;
using StsKlassifikation.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace StsKlassifikation
{
    [DisallowConcurrentExecution]
    class SyncJob : IJob
    {
        private readonly ILogger logger;
        private readonly ClassificationService classificationService;
        private readonly FacetService facetService;
        private readonly KlasseService klasseService;
        private readonly ClassificationContext classificationContext;

        public SyncJob(IServiceProvider sp)
        {
            logger = sp.GetService<ILogger<SyncJob>>();
            classificationService = sp.GetService<ClassificationService>();
            facetService = sp.GetService<FacetService>();
            klasseService = sp.GetService<KlasseService>();
            classificationContext = sp.GetService<ClassificationContext>();
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                logger.LogInformation("Synkroniserer klassifikationer");

                var classifications = classificationService.GetAllClassifications();
                classificationService.UpdateClassifications(classifications);

                foreach (var classification in classificationContext.Klassifikation.ToList())
                {
                    if (classification.Synkroniser)
                    {
                        logger.LogInformation("Synkroniserer: " + classification.Titel);

                        var facets = facetService.GetFacets(classification.UUID);
                        facetService.UpdateFacets(facets);

                        foreach (var facet in classificationContext.Facet.Where(f => f.Klassifikation.UUID.Equals(classification.UUID)).ToList())
                        {
                            var classes = klasseService.GetAllKlasse(facet.UUID);
                            klasseService.UpdateKlasse(classes);
                        }
                    }
                }

                logger.LogInformation("Synkronisering færdig");
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to execute SyncJob");
                return Task.CompletedTask;
            }
        }
    }
}
