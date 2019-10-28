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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateBookmarks();
        }

        List<BookmarkPoster> bookmarkPosters = new List<BookmarkPoster>();
        void UpdateBookmarks()
        {
            int height = 100;
            int width = 65;
            if (Device.RuntimePlatform == Device.UWP) {
                height = 130;
                width = 85;
            }

            int pheight = height * 2;
            int pwidth = width * 2;
            Bookmarks.HeightRequest = height;
            List<string> keys = App.GetKeys<string>("BookmarkData");
            List<string> data = new List<string>();
            bookmarkPosters = new List<BookmarkPoster>();
            Bookmarks.Children.Clear();
            for (int i = 0; i < keys.Count; i++) {
                string name = FindHTML(keys[i], "Name=", "PosterUrl=");
                string posterUrl = FindHTML(keys[i], "PosterUrl=", "Id=");
                string id = FindHTML(keys[i], "Id=", "=EndAll");
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

    }


}