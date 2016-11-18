namespace AventStack.ExtentReports.Model
{
    public class ScreenCapture : Media
    {
        public string Source
        {
            get
            {
                return "<img data-featherlight='" + Path + "' width='10%' src='' data-src='" + Path + "'>";
            }
        }
    }
}
