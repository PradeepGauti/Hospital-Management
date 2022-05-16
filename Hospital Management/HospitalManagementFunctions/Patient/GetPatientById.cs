using Hospital_Management.Models.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;
namespace Hospital_Management.HospitalManagementFunctions
{
    public static class GetPatientById
    {
        [FunctionName("GetPatientById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetPatientById/{patientGUID}/{partitionkey}")] HttpRequest req,
            string patientGUID, string partitionkey,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function - GetPatientById.");

            ItemResponse<Patient> objPatientResponse = null;
            log.LogInformation("Calling Azure Function -- GetPatientById");
            // initialising Azure Cosomosdb database connection.
            PatientCosmoDBActivity objCosmosDBActivitiy = new PatientCosmoDBActivity();
            await objCosmosDBActivitiy.InitiateConnection();
            // retriving existing patient information based on patient GUId and partition key i.e. patientId value
            objPatientResponse = await objCosmosDBActivitiy.GetPatientItem(patientGUID, partitionkey);

            if (objPatientResponse != null && objPatientResponse.Resource != null)
            {
                DateTime todaysDate = DateTime.Now, runningDate = DateTime.Now, rxDate;
                DateTime.TryParseExact(objPatientResponse.Resource.RXDate, "yyyy-MM-dd", null, DateTimeStyles.None, out rxDate);
                string strOrderDates = string.Empty;
                if (rxDate != null && rxDate >= runningDate)
                {
                    while (1 == 1)
                    {
                        strOrderDates = strOrderDates + runningDate.ToString("yyyy-MM-dd") + ",";
                        if (runningDate.Month < rxDate.Month && runningDate.Year <= rxDate.Year)
                        {
                            runningDate = runningDate.AddMonths(1);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (strOrderDates.EndsWith(","))
                    {
                        strOrderDates = strOrderDates.Substring(0, (strOrderDates.Length) - 1);
                    }
                    if (strOrderDates.StartsWith(","))
                    {
                        // removing comma from begining of result ie. removing first character.
                        strOrderDates = strOrderDates.Substring(1);
                    }
                }
                objPatientResponse.Resource.Orders = strOrderDates;
            }
            return new OkObjectResult(objPatientResponse.Resource);
        }
    }
}
