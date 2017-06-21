del blumind-3.1.zip

md "UI Themes"
cd "UI Themes"
del /q *.*
copy /Y "..\..\..\Code\Blumind\Resources\UIThemes\*.*" *
cd ..

"C:\program files\7-zip\7z.exe" a -tzip blumind-3.1.zip readme.txt readme.chs.txt readme.cht.txt 
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.1.zip Portable.bat RegBmd.bat UnRegBmd.bat 
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.1.zip ../../Code/Release/Icons 
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.1.zip ../../Code/Blumind/Resources/Languages 
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.1.zip "UI Themes"
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.1.zip "..\..\Code\Release\Dotfuscated\Blumind.exe"
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.1.zip "..\..\Code\Release\PdfSharp.dll"
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.1.zip "..\Include\Licence.txt"
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.1.zip "../../Documents/Blumind Quick Help.bmd"

rd /s /q "UI Themes"

Move blumind-3.1.zip ..\Output\Blumind-3.1.zip
