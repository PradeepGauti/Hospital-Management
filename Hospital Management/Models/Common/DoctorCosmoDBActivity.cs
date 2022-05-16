using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management.Models.Doctor
{
    public class DoctorCosmoDBActivity
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
        private string containerId = "DoctorDetails";

        public List<Doctor> DoctorId { get; private set; }
        public Task<Doctor> objDoctorDetails { get; internal set; }

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
            //this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/DoctorName");
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/DoctorId");
        }

        public async Task<ItemResponse<Doctor>> SaveNewDoctorItem(Doctor objDoctor)
        {
            ItemResponse<Doctor> doctorResponse = null;
            try
            {
                //  studentResponse = await this.container.CreateItemAsync<Student>(objStudent, new PartitionKey(objStudent.Name));
                doctorResponse = await this.container.CreateItemAsync<Doctor>(objDoctor, new PartitionKey(objDoctor.DoctorId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return doctorResponse;
        }

        //internal Task<ItemResponse<Doctors>> SaveNewDoctorItem(Doctors objDoctorDetails)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<ItemResponse<Doctor>> ModifyDoctorItem(Doctor objDoctor)
        {
            ItemResponse<Doctor> DoctorResponse = null;
            try
            {
                /* Note : Partition Key value should not change */
                // studentResponse = await this.container.ReplaceItemAsync<Student>(objStudent, objStudent.StudentId, new PartitionKey(objStudent.Name));
                DoctorResponse = await this.container.ReplaceItemAsync<Doctor>(objDoctor, objDoctor.DoctorGuid, new PartitionKey(objDoctor.DoctorId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return DoctorResponse;
        }


        public async Task<ItemResponse<Doctor>> GetDoctorItem(string DoctorId, string partionKey)
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
            //var sqlQueryText = "SELECT * FROM c.id FROM Doctor c JOIN c.Patient";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Doctor> queryResultSetIterator = this.container.GetItemQueryIterator<Doctor>(queryDefinition);

            List<Doctor> lstDoctors = new List<Doctor>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Doctor> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                lstDoctors = currentResultSet.Select(r => new Doctor()
                {
                    DoctorId = r.DoctorId,
                    DoctorName =r.DoctorName,
                    PatientNames=r.PatientNames,
                    DoctorGuid=r.DoctorGuid,
                    
                }).ToList();

            }
            return lstDoctors;
        }
    }
}
