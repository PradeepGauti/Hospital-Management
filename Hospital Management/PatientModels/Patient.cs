using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_Management.PatientModels
{
    public class Patient
    {
        [JsonProperty("PatientName")]
        public string PatientName { get; set; }
        [JsonProperty("PatientId")]
        public string PatientId { get; set; }
        [JsonProperty("Age")]
        public string Age { get; set; }
        [JsonProperty("DoctorName")]
        public string DoctorName { get; set; }
        [JsonProperty("RXDate")]
        public string RXDate { get; set; }
        [JsonProperty("id")]
        public string PatientGuid { get; set; }
    }
    public class Patients
    {
        public Patients()
        {
            LstPatient = new List<Patient>();
        }
        public List<Patient> LstPatient { get; set; }
    }
}