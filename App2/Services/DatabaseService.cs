using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SQLite;
using App2.Models;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace App2.Services
{
    class DatabaseService
    {
        private static SQLiteAsyncConnection _db;
        private static SQLiteConnection _dbConnection;
        static async Task Init()
        { 
            if(_db != null)
            {
                return;
            }
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Gadgets.db");

            _db = new SQLiteAsyncConnection(databasePath);
            _dbConnection = new SQLiteConnection(databasePath);

            await _db.CreateTableAsync<Assessment>();
            await _db.CreateTableAsync<Course>();
            await _db.CreateTableAsync<Term>();
        
         }
#region terms
        public static async Task AddTerm(string title, DateTime startDate, DateTime endDate)
        {
            await Init();
            var term = new Term()
            {
                Title = title,
                StartDate = startDate,
                EndDate = endDate,
                CourseCount = 0
            };
            await _db.InsertAsync(term);
        }

        public static async Task RemoveTerm(int id)
        {
            await Init();
            var courses = await GetCourses(id);
            foreach(Course x in courses)
            {
                var assessments = await GetAssessments(x.Id);
                foreach(Assessment y in assessments)
                {
                    await _db.DeleteAsync<Assessment>(y.Id);
                }
                await _db.DeleteAsync<Course>(x.Id);
            }
            await _db.DeleteAsync<Term>(id);
        }

        public static async Task<IEnumerable<Term>> GetTerms()
        {
            await Init();
            var terms = await _db.Table<Term>().ToListAsync();
            return terms;
        }

        public static async Task<Term> GetTerm(int id)
        {
            await Init();
            var termQuery = await _db.Table<Term>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();
            return termQuery;
        }

        public static async Task UpdateTerm(int id, string title, DateTime startDate, DateTime endDate, int courseCount)
        {
            await Init();
            var termQuery = await _db.Table<Term>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();

            if (termQuery != null)
            {
                termQuery.Title = title;
                termQuery.StartDate = startDate;
                termQuery.EndDate = endDate;
                termQuery.CourseCount = courseCount;

                await _db.UpdateAsync(termQuery);
            }
        }

        public static async Task AddCourse(int termId, string className, DateTime startDate, DateTime endDate,
                                           bool courseStatus, bool notifyStart, bool notifyEnd, string notes,
                                           string instructorName, string phone, string email)
        {
            await Init();
            var course = new Course()
            {
                TermId = termId,
                ClassName = className,
                StartDate = startDate,
                EndDate = endDate,
                CourseStatus = courseStatus,
                NotifyStart = notifyStart,
                NotifyEnd = notifyEnd,
                Notes = notes,
                InstructorName = instructorName,
                Phone = phone,
                Email = email
            };
            var term = await GetTerm(termId);
            term.CourseCount += 1;
            await UpdateTerm(term.Id, term.Title, term.StartDate, term.EndDate, term.CourseCount);
            await _db.InsertAsync(course);
        }
        #endregion
        #region course 


        public static async Task RemoveCourse(int id)
        {
            await Init();
            var assessments = await GetAssessments(id);
            foreach (Assessment y in assessments)
            {
                await _db.DeleteAsync<Assessment>(y.Id);
            }

            var course = await GetCourse(id);
            var term = await GetTerm(course.TermId);
            term.CourseCount += -1;
            await UpdateTerm(term.Id, term.Title, term.StartDate, term.EndDate, term.CourseCount);
            await _db.DeleteAsync<Course>(id);

        }
        public static async Task UpdateCourse(int id,int termId, string className, DateTime startDate, DateTime endDate, bool courseStatus,
            bool notifyStart, bool notifyEnd, string notes, string instructorName, string phone, string email,int assessmentCount)
        {
            await Init();
            var courseQuery = await _db.Table<Course>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();

            if (courseQuery != null)
            {
                courseQuery.TermId = termId;
                courseQuery.ClassName = className;
                courseQuery.StartDate = startDate;
                courseQuery.EndDate = endDate;
                courseQuery.CourseStatus = courseStatus;
                courseQuery.NotifyStart = notifyStart;
                courseQuery.NotifyEnd = notifyEnd;
                courseQuery.Notes = notes;
                courseQuery.InstructorName = instructorName;
                courseQuery.Phone = phone;
                courseQuery.Email = email;
                courseQuery.AssessmentCount = assessmentCount;

                await _db.UpdateAsync(courseQuery);
            }
        }

        public static async Task<IEnumerable<Course>> GetCourses(int id)
        {
            await Init();
            var courses = await _db.Table<Course>()
                .Where(i => i.TermId == id)
                .ToArrayAsync();
            return courses;
        }

        public static async Task<IEnumerable<Course>> GetCourses()
        {
            await Init();
            var courses = await _db.Table<Course>()
                .ToArrayAsync();
            return courses;
        }

        public static async Task<Course> GetCourse(int id)
        {
            await Init();
            var courseQuery = await _db.Table<Course>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();
            return courseQuery;

        }
        #endregion

        #region Assessment
        public static async Task AddAssessment(int courseId, string assessmentType, string assessmentCode, DateTime startDate,
            DateTime endDate, bool notifyStart, bool notifyend, string summary, string objectives)
        {
            await Init();
            var assessment = new Assessment()
            {
                CourseId = courseId,
                AssessmentType = assessmentType,
                AssessmentCode = assessmentCode,
                StartDate = startDate,
                EndDate = endDate,
                NotifyStart = notifyStart,
                NotifyEnd = notifyend,
                Summary = summary,
                Objectives = objectives
            };
            var course = await GetCourse(courseId);
            course.AssessmentCount += 1;
            await UpdateCourse(course.Id,course.TermId,course.ClassName,course.StartDate,course.EndDate,course.CourseStatus,
                               course.NotifyStart,course.NotifyEnd,course.Notes,course.InstructorName,course.Phone,course.Email,course.AssessmentCount);
            await _db.InsertAsync(assessment);
        }

        public static async Task RemoveAssessment(int id, int courseId)
        {
            await Init();
            var course = await GetCourse(courseId);
            course.AssessmentCount -= 1;
            await UpdateCourse(course.Id, course.TermId, course.ClassName, course.StartDate, course.EndDate, course.CourseStatus,
                               course.NotifyStart, course.NotifyEnd, course.Notes, course.InstructorName, course.Phone, course.Email, course.AssessmentCount);
            await _db.DeleteAsync<Assessment>(id);
        }

        public static async Task UpdateAssessment(int courseId, int id, string assessmentType, string assessmentCode, DateTime startDate,
            DateTime endDate, bool notifyStart, bool notifyEnd, string summary, string objectives)
        {
            await Init();
            var assessmentQuery = await _db.Table<Assessment>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();

            if (assessmentQuery != null)
            {
                assessmentQuery.CourseId = courseId;
                assessmentQuery.AssessmentType = assessmentType;
                assessmentQuery.AssessmentCode = assessmentCode;
                assessmentQuery.StartDate = startDate;
                assessmentQuery.EndDate = endDate;
                assessmentQuery.NotifyStart = notifyStart;
                assessmentQuery.NotifyEnd = notifyEnd;
                assessmentQuery.Summary = summary;
                assessmentQuery.Objectives = objectives;

                await _db.UpdateAsync(assessmentQuery);
            }
        }

        public static async Task<IEnumerable<Assessment>> GetAssessments(int id)
        {
            await Init();
            var assessments = await _db.Table<Assessment>()
                .Where(i => i.CourseId == id)
                .ToArrayAsync();
            return assessments;
        }

        public static async Task<IEnumerable<Assessment>> GetAssessments()
        {
            await Init();
            var assessments = await _db.Table<Assessment>()
                .ToArrayAsync();
            return assessments;
        }
        #endregion

        #region DemoData
        public static async void LoadSampleData()
        {
            await Init();
            Term term = new Term
            {
                Title = "meow Term",
                StartDate = DateTime.Parse("01/01/0001"),
                EndDate = DateTime.Parse("01/01/0001"),
                CourseCount = 0
            };
            await AddTerm(term.Title,term.StartDate,term.EndDate);

            Course course1 = new Course
            {
                TermId = term.Id,
                ClassName = "Sample Class",
                StartDate = DateTime.Parse("01/01/0001"),
                EndDate = DateTime.Parse("01/01/0001"),
                CourseStatus = true,
                NotifyStart = true,
                NotifyEnd = true,
                Notes = "NOTES NOTES NOTES NOTES NOTES NOTES NOTES",
                InstructorName = "John Doe",
                Phone = "000-000-0000",
                Email = "1234@wgu.edu",
                AssessmentCount = 0
            };
            await AddCourse(course1.TermId, course1.ClassName, course1.StartDate, course1.EndDate,course1.CourseStatus,
            course1.NotifyStart, course1.NotifyEnd, course1.Notes, course1.InstructorName, course1.Phone, course1.Email);
            Course course2 = new Course
            {
                TermId = term.Id,
                ClassName = "Sample Class",
                StartDate = DateTime.Parse("01/01/0001"),
                EndDate = DateTime.Parse("01/01/0001"),
                CourseStatus = true,
                NotifyStart = true,
                NotifyEnd = true,
                Notes = "NOTES NOTES NOTES NOTES NOTES NOTES NOTES",
                InstructorName = "John Doe",
                Phone = "000-000-0000",
                Email = "1234@wgu.edu",
                AssessmentCount =0
                
            };
            await AddCourse(course2.TermId, course2.ClassName, course2.StartDate, course2.EndDate, course2.CourseStatus,
            course2.NotifyStart, course2.NotifyEnd, course2.Notes, course2.InstructorName, course2.Phone, course2.Email);
            Assessment assessment1 = new Assessment
            {
                CourseId = course1.Id,
                AssessmentType = "Objective",
                AssessmentCode = "Code1",
                StartDate = DateTime.Parse("01/01/0001"),
                EndDate = DateTime.Parse("01/01/0001"),
                NotifyStart = true,
                NotifyEnd = true,
                Summary = "SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY ",
                Objectives = "OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES "

            };
            await AddAssessment(assessment1.CourseId, assessment1.AssessmentType, assessment1.AssessmentCode, assessment1.StartDate,
                assessment1.EndDate, assessment1.NotifyStart, assessment1.NotifyEnd, assessment1.Summary, assessment1.Objectives);
            Assessment assessment2 = new Assessment
            {
                CourseId = course1.Id,
                AssessmentType = "Objective",
                AssessmentCode = "Code2",
                StartDate = DateTime.Parse("01/01/0001"),
                EndDate = DateTime.Parse("01/01/0001"),
                NotifyStart = true,
                NotifyEnd = true,
                Summary = "SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY ",
                Objectives = "OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES "

            };
            await AddAssessment(assessment2.CourseId, assessment2.AssessmentType, assessment2.AssessmentCode, assessment2.StartDate,
                            assessment2.EndDate, assessment2.NotifyStart, assessment2.NotifyEnd, assessment2.Summary, assessment2.Objectives);
            Assessment assessment3 = new Assessment
            {
                CourseId = course2.Id,
                AssessmentType = "Objective",
                AssessmentCode = "Code1",
                StartDate = DateTime.Parse("01/01/0001"),
                EndDate = DateTime.Parse("01/01/0001"),
                NotifyStart = true,
                NotifyEnd = true,
                Summary = "SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY ",
                Objectives = "OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES "

            };
            await AddAssessment(assessment3.CourseId, assessment3.AssessmentType, assessment3.AssessmentCode, assessment3.StartDate,
                            assessment3.EndDate, assessment3.NotifyStart, assessment3.NotifyEnd, assessment3.Summary, assessment3.Objectives);
            Assessment assessment4 = new Assessment
            {
                CourseId = course2.Id,
                AssessmentType = "Objective",
                AssessmentCode = "Code2",
                StartDate = DateTime.Parse("01/01/0001"),
                EndDate = DateTime.Parse("01/01/0001"),
                NotifyStart = true,
                NotifyEnd = true,
                Summary = "SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY SUMMARY ",
                Objectives = "OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES OBJECTIVES "

            };
            await AddAssessment(assessment4.CourseId, assessment4.AssessmentType, assessment4.AssessmentCode, assessment4.StartDate,
                            assessment4.EndDate, assessment4.NotifyStart, assessment4.NotifyEnd, assessment4.Summary, assessment4.Objectives);
        }
        public static async void ClearSampleData()
        {
            await Init();
            await _db.DropTableAsync<Assessment>();
            await _db.DropTableAsync<Course>();
            await _db.DropTableAsync<Term>();

            _db = null;
            _dbConnection = null;
        }
        #endregion

    }
}
