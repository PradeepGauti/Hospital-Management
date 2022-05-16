using Hospital_Management.DoctorModels;
using Hospital_Management.PatientModels;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management.Common
{
    public class AzureCosmoDBActivity
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://localhost:8081";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "AzureLearning";
        // private string containerId = "HospitalManagementContainer";
        private string containerId = "HospitalManagement";
        private Patient objPatient;

        public List<Doctor> DoctorId { get; private set; }
        public async Task InitiateConnection()
        {
            // Create a new instance of the Cosmos Client 
            //configuring Azure Cosmosdb sql api details
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            await CreateDoctorDatabaseAsync();
            await CreateDoctorContainerAsync();
        }

        private async Task CreateDoctorContainerAsync()
        {
            // Create a new container
            //this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/DoctorName");
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/DoctorId");
        }

        private async Task CreateDoctorDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }

        public async Task<ItemResponse<Doctor>> SaveNewDoctorItem(Doctor objDoctor)
        {
            ItemResponse<Doctor> doctorResponse = null;
            try
            {
                doctorResponse = await this.container.CreateItemAsync<Doctor>(objDoctor, new PartitionKey(objDoctor.DoctorId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return doctorResponse;
        }
        public async Task<ItemResponse<Doctor>> ModifyDoctorItem(Doctor objDoctor)
        {
            ItemResponse<Doctor> DoctorResponse = null;
            try
            {
                /* Note : Partition Key value should not change */
                DoctorResponse = await this.container.ReplaceItemAsync<Doctor>(objDoctor, objDoctor.DoctorGuid, new PartitionKey(objDoctor.DoctorId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return DoctorResponse;
        }
        public async Task<ItemResponse<Doctor>> GetDoctorItem(string DoctorId, int partionKey)
        {
            ItemResponse<Doctor> DoctorResponse = null;
            try
            {
                DoctorResponse = await this.container.ReadItemAsync<Doctor>(DoctorId, new PartitionKey(partionKey));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return DoctorResponse;
        }
        public async Task<List<Doctor>> GetAllDoctors()
        {
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Doctor> queryResultSetIterator = this.container.GetItemQueryIterator<Doctor>(queryDefinition);

            List<Doctor> lstDoctors = new List<Doctor>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Doctor> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                 lstDoctors = currentResultSet.Select(r => new Doctor()
                {
                    DoctorName=r.DoctorName,
                    DoctorGuid=r.DoctorGuid,
                    DoctorId=r.DoctorId
                }).ToList();

            }
            return lstDoctors;
        }
        public List<Patient> PatientId { get; private set; }
        private async Task CreatePatientContainerAsync()
        {
            // Create a new container
            //this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/PatientName");
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/PatientId");
        }
        private async Task CreatePatientDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }
        public async Task<ItemResponse<Patient>> SaveNewPatientItem(Patient objPatient)
        {
            ItemResponse<Patient> patientResponse = null;
            try
            {
                patientResponse = await this.container.CreateItemAsync<Patient>(objPatient, new PartitionKey(objPatient.PatientId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return patientResponse;
        }
        public async Task<ItemResponse<Patient>> ModifyPatientItem(Patient objPatient)
        {
            ItemResponse<Patient> PatientResponse = null;
            try
            {
                /* Note : Partition Key value should not change */
                PatientResponse = await this.container.ReplaceItemAsync<Patient>(objPatient, objPatient.PatientGuid, new PartitionKey(objPatient.PatientId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return PatientResponse;
        }
        public async Task<ItemResponse<Patient>> GetPatientItem(string PatientId, int partionKey)
        {
            ItemResponse<Patient> PatientResponse = null;
            try
            {
                PatientResponse = await this.container.ReadItemAsync<Patient>(PatientId, new PartitionKey(partionKey));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return PatientResponse;
        }
        public async Task<List<Patient>> GetAllPatients()
        {
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Patient> queryResultSetIterator = this.container.GetItemQueryIterator<Patient>(queryDefinition);

            List<Patient> lstPatients = new List<Patient>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Patient> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                lstPatients = currentResultSet.Select(r => new Patient()
                {
                    PatientName=r.PatientName,
                    PatientId=r.PatientId,
                    Age=r.Age,
                    DoctorName=r.DoctorName,
                    RXDate=r.RXDate,
                    PatientGuid=r.PatientGuid
                }).ToList();

            }
            return lstPatients;
        }
        /*internal Task<ItemResponse<Doctor>> GetDoctorItem(object doctorGUID, int partitionkey)
        {
            throw new NotImplementedException();
        }*/
    }
}
