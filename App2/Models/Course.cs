using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace App2.Models
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int TermId { get; set; }
        public string ClassName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool CourseStatus { get; set; }
        public bool NotifyStart { get; set; }
        public bool NotifyEnd { get; set; }
        public string Notes { get; set; }
        public string InstructorName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public int AssessmentCount { get; set; }
    }
}
