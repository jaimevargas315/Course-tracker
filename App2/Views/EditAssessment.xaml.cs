using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App2.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App2.Services;

namespace App2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAssessment : ContentPage

    {
        Assessment assessment { get; set; }
        Course course { get; set; }
        public EditAssessment(Assessment Assessment, Course Course)
        {
            InitializeComponent();
            assessment = Assessment;
            course = Course;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            Class_name.Text = course.ClassName;
            Assessment_code.Text = assessment.AssessmentCode;
            if (assessment.AssessmentType == "Objective")
                Assessment_type.SelectedItem = "Objective";
            if (assessment.AssessmentType == "Performance")
                Assessment_type.SelectedItem = "Performance";
            Start_Date.Date = assessment.StartDate;
            End_Date.Date = assessment.EndDate;
            if (assessment.NotifyStart == true)
                Notify_start.IsChecked = true;
            if (assessment.NotifyEnd == true)
                Notify_end.IsChecked = true;
            Summary_.Text = assessment.Summary;
            Objectives_.Text = assessment.Objectives;
        }

        private async void Submit_Clicked(object sender, EventArgs e)
        {
            if (Assessment_type.SelectedItem.ToString() == "Objective" && course.AssessmentCount == 2 && assessment.AssessmentType != "Objective")
                await DisplayAlert("Error", "Only one of each assessment type is allowed", "Okay");
            else if (Assessment_type.SelectedItem.ToString() == "Performance" && course.AssessmentCount == 2 && assessment.AssessmentType != "Performance")
                await DisplayAlert("Error", "Only one of each assessment type is allowed", "Okay");
            else
            {
                await DatabaseService.UpdateAssessment(course.Id, assessment.Id, Assessment_type.SelectedItem.ToString(), Assessment_code.Text, Start_Date.Date,
                                                       End_Date.Date, Notify_start.IsChecked, Notify_end.IsChecked, Summary_.Text, Objectives_.Text);
                Navigation.RemovePage(Navigation.NavigationStack.ElementAt(Navigation.NavigationStack.Count() - 1));
                var updatedCourse = await DatabaseService.GetCourse(course.Id);
                Page1 x = new Page1(updatedCourse);
                Navigation.InsertPageBefore(x, this);
                Navigation.RemovePage(this);
            }
        }
    }
}