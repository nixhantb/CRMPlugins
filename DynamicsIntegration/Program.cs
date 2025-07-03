using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace DynamicsIntegration
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Option 1: Interactive login - will prompt for MFA
            string connectionString = "AuthType=OAuth;Url=https://org5f44d0ab.crm5.dynamics.com;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=http://localhost;LoginPrompt=Always";

            CrmServiceClient serviceClient = new CrmServiceClient(connectionString);

            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
	                        <entity name='contact'>
		                        <attribute name='fullname'/>
		                        <attribute name='telephone1'/>
		                        <attribute name='contactid'/>
		                        <order attribute='fullname' descending='false'/>
		                        <filter type='and'>
			                        <condition attribute='statecode' operator='eq' value='0'/>
		                        </filter>
	                        </entity>
                        </fetch>";

            
            EntityCollection entityCollection = serviceClient.RetrieveMultiple(new FetchExpression(query));

            foreach(Entity contact in entityCollection.Entities)
            {
                Console.WriteLine(contact.Attributes["fullname"].ToString());
            }
            // Check if connection is successful
            if (serviceClient.IsReady)
            {
                try
                {
                    // Create a contact in Dynamics 365
                    Entity contact = new Entity("contact");
                    contact.Attributes.Add("lastname", "BanjadeBrother");

                    Guid guid = serviceClient.Create(contact);
                    Console.WriteLine("Contact created with ID: " + guid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error creating contact: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Failed to connect to Dynamics 365");
                Console.WriteLine("Last error: " + serviceClient.LastCrmError);
            }

            Console.ReadLine(); // Changed from Console.Read() to Console.ReadLine()
        }
    }
}