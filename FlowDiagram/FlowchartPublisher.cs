using System.Text;

namespace FlowDiagram
{
    public class FlowchartPublisher
    {
        public static string Publish(FlowchartWorkspace workspace, string format)
        {

            string rtnVal = "";
            switch (format)
            {
                case "MERMAID":
                    rtnVal = PublishMermaid(workspace);
                    break;
                case "PLANT":
                    break;
            }

            return rtnVal;
        }

        private static string PublishMermaid(FlowchartWorkspace workspace)
        {
            StringBuilder sb = new();

            sb.Append(MermaidHeader());

            foreach (FlowchartItem item in workspace.items)
            {
                //item = workspace.items[itmNum];
                sb.Append(MermaidItem(item, 1));
            }

            foreach (FlowchartRelationship rel in workspace.relationships)
            {
                sb.Append(MermaidConnection(rel));
            }

            return sb.ToString();
        }

        private static string MermaidHeader()
        {
            StringBuilder sb = new();
            sb.Append("graph TB");
            sb.Append("\r\n");
            // classDef borderless stroke-width:0px
            // classDef darkBlue fill:#00008B, color:#fff
            // classDef brightBlue fill:#6082B6, color:#fff
            // classDef gray fill:#62524F, color:#fff
            // classDef gray2 fill:#4F625B, color:#fff

            // ");

            return sb.ToString();
        }

        private static string BuildIndentation(int level)
        {
            string rtnVal = "";

            for (var i = 0; i < (4 * level); i++)
            {
                rtnVal += " ";
            }
            return rtnVal;
        }

        private static string MermaidItem(FlowchartItem item, int indent = 1)
        {
            StringBuilder sb = new();

            string indentation = BuildIndentation(indent);

            // https://bobbyhadz.com/blog/javascript-typeerror-replaceall-is-not-a-function
            string brokenLabel = item.label.Replace("`", "<br/>");

            brokenLabel = $"\"{brokenLabel}\"";

            switch (item.itemType)
            {
                case "BOUNDARY":
                    if (item.items.Count == 0)
                    {
                        // a boundary with nothing in it, should not be displayed
                        // sb.appendLine(`${indentation}${item.id}[${item.label}]`);
                    }
                    else
                    {
                        sb.AppendLine("${indentation}subgraph ${item.id}[${brokenLabel}]");
                        indent++;

                        foreach (FlowchartItem item2 in item.items)
                        {
                            sb.AppendLine(MermaidItem(item2, indent).TrimEnd());
                        }
                        sb.AppendLine($"{indentation}end");
                    }
                    break;
                case "ACTION":
                    sb.AppendLine($"{indentation}{item.Id}[{brokenLabel}]");
                    break;
                case "DECISION":
                    sb.AppendLine($"{indentation}{item.Id}{{{brokenLabel}}}");
                    break;
                case "START":
                case "END":
                    sb.AppendLine($"{indentation}{item.Id}([{brokenLabel}])");
                    break;
                case "SUB":
                    sb.AppendLine($"{indentation}{item.Id}[[{brokenLabel}]]");
                    break;
            }

            return sb.ToString();
        }

        private static string MermaidConnection(FlowchartRelationship rel, int indent = 1)
        {
            StringBuilder sb = new();

            string indentation = BuildIndentation(indent);

            string From = rel.From;
            string To = rel.To;

            string brokenLabel = rel.Label.Replace("`", "<br/>");


            sb.AppendLine($"{indentation}{From}--\"{brokenLabel}\"-->{To}");

            return sb.ToString();
        }


    }
}
