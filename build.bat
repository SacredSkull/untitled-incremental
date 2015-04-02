@echo off
REM For details use "xsd2code -h" in cmd

echo Generating Research...
"%~dp0\xsd2code\xsd2code.exe" "%~dp0\Assets\Scripts\XML\Research.xsd"
echo Done.
echo Generating Part...
"%~dp0\xsd2code\xsd2code.exe" "%~dp0\Assets\Scripts\XML\Part.xsd"
echo Done.
echo Generating Project...
"%~dp0\xsd2code\xsd2code.exe" "%~dp0\Assets\Scripts\XML\Project.xsd"
echo Done.
echo Generating Save...
"%~dp0\xsd2code\xsd2code.exe" "%~dp0\Assets\Scripts\XML\Save.xsd"
echo Done.
REM "%~dp0xsd2code\fart.exe" "%~dp0\Assets\Scripts\XML\Research.designer.cs" "using System.Collections.Generic" "using System.Collections.Generic;\n#pragma warning disable 414, 169"
pause
