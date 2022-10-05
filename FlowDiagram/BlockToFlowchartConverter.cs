using DslParser;

namespace FlowDiagram
{
    public class BlockToFlowchartConverter
    {
        public static FlowchartWorkspace Convert(Block block) {
            FlowchartWorkspace rtnVal = new ();

        foreach (Block child in block.children) {
            if (child.blockText.Trim().Equals("flow", StringComparison.OrdinalIgnoreCase)) {
            rtnVal = ConvertWorkspace(child);
        }
        }

        return rtnVal;
    }

        public static FlowchartWorkspace ConvertWorkspace(Block block) {
            FlowchartWorkspace rtnVal = new ();

        foreach (Block child in block.children) {
            ConvertFlowItem(rtnVal.items, rtnVal.relationships, child);
    }
        LinkItems(rtnVal.items, rtnVal.relationships);

        return rtnVal;
    }

    public static void LinkItems(List<FlowchartItem> items, List<FlowchartRelationship> connections)
    {
        string prevItemName = "";
            bool prevItemHasDefinedConnections = false;
        foreach (FlowchartItem item in items)
        {
                // this prevents extra connections from automatically being created, when connections have already been defined
                if(prevItemHasDefinedConnections)
                {
                    prevItemName = "";
                    prevItemHasDefinedConnections = false;
                }

                if (item.HasConnections)
                {
                    prevItemHasDefinedConnections = true;
                }
                // end connection check

            if (prevItemName != "")
            {
                FlowchartRelationship newConn = new ();
                newConn.Label = " ";
                newConn.From = prevItemName;
                newConn.To = item.Id;
                connections.Add(newConn);
            }

            // if there are next steps listed, don't automatically connect
            if (item.itemType == "DECISION")
            {
                prevItemName = "";
            }
            else
            {
                prevItemName = item.Id;
            }
        }
    }

    static void ConvertFlowItem(List<FlowchartItem> items, List<FlowchartRelationship> connections, Block block)
    {
            FlowchartItem newItem = new ();

            List<string> parts = LineParser.Parse(block.blockText);

        string itemType = "";
            string label = "";
            string itemId = "";
            string description = "";

        // ignore a comment
        if (block.blockText[0] == '\'')
        {
            return;
        }

        if (parts.Count == 1)
        {
            switch (parts[0].ToUpper())
            {
                case "START":
                case "END":
                    itemType = parts[0];
                    label = parts[0].Trim();
                    itemId = FixId(parts[0]);
                    break;
                default:
                    itemType = "ACTION";
                    label = parts[0].Trim();
                    itemId = FixId(parts[0]);
                    break;
            }
        }
        else if (block.blockText[0] == '"')
        {
            itemType = "ACTION";
            label = parts[0].Trim();
            if (parts.Count == 1)
            {
                itemId = FixId(parts[0]);
            }
            else
            {
                itemId = FixId(parts[1]);
            }
        }
        else
        {
            for (var pn = 0; pn < parts.Count; pn++)
            {
                var str = parts[pn];

                if (pn == 0)
                {
                    itemType = str;
                }
                else if (pn == 1)
                {
                    label = str.Trim();
                }
                else
                {
                    itemId = FixId(str);
                }
            }
        }

        itemType = itemType.ToUpper();
        switch (itemType)
        {
            // case "BOUNDARY":
            //    newItem = new FlowchartItem ();
            //    newItem.itemType = itemType;
            //    newItem.label = label;
            //    newItem.id = itemId;
            //    newItem.description = description;
            //    for (var cn = 0; cn < block.children.length; cn++)
            //    {
            //         var child = block.children[cn];
            //         this.convertFlowItem(newItem.items, connections, child);
            //    }

            //     items.push(newItem);
            //     break;
            case "ACTION":
            case "SUB":
                newItem.itemType = itemType;
                newItem.label = label;
                newItem.Id = itemId;
                newItem.description = description;

                foreach (Block child in block.children)
                {
                        newItem.HasConnections = true;
                    ConvertConnection(itemId, connections, child);
                }

                items.Add(newItem);
                break;
            case "DECISION":
                newItem.itemType = itemType;
                newItem.label = label;
                if (itemId == "")
                {
                    itemId = FixId(label);
                }
                newItem.Id = itemId;
                newItem.description = description;
                foreach (Block child in block.children)
                {
                        newItem.HasConnections = true;
                        // for (var cn = 0; cn < block.children.length; cn++)
                        // {
                        //      var child = block.children[cn];
                        ConvertConnection(itemId, connections, child);
                }
                items.Add(newItem);
                break;
            case "START":
                newItem.itemType = itemType;
                newItem.label = "Start";
                if (itemId == "")
                {
                    itemId = FixId(label);
                }
                newItem.Id = itemId;
                items.Add(newItem);
                break;
            case "END":
                newItem.itemType = itemType;
                newItem.label = "End";
                if (itemId == "")
                {
                    itemId = FixId(label);
                }
                newItem.Id = itemId;
                items.Add(newItem);
                break;
            case "CONNECTION":
                    FlowchartRelationship newConn = new ();
                if (parts.Count == 4)
                {
                    newConn.From = parts[1];
                    newConn.To = parts[2];
                    newConn.Label = FixConnectionLabel(parts[3]);
                }

                if (parts.Count == 3)
                {
                    newConn.From = parts[1];
                    newConn.To = parts[2];
                    newConn.Label = " ";
                }

                if (newConn != null)
                {
                    connections.Add(newConn);
                }
                break;
            case "YES":
                    connections.Add(new FlowchartRelationship
                    {
                        From = parts[1],
                        To = parts[2],
                        Label = "Yes"
                    });
                    break;
            case "NO":
                connections.Add(new FlowchartRelationship {
                    From = parts[1],
                    To = parts[2],
                    Label = "No"
                });
                    break;
        }
    }

    static void ConvertConnection(string myId, List<FlowchartRelationship> connections, Block block)
    {
            List<string> parts = LineParser.Parse(block.blockText);

            FlowchartRelationship newConn = new ();
        newConn.Label = FixConnectionLabel(parts[0]);
        newConn.From = myId;
        newConn.To = FixId(parts[1]);
        connections.Add(newConn);
    }

    static string FixId(string input){
        // https://bobbyhadz.com/blog/javascript-typeerror-replaceall-is-not-a-function

        string brokenLabel = input.Trim().Replace(' ', '_');
        return brokenLabel;
    }

        static string FixConnectionLabel(string input)
{
    // https://bobbyhadz.com/blog/javascript-typeerror-replaceall-is-not-a-function

    string brokenLabel = input.Trim();
    if (brokenLabel == "_")
    {
        brokenLabel = " ";
    }
    return brokenLabel;
}
}
}