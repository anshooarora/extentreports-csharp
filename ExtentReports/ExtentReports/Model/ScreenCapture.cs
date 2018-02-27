namespace AventStack.ExtentReports.Model
{
    public class ScreenCapture : Media
    {
        public string Source
        {
            get
            {
                if (Base64String != null)
                    return "<br/><a href='" + GetScreenCapturePath() + "' data-featherlight='image'><span class='label grey white-text'>base64-img</span></a>";

                return "<img data-featherlight='" + GetScreenCapturePath() + "' class='step-img' src='" + GetScreenCapturePath() + "' data-src='" + GetScreenCapturePath() + "'>";
            }
        }

        private string GetScreenCapturePath()
        {
            string path = Path != null ? Path : Base64String;
            return path;
        }
    }
}
