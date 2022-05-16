using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_Management.DoctorModels
{
    public class Doctor
    {
        [JsonProperty("DoctorName")]
        public string DoctorName { get; set; }
        [JsonProperty("DoctorId")]
        public int DoctorId { get; set; }
        [JsonProperty("id")]
        public string DoctorGuid { get; set; }
    }
    public class Doctors
    {
        public Doctors()
        {
            LstDoctor = new List<Doctor>();
        }
        public List<Doctor> LstDoctor { get; set; }
    }
}