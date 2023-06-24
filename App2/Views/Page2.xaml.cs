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
    public partial class Page2 : ContentPage
    {
        public Course course { get; set; }
        public bool objective { get; set; }
        public bool performance { get; set; }
        public Assessment selectedAssessment { get; set; }
        public Page2(Course selectedCourse)
        {
            course = selectedCourse;
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var assessments = await DatabaseService.GetAssessments(course.Id);
            Class_name.Text = course.ClassName;
            Assessment objectiveAssessment = new Assessment();
            Assessment performanceAssessment = new Assessment();
            foreach (Assessment x in assessments)
            {
                if (x.AssessmentType == "Objective")
                     objectiveAssessment = x;
                if (x.AssessmentType == "Performance")
                    performanceAssessment = x;
            }
            if(objective)
            {
                Assessment_type.Text = objectiveAssessment.AssessmentType;
                Assessment_code.Text = objectiveAssessment.AssessmentCode;
                Start_date.Date = objectiveAssessment.StartDate;
                End_date.Date = objectiveAssessment.EndDate;
                if (objectiveAssessment.NotifyStart == true)
                    Notify_start.IsChecked = true;
                if (objectiveAssessment.NotifyEnd == true)
                    Notify_end.IsChecked = true;
                Summary_.Text = objectiveAssessment.Summary;
                Objectives_.Text = objectiveAssessment.Objectives;
                selectedAssessment = objectiveAssessment;
            }

            if (performance)
            {
                Assessment_type.Text = performanceAssessment.AssessmentType;
                Assessment_code.Text = performanceAssessment.AssessmentCode;
                Start_date.Date = performanceAssessment.StartDate;
                End_date.Date = performanceAssessment.EndDate;
                if (performanceAssessment.NotifyStart == true)
                    Notify_start.IsChecked = true;
                if (performanceAssessment.NotifyEnd == true)
                    Notify_end.IsChecked = true;
                Summary_.Text = performanceAssessment.Summary;
                Objectives_.Text = performanceAssessment.Objectives;
                selectedAssessment = performanceAssessment;
            }
        }

        private async void EditAssessment_Clicked(object sender, EventArgs e)
        {
            var assessments = await DatabaseService.GetAssessments();
            foreach(Assessment assessment in assessments)
            {
                if(assessment.Id== selectedAssessment.Id)
                {
                    EditAssessment x = new EditAssessment(assessment, course);
                    await Navigation.PushAsync(x);
                    Page2 y = new Page2(course);
                    if (objective)
                        y.objective = true;
                    if (performance)
                        y.performance = true;
                    Navigation.InsertPageBefore(y, x);
                    Navigation.RemovePage(this);
                }

            }

        }

        private async void DeleteAssessment_Clicked(object sender, EventArgs e)
        {
            await DatabaseService.RemoveAssessment(selectedAssessment.Id, course.Id);
            Navigation.RemovePage(Navigation.NavigationStack.ElementAt(Navigation.NavigationStack.Count() - 2));
            var updatedCourse = await DatabaseService.GetCourse(course.Id);
            Page1 x = new Page1(updatedCourse);
            Navigation.InsertPageBefore(x,this);
            Navigation.RemovePage(this);
            
        }
    }
}