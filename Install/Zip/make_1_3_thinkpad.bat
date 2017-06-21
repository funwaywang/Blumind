del blumind.zip

"C:\program files\7-zip\7z.exe" a -tzip blumind-1.3.zip readme.txt readme.chs.txt readme.cht.txt 
"C:\program files\7-zip\7z.exe" a -tzip blumind-1.3.zip ../../Code/Release/Icons 
"C:\program files\7-zip\7z.exe" a -tzip blumind-1.3.zip "..\..\Code\Release\Dotfuscated\Blumind.exe"
"C:\program files\7-zip\7z.exe" a -tzip blumind-1.3.zip "..\..\Code\Release\DocTypeReg.exe"
"C:\program files\7-zip\7z.exe" a -tzip blumind-1.3.zip "..\Include\Licence.txt"
"C:\program files\7-zip\7z.exe" a -tzip blumind-1.3.zip "../../Documents/Blumind Quick Help.bmd"

Move *.zip ..\Output\.
