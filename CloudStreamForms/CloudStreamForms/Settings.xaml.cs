using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        public const string errorEpisodeToast = "No Links Found";
        public static int loadingMiliSec = 5000;
        public Settings()
        {
            InitializeComponent();
        }
    }
}