# RomajiConverter.WinUI

使用WinUI 3框架开发的Win11风格罗马音转换器

![](/doc/icon.png)

[English Readme](https://github.com/xyh20180101/RomajiConverter.WinUI/blob/main/doc/README-en.md)

这里是面向用户的使用文档，如果你更关注开发相关内容，[请跳转此处](https://github.com/xyh20180101/RomajiConverter.WinUI/blob/main/doc/README-dev-cn.md)

**界面预览图(Win11)**

![](/doc/preview.png)

## 支持的系统版本
- 项目设置的受支持版本：Windows 10 1809（内部版本17763）及以上
- 经过开发者测试的系统版本：Windows 10 22H2（19045）、Windows 11 22H2（22621）
- 如果你的Windows版本不在以上范围，可以查看该项目[RomajiConverter](https://github.com/xyh20180101/RomajiConverter)（支持Win7以上）

## 功能
- 将假名、汉字转换为罗马音
- 支持直接转换带中文翻译的日语歌词（通过复制或导入）
- 支持网易云音乐PC端的当前播放的歌词导入
- 支持通过链接导入歌词（网易云、QQ音乐、酷狗）
- 提供单词的多个读音作为备选项
- 支持编辑转换结果，并将其导出为png图片

## 界面说明
1. 在左侧的文本框输入你的日文文本
    - 或者通过`获取网易云歌词`按钮从网易云音乐PC客户端获取当前正在播放的歌曲歌词，该功能不支持纯音乐和播客中的声音
    - 或者通过`通过链接导入歌词`按钮，输入从网易云、QQ音乐、酷狗的PC客户端获得的歌曲分享链接，从而导入歌词
2. 勾选`识别变体`，会尝试对无法识别的汉字进行简繁转换，直到可以正确获取罗马音为止。不勾选则直接转换原文本
3. 非`详细模式`下，点击`转换`按钮即可获取转换文本，并可在上方`文本显示选项`更改转换文本的格式
4. `详细模式`下，点击`转换`会生成转换预览，可以通过上方的`显示选项`改变显示元素，如果你发现了转换错误的地方，可以双击方框进入编辑模式进行修改，可以选择应用生成的读音备选项，也可以自由输入文本，点击空白区域或Enter退出编辑模式，修改好之后再点击`生成文本`即可获取转换文本
5. 可以点击`保存`按钮将当前的转换预览保存下来，以后点击`打开`按钮继续进行编辑
6. 可以点击`导出图片`按钮将当前的转换预览保存为图片，图片的样式将与转换预览一致（没有方框和分隔线），并且也受到`显示选项`的控制。至于字体、颜色、间距，则需要点击`设置`按钮进行设置
7. 界面文本均支持大小缩放，方法：Ctrl+滚轮

## 缺陷
- 汉字和多音假名（例如：は）的转换不一定准确，以实际发音为准
- 尽管提供了识别简繁变体的功能，但不一定准确，请保证歌词文本是正确的日文
- 双击方框有时不能进入编辑模式，再次尝试即可

## 常见问题
```
本能が云(い)う、嫌々(いやいや)  =>  honnou ga云(i)u、iyaiya(iyaiya)
どこかで微(かす)か伝うメーデー  =>  doko ka de bi(kasu)ka tsutau mee dee
```
- 上面这种汉字没转换、读音错误的情况的原因为：歌词上传者已经在括号里标注了读音。这种情况下直接使用括号里的读音就可以了

## 下载与更新
- 下载请查看[Release](https://github.com/xyh20180101/RomajiConverter.WinUI/releases)，选择适合你系统的最新版本下载
- 选择框架依赖版需要安装[.net core 8.0桌面运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0)，独立版则不需要

## 更新日志

### 1.2.2
- 新增通过链接导入歌词功能，支持网易云、QQ音乐、酷狗

### 1.2.1
- 修复转换罗马音时意外报错的问题
- 修复部分歌曲无法获取网易云歌词的问题
- 该版本重新实现了Lrc解析逻辑，如果仍有获取不到网易云歌词的情况，请反馈并在设置中改为旧版解析器再尝试

### 1.2.0
- 修复长音符号转换为平假名出错(`ー`变为`゜`)的问题

    | 例 | 修复前 | 修复后 |
    | - | - | - |
    | `アイロニー` | `あいろに゜` | `あいろにー` |

- 重新实现纯假名到罗马音的转换逻辑，效果：

    - 对于非外来词，长音符号将转为具体的发音假名
    - 修复部分长音转换为罗马音错误的问题

    | 例 | 修复前 | 修复后 |
    | - | - | - |
    | `秒` | `びょー` | `びょう` |
    | `秒` | `byo-` | `byou` |
    | `一方通行` | `いっぽー つーこー` | `いっぽう つうこう` |
    | `一方通行` | `ippo- tsuukoo` | `ippou tsuukou` |

- 修复主要用于外来语的扩展片假名的转换问题

    | 例 | 修复前 | 修复后 |
    | - | - | - |
    | `フェ` | `fue` | `fe` |
    | `ファ` | `fua` | `fa` |
    | `ティ` | `tei` | `ti` |
    | ... |

- 提取部分代码到core项目，并添加单元测试

### 1.1.0
- 新增备选项功能，双击方框可弹出单词所有读音的备选项，可以直接选择，也可以自由修改（输入文本后按Enter键）。选择备选项时，如果该选项是应用生成的，还支持罗马音和平假名的选项同步
- 新增方框显示设置，可配合备选项功能使用
- 优化罗马音转换逻辑，支持更换unidic词典资源（unidic文件夹）
- 优化内存使用（效果不是很明显）

### 1.0.0
- 从WPF项目迁移至WinUI 3，保留了原有的所有功能