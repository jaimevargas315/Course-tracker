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
    public partial class DeleteTerm : ContentPage
    {
        public DeleteTerm()
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
            var terms = await DatabaseService.GetTerms();
            foreach (Term term in terms)
            {
                if (Term_picker.SelectedItem.ToString() == term.Title)
                {
                    await DatabaseService.RemoveTerm(term.Id);
                    await Navigation.PushAsync(new MainPage());
                    Navigation.RemovePage(this);
                }
            }
        }

    }
}