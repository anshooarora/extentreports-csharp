using System;
using System.Collections.Generic;
using System.Linq;

using AventStack.ExtentReports.Model;
using AventStack.ExtentReports.Reporter.Configuration.Defaults;
using AventStack.ExtentReports.Resources.View;

using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.IO;
using System.Configuration;
using AventStack.ExtentReports.Configuration;
using System.Xml.Linq;
using AventStack.ExtentReports.Reporter.Configuration;

namespace AventStack.ExtentReports.Reporter
{
    /// <summary>
    /// The ExtentHtmlReporter creates a rich standalone HTML file. It allows several configuration options
    /// via the <code>Configuration()</code> method.
    /// </summary>
    public class ExtentHtmlReporter : BasicFileReporter, ReportAppendable
    {
        public bool AppendExisting { get; set; }

        public override DateTime StartTime { get; set; }
        public override List<Test> TestList { get; set; }
        public override TestAttributeTestContextProvider<Category> CategoryContext { get; set; }
        public override List<string> TestRunnerLogs { get; set; }
        public override SessionStatusStats SessionStatusStats { get; set; }
        public override SystemAttributeContext SystemAttributeContext { get; set; }
        public override ExceptionTestContextProvider ExceptionContext { get; set; }

        private ExtentHtmlReporterConfiguration _reporterConfig;
        private string _extentSource;
        private Assembly _assembly;
        private ConfigManager _configManager;

        public ExtentHtmlReporter(string filePath)
        {
            _filePath = filePath;
            _assembly = Assembly.GetExecutingAssembly();

            _configManager = new ConfigManager();
            _reporterConfig = new ExtentHtmlReporterConfiguration();

            // load default settings
            foreach (SettingsProperty setting in ExtentHtmlReporterSettings.Default.Properties)
            {
                var key = setting.Name;
                var value = ExtentHtmlReporterSettings.Default.Properties[setting.Name].DefaultValue.ToString();

                var c = new Config(key, value);
                _configManager.AddConfig(c);
            }
        }

        public ExtentHtmlReporterConfiguration Configuration()
        {
            return _reporterConfig;
        }

        public override void LoadConfig(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The file " + filePath + " was not found.");

            var xdoc = XDocument.Load(filePath, LoadOptions.None);
            if (xdoc == null)
            {
                throw new FileLoadException("Unable to configure report with the supplied configuration. Please check the input file and try again.");
            }

            LoadConfigFileContents(xdoc);
        }

        private void LoadConfigFileContents(XDocument xdoc)
        {
            foreach (var xe in xdoc.Descendants("configuration").First().Elements())
            {
                var key = xe.Name.ToString();
                var value = xe.Value;

                var c = new Config(key, value);
                _configManager.AddConfig(c);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Flush()
        {
            LoadUserConfig();

            _extentSource = Engine.Razor.RunCompile("index", typeof(ExtentHtmlReporter), this);
            File.WriteAllText(_filePath, _extentSource);
        }

        private void LoadUserConfig()
        {
            foreach (var pair in _reporterConfig.GetConfiguration())
            {
                var key = pair.Key;
                var value = pair.Value;

                var c = new Config(key, value);
                _configManager.AddConfig(c);
            }
        }

        public override void Start()
        {
            InitializeRazor();
            AddTemplates();
        }

        private void AddTemplates()
        {
            Engine.Razor.AddTemplate("logs", Views.TestRunnerLogs);
            Engine.Razor.AddTemplate("charts", Views.TestViewCharts);
            Engine.Razor.AddTemplate("test", Views.TestView);
            Engine.Razor.AddTemplate("standard", Views.Standard);
            Engine.Razor.AddTemplate("bdd", Views.Bdd);
            Engine.Razor.AddTemplate("dashboard", Views.DashboardView);
            Engine.Razor.AddTemplate("category", Views.CategoryView);
            Engine.Razor.AddTemplate("exception", Views.ExceptionView);
            Engine.Razor.AddTemplate("nav", Views.Nav);
            Engine.Razor.AddTemplate("head", Views.Head);
            Engine.Razor.AddTemplate("index", Views.Index);
        }

        private void InitializeRazor()
        {
            TemplateServiceConfiguration templateConfig = new TemplateServiceConfiguration();
            templateConfig.DisableTempFileLocking = true;
            templateConfig.EncodedStringFactory = new RawStringFactory();
            templateConfig.CachingProvider = new DefaultCachingProvider(x => { });
            var service = RazorEngineService.Create(templateConfig);
            Engine.Razor = service;
        }

        public override void Stop()
        {
            Flush();
            Engine.Razor = null;
        }

        public string GetReporterConfigurationSetting(string key)
        {
            return _configManager.GetValue(key);
        }
    }
}
