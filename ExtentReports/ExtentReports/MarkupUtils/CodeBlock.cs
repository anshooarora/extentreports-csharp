using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.MarkupUtils
{
    public class CodeBlock : IMarkup
    {
        internal string Code { get; set; }

        public string GetMarkup()
        {
            var lhs = "<textarea disabled class='code-block'>";
            var rhs = "</textarea>";

            return lhs + Code + rhs;
        }
    }
}
