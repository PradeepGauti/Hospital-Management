using Hospital_Management.Models;
using Hospital_Management.Models.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital_Management.HospitalManagementFunctions
{
    public static class PatientCreate
    {
        private static object objPatientDetails;

        [FunctionName("PatientCreate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PatientCreate")] HttpRequest req,
            ILogger log)
        {
            // we are initialised required variables 
            string requestBody = null;
            Patient objPatientDetails = null;
            MyAzureFunctionResponse objResponse = new MyAzureFunctionResponse();
            ItemResponse<Patient> objInsertResponse = null;
            log.LogInformation("Calling Azure Function -- PatientCreate");

            // we are reading or parsing the input request body
            requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            objPatientDetails = JsonConvert.DeserializeObject<Patient>(requestBody);

            if (objPatientDetails != null)
            {
                // initialising Azure Cosomosdb database connection.
                PatientCosmoDBActivity objCosmosDBActivitiy = new PatientCosmoDBActivity();
                await objCosmosDBActivitiy.InitiateConnection();

                // saving new patient information.
                // System need to generate one unique value for "id" property while saving new items in container
                objPatientDetails.PatientGuid = Guid.NewGuid().ToString();
                               
                objInsertResponse = await objCosmosDBActivitiy.SaveNewPatientItem(objPatientDetails);
                if (objInsertResponse == null)
                {
                    objResponse.ErrorCode = 100;
                    objResponse.Message = $"Error occured while inserting information of patients- {objPatientDetails.PatientName}";
                    log.LogInformation(objResponse.Message + "Error:" + objInsertResponse.StatusCode);
                }
                else
                {                    
                    objResponse.ErrorCode = 0;
                    objResponse.Message = "Successfully inserted information.";
                }

            }
            else
            {
                objResponse.ErrorCode = 100;
                objResponse.Message = "Failed to read or extract Patient information from Request body due to bad data.";
                log.LogInformation("Failed to read or extract Patient information from Request body due to bad data.");
            }
            return new OkObjectResult(objResponse);
        }
    }

    internal class MyAzureFunctionResponse
    {
        public int ErrorCode { get; internal set; }
        public string Message { get; internal set; }

        //public static implicit operator MyAzureFunctionResponse(MyAzureFunctionResponse v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}