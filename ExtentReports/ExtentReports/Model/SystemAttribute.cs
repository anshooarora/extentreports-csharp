namespace AventStack.ExtentReports.Model
{
    public class SystemAttribute : Attribute
    {
        public string Value { get; private set; }

        public SystemAttribute(string name, string value) : base(name)
        {
            Value = value;
        }
    }
}
