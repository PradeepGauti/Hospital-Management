using Hospital_Management.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management.Models.Patient
{
    public class PatientCosmoDBActivity
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
        // private string containerId = "DoctorContainer";
        private string containerId = "PatientDetails";

        public List<Patient> PatientId { get; private set; }
        public Task<Patient> objPatientDetails { get; internal set; }

        public async Task InitiateConnection()
        {
            // Create a new instance of the Cosmos Client 
            //configuring Azure Cosmosdb sql api details
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            await CreateDatabaseAsync();
            await CreateContainerAsync();
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }


        /// <summary>
        /// Create the container if it does not exist. 
        /// Specifiy "/StudentName" as the partition key since we're storing Student information, to ensure good distribution of requests and storage.
        /// </summary>
        /// <returns></returns>
        private async Task CreateContainerAsync()
        {
            // Create a new container
            //this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/PatientName");
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/PatientId");
        }

        public async Task<ItemResponse<Patient>> SaveNewPatientItem(Patient objPatient)
        {
            ItemResponse<Patient> patientResponse = null;
            try
            {
                //  studentResponse = await this.container.CreateItemAsync<Student>(objStudent, new PartitionKey(objStudent.Name));
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
                // studentResponse = await this.container.ReplaceItemAsync<Student>(objStudent, objStudent.StudentId, new PartitionKey(objStudent.Name));
                PatientResponse = await this.container.ReplaceItemAsync<Patient>(objPatient, objPatient.PatientGuid, new PartitionKey(objPatient.PatientId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return PatientResponse;
        }


        public async Task<ItemResponse<Patient>> GetPatientItem(string PatientId, string partionKey)
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
            //var sqlQueryText = "SELECT * FROM c.id FROM Doctor c JOIN c.Patient";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Patient> queryResultSetIterator = this.container.GetItemQueryIterator<Patient>(queryDefinition);

            List<Patient> lstPatients = new List<Patient>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Patient> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                lstPatients = currentResultSet.Select(r => new Patient()
                {
                    PatientName = r.PatientName,
                    Age = r.Age,
                    DoctorName = r.DoctorName,
                    RXDate=r.RXDate,
                    PatientGuid=r.PatientGuid,
                    PatientId=r.PatientId
                }).ToList();

            }
            return lstPatients;
        }

        public async Task<string> GetPatientNamesByDoctorName(string doctorName)
        {
            var sqlQueryText = $"select * from c where c.DoctorName ='{doctorName}'";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Patient> queryResultSetIterator = this.container.GetItemQueryIterator<Patient>(queryDefinition);

            string lstPatients = string.Empty;

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Patient> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                lstPatients = string.Join(",",currentResultSet.Select(r => r.PatientName).ToList<string>());
            }
            return lstPatients;

        }
    }
}
