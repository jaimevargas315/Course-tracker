using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App2.Models;
using App2.Services;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace App2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        public Course course { get; set; }
        public Page1(Course selectedCourse)
        {
            course = selectedCourse;
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var assessments = await DatabaseService.GetAssessments(course.Id);

            Course_Name.Text = course.ClassName;
            Start_date.Date = course.StartDate;
            End_date.Date = course.EndDate;
            if (course.CourseStatus == true)
            {
                Course_status.SelectedItem = "Active";
            }
            else if (course.CourseStatus == false)
            {
                Course_status.SelectedItem = "Not Active";
            }
            if (course.NotifyStart == true)
            {
                Notify_start.IsChecked = true;
            }
            if (course.NotifyEnd == true)
            {
                Notify_end.IsChecked = true;
            }
            Notes_.Text = course.Notes;
            Instructor_name.Text = course.InstructorName;
            Phone_.Text = course.Phone;
            Email_.Text = course.Email;

            if (assessments.Count() == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (assessments.ElementAt(i).AssessmentType == "Objective")
                    {
                        Code1.Text = assessments.ElementAt(i).AssessmentCode;
                        Summary1.Text = assessments.ElementAt(i).Summary;
                        Objective_Button.IsEnabled = true;
                    }
                    if (assessments.ElementAt(i).AssessmentType == "Performance")
                    {
                        Code2.Text = assessments.ElementAt(i).AssessmentCode;
                        Summary2.Text = assessments.ElementAt(i).Summary;
                        Performance_Button.IsEnabled = true;
                    }
                }
            }

            if (assessments.Count() == 1)
            {
                if (assessments.ElementAt(0).AssessmentType == "Objective")
                {
                    Code1.Text = assessments.ElementAt(0).AssessmentCode;
                    Summary1.Text = assessments.ElementAt(0).Summary;
                    Objective_Button.IsEnabled = true;
                }
                if (assessments.ElementAt(0).AssessmentType == "Performance")
                {
                    Code2.Text = assessments.ElementAt(0).AssessmentCode;
                    Summary2.Text = assessments.ElementAt(0).Summary;
                    Performance_Button.IsEnabled = true;
                }
            }

        }

        public async void ObjectiveButton_Clicked(object sender, EventArgs e)
        {
            Page2 page2 = new Page2(course);
            page2.objective = true;
            await Navigation.PushAsync(page2);
            Navigation.InsertPageBefore(new Page1(course), page2);
            Navigation.RemovePage(this);

        }
        public async void PerformanceButton_Clicked(object sender, EventArgs e)
        {
            Page2 page2 = new Page2(course);
            page2.performance = true;
            await Navigation.PushAsync(page2);
            Navigation.InsertPageBefore(new Page1(course), page2);
            Navigation.RemovePage(this);
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Delete course", "Are you sure you want to delete this course?", "Yes", "No");
            if (answer)
            {
                await DatabaseService.RemoveCourse(course.Id);
                await Navigation.PopAsync();
            }
        }

        private async void EditCourseButton_Clicked(object sender, EventArgs e)
        {
            EditCourse x = new EditCourse(course);
            await Navigation.PushAsync(x);
            Navigation.InsertPageBefore(new Page1(course), x);
            Navigation.RemovePage(this);
        }

        private async void AddAssessmentButton_Clicked(object sender, EventArgs e)
        {
            if (course.AssessmentCount >= 2)
            {
                await DisplayAlert("Error", "Maximum number of assessments reached, unable to add any more.", "Okay");
            }
            else
            {
                NewAssessment x = new NewAssessment(course);
                await Navigation.PushAsync(x);
                Navigation.InsertPageBefore(new Page1(course), x);
                Navigation.RemovePage(this);
            }

        }
        private async void Share_pressed(object sender, EventArgs e)
        {
            try
            {
                var message = new SmsMessage("Notes for Class "+ Course_Name.Text + ": " +  Notes_.Text, "");
                await Sms.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException ex)
            {
                await DisplayAlert("Error", "Sms is not supported on this device.", "Okay");
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }
    }
}