# CloudStream 2

***THIS PROJECT IS NOT DONE YET***

CloudStream 2 is the successor to CloudStream and will have many more features.

**NEW:**
+ Instant Search
+ IMDb and MAL integration
+ Recommendations
+ Trailer
+ Movie and Episode description
+ Subtitles (From https://www.opensubtitles.org)

**WILL BE ADDED BEFORE FIRST RELEASE:**
+ Bookmarks
+ Download 
+ Title sharing (Something like: https://js.do/code/348388)

**COMMING SOON:**
+ Chromecast
---------------------------------------
***How it works:***

This app dosen't use a p2p connection or any private servers hosted by me, **IT IS NOT A BITTORRENT**. It takes all the links from established streaming sites by downloading the sites and extracting the useful information.

CloudStream and CloudStream 2 works in diffrent ways. CloudStream gets the links and info from the link site directly. CloudStream 2 will first search IMDb and then crossreference with the link sites and MAL to get the links. This will result in CloudStream 2 having fewer providers overall, and sometimes having links to diffrent movies or shows with the same name. It will also make CloudStream 2 slower because it has to download many more sites to get all the information.

***Sites used:***

https://www.imdb.com/ (Seach, rating, trailer, recommendations and descriptions)

https://myanimelist.net/ (Only used to crossreference anime)

https://movies123.pro/ (Movie and Tv-Show mirror links)

https://gomostream.com/ (Movie and Tv-Show HD links)

https://www9.gogoanime.io/ (Anime HD links)

https://www.opensubtitles.org (Subtitles)

https://js.do/code (Title sharing)

***Github Libraries used:***

https://github.com/sebastienros/jint (JS interpreter, to avoid bot detection)

