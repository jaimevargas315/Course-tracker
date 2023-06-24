using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App2.Services;
using App2.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCourse : ContentPage
    {
        public AddCourse()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var terms = await DatabaseService.GetTerms();
            foreach (Term term in terms)
                Term_picker.Items.Add(term.Title);
        }

        private async void Term_picker_SelectedIndexChanged(object sender, EventArgs e)
        { 
            
            var terms = await DatabaseService.GetTerms();
            foreach (Term term in terms)
            { 
                if (term.CourseCount >= 6)
                {
                    await DisplayAlert("Error", "The selected term has reached the maximum amount of courses (6), unable to add any more.", "Okay");

                } 
            }
        }

        async void Submit_Clicked(object sender, EventArgs e)
        {
            var terms = await DatabaseService.GetTerms();
            bool status = false;
            if (Term_picker.SelectedIndex == -1)
                await DisplayAlert("Error", "Please select a term", "Okay");
            if (course_status.SelectedIndex != -1 && course_status.SelectedItem.ToString() == "Active")
                status = true;
            if(string.IsNullOrEmpty(Class_name_2.Text) || string.IsNullOrEmpty(instructor_name.Text) || string.IsNullOrEmpty(email_.Text) ||
                string.IsNullOrEmpty(phone_.Text) || string.IsNullOrEmpty(notes_.Text))            
            {
                await DisplayAlert("Error", "All entries must be filled", "Okay");
            }       
            else
            {
                foreach (Term term in terms)
                {
                    if (term.Title == Term_picker.SelectedItem.ToString())
                    {
                        if (term.CourseCount >= 6)
                        {
                            await DisplayAlert("Error", "The selected term has reached the maximum amount of courses (6), unable to add any more.", "Okay");
                        }
                        else
                        {
                            await DatabaseService.AddCourse(term.Id, Class_name_2.Text, start_date.Date, end_date.Date, status, notify_start.IsChecked,
                                                            notify_end.IsChecked, notes_.Text, instructor_name.Text, phone_.Text, email_.Text);
                            await Navigation.PopAsync();
                        }
                    }

                }
            }
            
        }

    }
}