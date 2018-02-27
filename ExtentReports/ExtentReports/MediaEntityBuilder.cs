using System.IO;
using System.Threading;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports
{
    public class MediaEntityBuilder
    {
        private static readonly MediaEntityBuilder _instance = new MediaEntityBuilder();
        private static ThreadLocal<Media> _media;

        public MediaEntityModelProvider Build()
        {
            return new MediaEntityModelProvider(_media.Value);
        }

        /// <summary>
        /// Adds a snapshot to the test or log with title
        /// </summary>
        /// <param name="path">Image path</param>
        /// <param name="title">Image title</param>
        /// <returns>A <see cref="MediaEntityBuilder"/> object</returns>
        public static MediaEntityBuilder CreateScreenCaptureFromPath(string path, string title = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new IOException("ScreenCapture path cannot be null or empty.");

            var sc = new ScreenCapture();
            sc.Path = path;
            sc.Title = title;
            sc.MediaType = MediaType.IMG;

            _media = new ThreadLocal<Media>();
            _media.Value = sc;

            return _instance;
        }

        public static MediaEntityBuilder CreateScreenCaptureFromBase64String(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                throw new IOException("Base64 string cannot be null or empty.");
        
            return createScreenCapture(base64String, null, true);
        }

        private static MediaEntityBuilder createScreenCapture(string pathOrBase64String, string title, bool isBase64String)
            {
            var sc = new ScreenCapture();
            sc.MediaType = MediaType.IMG;
            
            if (isBase64String)
                sc.Base64String = pathOrBase64String;
            else
                sc.Path = pathOrBase64String;

            if (title != null)
                sc.Title = title;

            _media = new ThreadLocal<Media>
            {
                Value = sc
            };

            return _instance;
        }
    }
}