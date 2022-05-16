using Hospital_Management.Common;
using Hospital_Management.PatientModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management.Patient_Functions
{
    public static class GetPatientById
    {
        [FunctionName("GetPatientById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetPatientById/{patientGUID}/{partitionkey}")] HttpRequest req,
            string patientGUID, int partitionkey,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function - GetPatientById.");

            ItemResponse<Patient> objPatientResponse = null;
            log.LogInformation("Calling Azure Function -- GetPatientById");
            // initialising Azure Cosomosdb database connection.
            AzureCosmoDBActivity objCosmosDBActivitiy = new AzureCosmoDBActivity();
            await objCosmosDBActivitiy.InitiateConnection();
            // retriving existing employee information based on patient GUId and partition key i.e. patientId value
            objPatientResponse = await objCosmosDBActivitiy.GetPatientItem(patientGUID, partitionkey);
            return new OkObjectResult(objPatientResponse.Resource);
        }
    }
}
