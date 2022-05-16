using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management.Models.Patient
{
    public static class AllPatients
    {
        [FunctionName("AllPatients")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "AllPatients")] HttpRequest req,
            ILogger log)
        {
            //declaring the response
            List<Patient> lstPatients = null;
            log.LogInformation("Calling Azure Function -- PatientGetId");
            // initialising Azure Cosomosdb database connection.
            PatientCosmoDBActivity objCosmosDBActivitiy = new PatientCosmoDBActivity();
            await objCosmosDBActivitiy.InitiateConnection();
            // retriving existing student information based on patientGuid and partition key i.e. PatientId value
            lstPatients = await objCosmosDBActivitiy.GetAllPatients();
            return new OkObjectResult(lstPatients);
        }
    }
}
