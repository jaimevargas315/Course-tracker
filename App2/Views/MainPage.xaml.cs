using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using App2.Services;
using App2.Models;
using App2.Views;
using SQLite;

namespace App2
{
    public partial class MainPage : ContentPage
    {
        int courseToEdit { get; set; }
        public bool changed { get; set; }


        public MainPage()
        {
            InitializeComponent();
            changed = true;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
           // DatabaseService.LoadSampleData();
           // DatabaseService.ClearSampleData();
            var courses1 = await DatabaseService.GetCourses();
            string notifications = "";
            foreach (Course x in courses1)
            {
                if (x.NotifyStart && (x.StartDate - DateTime.Now).TotalDays < 7  && (x.StartDate - DateTime.Now).TotalDays > 0)
                {
                    notifications += x.ClassName + " beginning soon : \n" + x.StartDate.ToString("d") + " \n \n";
                }
            }
            foreach (Course x in courses1)
            {
                if (x.NotifyStart && (x.EndDate - DateTime.Now).TotalDays < 7 && (x.EndDate - DateTime.Now).TotalDays > 0)
                {
                    notifications += x.ClassName + " ending soon : \n" + x.EndDate.ToString("d") + " \n \n";
                }
            }
            var assessments = await DatabaseService.GetAssessments();

            foreach (Assessment x in assessments)
            {
                if(x.NotifyStart)
                if (x.NotifyStart && (x.StartDate - DateTime.Now).TotalDays < 7 && (x.StartDate - DateTime.Now).TotalDays > 0)
                {
                    notifications += x.AssessmentCode + " beginning soon : \n" + x.StartDate.ToString("d") + " \n \n";
                }
            }
            foreach (Assessment x in assessments)
            {
                if (x.NotifyStart && (x.EndDate - DateTime.Now).TotalDays < 7 && (x.EndDate - DateTime.Now).TotalDays > 0)
                {
                    notifications += x.AssessmentCode + " ending soon : \n" + x.EndDate.ToString("d") + " \n \n";
                }
            }
            if (string.IsNullOrEmpty(notifications) == false)
            await DisplayAlert("Upcoming dates", notifications, "Okay");

            Course_Grid.RowDefinitions.Clear();
            Course_Grid.RowSpacing = 0;
            Course_Grid.ColumnSpacing = 0;
            Course_Grid.Padding = 0;
            Course_Grid.Margin = 0;  

            var terms = await DatabaseService.GetTerms();
            foreach (Term term in terms)
            {
                Course_Grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                StackLayout termStack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    BackgroundColor = Color.LightBlue,
                    Spacing = 0,
                    Margin = 0,
                    Padding = 0,
                    WidthRequest = 400,
                    HeightRequest = 75,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center,

                };
                Grid.SetColumnSpan(termStack, 2);
                Grid.SetRow(termStack, Course_Grid.RowDefinitions.Count);
                Course_Grid.Children.Add(termStack);

                termStack.Children.Add(new Label
                {
                    Text = term.Title,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Padding = 3,
                    FontSize = 23,
                });

                termStack.Children.Add(new DatePicker
                {
                    Date = term.StartDate,
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    IsEnabled = false
                });

                termStack.Children.Add(new Label
                {
                    Text = "-",
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = 3,
                    FontSize = 20,
                });

                termStack.Children.Add(new DatePicker
                {
                    Date = term.EndDate,
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    IsEnabled = false

                }); ;
                Course_Grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                StackLayout labelStack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    BackgroundColor = Color.White,
                    Spacing = 0,
                    Margin = 3,
                    Padding = 0,
                    WidthRequest = 400,
                    HeightRequest = 25,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center
                };
                labelStack.Children.Add(new Label
                {
                    Text = "Course Name",
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    FontAttributes = FontAttributes.Bold
                });
                labelStack.Children.Add(new Label
                {
                    Text = "Start Date - End Date",
                    FontSize = 15,
                    FontAttributes = FontAttributes.Bold
                });
                Grid.SetColumnSpan(labelStack, 2);
                Grid.SetRow(labelStack, Course_Grid.RowDefinitions.Count);
                Course_Grid.Children.Add(labelStack);

                var courses2 = await DatabaseService.GetCourses(term.Id);
                foreach (Course x in courses2)
                {
                    Course_Grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    var courseTap = new TapGestureRecognizer();
                    courseTap.Tapped += (s, e) =>
                    {
                        Course_Tapped(this, null, x);
                    };
                    StackLayout stack = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        BackgroundColor = Color.RoyalBlue,
                        Margin = 3,
                    };
                    stack.GestureRecognizers.Add(courseTap);
                    Grid.SetColumnSpan(stack, 2);
                    Grid.SetRow(stack, Course_Grid.RowDefinitions.Count);
                    Course_Grid.Children.Add(stack);
                    // column start, column end, row start, row end
                    stack.Children.Add(new Label
                    {
                        Text = x.ClassName,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        BackgroundColor = Color.RoyalBlue,
                        TextColor = Color.White,
                        FontAttributes = FontAttributes.Bold,
                        Padding = 3,
                        FontSize = 15,
                        HorizontalOptions = LayoutOptions.StartAndExpand

                    });

                    stack.Children.Add(new Label
                    {
                        Text = x.StartDate.ToString("MM/dd/yy"),
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        BackgroundColor = Color.RoyalBlue,
                        TextColor = Color.White,
                        FontAttributes = FontAttributes.Bold,
                        Padding = 3,
                        FontSize = 12
                    });

                    stack.Children.Add(new Label
                    {
                        Text = "-",
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        BackgroundColor = Color.RoyalBlue,
                        TextColor = Color.White,
                        FontAttributes = FontAttributes.Bold,
                        Padding = 3,
                        FontSize = 12
                    });


                    stack.Children.Add(new Label
                    {
                        Text = x.EndDate.ToString("MM/dd/yy"),
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        BackgroundColor = Color.RoyalBlue,
                        TextColor = Color.White,
                        FontAttributes = FontAttributes.Bold,
                        Padding = 3,
                        FontSize = 12
                    });
                }
            }
            
        }
        async void Course_Tapped(object sender, EventArgs e, Course selectedCourse)
        {
            Page1 x = new Page1(selectedCourse) ;
            await Navigation.PushAsync(x);
            Navigation.InsertPageBefore(new MainPage(),x);
            Navigation.RemovePage(this);

        }

        async void Delete_Clicked(object sender, EventArgs e)
        {
            var terms = await DatabaseService.GetTerms();
            if (terms.Count() < 1)
            {
                await DisplayAlert("Error", "There are no terms to delete", "Okay");
            }
            else
            {
                DeleteTerm x = new DeleteTerm();
                await Navigation.PushAsync(x);
                Navigation.InsertPageBefore(new MainPage(), x);
                Navigation.RemovePage(this);
            }
        }

        async void Add_Clicked(object sender, EventArgs e)
        {
            AddTerm x = new AddTerm();
            await Navigation.PushAsync(x);
            Navigation.InsertPageBefore(new MainPage(), x);
            Navigation.RemovePage(this);
        }

        async void Edit_Clicked(object sender, EventArgs e)
        {
            var terms = await DatabaseService.GetTerms();
            if (terms.Count() < 1)
            {
                await DisplayAlert("Error", "There are no terms to edit", "Okay");
            }
            else
            {
                EditTerm x = new EditTerm();
                await Navigation.PushAsync(x);
                Navigation.InsertPageBefore(new MainPage(), x);
                Navigation.RemovePage(this);
            }
        }

        async void Course_Add_Clicked(object sender, EventArgs e)
        {
            var terms = await DatabaseService.GetTerms();
            if (terms.Count() < 1)
            {
                await DisplayAlert("Error", "There are no terms to edit", "Okay");
            }
            else
            {
                AddCourse x = new AddCourse();
                await Navigation.PushAsync(x);
                Navigation.InsertPageBefore(new MainPage(), x);
                Navigation.RemovePage(this);
            }
        }

    }
}
