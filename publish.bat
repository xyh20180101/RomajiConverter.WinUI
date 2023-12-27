set folderName=RomajiConverter.WinUI

ren fd-win-x64 %folderName%
powershell -command "Compress-Archive -CompressionLevel Optimal -Path %folderName% -DestinationPath 'RomajiConverter.WinUI-framework-dependent-win-x64.zip'"
ren %folderName% fd-win-x64

ren fd-win-x86 %folderName%
powershell -command "Compress-Archive -CompressionLevel Optimal -Path %folderName% -DestinationPath 'RomajiConverter.WinUI-framework-dependent-win-x86.zip'"
ren %folderName% fd-win-x86

ren sc-win-x64 %folderName%
powershell -command "Compress-Archive -CompressionLevel Optimal -Path %folderName% -DestinationPath 'RomajiConverter.WinUI-self-contained-win-x64.zip'"
ren %folderName% sc-win-x64

ren sc-win-x86 %folderName%
powershell -command "Compress-Archive -CompressionLevel Optimal -Path %folderName% -DestinationPath 'RomajiConverter.WinUI-self-contained-win-x86.zip'"
ren %folderName% sc-win-x86