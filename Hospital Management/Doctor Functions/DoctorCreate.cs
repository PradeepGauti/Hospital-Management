using Hospital_Management.Common;
using Hospital_Management.DoctorModels;
using Hospital_Management.PatientModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management.Doctor_Functions
{
    public static class DoctorCreate
    {
        private static object objDoctorDetails;

        [FunctionName("DoctorCreate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DoctorCreate")] HttpRequest req,
            ILogger log)
        {
            // we are initialised required variables 
            string requestBody = null;
            Doctor objDoctorDetails = null;
            MyAzureFunctionResponse objResponse = new DoctorModels.AzureFunctionResponse();
            ItemResponse<Doctor> objInsertResponse = null;
            log.LogInformation("Calling Azure Function -- DoctorCreate");

            // we are reading or parsing the input request body
            requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            objDoctorDetails = JsonConvert.DeserializeObject<Doctor>(requestBody);

            if (objDoctorDetails != null)
            {
                // initialising Azure Cosomosdb database connection.
                AzureCosmoDBActivity objCosmosDBActivitiy = new AzureCosmoDBActivity();
                await objCosmosDBActivitiy.InitiateConnection();

                // saving new student information.
                // System need to generate one unique value for "id" property while saving new items in container
                objDoctorDetails.DoctorGuid = Guid.NewGuid().ToString();

                objInsertResponse = await objCosmosDBActivitiy.SaveNewDoctorItem(objDoctorDetails);
                if (objInsertResponse == null)
                {
                    objResponse.ErrorCode = 100;
                    objResponse.Message = $"Error occured while inserting information of student- {objDoctorDetails.DoctorName}";
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
                objResponse.Message = "Failed to read or extract Student information from Request body due to bad data.";
                log.LogInformation("Failed to read or extract Student information from Request body due to bad data.");
            }
            return new OkObjectResult(objResponse);
        }
    }

    internal class MyAzureFunctionResponse
    {
        public int ErrorCode { get; internal set; }
        public string Message { get; internal set; }

        public static implicit operator MyAzureFunctionResponse(DoctorModels.AzureFunctionResponse v)
        {
            throw new NotImplementedException();
        }
    }
}
