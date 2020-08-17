using Microsoft.AspNetCore.Hosting;
using Topshelf;

namespace StsKlassifikation
{
    class Program
    {        
        static void Main(string[] args)
        {
            HostFactory.Run(h =>
            {
                h.Service<MainService>(sc =>
                {
                    sc.ConstructUsing(name => new MainService());
                    sc.WhenStarted(service => service.Start());
                    sc.WhenStopped(service => service.Stop());
                    sc.WhenShutdown(service => service.Stop());
                });

                h.RunAsLocalSystem();
                h.StartAutomaticallyDelayed();
                h.SetDescription("Synkroniserer data fra STS Klassifikation til en lokal SQL database");
                h.SetDisplayName("OS2sync Klassifikation");
                h.SetServiceName("OS2sync Klassifikation");
            });
        }
    }
}