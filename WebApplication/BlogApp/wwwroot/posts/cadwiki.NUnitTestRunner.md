# cadwiki.NUnitTestRunner  
I made a new addition to the CadDevTools package.  
The cadwiki.NUnitTestRunner is a package that let's me run automated tests within AutoCAD's appdomain.  
This package has helped increase my code quality and I wanted to share incase others find it useful.  
I will post more details next week.  
[https://github.com/cadwiki/cadwiki-nuget/tree/main/cadwiki-nuget/cadwiki.NUnitTestRunner](https://github.com/cadwiki/cadwiki-nuget/tree/main/cadwiki-nuget/cadwiki.NUnitTestRunner)  

Here's an example of how I created an AutoCAD workflow to run tests:  
[VbNet example](https://github.com/cadwiki/cadwiki-nuget-examples/blob/main/VbNetApp/MainApp/Workflows/NUnitTestRunner.vb) 
```
	Dim doc As Document = Application.DocumentManager.MdiActiveDocument
	Dim ed As Editor = doc.Editor
	Dim results As New Results.ObservableTestSuiteResults()
	Dim driver As New Ui.WpfDriver(results, regressionTestTypes)
	Dim window As Ui.WindowTestRunner = driver.GetWindow()
	Application.ShowModelessWindow(window)
	driver.ExecuteTests()
``` 
[CSharp example](https://github.com/cadwiki/cadwiki-nuget-examples/blob/main/CSharpApp/MainApp/Workflows/NUnitTestRunner.cs) 
```
	var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
	var ed = doc.Editor;
	var results = new cadwiki.NUnitTestRunner.Results.ObservableTestSuiteResults();
	var driver = new cadwiki.NUnitTestRunner.Ui.WpfDriver(ref results, regressionTestTypes);
	var window = driver.GetWindow();
	Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowModelessWindow(window);
	driver.ExecuteTests();
```
 

Here's how I get an array of types that contain tests to run:  
[Vbnet Example](https://github.com/cadwiki/cadwiki-nuget-examples/blob/main/VbNetApp/MainApp/UiRibbon/Panels/Test.vb)  
```
    Dim allRegressionTests As Type = GetType(RegressionTests.RegressionTests)
    Dim allRegressionTestTypes As Type() = {allRegressionTests}
```
[CSharp Example](https://github.com/cadwiki/cadwiki-nuget-examples/blob/main/CSharpApp/MainApp/UiRibbon/Panels/Test.cs)  
```
    var allRegressionTests = typeof(RegressionTests.RegressionTests);
    var allIntegrationTests = typeof(IntegrationTests.Tests);
    var allRegressionTestTypes = new[] { allRegressionTests, allIntegrationTests };
```
