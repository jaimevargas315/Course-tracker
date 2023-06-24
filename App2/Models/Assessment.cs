using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace App2.Models
{
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string AssessmentType { get; set; }
        public string AssessmentCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool NotifyStart { get; set; }
        public bool NotifyEnd { get; set; }
        public string Summary { get; set; }

        public string Objectives { get; set; }
    }
}
