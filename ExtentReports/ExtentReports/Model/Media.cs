using System;

using MongoDB.Bson;

namespace AventStack.ExtentReports.Model
{
    [Serializable]
    public class Media
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public ObjectId ObjectId { get; set; }
        public ObjectId ReportObjectId { get; set; }
        public ObjectId TestObjectId { get; set; }
        public MediaType MediaType { get; set; }
        public int Sequence { get; set; }
    }
}
