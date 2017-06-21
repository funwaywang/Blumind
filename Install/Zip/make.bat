copy ..\..\Code\Release\Dotfuscated\Blumind.exe .
copy ..\..\Code\Release\DocTypeReg.exe .
copy ..\Include\Licence.txt .

del blumind.zip

"d:\program files\7-zip\7z.exe" a -tzip blumind-1.1.zip blumind.exe doctypereg.exe readme.txt readme.chs.txt readme.cht.txt licence.txt 

del blumind.exe
del doctypereg.exe
del licence.txt

Move *.zip ..\Output\.
