# RomajiConverter.WinUI

使用WinUI 3框架开发的Win11风格罗马音转换器

![](https://raw.githubusercontent.com/xyh20180101/RomajiConverter.WinUI/main/doc/icon.png)

[English Readme](https://github.com/xyh20180101/RomajiConverter.WinUI/blob/main/doc/README-en.md)

这里是面向用户的使用文档，如果你更关注开发相关内容，[请跳转此处](https://github.com/xyh20180101/RomajiConverter.WinUI/blob/main/doc/README-dev-cn.md)

**界面预览图(Win11)**

![](https://raw.githubusercontent.com/xyh20180101/RomajiConverter.WinUI/main/doc/preview.png)

## 支持的系统版本
- 项目设置的受支持版本：Windows 10 1809（内部版本17763）及以上
- 经过开发者测试的系统版本：Windows 10 22H2（19045）、Windows 11 22H2（22621）
- 如果你的Windows版本不在以上范围，可以查看该项目[RomajiConverter](https://github.com/xyh20180101/RomajiConverter)（支持Win7以上）

## 功能
- 将假名、汉字转换为罗马音
- 支持直接转换带中文翻译的日语歌词（通过复制或导入）
- 支持网易云音乐PC端的当前播放的歌词导入
- 支持编辑转换结果，并将其导出为png图片

## 界面说明
1. 在左侧的文本框输入你的日文文本，或者通过`获取网易云歌词`按钮从网易云音乐PC客户端获取当前正在播放的歌曲歌词，该功能不支持播客中的声音
2. 勾选`识别变体`，会尝试对无法识别的汉字进行简繁转换，直到可以正确获取罗马音为止。不勾选则直接转换原文本
3. 非`详细模式`下，点击`转换`按钮即可获取转换文本，并可在上方`文本显示选项`更改转换文本的格式
4. `详细模式`下，点击`转换`会生成转换预览，可以通过上方的`显示选项`改变显示元素，如果你发现了转换错误的地方，可以双击方框进入编辑模式进行修改，点击空白区域、Enter或Esc退出编辑模式，修改好之后再点击`生成文本`即可获取转换文本
5. 可以点击`保存`按钮将当前的转换预览保存下来，以后点击`打开`按钮继续进行编辑
6. 可以点击`导出图片`按钮将当前的转换预览保存为图片，图片的样式将与转换预览一致（没有方框和分隔线），并且也受到`显示选项`的控制。至于字体、颜色、间距，则需要点击`设置`按钮进行设置
7. 界面文本均支持大小缩放，方法：Ctrl+滚轮

## 缺陷
- 汉字和多音假名（例如：は）的转换不一定准确，以实际发音为准
- 尽管提供了识别简繁变体的功能，但不一定准确，请保证歌词文本是正确的日文

## 常见问题
```
本能が云(い)う、嫌々(いやいや)  =>  honnou ga云(i)u、iyaiya(iyaiya)
どこかで微(かす)か伝うメーデー  =>  doko ka de bi(kasu)ka tsutau mee dee
```
- 上面这种汉字没转换、读音错误的情况的原因为：歌词上传者已经在括号里标注了读音。这种情况下直接使用括号里的读音就可以了

## 下载与更新
下载请查看[Release](https://github.com/xyh20180101/RomajiConverter.WinUI/releases)，选择最新版本下载

## 更新日志

### 1.0.0
- 从WPF项目迁移至WinUI 3，保留了原有的所有功能