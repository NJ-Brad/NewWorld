using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewWorld
{
    internal class MermaidPageGenerator
    {
        public static void Generate(string mermaidText, string title, string fileName)
        {
//            File.WriteAllText(fileName, mermaidText);

            string output = @$"<html>
    <body>
        <script src=""https://cdn.jsdelivr.net/npm/mermaid/dist/mermaid.min.js""></script>
        <script>
            mermaid.initialize({{ startOnLoad: true }});
        </script>

        <h1>{title}</h1>
        </hl>
        <div class=""mermaid"">
{mermaidText}
        </div>
    </body>
</html>";

            File.WriteAllText(fileName, output);


        }
    }
}
