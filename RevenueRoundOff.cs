

using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System;

namespace CRM
{
    public class RevenueRoundOff : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                

                // Obtain the IOrganizationService instance which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                // Obtain the target entity from the input parameters.  
                Entity account = (Entity)context.InputParameters["Target"];
                try
                {
                    tracingService.Trace($"Plugin Depth: {context.Depth}");

                    if (context.Depth > 1)
                    {
                        tracingService.Trace("Exiting due to recursive call.");
                        return;
                    }

                    if (account.Attributes.Contains("revenue") && account.Attributes["revenue"] != null)
                    {
                        decimal revenue = ((Money)account.Attributes["revenue"]).Value;
                        revenue = Math.Round(revenue, 1);

                        tracingService.Trace($"Rounded revenue: {revenue}");

                        account.Attributes["revenue"] = new Money(revenue);
                    }
                    else
                    {
                        tracingService.Trace("Revenue attribute is missing or null.");
                    }

                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
