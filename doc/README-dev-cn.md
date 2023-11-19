# RomajiConverter.WinUI
这里是面向开发者的文档，如果你更关注如何使用本应用，[请跳转此处](https://github.com/xyh20180101/RomajiConverter.WinUI/blob/main/README.md)

## 项目框架版本
.NET 8.0

## 词典资源
- 拉取下来的代码可以直接编译，但如果要正常运行应用，还需要词典资源文件(unidic)，该资源可以从release中的压缩包里获得
- 词典真正的来源是 https://clrd.ninjal.ac.jp/unidic_archive/cwj/2.1.2/ 中的`unidic-mecab-2.1.2_bin.zip`，选择该版本词典的原因是文件大小相对较小，解析效果相对不错。你也可以在 https://clrd.ninjal.ac.jp/unidic/back_number.html 选择其他版本的词典，但由于我只针对`unidic-mecab-2.1.2_bin.zip`进行开发，使用其他词典代码可能会报错

## 网易云歌词
- 实现过程是读取网易云客户端本地的数据文件，里面有最后播放的歌曲（当前播放的歌曲），拿到songId后请求官方的api接口（不需要鉴权）拿到歌词
- 网易云（3.0版本）本地数据保存在`文档\AppData\Local\Netease\CloudMusic\Library\webdb.dat`，是sqlite文件


## 项目引用
[MeCab.DotNet](https://github.com/kekyo/MeCab.DotNet)  
[NTextCat](https://github.com/ivanakcheurov/ntextcat)  
[LrcParser](https://github.com/OpportunityLiu/LrcParser)  
[WanaKanaSharp](https://github.com/caguiclajmg/WanaKanaSharp)  
[UniDic](https://clrd.ninjal.ac.jp/unidic/)  
[unihan-database](https://github.com/unicode-org/unihan-database)