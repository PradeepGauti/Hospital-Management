using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Hospital_Management.Models.Doctor;
using Hospital_Management.Models.Patient;

namespace Hospital_Management.HospitalManagementFunctions
{
    public static class GetDoctorById
    {
        [FunctionName("GetDoctorById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetDoctorById/{doctorGUID}/{partitionkey}")] HttpRequest req,
            string DoctorGUID, string partitionkey,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function - GetDoctorById.");

            ItemResponse<Doctor> objDoctorResponse = null;
            log.LogInformation("Calling Azure Function -- GetDoctorById");
            // initialising Azure Cosomosdb database connection.
            DoctorCosmoDBActivity objCosmosDBActivitiy = new DoctorCosmoDBActivity();
            await objCosmosDBActivitiy.InitiateConnection();
            // retriving existing doctor information based on doctor GUId and partition key i.e. doctorId value
            objDoctorResponse = await objCosmosDBActivitiy.GetDoctorItem(DoctorGUID, partitionkey);
            if(objDoctorResponse != null && objDoctorResponse.Resource != null)
            {
                PatientCosmoDBActivity objPatientCosmosDBActivitiy = new PatientCosmoDBActivity();
                await objPatientCosmosDBActivitiy.InitiateConnection();
                objDoctorResponse.Resource.PatientNames = await objPatientCosmosDBActivitiy.GetPatientNamesByDoctorName(objDoctorResponse.Resource.DoctorName);
            }
           
            return new OkObjectResult(objDoctorResponse.Resource);
        }
    }
}
