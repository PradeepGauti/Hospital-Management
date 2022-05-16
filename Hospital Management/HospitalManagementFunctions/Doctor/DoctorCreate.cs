using Hospital_Management.Models;
using Hospital_Management.Models.Doctor;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management.HospitalManagementFunctions
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
            MyAzureFunctionResponse objResponse = new MyAzureFunctionResponse();
            ItemResponse<Doctor> objInsertResponse = null;
            log.LogInformation("Calling Azure Function -- DoctorCreate");

            // we are reading or parsing the input request body
            requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            objDoctorDetails = JsonConvert.DeserializeObject<Doctor>(requestBody);

            if (objDoctorDetails != null)
            {
                // initialising Azure Cosomosdb database connection.
                DoctorCosmoDBActivity objCosmosDBActivitiy = new DoctorCosmoDBActivity();
                await objCosmosDBActivitiy.InitiateConnection();

                // saving new doctor information.
                // System need to generate one unique value for "id" property while saving new items in container
                //objDoctorDetails.DoctorId = Guid.NewGuid().ToString();
                //objDoctorDetails.DoctorDate = DateTime.UtcNow;
                //DateTime d1 = new DateTime(2022);
                //DateTime d2 = objDoctorDetails.AddMonths(30);
                // saving new doctor information.
                // System need to generate one unique value for "id" property while saving new items in container
                objDoctorDetails.DoctorGuid = Guid.NewGuid().ToString();
                //var obj = await objCosmosDBActivitiy.GetAllDoctors();
                //var a = obj.OrderByDescending(p => p.PatientNames).Select(d => d.PatientNames);
                //Doctor insertResponse = await objCosmosDBActivitiy.objDoctorDetails;
                objInsertResponse = await objCosmosDBActivitiy.SaveNewDoctorItem(objDoctorDetails);
                if (objInsertResponse == null)
                {
                    objResponse.ErrorCode = 100;
                    objResponse.Message = $"Error occured while inserting information of doctor- {objDoctorDetails.DoctorName}";
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
                objResponse.Message = "Failed to read or extract Doctor information from Request body due to bad data.";
                log.LogInformation("Failed to read or extract Doctor information from Request body due to bad data.");
            }
            return new OkObjectResult(objResponse);
        }
    }
}
