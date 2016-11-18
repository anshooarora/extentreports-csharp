using System;

namespace AventStack.ExtentReports.MarkupUtils
{
    internal class Label : IMarkup
    {
        internal string Text { get; set; }
        internal ExtentColor Color = ExtentColor.Transparent;

        public string GetMarkup()
        {
            var lhs = "<span class='label white-text " + Enum.GetName(typeof(ExtentColor), Color).ToLower() + "'>";
            var rhs = "</span>";

            return lhs + Text + rhs;
        }
    }
}
