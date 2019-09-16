using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static CloudSteamForms.Main;
using CloudSteamForms.Models;

namespace CloudSteamForms
{

    public class SearchPageViewer
    {
        public ObservableCollection<SearchResult> mySearchResultCollection;

        public SearchPageViewer()
        {
            print("daaaaaaaaaaaaaaaaaaaaaaaaaa");
            mySearchResultCollection = new ObservableCollection<SearchResult>() {
                new SearchResult() {id=0,extra="da",title="AAAAA"},
                new SearchResult() {id=1,extra="da12",title="a"},
            };
            print(mySearchResultCollection.Count);
            
        }
    }
}
