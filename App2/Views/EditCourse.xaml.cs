using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App2.Models;
using App2.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCourse : ContentPage
    {
        public Course course { get; set; }
        public EditCourse(Course selectedCourse)
        {
            course = selectedCourse;
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            Class_name.Text = course.ClassName;
            Class_name_2.Text = course.ClassName;
            instructor_name.Text = course.InstructorName;
            email_.Text = course.Email;
            phone_.Text = course.Phone;
            start_date.Date = course.StartDate;
            end_date.Date = course.EndDate;
            if (course.CourseStatus == true)
                course_status.SelectedItem = "Active";
            if (course.CourseStatus == false)
                course_status.SelectedItem = "Not Active";
            if (course.NotifyStart == true)
                notify_start.IsChecked = true;
            if (course.NotifyEnd == true)
                notify_end.IsChecked = true;
            notes_.Text = course.Notes;

        }
        //public static async Task UpdateCourse(int id, int termId, string className, DateTime startDate, DateTime endDate, bool courseStatus,
        //bool notifyStart, bool notifyEnd, string notes, string instructorName, string phone, string email)
        async private void Submit_Clicked(object sender, EventArgs e)
        {
            bool courseStatus = false;
            if (course_status.SelectedItem.ToString() == "Active")
                courseStatus = true;
            await DatabaseService.UpdateCourse(course.Id, course.TermId, Class_name_2.Text, start_date.Date, end_date.Date, courseStatus,
                                               notify_start.IsChecked, notify_end.IsChecked, notes_.Text, instructor_name.Text, email_.Text, phone_.Text, course.AssessmentCount);
            var courses = await DatabaseService.GetCourses(course.TermId);
            foreach (Course x in courses)
            {
                if (x.Id == course.Id)
                {
                    course = x;
                    await Navigation.PushAsync(new Page1(course));
                    Navigation.RemovePage(this);
                }
            }
        }
    }
}