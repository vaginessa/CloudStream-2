using CloudStreamForms.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CloudStreamForms.Main;

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Search : ContentPage
    {
        public ObservableCollection<SearchResult> mySearchResultCollection;
        public static Poster mainPoster;
        ListView listView;
        public Search()
        {
            InitializeComponent();

            searchLoaded += Search_searchLoaded;

            //BindingContext = new SearchPageViewer();

            mySearchResultCollection = new ObservableCollection<SearchResult>() {
            };

            SearchBar searchBar = new SearchBar() {
                Placeholder = "Movie Search",
            };
            searchBar.TextChanged += SearchBar_TextChanged;
            listView = new ListView {
                // Source of data items.
                ItemsSource = mySearchResultCollection,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() => {
                    // Create views with bindings for displaying each property.
                    Label nameLabel = new Label();
                    Label desLabel = new Label();
                    // Image poster = new Image();
                    nameLabel.SetBinding(Label.TextProperty, "Title");
                    desLabel.SetBinding(Label.TextProperty, "Extra");
                    // poster.SetBinding(Image.SourceProperty, "Poster");
                    desLabel.FontSize = nameLabel.FontSize / 1.2f;
                    desLabel.TextColor = Color.Gray;

                    desLabel.TranslationX = 10;
                    nameLabel.TranslationX = 10;
                    //nameLabel.SetBinding(Label.d, "Extra");
                    /*
                    Label birthdayLabel = new Label();
                    birthdayLabel.SetBinding(Label.TextProperty,
                        new Binding("Birthday", BindingMode.OneWay,
                            null, null, "Born {0:d}"));

                    BoxView boxView = new BoxView();
                    boxView.SetBinding(BoxView.ColorProperty, "FavoriteColor");*/

                    // Return an assembled ViewCell.
                    return new ViewCell {
                        View = new StackLayout {
                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                    //boxView,
                                    new StackLayout
                                    {
                                        VerticalOptions = LayoutOptions.CenterAndExpand,
                                        Spacing = 0,
                                        Children =
                                        {
                                        //    poster,
                                            nameLabel,
                                            desLabel,
                                            //birthdayLabel
                                        }
                                        }
                                }
                        }
                    };
                })
            };
            listView.ItemTapped += ListView_ItemTapped;
            // Accomodate iPhone status bar.
            // this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

            // Build the page.
            this.Content = new StackLayout {
                Children =
                {
                    searchBar,
                    listView
                }
            };
            // print(MainSearchResultList.ItemsSource.ToString()  + "<<<<<<<<<");

        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            print(e.ItemIndex);
            print(activePosters[e.ItemIndex].name + "<<<");
            listView.SelectedItem = null;
            PushPage(activePosters[e.ItemIndex], Navigation);
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // base.OnBackButtonPressed();
        }


        public static async void PushPage(Poster _mainPoster, INavigation navigation)
        {
            mainPoster = _mainPoster;
            Page p = new MovieResult();// { mainPoster = mainPoster };

            await navigation.PushModalAsync(p, false);
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuickSearch(e.NewTextValue);
        }

        List<Poster> activePosters = new List<Poster>();

        private void Search_searchLoaded(object sender, List<Poster> e)
        {
            activePosters = e;
            MainThread.BeginInvokeOnMainThread(() => {

                mySearchResultCollection.Clear();
                for (int i = 0; i < activeSearchResults.Count; i++) {
                    string extra = activeSearchResults[i].extra;
                    if (extra != "") {
                        extra = " - " + extra;
                    }
                    mySearchResultCollection.Add(new SearchResult() { Id = i, Title = activeSearchResults[i].name + extra, Extra = activeSearchResults[i].year, Poster = activeSearchResults[i].posterUrl });
                }
            });


        }
    }
}