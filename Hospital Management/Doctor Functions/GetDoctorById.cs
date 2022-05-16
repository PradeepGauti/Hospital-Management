using Hospital_Management.Common;
using Hospital_Management.DoctorModels;
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

namespace Hospital_Management.Doctor_Functions
{
    public static class GetDoctorById
    {
        [FunctionName("GetDoctorById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetDoctorById/{doctorGUID}/{partitionkey}")] HttpRequest req,
            string doctorGUID, int partitionkey,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function - GetDoctorById.");

            ItemResponse<Doctor> objDoctorResponse = null;
            log.LogInformation("Calling Azure Function -- GetDoctorById");
            // initialising Azure Cosomosdb database connection.
            AzureCosmoDBActivity objCosmosDBActivitiy = new AzureCosmoDBActivity();
            await objCosmosDBActivitiy.InitiateConnection();
            // retriving existing employee information based on doctor GUId and partition key i.e. doctorId value
            objDoctorResponse = await objCosmosDBActivitiy.GetDoctorItem(doctorGUID, partitionkey);
            return new OkObjectResult(objDoctorResponse.Resource);
        }
    }
}
