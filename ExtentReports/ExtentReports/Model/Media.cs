using System;
using System.Threading;

using MongoDB.Bson;

namespace AventStack.ExtentReports.Model
{
    [Serializable]
    public class Media
    {
        private static int _seq;

        public Media()
        {
            Interlocked.Increment(ref _seq);
        }

        public string Path { get; set; }
        public string RelativePath { get; set; }
        public string Title { get; set; }
        public ObjectId ObjectId { get; set; }
        public ObjectId ReportObjectId { get; set; }
        public ObjectId TestObjectId { get; set; }
        public ObjectId LogObjectId { get; set; }
        public MediaType MediaType { get; set; }

        public int Sequence
        {
            get
            {
                return _seq;
            }
        }
    }
}