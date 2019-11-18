using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CloudStreamForms.MainPage;
using static CloudStreamForms.Main;

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();
            BackgroundColor = Color.FromHex(Settings.MainBackgroundColor);
            MovieTypePicker.ItemsSource = genresNames;
            MovieTypePicker.SelectedIndexChanged += (o, e) => {
                print(MovieTypePicker.SelectedIndex + "<<Selected" );
            };
            MovieTypePicker.SelectedIndex = 0;
            MovieTypePicker.IsEnabled = false;
            MovieTypePicker.IsVisible = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateBookmarks();
            BackgroundColor = Color.FromHex(Settings.MainBackgroundColor);
        }

        List<BookmarkPoster> bookmarkPosters = new List<BookmarkPoster>();
        void UpdateBookmarks()
        {
            double multi = 1.2;
            int height = 100;
            int width = 65;
            if (Device.RuntimePlatform == Device.UWP) {
                height = 130;
                width = 85;
            }

            height = (int)Math.Round(height * multi);
            width = (int)Math.Round(width * multi);

            Bookmarks.HeightRequest = height;
            List<string> keys = App.GetKeys<string>("BookmarkData");
            List<string> data = new List<string>();
            bookmarkPosters = new List<BookmarkPoster>();
            Bookmarks.Children.Clear();
            for (int i = 0; i < keys.Count; i++) {
                string name = FindHTML(keys[i], "Name=", "|||");
                string posterUrl = FindHTML(keys[i], "PosterUrl=", "|||");
                string id = FindHTML(keys[i], "Id=", "|||");
                if (name != "" && posterUrl != "" && id != "") {
                    if (CheckIfURLIsValid(posterUrl)) {
                        print(">>>>" + posterUrl);
                        Grid stackLayout = new Grid();
                        Button imageButton = new Button() { HeightRequest = height, WidthRequest = width, BackgroundColor = Color.Transparent, VerticalOptions = LayoutOptions.Start };
                        var ff = new FFImageLoading.Forms.CachedImage {
                            Source = posterUrl,
                            HeightRequest = height,
                            WidthRequest = width,
                            BackgroundColor = Color.Transparent,
                            VerticalOptions = LayoutOptions.Start,
                            Transformations = {
                                new FFImageLoading.Transformations.RoundedTransformation(10,1,1.5,10,"#303F9F")
                            },
                            InputTransparent = true,
                        };

                        //Source = p.posterUrl

                        stackLayout.Children.Add(ff);
                        stackLayout.Children.Add(imageButton);
                        bookmarkPosters.Add(new BookmarkPoster() { button = imageButton, id = id, name = name, posterUrl = posterUrl });
                        Grid.SetColumn(stackLayout, Bookmarks.Children.Count);
                        Bookmarks.Children.Add(stackLayout);

                        // --- RECOMMENDATIONS CLICKED -----
                        imageButton.Clicked += (o, _e) => {
                            for (int z = 0; z < bookmarkPosters.Count; z++) {
                                if (((Button)o).Id == bookmarkPosters[z].button.Id) {
                                    PushPageFromUrlAndName(bookmarkPosters[z].id, bookmarkPosters[z].name);
                                }
                            }
                        };
                    }
                }
                print(keys[i] + "<<KEy");
                // data.Add(App.GetKey("BookmarkData"))
            }
        }

        List<string> genres = new List<string>() { "action", "adventure", "animation", "biography", "comedy", "crime", "drama", "family", "fantasy", "film-noir", "history", "horror", "music", "musical", "mystery", "romance", "sci-fi", "sport", "thriller", "war", "western" };
        List<string> genresNames = new List<string>() { "Action", "Adventure", "Animation", "Biography", "Comedy", "Crime", "Drama", "Family", "Fantasy", "Film-Noir", "History", "Horror", "Music", "Musical", "Mystery", "Romance", "Sci-Fi", "Sport", "Thriller", "War", "Western" };

    }


}