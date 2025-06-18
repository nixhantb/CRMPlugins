using Microsoft.Xrm.Sdk;
using System;

using System.ServiceModel;


namespace CRM
{
    public class TaskPlugin : IPlugin
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
                // Obtain the target entity from the input parameters.  
                Entity contact = (Entity)context.InputParameters["Target"];

                // Obtain the IOrganizationService instance which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // Plug-in business logic goes here.  
                    // logical name of the task

                    Entity taskRecord = new Entity("task");
                    // subject
                    taskRecord.Attributes.Add("subject", "Follow Up");
                    // Description
                    taskRecord.Attributes.Add("description", "Please Follow up with contact");
                    // Due 
                    taskRecord.Attributes.Add("scheduledend", DateTime.Now.AddDays(2));

                    // priority code 
                    taskRecord.Attributes.Add("prioritycode", new OptionSetValue(2));  
                    // regarding object
                    // parent record look up
                   // taskRecord.Attributes.Add("regardingobjectid", new EntityReference("contact", contact.Id));
                    taskRecord.Attributes.Add("regardingobjectid", contact.ToEntityReference());
                    // owner id
                    Guid taskGuid = service.Create(taskRecord);


                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
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
