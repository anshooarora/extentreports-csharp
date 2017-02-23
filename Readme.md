# ExtentAPI 3.x .NET

### Samples

 * <a href='http://extentreports.com/os/3/extent.html'>Standard</a>
 * <a href='http://extentreports.com/os/3/bdd.html'>BDD</a>

### Shortcuts

#### Views

```
t - test-view
c - category-view
x - exception-view
d - dashboard
```

#### Filters

```
p - show passed tests
e - show error tests
f - show failed tests
s - show skipped tests
w - show warning tests
esc - clear filters
```

#### Scroll

```
down-arrow - scroll down
up-arrow - scroll up
```

### Starting and Attaching Reporters

```
var htmlReporter = new ExtentHtmlReporter("filePath");

var extentxReporter = new ExtentXReporter();
extentxReporter.Configuration().ReportName = "Build123";
extentxReporter.Configuration().ProjectName = "MyProject";
extentxReporter.Configuration().ServerURL = "http://localhost:1337/";

var extent = new ExtentReports();
extent.AttachReporter(htmlReporter, extentxReporter);
```

### Creating Tests

```
var test = extent.CreateTest("My First Test");
test.Log(Status.Pass, "pass");
// or, shorthand:
test.Pass("pass");
```

The above can also be written in a single line:

```
extent.CreateTest("My First Test").Pass("pass");
```

### Logs

```
var test = extent.CreateTest("TestName");
test.Log(Status.Pass, "pass");
test.Pass("pass");

test.Log(Status.Fail, "fail");
test.Fail("fail");
```

#### Logging exceptions

To log exceptions, simply pass the exception.

> Note: doing this will also enable the defect/bug tab in the report.

```
Exception e;
test.Fail(e);
```

### Assign Category

Assigning a category will enable the category-view.

```
extent.CreateTest("My First Test").AssignCategory("Category").Pass("pass");
```

### Create Nodes

```
var test = extent.CreateTest("Test With Nodes");

var child = test.CreateNode("Node 1").AssignCategory("Nodes");
child.Pass("pass");

test.CreateNode("Node 2").Warning("warning");
```

### Screenshots

To attach screenshots to test, use:

```
extent.CreateTest("Media").AddScreenCaptureFromPath("file.png").Fail("fail");
```

To attach screenshots to logs, use `MediaEntityBuilder`:

```
test.Fail("message", MediaEntityBuilder.CreateScreenCaptureFromPath("file.png").Build());
```

### BDD Style

```
// source: https://cucumber.io/docs/reference

// feature
var feature = extent.CreateTest<Feature>("Refund item");

// scenario
var scenario = feature.CreateNode<Scenario>("Jeff returns a faulty microwave");
scenario.CreateNode<Given>("Jeff has bought a microwave for $100").Pass("pass");
scenario.CreateNode<And>("he has a receipt").Pass("pass");
scenario.CreateNode<When>("he returns the microwave").Pass("pass");
scenario.CreateNode<Then>("Jeff should be refunded $100").Fail("fail");
```

If you do not want to deal with Gherkin classes, you can pass in strings:

```
// feature
var feature = extent.createTest(new GherkinKeyword("Feature"), "Refund item");

// scenario
ExtentTest scenario = feature.CreateNode(new GherkinKeyword("Scenario") , "Jeff returns a faulty microwave");
scenario.CreateNode(new GherkinKeyword("Given"), "Jeff has bought a microwave for $100").pass("pass");
scenario.CreateNode(new GherkinKeyword("And"), "he has a receipt").pass("pass");
scenario.CreateNode(new GherkinKeyword("When"), "he returns the microwave").pass("pass");
scenario.CreateNode(new GherkinKeyword("Then"), "Jeff should be refunded $100").fail("fail");
```

### Writing Results to File/Database

```
extent.Flush();
```

### Test-runner output

Passing any output from your test-runner to extent will enable the test-runner logs view.

```
var extent = new ExtentReports();

extent.AddTestRunnerLogs("log 1");
extent.AddTestRunnerLogs("<pre>Log 2</pre>");
extent.AddTestRunnerLogs("<h2>heading 2</h2>");
```

### Reporter Configuration

To access configuration of each reporter, use `Configuration()`:

```
htmlReporter.Configuration()
extentxReporter.Configuration()
```

To use external configuration files, use:

```
// if loading from an xml file
reporter.LoadConfig(xml-file);
```

#### ExtentHtmlReporter Configuration

```
// chart location (top, bottom)
htmlReporter.Configuration().TestViewChartLocation = ChartLocation.Top;

// show chart on report open
htmlReporter.Configuration().ChartVisibilityOnOpen = true;

// set theme
htmlReporter.Configuration().Theme = Theme.Standard;

// protocol for resources (http, https)
htmlReporter.Configuration().Protocol = Protocol.HTTPS;

htmlReporter.Configuration().ReportName = "report-name";
htmlReporter.Configuration().DocumentTitle = "doc-title";
htmlReporter.Configuration().Encoding = "utf-8";
htmlReporter.Configuration().JS = "js-string";
htmlReporter.Configuration().CSS = "css-string";
```

### Markup Helpers

A few helpers are provided to allow:

 * Code block
 * Table
 * Label

#### Code block

```
var code = "<xml>\n\t<node>\n\t\tText\n\t</node>\n</xml>";
var m = MarkupHelper.CreateCodeBlock(code);

test.Pass(m);
// or
test.Log(Status.Pass, m);
```

#### Label

```
var text = "extent";
var m = MarkupHelper.CreateLabel(text, ExtentColor.Blue);

test.Pass(m);
// or
test.Log(Status.Pass, m);
```
