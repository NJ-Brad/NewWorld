using DslParser;

namespace GanttDiagram
{
    public class BlockToWorkItemsConverter
    {
        public static WorkItemWorkspace Convert(Block block) {
            WorkItemWorkspace rtnVal = new();

            foreach (Block child in block.children)
            {
                //if (this.ciEquals(child.blockText, "workspace")) {
                if (child.blockText.StartsWith("Title:"))
                {
                    rtnVal.title = child.blockText[6..];
                }
                else if (child.blockText.StartsWith("StartDate:"))
                {
                    rtnVal.startDate = child.blockText[10..];
                }
                else
                {
                    ConvertItem(rtnVal.items, child);
                }
            }

            return rtnVal;
        }

        //WorkItemWorkspace ConvertWorkspace(Block block)
        //{
        //    WorkItemWorkspace rtnVal = new();

        //    foreach (Block child in block.children)
        //    {
        //        if (child.blockText.Equals("items", StringComparison.OrdinalIgnoreCase))
        //        {
        //            foreach (Block grandChild in child.children)
        //            {
        //                ConvertItem(rtnVal.items, grandChild);
        //            }
        //        }
        //    }

        //    return rtnVal;
        //}

        static void ConvertItem(List<WorkItem> items, Block block)
        {
            WorkItem newItem = new();

            // allow for comment
            if (block.blockText.Trim().StartsWith("'"))
            {
                return;
            }
            List<string> parts = LineParser.Parse2(block.blockText, '`');

            int pn = 0;

            foreach (string str in parts)
            {
                switch (pn)
                {
                    case 0:
                        {
                            newItem.id = str;
                            break;
                        }
                    case 1:
                        {
                            newItem.label = str;
                            break;
                        }
                    case 2:
                        {
                            newItem.duration = str;
                            break;
                        }
                    case 3:
                        {
                            FillDependencies(newItem.dependencies, parts[3]);
                            break;
                        }
                }
                pn++;
            }
            items.Add(newItem);
        }

        static void FillDependencies(List<WorkItemDependency> dependencies, string dependencyList)
        {
            WorkItemDependency newItem = new();

            List<string> parts = LineParser.Parse2(dependencyList, ',');

            foreach (string str in parts)
            {
                bool isDate = DateTime.TryParse(str, out DateTime dt);

                if (isDate)
                {
                    newItem.id = str;
                    newItem.dependencyType = "WorkItem";
                }
                else
                {
                    newItem.startDate = str;
                    newItem.dependencyType = "StartDate";
                }
                dependencies.Add(newItem);
                newItem = new WorkItemDependency();
            }
        }
    }
}
