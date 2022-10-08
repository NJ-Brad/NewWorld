using System.Text;

namespace C4Diagram
{
    public class C4Publisher
    {
        Dictionary<string, string> redirections = new();
//        public string diagramType = "";

        public string Publish(C4Workspace workspace, string diagramType, string format)
        {
            redirections.Clear();

            string rtnVal = "";
            switch (format)
            {
                case "MERMAID":
                    switch (diagramType)
                    {
                        case "Context":
                            rtnVal = PublishMermaidContext(workspace);
                            break;
                        case "Container":
                            rtnVal = PublishMermaidContainer(workspace);
                            break;
                        case "Component":
                            rtnVal = PublishMermaidComponent(workspace);
                            break;
                        default:
                            rtnVal = PublishMermaid(workspace);
                            break;
                    }
                    break;
                case "PLANT":
                    switch (diagramType)
                    {
                        case "Context":
                            rtnVal = PublishPlantContext(workspace);
                            break;
                        case "Container":
                            rtnVal = PublishPlantContainer(workspace);
                            break;
                        case "Component":
                            rtnVal = PublishPlantComponent(workspace);
                            break;
                        default:
                            rtnVal = PublishPlant(workspace);
                            break;
                    }
                    break;
            }

            return rtnVal;
        }

        private string PublishMermaidComponent(C4Workspace workspace)
        {
            return this.PublishMermaid(workspace);
        }

        private string PublishMermaid(C4Workspace workspace)
        {
            StringBuilder sb = new();

            sb.Append(MermaidHeader(workspace));

            foreach (C4Item item in workspace.items)
            {
                sb.Append(MermaidItem(item, "Component"));
            }

            foreach (C4Relationship rel in workspace.relationships)
            {
                sb.Append(this.MermaidConnection(rel));
            }

            return sb.ToString();
        }

        private string PublishMermaidContext(C4Workspace workspace)
        {
            StringBuilder sb = new();

            sb.Append(MermaidHeader(workspace));

            CreateContextRedirects(workspace.items);

            foreach (C4Item item in workspace.items)
            {
                sb.Append(MermaidItem(item, "Context"));
            }

            List<string> connections = new();
            string newConn;
            foreach (C4Relationship rel in workspace.relationships)
            {
                newConn = MermaidConnection(rel);

                if (!connections.Contains(newConn))
                {
                    sb.Append(MermaidConnection(rel));
                }
            }

            return sb.ToString();
        }

        private string PublishMermaidContainer(C4Workspace workspace)
        {
            StringBuilder sb = new();

            sb.Append(MermaidHeader(workspace));

            CreateContainerRedirects(workspace.items);

            foreach (C4Item item in workspace.items)
            {
                sb.Append(MermaidItem(item, "Container"));
            }

            foreach (C4Relationship rel in workspace.relationships)
            {
                sb.Append(MermaidConnection(rel));
            }

            return sb.ToString();
        }

        private string PublishPlantComponent(C4Workspace workspace)
        {
            return this.PublishPlant(workspace);
        }

        private string PublishPlant(C4Workspace workspace)
        {
            StringBuilder sb = new();

            sb.Append(PlantHeader(workspace));

            foreach (C4Item item in workspace.items)
            {
                sb.Append(PlantItem(item, "Component"));
            }

            foreach (C4Relationship rel in workspace.relationships)
            {
                sb.Append(PlantConnection(rel));
            }

            return sb.ToString();
        }

        private string PublishPlantContext(C4Workspace workspace)
        {
            StringBuilder sb = new();

            sb.Append(PlantHeader(workspace, "Context"));

            CreateContextRedirects(workspace.items);

            foreach (C4Item item in workspace.items)
            {
                sb.Append(PlantItem(item, "Context"));
            }

            List<string> connections = new();
            string newConn;
            foreach (C4Relationship rel in workspace.relationships)
            {
                newConn = PlantConnection(rel);

                if (!connections.Contains(newConn))
                {
                    connections.Add(newConn);
                    sb.Append(PlantConnection(rel));
                }
            }

            return sb.ToString();
        }

        private string PublishPlantContainer(C4Workspace workspace)
        {
            StringBuilder sb = new();

            sb.Append(PlantHeader(workspace, "Container"));

            CreateContainerRedirects(workspace.items);

            foreach (C4Item item in workspace.items)
            {
                sb.Append(PlantItem(item, "Container"));
            }

            foreach (C4Relationship rel in workspace.relationships)
            {
                sb.Append(PlantConnection(rel));
            }

            return sb.ToString();
        }


        private void CreateContextRedirects(List<C4Item> items, string redirectTo = "")
        {

            C4Item item;
            for (var itmNum = 0; itmNum < items.Count; itmNum++)
            {
                item = items[itmNum];

                if (redirectTo != "")
                {
                    redirections.Add(item.id, redirectTo);
                    if (item.items.Count > 0)
                    {
                        CreateContextRedirects(item.items, redirectTo);
                    }
                }
                else
                {
                    switch (item.itemType)
                    {
                        case "SYSTEM":
                        case "DATABASE":
                            this.CreateContextRedirects(item.items, item.id);
                            break;
                        default:
                            // drill down beyond the first level
                            this.CreateContextRedirects(item.items);
                            break;
                    }
                }
            }
        }

        private void CreateContainerRedirects(List<C4Item> items, string redirectTo = "")
        {
            C4Item item;
            for (var itmNum = 0; itmNum < items.Count; itmNum++)
            {
                item = items[itmNum];
                if (redirectTo != "")
                {
                    this.redirections.Add(item.id, redirectTo);
                    if (item.items.Count > 0)
                    {
                        CreateContainerRedirects(item.items, redirectTo);
                    }
                }
                else
                {
                    switch (item.itemType)
                    {
                        case "CONTAINER":
                            CreateContainerRedirects(item.items, item.id);
                            break;
                        default:
                            CreateContainerRedirects(item.items);
                            break;
                    }
                }
            }
        }

        private string MermaidHeader(C4Workspace workspace)
        {
            StringBuilder sb = new();
            sb.Append("flowchart TB");
            sb.Append("\r\n");
            // classDef borderless stroke-width:0px
            // classDef darkBlue fill:#00008B, color:#fff
            // classDef brightBlue fill:#6082B6, color:#fff
            // classDef gray fill:#62524F, color:#fff
            // classDef gray2 fill:#4F625B, color:#fff

            // ");

            return sb.ToString();
        }

        private string PlantHeader(C4Workspace workspace, string diagramType = "Component")
        {
            StringBuilder sb = new();

            sb.AppendLine($"C4{diagramType}");

            return sb.ToString();
        }

        private string BuildIndentation(int level)
        {
            string rtnVal = "";

            for (var i = 0; i < (4 * level); i++)
            {
                rtnVal = rtnVal + " ";
            }
            return rtnVal;
        }

        private string MermaidItem(C4Item item, string diagramType, int indent = 1)
        {
            StringBuilder sb = new();

            string indentation = BuildIndentation(indent);
            string displayType = item.itemType;
            bool goDeeper = true;

            switch (item.itemType)
            {
                case "PERSON":
                    if (item.external)
                    {
                        displayType = "External Person";
                    }
                    else
                    {
                        displayType = "Person";
                    }
                    break;
                case "SYSTEM":
                    if (item.external)
                    {
                        displayType = "External System";
                    }
                    else
                    {
                        if (diagramType.Equals("Context", StringComparison.OrdinalIgnoreCase))
                        {
                            goDeeper = false;
                            displayType = "System";
                        }
                        else if (item.items.Count == 0)
                        {
                            displayType = "System";
                        }
                        else
                        {
                            displayType = "System Boundary";
                        }
                    }
                    break;
                case "CONTAINER":
                    if (item.external)
                    {
                        displayType = "External Container";
                    }
                    else
                    {
                        if (diagramType.Equals("Container", StringComparison.OrdinalIgnoreCase))
                        {
                            goDeeper = false;
                            displayType = "Container";
                        }
                        else if (item.items.Count == 0)
                        {
                            displayType = "Container";
                        }
                        else
                        {
                            displayType = "Container Boundary";
                        }
                    }
                    break;
                case "DATABASE":
                    if (item.external)
                    {
                        displayType = "External Database";
                    }
                    else
                    {
                        if (diagramType.Equals("Container", StringComparison.OrdinalIgnoreCase))
                        {
                            goDeeper = false;
                            displayType = "Database";
                        }
                        else if (item.items.Count == 0)
                        {
                            displayType = "Database";
                        }
                        else
                        {
                            displayType = "Database Boundary";
                        }
                    }
                    break;
            }

            string displayLabel = $"\"<strong><u>{item.label}</u></strong>";
            string brokenDescription = item.description.Replace("`", "<br/>");

            if (item.description.Length != 0)
            {
                displayLabel = displayLabel + $"<br/>{brokenDescription}";
            }

            displayLabel += $"<br/>&#171;{displayType}&#187;\"";

            if (!goDeeper || (item.items.Count == 0))
            {
                sb.Append($"{indentation}{item.id}[{displayLabel}]");
                sb.Append("\r\n");
            }
            else
            {
                sb.Append($"{indentation}subgraph {item.id}[{displayLabel}]");
                sb.Append("\r\n");
                indent++;

                C4Item item2;
                for (var itmNum = 0; itmNum < item.items.Count; itmNum++)
                {
                    item2 = item.items[itmNum];
                    sb.Append(MermaidItem(item2, diagramType, indent).TrimEnd());
                    sb.Append("\r\n");
                }
                sb.Append($"{indentation}end");
                sb.Append("\r\n");
            }

            return sb.ToString();
        }

        private string MermaidConnection(C4Relationship rel, int indent = 1)
        {
            StringBuilder sb = new();

            string indentation = BuildIndentation(indent);

            string from = rel.from;
            string to = rel.to;
            bool redirected = false;

            if (redirections.ContainsKey(from))
            {
                from = redirections[from]!;
                redirected = true;
            }

            if (redirections.ContainsKey(to))
            {
                to = this.redirections[to]!;
                redirected = true;
            }

            if (from == to)
            {
                return "";
            }

            if (redirected || (rel.technology.Length == 0))
            {
                sb.Append($"{indentation}{from}--\"{rel.label}\"-->{to}");
                sb.Append("\r\n");
            }
            else
            {
                sb.Append($"{indentation}{from}--\"{rel.label}<br>[{rel.technology}]\"-->{to}");
                sb.Append("\r\n");
            }

            return sb.ToString();
        }

        private string PlantItem(C4Item item, string diagramType, int indent = 1)
        {
            StringBuilder sb = new();

            string indentation = BuildIndentation(indent);
            string displayType = item.itemType;
            bool goDeeper = true;

            switch (item.itemType)
            {
                case "PERSON":
                    displayType = "Person";
                    break;
                case "SYSTEM":
                    if (item.external)
                    {
                        displayType = "System";
                    }
                    else
                    {
                        if (diagramType.Equals("Context", StringComparison.OrdinalIgnoreCase))
                        {
                            goDeeper = false;
                            displayType = "System";
                        }
                        else if (item.items.Count == 0)
                        {
                            displayType = "System";
                        }
                        else
                        {
                            displayType = "System_Boundary";
                        }
                    }
                    break;
                case "CONTAINER":
                    if (item.external)
                    {
                        displayType = "Container";
                    }
                    else
                    {
                        if (diagramType.Equals("Container", StringComparison.OrdinalIgnoreCase))
                        {
                            goDeeper = false;
                            displayType = "Container";
                        }
                        else if (item.items.Count == 0)
                        {
                            displayType = "Container";
                        }
                        else
                        {
                            displayType = "Container_Boundary";
                        }
                    }
                    break;
                case "ENTERPRISE":
                    displayType = "Enterprise_Boundary";
                    break;
                case "DATABASE":
                    goDeeper = false;
                    displayType = diagramType;
                    break;
            }

            string plantText = FormatPlantItem(displayType, item);

            if (!goDeeper || (item.items.Count == 0))
            {
                sb.AppendLine($"{indentation}{plantText}");
            }
            else
            {
                sb.AppendLine($@"{indentation}{plantText}
{{");
                indent++;

                C4Item item2;
                for (var itmNum = 0; itmNum < item.items.Count; itmNum++)
                {
                    item2 = item.items[itmNum];
                    sb.AppendLine(PlantItem(item2, diagramType, indent).TrimEnd());
                }
                sb.AppendLine($@"{indentation}
\r\n}}");
            }

            return sb.ToString();
        }

        //public label: string = "";
        //public description: string = "";
        //public external: boolean = false;
        //public technology: string = "";
        //public database: boolean = false;
        //private _id: string = "";


        private string FormatPlantItem(string command, C4Item item)
        {
            StringBuilder sb = new();

            sb.Append(command);

            if (item.database)
            {
                sb.Append("Db");
            }
            if (item.external)
            {
                sb.Append("_Ext");
            }

            sb.Append("(");

            sb.Append(item.id);

            if (item.label != "")
            {
                sb.Append($", \"{item.label}\"");

                if (item.description != "")
                {
                    sb.Append($", \"{item.description}\"");

                    if (item.technology != "")
                    {
                        sb.Append($", \"{item.technology}\"");
                    }
                }
            }

            sb.Append(")");

            return sb.ToString();
        }

        private string PlantConnection(C4Relationship rel, int indent = 1)
        {
            StringBuilder sb = new();

            string indentation = BuildIndentation(indent);

            string from = rel.from;
            string to = rel.to;
            bool redirected = false;

            if (redirections.ContainsKey(from))
            {
                from = redirections[from]!;
                redirected = true;
            }

            if (redirections.ContainsKey(to))
            {
                to = redirections[to]!;
                redirected = true;
            }

            if (from == to)
            {
                return "";
            }

            if (redirected || (rel.technology.Length == 0))
            {
                sb.AppendLine($"{indentation}Rel({from}, {to}, \"{rel.label}\")");
            }
            else
            {
                sb.AppendLine($"{indentation}Rel({from}, {to}, \"{rel.label}\", \"{rel.technology}\")");
            }

            return sb.ToString();
        }

        //// https://stackoverflow.com/questions/2140627/how-to-do-case-insensitive-string-comparison
        //ciEquals(a: string, b: string) {
        //    return typeof a === 'string' && typeof b === 'string'
        //        ? a.localeCompare(b, undefined, { sensitivity: 'accent' }) === 0
        //        : a === b;
        //}
    }
}