using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using MongoDB.Driver;
using MongoDB.Bson;

using AventStack.ExtentReports.Model;
using AventStack.ExtentReports.Reporter.Configuration;
using AventStack.ExtentReports.Configuration;
using AventStack.ExtentReports.MediaStorageNS;

namespace AventStack.ExtentReports.Reporter
{
    public class KlovReporter : AbstractReporter, ReportAppendable
    {
        private const string DEFAULT_PROJECT_NAME = "Default";
        private const string _DB = "klov";

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

        private MediaStorage _media;
        private ExtentXReporterConfiguration _reporterConfig;
        private ConfigManager _configManager;

        private Dictionary<string, ObjectId> _categoryNameObjectIdCollection;
        private Dictionary<string, ObjectId> _exceptionNameObjectIdCollection;

        private string _reportName;
        private string _projectName;


        public string ReportName
        {
            get
            {
                return _reportName;
            }
            set
            {
                _reportName = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
            }
        }

        public string KlovUrl
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        /// <summary>
        /// MongoDB id of the report
        /// </summary>
        public ObjectId ReportId
        {
            get
            {
                return _reportId;
            }
            private set
            {
                _reportId = value;
            }
        }

        public ObjectId LastRunReportId
        {
            get
            {
                CreateCollections();
                LoadCurrentConfig();

                BsonDocument finder = null;
                if (_projectId == new ObjectId("000000000000000000000000"))
                {
                    SetupProject(false);
                }

                if (_projectId != new ObjectId("000000000000000000000000"))
                {
                    finder = new BsonDocument { { "project", _projectId } };
                }

                var sorter = new BsonDocument { { "_id", -1 } };
                IFindFluent<BsonDocument, BsonDocument> result;
                if (finder == null)
                    result = _reportCollection.Find(_ => true).Sort(sorter).Limit(1);
                else
                    result = _reportCollection.Find(finder).Sort(sorter).Limit(1);

                if (result != null && result.First() != null)
                    return result.First()["_id"].AsObjectId;

                return new ObjectId();
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
        public void InitMongoDbConnection()
        {
            _mongoClient = new MongoClient();
        }

        public void InitMongoDbConnection(string host, int port = -1)
        {
            var conn = "mongodb://" + host;
            conn += port > -1 ? ":" + port : "";
            _mongoClient = new MongoClient(conn);
        }

        /// <summary>
        /// Connects to MongoDB using a connection string.
        /// Example: mongodb://host:27017,host2:27017/?replicaSet=rs0
        /// </summary>
        /// <param name="connectionString"></param>
        public void InitMongoDbConnection(string connectionString)
        {
            _mongoClient = new MongoClient(connectionString);
        }

        public void InitMongoDbConnection(MongoClientSettings settings)
        {
            _mongoClient = new MongoClient(settings);
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

        public override void Start()
        {
            CreateCollections();
            SetupProject(true);
        }

        private void LoadCurrentConfig()
        {
            foreach (KeyValuePair<string, string> entry in _reporterConfig.UserConfiguration)
            {
                var c = new Config(entry.Key, entry.Value);
                _configManager.AddConfig(c);
            }
        }

        private void CreateCollections()
        {
            if (_db != null || _projectCollection != null)
                return;

            // database
            _db = _mongoClient.GetDatabase(_DB);

            // collections
            _projectCollection = _db.GetCollection<BsonDocument>("project");
            _reportCollection = _db.GetCollection<BsonDocument>("report");
            _testCollection = _db.GetCollection<BsonDocument>("test");
            _logCollection = _db.GetCollection<BsonDocument>("log");
            _exceptionCollection = _db.GetCollection<BsonDocument>("exception");
            _mediaCollection = _db.GetCollection<BsonDocument>("media");
            _categoryCollection = _db.GetCollection<BsonDocument>("category");
            _authorCollection = _db.GetCollection<BsonDocument>("author");
        }

        private void SetupProject(bool setupReport)
        {
            string projectName = string.IsNullOrEmpty(_projectName) ? DEFAULT_PROJECT_NAME : _projectName;

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

            if (setupReport)
                SetupReport(projectName);
        }

        private void SetupReport(string projectName)
        {
            if (ReportId != new ObjectId("000000000000000000000000"))
                return;

            string reportName = string.IsNullOrEmpty(_reportName) ? DateTime.Now.ToString() : _reportName;

            if (string.IsNullOrEmpty(reportName))
                reportName = "[" + projectName + "] " + DateTime.Now.ToString();

            BsonDocument document;

            //var reportId = _configManager.GetValue("reportId");
            string reportId = null;

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

            var duration = DateTime.Now.Subtract(StartTime).TotalMilliseconds;

            if (duration.ToString().Contains("."))
                duration = Convert.ToDouble(duration.ToString().Split('.')[0]);

            List<String> categoryNameList = null;
            List<ObjectId> categoryIdList = null;

            if (_categoryNameObjectIdCollection == null)
                _categoryNameObjectIdCollection = new Dictionary<string, ObjectId>();

            if (_categoryNameObjectIdCollection.Any())
            {
                categoryNameList = new List<string>(_categoryNameObjectIdCollection.Keys);
                categoryIdList = new List<ObjectId>(_categoryNameObjectIdCollection.Values);
            }

            var filter = Builders<BsonDocument>.Filter.Eq("_id", _reportId);
            var update = Builders<BsonDocument>.Update
                .Set("endTime", DateTime.Now)
                .Set("duration", duration)
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
                .Set("exceptionsGrandChildLength", SessionStatusStats.GrandChildExceptions)

                .Set("categoryNameList", categoryNameList)
                .Set("categoryIdList", categoryIdList);

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
                { "leaf", test.HasChildren() },
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
                { "leaf", node.HasChildren() },
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

            var id = document["_id"].AsObjectId;
            log.ObjectId = id;

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
                .Set("leaf", test.HasChildren())
                .Set("childNodesLength", test.NodeContext().Count)
                .Set("categorized", test.HasCategory());

            if (test.HasCategory())
            {
                var categoryNameList = test.CategoryContext().GetAllItems().Select(x => x.Name);
                update.Set("categoryNameList", categoryNameList);
            }

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
        }

        public override void OnScreenCaptureAdded(Test test, ScreenCapture screenCapture)
        {
            OnScreenCaptureInit(screenCapture);

            CreateMedia(test, screenCapture);
            InitMedia();
            _media.StoreMedia(screenCapture);
        }

        public override void OnScreenCaptureAdded(Log log, ScreenCapture screenCapture)
        {
            screenCapture.LogObjectId = log.ObjectId;

            OnScreenCaptureInit(screenCapture);

            CreateMedia(log, screenCapture);
            InitMedia();
            _media.StoreMedia(screenCapture);
        }

        private void OnScreenCaptureInit(ScreenCapture screenCapture)
        {
            StoreURL();
            screenCapture.ReportObjectId = _reportId;
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

        private void CreateMedia(Log log, Media media)
        {
            var document = new BsonDocument
            {
                { "log", log.ObjectId },
                { "project", _projectId },
                { "report", _reportId },
                { "testName", log.ParentModel.Name },
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
                _media = new MediaStorageManagerFactory().GetManager("http-klov");
                _media.Init(_url);
            }
        }

        public void AppendToLastRunReport()
        {
            var id = LastRunReportId;

            if (id == new ObjectId("000000000000000000000000"))
                return;

            ReportId = id;
        }

        public override void LoadConfig(string filePath) { }
    }
}
