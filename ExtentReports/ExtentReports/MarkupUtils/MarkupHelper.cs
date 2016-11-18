using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.MarkupUtils
{
    public static class MarkupHelper
    {
        public static IMarkup CreateLabel(string text, ExtentColor color)
        {
            var l = new Label();
            l.Text = text;
            l.Color = color;
            return l;
        }

        public static IMarkup CreateCodeBlock(string code)
        {
            var c = new CodeBlock();
            c.Code = code;
            return c;
        }

        public static IMarkup CreateTable(string[][] data)
        {
            Table t = new Table();
            t.Data = data;
            return t;
        }
    }
}
