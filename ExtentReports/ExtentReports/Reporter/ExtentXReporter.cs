using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Bson;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports.Reporter
{
    public class ExtentXReporter
    {
        private const string DEFAULT_PROJECT_NAME = "Default";

        private bool _appendExistingReport;
        private string _url;

        private ObjectId _reportId;
        private ObjectId _projectId;

        private MongoClient _mongoClient;
        private IMongoDatabase _db;
        private IMongoCollection<BsonDocument> _projectCollection;
        private IMongoCollection<BsonDocument> _reportCollection;
        private IMongoCollection<BsonDocument> _testCollection;
        private IMongoCollection<BsonDocument> _nodeCollection;
        private IMongoCollection<BsonDocument> _logCollection;
        private IMongoCollection<BsonDocument> _categoryCollection;
        private IMongoCollection<BsonDocument> _authorCollection;
        private IMongoCollection<BsonDocument> _categoriesTests;
        private IMongoCollection<BsonDocument> _authorsTests;

        public ExtentXReporter()
        {
            _mongoClient = new MongoClient();
        }
    }
}
