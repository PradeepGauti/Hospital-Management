using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_Management.Models.Doctor
{
    public class Doctor
    {
        [JsonProperty("DoctorName")]
        public string DoctorName { get; set; }

        [JsonProperty("DoctorId")]
        public string DoctorId { get; set; }

        [JsonProperty("id")]
        public string DoctorGuid { get; set; }

        [JsonProperty("PatientNames")]
        public string PatientNames { get; set; }
        //public object PatientId { get; set; }
        //public DateTime DoctorDate { get; set; }
    }
    public class Doctors
    {
        public Doctors()
        {
            LstDoctor = new List<Doctor>();
        }
        public List<Doctor> LstDoctor { get; set; }
        //public string DoctorId { get; set; }
        //public object DoctorName { get; set; }

        //public static implicit operator Doctors(Doctor v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
