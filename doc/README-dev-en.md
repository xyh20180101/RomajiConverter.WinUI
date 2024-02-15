# RomajiConverter.WinUI
Here's the document for developers, if you're more interested in using the app, [jump here](https://github.com/xyh20180101/RomajiConverter.WinUI/blob/main/doc/README-en.md)

## Framework Version
.NET 8.0

## Dictionary Resource
- The pulled code can be compiled directly, but if you want to run the application properly, you will need a dictionary resource file (unidic), which can be obtained from the zip in the release
- The real source of dictionary is `unidic-mecab-2.1.2_bin.zip` from https://clrd.ninjal.ac.jp/unidic_archive/cwj/2.1.2/ This version of the dictionary was chosen because of the relatively small file size and relatively good parsing effect. You can also select a different version of the dictionary in https://clrd.ninjal.ac.jp/unidic/back_number.html After version 1.1.0, the code can support different dictionary resources, but I tried a few but there was not much difference

## CloudMusic Lyrics
- The implementation process is to read the local data file of the NetEase Cloud Music client, which contains the last played song (the currently played song), after getting the songId, request the official api interface (no authentication required) to get the lyrics
- NetEase Cloud Music (V3.0) local data is stored in `User\AppData\Local\Netease\CloudMusic\Library\webdb.dat`, which is a sqlite file

## QQ Music Lyrics & KuGou Lyrics
- Access the song sharing link, get the song ID and other information, and get the lyrics and Chinese translation through the official interface

## Project references
[MeCab.DotNet](https://github.com/kekyo/MeCab.DotNet)  
[NTextCat](https://github.com/ivanakcheurov/ntextcat)  
[LrcParser](https://github.com/OpportunityLiu/LrcParser)  
[UniDic](https://clrd.ninjal.ac.jp/unidic/)  
[unihan-database](https://github.com/unicode-org/unihan-database)