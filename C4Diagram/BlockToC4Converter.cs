using DslParser;

namespace C4Diagram
{
    public class BlockToC4Converter
    {
        public static C4Workspace Convert(Block block)
        {
            C4Workspace rtnVal = new();

            foreach (Block child in block.children)
            {
                if (child.blockText.Equals("workspace", StringComparison.OrdinalIgnoreCase))
                {
                    rtnVal = ConvertWorkspace(child);
                }
            }

            return rtnVal;
        }

        static C4Workspace ConvertWorkspace(Block block)
        {
            C4Workspace rtnVal = new();

            foreach (Block child in block.children)
            {
                if (child.blockText.Equals("items", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Block grandChild in child.children)
                    {
                        ConvertItem(rtnVal.items, grandChild);
                    }
                }

                if (child.blockText.Equals("connections", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Block grandChild in child.children)
                    {
                        ConvertConnection(rtnVal.relationships, grandChild);
                    }
                }
            }

            return rtnVal;
        }

        static void ConvertItem(List<C4Item> items, Block block)
        {
            C4Item newItem = new();

            // allow for comment
            if (block.blockText.Trim().StartsWith("'"))
            {
                return;
            }

            List<string> parts = LineParser.Parse(block.blockText);

            string itemType = "";
            string itemId = "";
            string technology = "";
            string label = "";
            string description = "";

            bool nextIsTechnology = false;

            int pn = 0;

            foreach (string str in parts)
            {
                if (pn == 0)
                {
                    itemType = str;
                }
                else if (pn == 1)
                {
                    itemId = str;
                }
                else
                {
                    if (str.Equals("utilizing", StringComparison.OrdinalIgnoreCase))
                    {
                        nextIsTechnology = true;
                    }
                    else if (nextIsTechnology)
                    {
                        technology = str;
                        nextIsTechnology = false;
                    }
                    else if (str.StartsWith("("))
                    {
                        description = str.Trim();
                        description = description.Substring(1, description.Length - 1);
                    }
                    else
                    {
                        label = str.Trim();
                    }
                }
                pn++;
            }

            itemType = itemType.ToUpper();
            switch (itemType)
            {
                case "PERSON":
                    newItem = new C4Item();
                    newItem.itemType = itemType;
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    break;
                case "EXTERNAL_PERSON":
                    newItem = new C4Item();
                    newItem.itemType = "PERSON";
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    newItem.external = true;
                    break;
                case "COMPONENT":
                    newItem = new C4Item();
                    newItem.itemType = itemType;
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    newItem.technology = technology;
                    break;
                case "EXTERNAL_COMPONENT":
                    newItem = new C4Item();
                    newItem.itemType = "COMPONENT";
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    newItem.technology = technology;
                    newItem.external = true;
                    break;
                case "SYSTEM":
                    newItem = new C4Item();
                    newItem.itemType = itemType;
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    break;
                case "EXTERNAL_SYSTEM":
                    newItem = new C4Item();
                    newItem.itemType = "SYSTEM";
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    newItem.external = true;
                    break;
                case "DATABASE":
                    newItem = new C4Item();
                    newItem.itemType = itemType;
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    newItem.technology = technology;
                    newItem.database = true;
                    break;
                case "EXTERNAL_DATABASE":
                    newItem = new C4Item();
                    newItem.itemType = "DATABASE";
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    newItem.technology = technology;
                    newItem.external = true;
                    newItem.database = true;
                    break;
                case "SYSTEM_BOUNDARY":
                    newItem = new C4Item();
                    newItem.itemType = itemType;
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    break;
                case "CONTAINER_BOUNDARY":
                    newItem = new C4Item();
                    newItem.itemType = itemType;
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    break;
                case "ENTERPRISE_BOUNDARY":
                    newItem = new C4Item();
                    newItem.itemType = itemType;
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    break;
                case "NODE":
                    newItem = new C4Item();
                    newItem.itemType = itemType;
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    break;
                case "ENTERPRISE":
                    newItem = new C4Item();
                    newItem.itemType = itemType;
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    break;
                case "CONTAINER":
                    newItem = new C4Item();
                    newItem.itemType = itemType;
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    newItem.technology = technology;
                    break;
                case "TABLE":
                    newItem = new C4Item();
                    newItem.itemType = "TABLE";
                    newItem.label = label;
                    newItem.id = itemId;
                    newItem.description = description;
                    newItem.technology = technology;
                    break;
            }

            if (newItem != null)
            {
                foreach (Block child in block.children)
                {
                    ConvertItem(newItem.items, child);
                }

                items.Add(newItem);
            }
        }

        static void ConvertConnection(List<C4Relationship> connections, Block block)
        {
            C4Relationship newItem = new();


            // allow for comment
            if (block.blockText.Trim().StartsWith("'"))
            {
                return;
            }
            List<string> parts = LineParser.Parse(block.blockText);

            string origin = "";
            string destination = "";
            string technology = "";
            string label = "";

            bool nextIsTechnology = false;

            int pn = 0;

            foreach (string str in parts)
            {
                if (pn == 0)
                {
                    origin = str;
                }
                else if (pn == 1)
                {
                    label = str.Trim();
                }
                else if (pn == 2)
                {
                    destination = str;
                }
                else
                {
                    if (str.Equals("utilizing", StringComparison.OrdinalIgnoreCase))
                    {
                        nextIsTechnology = true;
                    }
                    else if (nextIsTechnology)
                    {
                        technology = str;
                        nextIsTechnology = false;
                    }
                }
                pn++;
            }

            newItem = new C4Relationship();
            newItem.from = origin;
            newItem.to = destination;
            newItem.label = label;
            newItem.technology = technology;

            if (newItem != null)
            {
                connections.Add(newItem);
            }
        }
    }
}