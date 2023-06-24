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
    public partial class NewAssessment : ContentPage
    {
        public Course course { get; set; }
        public NewAssessment(Course Course)
        {
            InitializeComponent();
            course = Course;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var assessments = await DatabaseService.GetAssessments(course.Id);
            Course_title.Text = course.ClassName;
            Assessment_type.Items.Add("Objective");
            Assessment_type.Items.Add("Performance");

            if (assessments.Count() == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (assessments.ElementAt(i).AssessmentType == "Objective")
                    {
                        Assessment_type.Items.RemoveAt(0);
                    }
                    if (assessments.ElementAt(i).AssessmentType == "Performance")
                    {
                        Assessment_type.Items.RemoveAt(1);
                    }
                }
            }
            if (assessments.Count() == 1)
            {
                if (assessments.ElementAt(0).AssessmentType == "Objective")
                {
                    Assessment_type.Items.RemoveAt(0);
                }
                if (assessments.ElementAt(0).AssessmentType == "Performance")
                {
                    Assessment_type.Items.RemoveAt(1);
                }
            }

        }
        private async void Submit_Clicked(object sender, EventArgs e)
        {

            await DatabaseService.AddAssessment(course.Id, Assessment_type.SelectedItem.ToString(), Code_.Text, Start_date.Date, End_date.Date, Notify_start.IsChecked,
                                                Notify_end.IsChecked, summary_.Text, objectives_.Text);
            Navigation.RemovePage(Navigation.NavigationStack.ElementAt(Navigation.NavigationStack.Count() - 1));
            var updatedCourse = await DatabaseService.GetCourse(course.Id);
            Page1 x = new Page1(updatedCourse);
            Navigation.InsertPageBefore(x,this);
            Navigation.RemovePage(this);
        }
    }
}