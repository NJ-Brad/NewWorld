namespace GanttDiagram
{
    public class WorkItem
    {
        //public items: WorkItem[] = [];

        public string id = "";
        public string label = "";
        public string duration = "";
        public List<WorkItemDependency> dependencies = new ();
    }
}