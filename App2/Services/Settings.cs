using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace App2.Services
{
    class Settings
    {

        public static bool FirstRun
        {
            get => Preferences.Get(nameof(FirstRun), true);
            set => Preferences.Set(nameof(FirstRun), value);
        }
        // https://devblogs.microsoft.com//xamarin/first-run=xamarin-essentials/
    }
}
