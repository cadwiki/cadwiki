# New post  
cadwiki has released a free nuget package with this solution implemented.  
refer to this blog post for the free solution:  
[http://www.cadwiki.net/blogpost/cadwiki.DllReloader-nuget-package](http://www.cadwiki.net/blogpost/cadwiki.DllReloader-nuget-package)  


# How to reload a .dll into AutoCAD

It's true, reloading a .dll into AutoCAD without closing the program is possible.  
It requires some finessing and the result is well worth it.  
Imagine being able to make changes in Visual Studio and reload them without closing AutoCAD.  
I use this workflow in my own development process and it works great for my own agile workflow.   

# Video demo
In this video I demonstrate that reloading allowed me to change my Ui elements from green to orange.  
As well as change the Ui title text.  
This reloader solution is designed to work excellently with Visual Studio dev workflows.  
No more closing CAD before seeing changes to a recompiled .dll.    
Also, no more MSBuild errors due to AutoCAD locking a .dll that needs to be deleted by visual studio during a rebuild.   
Check it out.  

<style>
    .video {
        object-fit: contain;
    }
</style>

<video controls>
  <source  src="/videos/How_to_reload_a_.dll_into_AutoCAD_with_changes_from_Visual_Studio.mp4" type="video/mp4">
Your browser does not support the video tag.
</video>
  




# Contents
  
These contents are ordered from least to the most complex.  
 
1. [Step by Step "How I" created this solution](#step-by-step-how-I) 
1. [Why can't I reload the same .dll into AutoCAD out of the box?](#why) 
1. [Command definitions](#command-defs) 
1. [File locks](#file-locks) 
1. [App domains](#app-domains) 
1. [References](#references) 

 
<div id="step-by-step-how-I"></div>

## Step by Step "How I" created this solution
This first section is a step by step How I created this solution.  
Also, keep in mind I already have this full solution available through GitHub

1. Create a build file that will **'bag and tag'** all the dlls into a new subfolder for each build.  
    * This solves the file lock issue when netloading/reloading
1. Create a method that will call a function from the AutoCAD C++ api to unload commands by groupname.
    * This makes sure that before a dll is reloaded, all the commands can be removed.
1. Create a UiRibbon button that will handle reloading all the necessary .dlls into AutoCAD's AppDomain
    * This is the magic of reloading .dlls
    * The UiRibbon button handles all the logic needed to reload .dlls
	* The logic includes skipping any and all dlls that are already in the app domain with the same, or newer version number
 
<div id="why"></div>

## Why can't I reload the same .dll into AutoCAD out of the box?
Put quite simply, Autodesk has not built this feature yet.  
And it doesn't seem like they will anytime soon.    



<div id="command-defs"></div>


## Command definitions
![This is a alt text.](/images/c-plus-plus.png "")  
The first concept topic is Command definitions.  
**An important note before getting started on command definitions.**  
**This reloader solution makes use of custom AutoCAD Ribbons instead of AutoCAD command definitions.**    
**At the time of writing, I have not figured out how to use command definitions in a reloadable way.**  
Out of necessity, my current solution is just to remove command definitions and use the UiRibbon to drive my AutoCAD logic.  
Maybe in the future, I will be able to discover a way to reload command definitions.  
For now, the UiRibbon seems to be the way to go.  

So, for this part of the solution I will post the code used to remove command defs.   
The AutoCAD .Net Api does not have a way to unload command definitions.  
However, there is a way to do this in C++ using some library code from AutoCAD's api.  
To do this, I made sure to give all my commands a command group name.  
The command group name is how the C++ code unloads the command.  
**Here's C++ code I use to unload my commands.**  
```
#include "stdafx.h"
#include <aced.h> 
#include <rxregsvc.h> 
#include <string>

// ACHAR is a typedef (made by Autodesk in file AdAChar.h) of wchar_t.So the question is how to convert a char to wchar_t.
// https://stackoverflow.com/questions/1195675/convert-a-char-to-stdstring
// wchar_t to string
// https://stackoverflow.com/questions/27720553/conversion-of-wchar-t-to-string
extern "C" Acad::ErrorStatus removeCommandGroup(const ACHAR* groupName)
{
    acutPrintf(_T("\nAttempting to remove command group: "));
    std::wstring wStringGroupName(groupName);
    std::string stringGroupName(wStringGroupName.begin(), wStringGroupName.end());
    acutPrintf(groupName);
    return acedRegCmds->removeGroup(groupName);
}


extern "C" Acad::ErrorStatus removeCommand(const ACHAR * cmdGroupName, const ACHAR * cmdGlobalName)

{
    acutPrintf(_T("\r\nAttempting to remove command."));
    acutPrintf(_T("\r\nCommand group: "));
    acutPrintf(cmdGroupName);
    acutPrintf(_T("\r\nCommand global name: "));
    acutPrintf(cmdGlobalName);
    return acedRegCmds->removeCmd(cmdGroupName, cmdGlobalName);
}
```
**And here is the code needed to call unmanaged C++ code from Vbnet**  
**Note, the dll containing the C++ code must be in the same directory as the Vbnet .dll that will call it**  
```
Public Declare Auto Function RemoveCommand Lib "AcRemoveCmdGroup.dll" Alias "removeCommand" (<MarshalAs(UnmanagedType.LPWStr)> ByVal groupName As StringBuilder, <MarshalAs(UnmanagedType.LPWStr)> ByVal commandGlobalName As StringBuilder) As Integer

Dim wasCommandUndefined As Integer = RemoveCommand(groupName, commandGlobalName)
If wasCommandUndefined = 0 Then
	doc.Editor.WriteMessage(vbLf & "Command successfully removed: " + commandGlobalName.ToString)
Else
	doc.Editor.WriteMessage(vbLf & "Undefine command failed: " & commandLineMethodString & ".")
End If
```


<div id="file-locks"></div>

## File locks
![This is a alt text.](/images/lock.png "File locks...so fun")  
Second concept topic is file locks.  
When I compile source code with Visual Studio, the compiled code will end up in some directory such as:  
```
C:\Temp\bin\x64\Debug\MyNewShinyApp.dll
```
If I netload "C:\Temp\bin\x64\Debug\MyNewShinyApp.dll" into AutoCAD, the file will become locked.  
And of course, **locking this particular file** is a huge problem for dev workflows.  
Because, if I try to rebuild the solution while the above file is locked, I get this fun error message below.  
**Example output from MSBuild stuck on a file lock:**
```
>C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\Microsoft.Common.CurrentVersion.targets(4679,5): 
error MSB3021: Unable to copy file "C:\Temp\bin\x64\Debug\MyNewShinyApp.dll" to "bin\x64\Debug\MyNewShinyApp.dll ". 
The process cannot access the file 'bin\x64\Debug\MyNewShinyApp.dll ' because it is being used by another process.
```
**And the eventual build failure after the lock causes MSBuild to get stuck**
```
Warning		Could not copy "C:\Temp\MyNewShinyApp.dll" to "bin\x64\Debug\MyNewShinyApp.dll". Beginning retry 1 in 1000ms. 
The process cannot access the file 'bin\x64\Debug\MyNewShinyApp.dll' because it is being used by another process. 
The file is locked by: "AutoCAD Application (35968)"	CadApp			
```
**The solution is to create a MSBuild file that can 'bag and tag' every release into a subfolder.**  
The MSBuild file can autoincrement all the AssemblyInfo.vb files and then create a subfolder.  
If I use DateFormat and AutoIncrement on the build info,   
the directory will look something like this on 01/18/2022 after two builds:  
```
First, build of the day
C:\Temp\MyNewShinyApp.dll          <- visual studio needs this one to stay unlocked  
C:\Temp\v1.0.0118.0\MyNewShinyApp.dll   <- so netload this one instead 
```
```
Second build of the day
C:\Temp\MyNewShinyApp.dll          <- visual studio needs this one to stay unlocked 
C:\Temp\v1.0.0118.0\MyNewShinyApp.dll   <- locked,because it's netloaded into AutoCAD
C:\Temp\v1.0.0118.1\MyNewShinyApp.dll   <- I reload this into the AppDomain to test recently compiled changes
```
MSBuild plays a crucial role in this solution.  
It keeps Visual Studio free and clear of AutoCAD's file locks due to netloading.



<div id="app-domains"></div>

## App domains
![This is a alt text.](/images/appdomain.png "")  
The final topic is app domains, and this is where the magic happens.  
When a dll is netloaded, AutoCAD puts the .dll contents into AutoCAD's app domain.  
After that happens, the same dll cannot be netloaded again into AutoCAD's session.  
However, I can go directly to the AppDomain and load the dll myself.  

```
Dim assemblyBytes As Byte() = System.IO.File.ReadAllBytes(pathToSomeDll)
Dim reloadedAssembly As Assembly = AppDomain.CurrentDomain.Load(assemblyBytes)
```

## App domains woes
Now, there are some more subtleties and things to keep in mind with AppDomains.

1. **Bad things happen when trying to load multiple dlls with the same file name and assembly version**  
    * So I had to make sure to handle the condition when a user tries to reload the same versioned assembly.   
    * **If I load a new dll into an AppDomain that already has a old copy  
    of the same dll, I made sure that the new copy and old copy have different assembly versions**.
    * Otherwise my code might encounter strange errors similar to this one I spent 2 days trying to solve:
    * **Here is an example exception thrown in AutoCAD when trying to use a WPF Window  
AutoCAD can't find the WPF resource since there are two .dlls   
(with the same assembly version)**
    * Root cause: The the dlls have with the same resource URI to the .xaml file
    
```
System.Exception: The component 'SomeNamespace.Window' does not have a resource identified by the URI '/SomeNamespace;component/Window.xaml'.
```
![This is a alt text.](/images/wpf-uri-error.png "")  

1. AutoCAD's .Net API will not do anything for me automatically once I load a .dll into it's AppDomain
    * Just because my ShinyNewApp.dll has been reloaded into the current AppDomain does not mean it will work as intended.
    * I build my own architecture that support's routing between  .dlls in the AppDomain during Ribbon button clicks
1. AutoCAD's .Net API does not have a way to unload previous  
    Command definitions when loading a dll into its AppDomain  
    * I had to unload my commands via C++ AutoCAD API

## Conclusion
It is possible to reload a .dll into AutoCAD without closing or restarting CAD.  
I've used this in my development workflows and it has saved me countless hours.   


<div id="references"></div>  

## References
Article describing what AutoCAD's AppDomain is  
[https://forums.autodesk.com/t5/net/application-domain-and-managed-arx/td-p/2606295](https://forums.autodesk.com/t5/net/application-domain-and-managed-arx/td-p/2606295)

Article describing how to use AppDomains   
[https://www.theswamp.org/index.php?topic=38675.0](https://www.theswamp.org/index.php?topic=38675.0)
 

Article showing a net reload utility that doesn't work so well  
This article was my inspiration for building a new tool that works better.  
[https://forums.autodesk.com/t5/net/net-reload-utility-for-visual-studio-download-here/td-p/3185104  ](https://forums.autodesk.com/t5/net/net-reload-utility-for-visual-studio-download-here/td-p/3185104  )

