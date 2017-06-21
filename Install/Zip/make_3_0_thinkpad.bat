del blumind-3.0.zip

"C:\program files\7-zip\7z.exe" a -tzip blumind-3.0.zip readme.txt readme.chs.txt readme.cht.txt 
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.0.zip Portable.bat RegBmd.bat UnRegBmd.bat 
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.0.zip ../../Code/Release/Icons 
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.0.zip ../../Code/Blumind/Resources/Languages 
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.0.zip ../../Code/Blumind/Resources/UIThemes 
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.0.zip "..\..\Code\Release\Dotfuscated\Blumind.exe"
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.0.zip "..\..\Code\Release\PdfSharp.dll"
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.0.zip "..\Include\Licence.txt"
"C:\program files\7-zip\7z.exe" a -tzip blumind-3.0.zip "../../Documents/Blumind Quick Help.bmd"

Move blumind-3.0.zip ..\Output\blumind-3.0.beta.zip
