using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App2.Services;
using App2.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTerm : ContentPage
    {

        public EditTerm()
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

        async void Submit_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Term_title.Text) || string.IsNullOrWhiteSpace(Term_title.Text))
            {
                await DisplayAlert("Missing entry", "Please enter a term title", "Okay");

            }
            else if (Term_picker.SelectedIndex == -1)
            {
                await DisplayAlert("Missing entry", "Please select a term", "Okay");
            }
            else
            {
                var terms = await DatabaseService.GetTerms();
                foreach (Term term in terms)
                {
                    if (Term_picker.SelectedItem.ToString() == term.Title)
                    {
                        await DatabaseService.UpdateTerm(term.Id, Term_title.Text, Start_date.Date, End_date.Date, term.CourseCount);
                        await Navigation.PopAsync();
                    }
                }
            }
        }

        private async void Term_picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var terms = await DatabaseService.GetTerms();
            Term_title.Text = Term_picker.SelectedItem.ToString();
            foreach (Term term in terms)
            {
                if(Term_picker.SelectedItem.ToString() == term.Title)
                {
                    Start_date.Date = term.StartDate;
                    End_date.Date = term.EndDate;
                }
            } 
        }
    }
}