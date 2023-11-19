# RomajiConverter.WinUI

Win11 style romaji converter developed using WinUI 3 framework

![](/doc/icon.png)

[中文使用文档](https://github.com/xyh20180101/RomajiConverter.WinUI/blob/main/README.md)

Here's the user-facing document, if you're more focused on development-related content, [jump here](https://github.com/xyh20180101/RomajiConverter.WinUI/blob/main/doc/README-dev-en.md)

**Preview(Win11)**

![](/doc/preview.png)

## Supported system versions
- Supported versions of project settings: Windows 10 1809 (Build 17763) or above
- Developer-tested system versions: Windows 10 22H2 (19045), Windows 11 22H2 (22621)
- If your version of Windows is not in the above scope, you can view the project [RomajiConverter](https://github.com/xyh20180101/RomajiConverter)（Support Win7 or above）

## Function
- Convert kana and kanji to romaji
- Support direct conversion of Japanese lyrics with Chinese translation (by copying or importing)
- Support the import of currently playing lyrics on NetEase Cloud Music PC Client
- You can edit the conversion result and export it as a PNG image

## UI Description
1. Enter your Japanese text in the text box on the left, or get the lyrics of the song currently being played from the NetEase Cloud Music PC client via the `Get CloudMusic Lyrics` button, which does not support the sound in the podcast
2. Select `Consider variants`, it will try to convert unrecognized kanji characters to traditional or simplified until get the romaji correctly. If this option is not selected, the original text will be converted directly
3. In non-`Detail Mode`, click the `Convert` button to get the converted text, and you can change the format of the converted text in the `Text Display Option` above
4. In `Detail Mode`, click `Convert` will generate a preview of the conversion, the display elements can be changed via the `Display Option` above, if you find a conversion error, you can double-click the box to enter edit mode to modify it, click on the blank space, Enter or Esc to exit the editing mode, and then click `Generate Text` to get the converted text
5. You can click the `Save` button to save the current conversion preview, and then click the `Open` button to continue editing
6. You can click the `Export Image` button to save the current conversion preview as an image, and the image will be styled in the same way as the conversion preview (without boxes and dividers), and will also be controlled by the `Display Options`. As for the font, color, and spacing, you need to click the `Settings` button to set it
7. The text on the interface supports size scaling, method: Ctrl + scroll wheel

## Bug
- The conversion of kanji and polyphonic kana (e.g. は) is not necessarily accurate, and the actual pronunciation prevails
- Although the function of identifying the simplified and traditional variants is provided, it may not be accurate, so please ensure that the lyrics are in correct Japanese

## Common Problem
```
本能が云(い)う、嫌々(いやいや)  =>  honnou ga云(i)u、iyaiya(iyaiya)
どこかで微(かす)か伝うメーデー  =>  doko ka de bi(kasu)ka tsutau mee dee
```
- The reason for the above situation where the kanji characters are not converted and the pronunciation is wrong is that the uploader of the lyrics has already marked the pronunciation in parentheses. In this case, just use the pronunciation in parentheses

## Download and update
Download please check [Release](https://github.com/xyh20180101/RomajiConverter.WinUI/releases), choose the latest version to download

## Update log

### 1.0.0
- Migrated from a WPF project to WinUI 3 with all the original functionality