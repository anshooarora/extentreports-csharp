using System;

namespace AventStack.ExtentReports.Model
{
    [Serializable]
    public abstract class Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public Attribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public Attribute(string name) : this(name, null) { }
    }
}
