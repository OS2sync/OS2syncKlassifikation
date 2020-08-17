using KlassifikationSystem;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StsKlassifikation.DBContext;
using StsKlassifikation.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StsKlassifikation.Service
{
    class ClassificationService
    {
        private readonly ClassificationContext classificationContext;
        private readonly IConfiguration configuration;

        public ClassificationService(IServiceProvider sp)
        {
            classificationContext = sp.GetService<ClassificationContext>();
            configuration = sp.GetService<IConfiguration>();
        }

        public List<Klassifikation> GetAllClassifications()
        {
            KlassifikationSystemPortTypeClient port = new KlassifikationSystemPortTypeClient(configuration["KlassifikationService:serviceUrl"], configuration["certPath"], configuration["certPassword"]);

            RequestHeaderType requestHeaderType = new RequestHeaderType();
            requestHeaderType.TransactionUUID = "";

            FremsoegobjekthierarkiRequestType _fremsoegobjekthierarkiRequest = new FremsoegobjekthierarkiRequestType();
            _fremsoegobjekthierarkiRequest.AuthorityContext = new AuthorityContextType();
            _fremsoegobjekthierarkiRequest.AuthorityContext.MunicipalityCVR = configuration["cvr"];
            _fremsoegobjekthierarkiRequest.FremsoegObjekthierarkiInput = new FremsoegObjekthierarkiInputType();
            _fremsoegobjekthierarkiRequest.FremsoegObjekthierarkiInput.FacetSoegEgenskab = new EgenskabType2();
            _fremsoegobjekthierarkiRequest.FremsoegObjekthierarkiInput.FacetSoegEgenskab.BrugervendtNoegleTekst = "XXXX";

            fremsoegobjekthierarkiResponse response = port.fremsoegobjekthierarkiAsync(null, _fremsoegobjekthierarkiRequest).Result;

            List<Klassifikation> classifications = ResponseToModel(response);
            return classifications;
        }

        private List<Klassifikation> ResponseToModel(fremsoegobjekthierarkiResponse response)
        {
            List<Klassifikation> result = new List<Klassifikation>();
            var classificationDTOs = response.FremsoegobjekthierarkiResponse.FremsoegObjekthierarkiOutput.Klassifikationer;
            foreach (var dto in classificationDTOs)
            {
                Klassifikation classification = new Klassifikation();
                classification.UUID = dto.ObjektType.UUIDIdentifikator;
                classification.Titel = dto.Registrering?[0]?.AttributListe?.Egenskab[0].TitelTekst;
                classification.BrugervendtNoegle = dto.Registrering?[0]?.AttributListe?.Egenskab[0].BrugervendtNoegleTekst;
                classification.Beskrivelse = dto.Registrering?[0]?.AttributListe?.Egenskab[0].BeskrivelseTekst;
                classification.Livscykluskode = dto.Registrering?[0]?.LivscyklusKode;
                classification.Timestamp = dto.Registrering?[0]?.Tidspunkt;
                classification.Synkroniser = false;
                classification.Ansvarlig = dto.Registrering?[0]?.RelationListe.Ansvarlig?.ReferenceID.Item;
                classification.Ejer = dto.Registrering?[0]?.RelationListe.Ejer?.ReferenceID.Item;
                classification.Publiceret = dto.Registrering?[0]?.TilstandListe?.PubliceretStatus?[0]?.ErPubliceretIndikator;

                result.Add(classification);
            }

            return result;
        }

        public void UpdateClassifications(List<Klassifikation> classifications)
        {
            foreach (var classification in classifications)
            {
                Klassifikation result = classificationContext.Klassifikation.SingleOrDefault(c => c.UUID.Equals(classification.UUID));
                if (result != null)
                {
                    result.Beskrivelse = classification.Beskrivelse;
                    result.Livscykluskode = classification.Livscykluskode;
                    result.Ejer = classification.Ejer;
                    result.Ansvarlig = classification.Ansvarlig;
                    result.BrugervendtNoegle = classification.BrugervendtNoegle;
                    result.Publiceret = classification.Publiceret;
                    result.Timestamp = classification.Timestamp;
                    result.Titel = classification.Titel;
                }
                else
                {
                    classificationContext.Klassifikation.Add(classification);
                }
            }

            classificationContext.SaveChanges();
        }
    }
}
