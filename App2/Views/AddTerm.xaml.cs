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
    public partial class AddTerm : ContentPage
    {
        public AddTerm()
        {
            InitializeComponent();
        }
        async void Submit_Clicked(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(Term_title.Text) || string.IsNullOrWhiteSpace(Term_title.Text))
            {
                await DisplayAlert("Missing entry", "Please enter a term title", "Okay");
            }
            else
            {
                 await DatabaseService.AddTerm(Term_title.Text, Start_date.Date, End_date.Date);
                 await Navigation.PopAsync();
            }
        }
    }
}