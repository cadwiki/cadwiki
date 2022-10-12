# nuget-examples-start-here
The nuget example repos now have "start here" comments.
I often use these comments as search strings to remind myself what I need to change when using these packages in my existing apps.   
Many thanks to Eslam Gamal for testing out the code and providing feedback on what types of documentation is needed for someone starting out.  


[start here comments for VbNetApp](https://github.com/cadwiki/cadwiki-nuget-examples/commit/27085b2417b5d0e57e71c71f137aff897a82de21)

[start here comments for CSharpApp](https://github.com/cadwiki/cadwiki-nuget-examples/commit/d66f7c91145cea82583e9a8164e315dc9a848a5d)

To get started with this nuget package I usually only need to edit 3 items in my current app:   

1. Add a static AutoCADAppDomainDllReloader variable to the IExtensionApplication class 
1. Modify the IExtensionApplication Initialize  
1. Modify the IExtensionApplication Terminate  
1. Create a RibbonButton to reload a dll
1. Modify all other existing ribbon buttons to use UiRouter and GenericCommandHandler

## Add a static AutoCADAppDomainDllReloader variable to the IExtensionApplication class

```
Vbnet example
Public Shared AcadAppDomainDllReloader As New AutoCADAppDomainDllReloader

CSharpExample
public static AutoCADAppDomainDllReloader AcadAppDomainDllReloader = new AutoCADAppDomainDllReloader();
```

## Modify the IExtensionApplication Initialize  
This is cookie cutter copy and paste.  
No modification needed.  
Remove any method calls the existing application doesn't have.  
UiRibbon.Tabs.TabCreator.AddTabs(doc) sets up the UiRibbon with all the tabs.  
```
VbNet
    Public Sub Initialize() Implements IExtensionApplication.Initialize
        'This Event Handler allows the IExtensionApplication to Resolve any Assemblies
        'The AssemblyResolve method finds the correct assembly in the AppDomain when there are multiple assemblies
        'with the same name and differing version number
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf AutoCADAppDomainDllReloader.AssemblyResolve
        Dim iExtensionAppAssembly As Assembly = Assembly.GetExecutingAssembly
        Dim iExtensionAppVersion As Version = cadwiki.NetUtils.AssemblyUtils.GetVersion(iExtensionAppAssembly)
        AcadAppDomainDllReloader.Configure(iExtensionAppAssembly)
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        doc.Editor.WriteMessage(vbLf & "App " & iExtensionAppVersion.ToString & " initialized...")
        doc.Editor.WriteMessage(vbLf)
        UiRibbon.Tabs.TabCreator.AddTabs(doc)
        BusinessLogic.App.Initialize()
    End Sub

CSharp
        public void Initialize()
        {
            // This Event Handler allows the IExtensionApplication to Resolve any Assemblies
            // The AssemblyResolve method finds the correct assembly in the AppDomain when there are multiple assemblies
            // with the same name and differing version number
            AppDomain.CurrentDomain.AssemblyResolve += AutoCADAppDomainDllReloader.AssemblyResolve;
            var iExtensionAppAssembly = Assembly.GetExecutingAssembly();
            var iExtensionAppVersion = cadwiki.NetUtils.AssemblyUtils.GetVersion(iExtensionAppAssembly);
            AcadAppDomainDllReloader.Configure(iExtensionAppAssembly);
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            doc.Editor.WriteMessage(Constants.vbLf + "App " + iExtensionAppVersion.ToString() + " initialized...");
            doc.Editor.WriteMessage(Constants.vbLf);
            UiRibbon.Tabs.TabCreator.AddTabs(doc);
            BusinessLogic.App.Initialize();
        }
```

## Modify the IExtensionApplication Terminate  
This set is optional, however it is a good idea to terminate the AcadAppDomainDllReloader when the .Net application terminates.  

```
Vbnet
    Public Sub Terminate() Implements IExtensionApplication.Terminate
        AcadAppDomainDllReloader.Terminate()
    End Sub

CSharp
        public void Terminate()
        {
            AcadAppDomainDllReloader.Terminate();
        }
```

## Create a RibbonButton to reload the dll  
For this section I would just copy paste this exact button definition and add it to one of the existing Ribbon tabs.  
```
Vbnet
        Private Shared Function CreateReloadCountButton(exeName As String) As RibbonButton
            Dim button As RibbonButton = New RibbonButton()
            button.Name = "ReloadCount"
            button.ShowText = True
            button.Text = " Reload Count: " + App.AcadAppDomainDllReloader.GetReloadCount().ToString()
            button.Size = RibbonItemSize.Standard
            button.CommandHandler = New DllReloadClickCommandHandler()
            button.ToolTip = "Reload the " + exeName + " dll into AutoCAD"

            Dim uiRouter As UiRouter = New UiRouter(
                "assemblyName: not used by DllReloadClickCommandHandler",
                "fullClassName: not used by DllReloadClickCommandHandler",
                "methodName: not used by DllReloadClickCommandHandler",
                Nothing,
                App.AcadAppDomainDllReloader,
                Assembly.GetExecutingAssembly())
            button.CommandParameter = uiRouter
            Return button
        End Function
        
CSharp
        private static RibbonButton CreateReloadCountButton(string exeName)
        {
            var button = new RibbonButton();
            button.Name = "ReloadCount";
            button.ShowText = true;
            button.Text = " Reload Count: " + App.AcadAppDomainDllReloader.GetReloadCount().ToString();
            button.Size = RibbonItemSize.Standard;
            button.CommandHandler = new DllReloadClickCommandHandler();
            button.ToolTip = "Reload the " + exeName + " dll into AutoCAD";
            object[] parameters = new object[] { };

            var uiRouter = new UiRouter(
                "assemblyName: not used by DllReloadClickCommandHandler",
                "fullClassName: not used by DllReloadClickCommandHandler",
                "methodName: not used by DllReloadClickCommandHandler", 
                parameters, 
                App.AcadAppDomainDllReloader, 
                Assembly.GetExecutingAssembly()
            );
            button.CommandParameter = uiRouter;
            return button;
        }
```
## Modify all other existing ribbon buttons to use UiRouter and GenericCommandHandler  
The UiRouters below call the following method:  
AssemblyName: BusinessLogic  
FullClassName: BusinessLogic.Commands.HelloFromCadWiki  
MethodName: Run  

At this point, I need to implement this method below. 
I usually copy and paste, then change the hard coded strings to point to a different method I want to call.

```
Vbnet
Shared Function CreatHelloButton() As RibbonButton
            Dim ribbonButton As RibbonButton = New RibbonButton()
            ribbonButton.Name = "Hello"
            ribbonButton.ShowText = True
            ribbonButton.Text = "Hello"
            ribbonButton.Size = RibbonItemSize.Standard
            'start here 5 - UiRouter
            'The UiRouter contains all the information necessary for the AutoCADAppDomainDllReloader to
            'parse a dll in the current app domain, and call a method
            'the AutoCADAppDomainDllReloader will call a method from the most recently reloaded dll
            Dim uiRouter As UiRouter = New UiRouter(
                "BusinessLogic",
                "BusinessLogic.Commands.HelloFromCadWiki",
                "Run",
                Nothing,
                App.AcadAppDomainDllReloader,
                Assembly.GetExecutingAssembly())
            'start here 6 - RibbonButton CommandParameter = uiRouter
            'the UiRouter is stored on the ribbonButton.CommandParameter
            ribbonButton.CommandParameter = uiRouter
            'start here 7 - GenericClickCommandHandler
            'the GenericClickCommandHandler handles all Execute calls by utilizing the CommandParameter above
            ribbonButton.CommandHandler = New GenericClickCommandHandler(Application.DocumentManager.MdiActiveDocument)
            ribbonButton.ToolTip = "Click to run HelloFromCadWiki"
            Return ribbonButton
        End Function

CSharp

public static RibbonButton CreatHelloButton()
        {
            var ribbonButton = new RibbonButton();
            ribbonButton.Name = "Hello";
            ribbonButton.ShowText = true;
            ribbonButton.Text = "Hello";
            ribbonButton.Size = RibbonItemSize.Standard;
            //start here 5 - UiRouter
            //The UiRouter contains all the information necessary for the AutoCADAppDomainDllReloader to
            //parse a dll in the current app domain, and call a method
            //the AutoCADAppDomainDllReloader will call a method from the most recently reloaded dll
            var uiRouter = new UiRouter(
                "BusinessLogic",
                "BusinessLogic.Commands.HelloFromCadWiki", 
                "Run", 
                null, 
                App.AcadAppDomainDllReloader, 
                Assembly.GetExecutingAssembly());
            //start here 6 - RibbonButton CommandParameter = uiRouter
            //the UiRouter is stored on the ribbonButton.CommandParameter
            ribbonButton.CommandParameter = uiRouter;
            //start here 7 - GenericClickCommandHandler
            //the GenericClickCommandHandler handles all Execute calls by utilizing the CommandParameter above
            ribbonButton.CommandHandler = new GenericClickCommandHandler();
            ribbonButton.ToolTip = "Click to run HelloFromCadWiki";
            return ribbonButton;
        }
        
```
