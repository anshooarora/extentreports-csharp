using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Configuration;

using MongoDB.Driver;
using MongoDB.Bson;

using AventStack.ExtentReports.Model;
using AventStack.ExtentReports.Reporter.Configuration;
using AventStack.ExtentReports.Configuration;
using AventStack.ExtentReports.Reporter.Configuration.Defaults;
using AventStack.ExtentReports.MediaStorageNS;

namespace AventStack.ExtentReports.Reporter
{
    /// <summary>
    /// ExtentXReporter is a NoSQL database reporter (MongoDB by default), which updates information in
    /// the database which is then used by the ExtentX server to display in-depth analysis. 
    /// </summary>
    public class ExtentXReporter : AbstractReporter, ReportAppendable
    {
        private const string DEFAULT_PROJECT_NAME = "Default";

        private bool _appendExistingReport = false;
        private string _url;

        private ObjectId _reportId;
        private ObjectId _projectId;

        private MongoClient _mongoClient;
        private IMongoDatabase _db;
        private IMongoCollection<BsonDocument> _projectCollection;
        private IMongoCollection<BsonDocument> _reportCollection;
        private IMongoCollection<BsonDocument> _testCollection;
        private IMongoCollection<BsonDocument> _logCollection;
        private IMongoCollection<BsonDocument> _exceptionCollection;
        private IMongoCollection<BsonDocument> _mediaCollection;
        private IMongoCollection<BsonDocument> _categoryCollection;
        private IMongoCollection<BsonDocument> _authorCollection;
        private IMongoCollection<BsonDocument> _categoryTestsTestCategories;
        private IMongoCollection<BsonDocument> _authorTestsTestAuthors;

        private MediaStorage _media;
        private ExtentXReporterConfiguration _reporterConfig;
        private ConfigManager _configManager;

        private Dictionary<string, ObjectId> _categoryNameObjectIdCollection;
        private Dictionary<string, ObjectId> _exceptionNameObjectIdCollection;

        /// <summary>
        /// MongoDB id of the report
        /// </summary>
        public ObjectId ReportId
        {
            get
            {
                return _reportId;
            }
        }

        /// <summary>
        /// MongoDB id of the project
        /// </summary>
        public ObjectId ProjectId
        {
            get
            {
                return _projectId;
            }
        }

        /// <summary>
        /// Connects to MongoDB default settings, localhost:27017
        /// </summary>
        public ExtentXReporter()
        {
            LoadDefaultConfig();
            _mongoClient = new MongoClient();
        }

        public ExtentXReporter(string host, int port = -1)
        {
            LoadDefaultConfig();
            var conn = "mongodb://" + host;
            conn += port > -1 ? ":" + port : "";
            _mongoClient = new MongoClient(conn);
        }

        /// <summary>
        /// Connects to MongoDB using a connection string.
        /// Example: mongodb://host:27017,host2:27017/?replicaSet=rs0
        /// </summary>
        /// <param name="connectionString"></param>
        public ExtentXReporter(string connectionString)
        {
            LoadDefaultConfig();
            _mongoClient = new MongoClient(connectionString);
        }

        public ExtentXReporter(MongoClientSettings settings)
        {
            LoadDefaultConfig();
            _mongoClient = new MongoClient(settings);
        }

        private void LoadDefaultConfig()
        {
            _configManager = new ConfigManager();
            _reporterConfig = new ExtentXReporterConfiguration();

            // load default settings
            foreach (SettingsProperty setting in ExtentXReporterSettings.Default.Properties)
            {
                var key = setting.Name;
                var value = ExtentXReporterSettings.Default.Properties[setting.Name].DefaultValue.ToString();

                var c = new Config(key, value);
                _configManager.AddConfig(c);
            }
        }

        public ExtentXReporterConfiguration Configuration()
        {
            return _reporterConfig;
        }

        public bool AppendExisting
        {
            get
            {
                return _appendExistingReport;
            }
            set
            {
                _appendExistingReport = value;
            }
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

        public override void Start()
        {
            foreach (KeyValuePair<string, string> entry in _reporterConfig.UserConfiguration)
            {
                var c = new Config(entry.Key, entry.Value);
                _configManager.AddConfig(c);
            }

            // database
            _db = _mongoClient.GetDatabase("extent");

            // collections
            _projectCollection = _db.GetCollection<BsonDocument>("project");
            _reportCollection = _db.GetCollection<BsonDocument>("report");
            _testCollection = _db.GetCollection<BsonDocument>("test");
            _logCollection = _db.GetCollection<BsonDocument>("log");
            _exceptionCollection = _db.GetCollection<BsonDocument>("exception");
            _mediaCollection = _db.GetCollection<BsonDocument>("media");
            _categoryCollection = _db.GetCollection<BsonDocument>("category");
            _authorCollection = _db.GetCollection<BsonDocument>("author");

            // many-to-many
            _categoryTestsTestCategories = _db.GetCollection<BsonDocument>("category_tests__test_categories");
            _authorTestsTestAuthors = _db.GetCollection<BsonDocument>("author_tests__test_authors");

            SetupProject();
        }

        private void SetupProject()
        {
            string projectName = _configManager.GetValue("projectName");

            if (string.IsNullOrEmpty(projectName))
                projectName = DEFAULT_PROJECT_NAME;

            var document = new BsonDocument
            {
                { "name", projectName }
            };

            var bsonProject = _projectCollection.Find(document).FirstOrDefault();
            if (bsonProject != null)
            {
                _projectId = bsonProject["_id"].AsObjectId;
            }
            else
            {
                _projectCollection.InsertOne(document);
                _projectId = document["_id"].AsObjectId;
            }

            SetupReport(projectName);
        }

        private void SetupReport(string projectName)
        {
            string reportName = _configManager.GetValue("reportName");

            if (string.IsNullOrEmpty(reportName))
                reportName = "[" + projectName + "] " + DateTime.Now.ToString();

            BsonDocument document;

            var reportId = _configManager.GetValue("reportId");

            // if extent is started with [AppendExisting = false] and ExtentX is used,
            // use the same report ID for the 1st report run and update the database for
            // the corresponding report-ID
            if (!string.IsNullOrEmpty(reportId) && AppendExisting)
            {
                document = new BsonDocument
                {
                    { "_id", new ObjectId(reportId) }
                };

                var bsonReport = _reportCollection.Find(document).FirstOrDefault();

                if (bsonReport != null)
                {
                    _reportId = bsonReport["_id"].AsObjectId;
                    return;
                }
            }

            // if [AppendExisting = true] or the file does not exist, create a new
            // report-ID and assign all components to it
            document = new BsonDocument
            {
                { "name", reportName },
                { "project", _projectId },
                { "startTime", StartTime }
            };

            _reportCollection.InsertOne(document);
            _reportId = document["_id"].AsObjectId;
        }

        public override void Stop() { }

        public override void Flush()
        {
            if (TestList == null || TestList.Count == 0)
                return;

            var filter = Builders<BsonDocument>.Filter.Eq("_id", _reportId);
            var update = Builders<BsonDocument>.Update
                .Set("endTime", DateTime.Now)
                .Set("duration", DateTime.Now - StartTime)
                .Set("parentLength", SessionStatusStats.ParentCount)
                .Set("passParentLength", SessionStatusStats.ParentPass)
                .Set("failParentLength", SessionStatusStats.ParentFail)
                .Set("fatalParentLength", SessionStatusStats.ParentFatal)
                .Set("errorParentLength", SessionStatusStats.ParentError)
                .Set("warningParentLength", SessionStatusStats.ParentWarning)
                .Set("skipParentLength", SessionStatusStats.ParentSkip)
                .Set("exceptionsParentLength", SessionStatusStats.ChildExceptions)

                .Set("childLength", SessionStatusStats.ChildCount)
                .Set("passChildLength", SessionStatusStats.ChildPass)
                .Set("failChildLength", SessionStatusStats.ChildFail)
                .Set("fatalChildLength", SessionStatusStats.ChildFatal)
                .Set("errorChildLength", SessionStatusStats.ChildError)
                .Set("warningChildLength", SessionStatusStats.ChildWarning)
                .Set("skipChildLength", SessionStatusStats.ChildSkip)
                .Set("infoChildLength", SessionStatusStats.ChildInfo)
                .Set("exceptionsChildLength", SessionStatusStats.ChildExceptions)

                .Set("grandChildLength", SessionStatusStats.GrandChildCount)
                .Set("passGrandChildLength", SessionStatusStats.GrandChildPass)
                .Set("failGrandChildLength", SessionStatusStats.GrandChildFail)
                .Set("fatalGrandChildLength", SessionStatusStats.GrandChildFatal)
                .Set("errorGrandChildLength", SessionStatusStats.GrandChildError)
                .Set("warningGrandChildLength", SessionStatusStats.GrandChildWarning)
                .Set("skipGrandChildLength", SessionStatusStats.GrandChildSkip)
                .Set("exceptionsGrandChildLength", SessionStatusStats.GrandChildExceptions);

            _reportCollection.UpdateOne(filter, update);
        }

        public override void OnTestStarted(Test test)
        {
            var document = new BsonDocument
            {
                { "project", _projectId },
                { "report", _reportId },
                { "level", test.Level },
                { "name", test.Name },
                { "status", test.Status.ToString().ToLower() },
                { "description", test.Description == null ? "" : test.Description },
                { "startTime", test.StartTime },
                { "endTime", test.EndTime },
                { "bdd", test.IsBehaviorDrivenType },
                { "childNodesLength", test.NodeContext().Count }
            };

            if (test.IsBehaviorDrivenType)
                document.Add("bddType", test.BehaviorDrivenType.ToString());

            _testCollection.InsertOne(document);

            test.ObjectId = document["_id"].AsObjectId;
        }

        public override void OnNodeStarted(Test node)
        {
            var document = new BsonDocument
            {
                { "parent", node.Parent.ObjectId },
                { "parentName", node.Parent.Name },
                { "project", _projectId },
                { "report", _reportId },
                { "level", node.Level },
                { "name", node.Name },
                { "status", node.Status.ToString().ToLower() },
                { "description", node.Description == null ? "" : node.Description },
                { "startTime", node.StartTime },
                { "endTime", node.EndTime },
                { "bdd", node.IsBehaviorDrivenType },
                { "childNodesLength", node.NodeContext().Count }
            };

            if (node.IsBehaviorDrivenType)
                document.Add("bddType", node.BehaviorDrivenType.ToString());

            _testCollection.InsertOne(document);

            node.ObjectId = document["_id"].AsObjectId;

            UpdateTestInfoWithNodeDetails(node.Parent);
        }

        private void UpdateTestInfoWithNodeDetails(Test test)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", test.ObjectId);
            var update = Builders<BsonDocument>.Update.Set("childNodesLength", test.NodeContext().Count);

            _testCollection.UpdateOne(filter, update);
        }

        public override void OnLogAdded(Test test, Log log)
        {
            var document = new BsonDocument
            {
                { "test", test.ObjectId },
                { "project", _projectId },
                { "report", _reportId },
                { "testName", test.Name },
                { "sequence", log.Sequence },
                { "status", log.Status.ToString().ToLower() },
                { "timestamp", log.Timestamp },
                { "details", log.Details }
            };

            _logCollection.InsertOne(document);

            if (test.HasException())
            {
                if (_exceptionNameObjectIdCollection == null)
                    _exceptionNameObjectIdCollection = new Dictionary<string, ObjectId>();

                var ex = test.ExceptionInfo;

                document = new BsonDocument
                {
                    { "report", _reportId },
                    { "project", _projectId },
                    { "name", ex.Name }
                };

                var findResult = _exceptionCollection.Find(document).FirstOrDefault();

                if (!_exceptionNameObjectIdCollection.ContainsKey(ex.Name))
                {
                    if (findResult != null)
                    {
                        _exceptionNameObjectIdCollection.Add(ex.Name, findResult["_id"].AsObjectId);
                    }
                    else
                    {
                        document = new BsonDocument
                    {
                        { "project", _projectId },
                        { "report", _reportId },
                        { "name", ex.Name },
                        { "stacktrace", ex.StackTrace },
                        { "testCount", 0 }
                    };

                        _exceptionCollection.InsertOne(document);

                        var exId = document["_id"].AsObjectId;

                        document = new BsonDocument
                        {
                            { "_id", exId }
                        };
                        findResult = _exceptionCollection.Find(document).FirstOrDefault();

                        _exceptionNameObjectIdCollection.Add(ex.Name, exId);
                    }
                }

                var testCount = ((int)(findResult["testCount"])) + 1;
                var filter = Builders<BsonDocument>.Filter.Eq("_id", findResult["_id"].AsObjectId);
                var update = Builders<BsonDocument>.Update.Set("testCount", testCount);

                _exceptionCollection.UpdateOne(filter, update);
            }

            EndTestRecursive(test);
        }

        private void EndTestRecursive(Test test)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", test.ObjectId);
            var update = Builders<BsonDocument>.Update
                .Set("status", test.Status.ToString().ToLower())
                .Set("endTime", test.EndTime)
                .Set("duration", test.RunDuration.ToString())
                .Set("categorized", test.HasCategory());

            _testCollection.UpdateOne(filter, update);

            if (test.Level > 0)
                EndTestRecursive(test.Parent);
        }

        public override void OnAuthorAssigned(Test test, Author author)
        {
            var document = new BsonDocument
            {
                { "tests", test.ObjectId },
                { "project", _projectId },
                { "report", _reportId },
                { "name", author.Name },
                { "status", test.Status.ToString().ToLower() },
                { "testName", test.Name }
            };

            _authorCollection.InsertOne(document);

            var authorId = document["_id"].AsObjectId;

            document = new BsonDocument
            {
                { "test_authors", test.ObjectId },
                { "author_tests", authorId },
                { "author", author.Name },
                { "test", test.Name }
            };

            _authorTestsTestAuthors.InsertOne(document);
        }

        public override void OnCategoryAssigned(Test test, Category category)
        {
            if (_categoryNameObjectIdCollection == null)
                _categoryNameObjectIdCollection = new Dictionary<string, ObjectId>();

            BsonDocument document;
            ObjectId categoryId;

            if (!_categoryNameObjectIdCollection.ContainsKey(category.Name))
            {
                document = new BsonDocument
                {
                    { "report", _reportId },
                    { "project", _projectId },
                    { "name", category.Name }
                };

                var bsonCategory = _categoryCollection.Find(document).FirstOrDefault();

                if (bsonCategory != null)
                {
                    _categoryNameObjectIdCollection.Add(category.Name, bsonCategory["_id"].AsObjectId);
                }
                else
                {
                    document = new BsonDocument
                    {
                        { "tests", test.ObjectId },
                        { "project", _projectId },
                        { "report", _reportId },
                        { "name", category.Name },
                        { "status", test.Status.ToString().ToLower() },
                        { "testName", test.Name }
                    };

                    _categoryCollection.InsertOne(document);

                    categoryId = document["_id"].AsObjectId;
                    _categoryNameObjectIdCollection.Add(category.Name, categoryId);
                }
            }

            /* create association with category
             * tests (many) <-> categories (many)
             * tests and categories have a many to many relationship
             *   - a test can be assigned with one or more categories
             *   - a category can have one or more tests
             */
            document = new BsonDocument
            {
                { "test_categories", test.ObjectId },
                { "category_tests", _categoryNameObjectIdCollection[category.Name] },
                { "category", category.Name },
                { "test", test.Name }
            };

            _categoryTestsTestCategories.InsertOne(document);
        }

        public override void OnScreenCaptureAdded(Test test, ScreenCapture screenCapture)
        {
            StoreURL();
            screenCapture.ReportObjectId = _reportId;

            CreateMedia(test, screenCapture);
            InitMedia();
            _media.StoreMedia(screenCapture);
        }

        private void StoreURL()
        {
            if (_url == null)
                _url = _configManager.GetValue("serverUrl");

            if (string.IsNullOrEmpty(_url))
                throw new IOException("Server URL cannot be null, use ExtentXReporter.Configuration().ServerURL to specify where ExtentX is running.");
        }

        private void CreateMedia(Test test, Media media)
        {
            var document = new BsonDocument
            {
                { "test", test.ObjectId },
                { "project", _projectId },
                { "report", _reportId },
                { "testName", test.Name },
                { "sequence", media.Sequence },
                { "mediaType", media.MediaType.ToString().ToLower() }
            };

            _mediaCollection.InsertOne(document);

            var id = document["_id"].AsObjectId;
            media.ObjectId = id;
        }

        private void InitMedia()
        {
            if (_media == null)
            {
                _media = new MediaStorageManagerFactory().GetManager("http");
                _media.Init(_url);
            }
        }
    }
}
