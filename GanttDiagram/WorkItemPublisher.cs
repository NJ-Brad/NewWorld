using System.Text;
using DslParser;

namespace GanttDiagram
{
    public class WorkItemPublisher {

    public static string Publish(WorkItemWorkspace workspace)
        {

            return PublishMermaid(workspace);
    }

    private static string PublishMermaid(WorkItemWorkspace workspace)  {
        StringBuilder sb = new ();

        sb.Append(MermaidHeader(workspace));

        foreach (WorkItem item in workspace.items) {
            sb.Append(MermaidItem(item));
        }

        return sb.ToString();
    }

    //private static bool IsInList(string lookFor, List<string> lookIn) {
    //    bool rtnVal = false;

    //    foreach (string lookInItem in lookIn) {
    //        if (lookFor.Equals(lookInItem, StringComparison.OrdinalIgnoreCase)) {
    //            rtnVal = true;
    //        }
    //    }

    //    return rtnVal;
    //}

    private static string MermaidHeader(WorkItemWorkspace workspace) {
        StringBuilder sb = new ();
//        sb.append("flowchart TB");
//        sb.append("\r\n");
        // classDef borderless stroke-width:0px
        // classDef darkBlue fill:#00008B, color:#fff
        // classDef brightBlue fill:#6082B6, color:#fff
        // classDef gray fill:#62524F, color:#fff
        // classDef gray2 fill:#4F625B, color:#fff

        // ");

        sb.AppendLine("gantt");
        // gantt
        sb.AppendLine("    dateFormat  YYYY-MM-DD");
        sb.AppendLine($"    title       {workspace.title}");
        sb.AppendLine("    excludes    weekends");
//        sb.appendLine(`    %% (`excludes` accepts specific dates in YYYY-MM-DD format, days of the week ("sunday") or "weekends", but not the word "weekdays".)`);
        sb.AppendLine($"`    Start : milestone, start, {workspace.startDate}, 0min");

        return sb.ToString();
    }

        //private string BuildIndentation(int level) {
        //    string rtnVal = "";

        //    for (var i = 0; i < (4 * level); i++) {
        //        rtnVal = rtnVal + " ";
        //    }
        //    return rtnVal;
        //}

        private static string MermaidItem(WorkItem item)
        {
            //    private static string MermaidItem(WorkItem item, int indent = 1) {
            StringBuilder sb = new ();

//            string indentation = this.buildIndentation(indent);
        //var displayType: string = item.itemType;

            StringBuilder deps = new ();

        deps.Append("start");

        foreach (WorkItemDependency dependency in item.dependencies) {
            if(dependency.dependencyType == "StartDate")
            {
                deps.Append($" {dependency.startDate}");
            }
            else
            {
                deps.Append($" {dependency.id}");
            }
        }
        
        // if(firstItem)
        // {
        //     const start: Date = new Date();
        //     var datePart = this.toIsoString(start);
        //     sb.appendLine(`${item.label} : ${datePart}, 1d`);
        // }
        // else{
            sb.Append($"{item.label} :{item.id} ");

            if(deps.Length > 0){
                sb.Append($", after {deps.ToString()}");
            }

            if(item.duration.Length > 0){
                sb.Append($", {item.duration}d");
            }

            sb.AppendLine("");
//        }

        return sb.ToString();
    }

    public static string ToIsoString(DateTime date) {
        int year = date.Year;
        var month = date.Month + 1;
        var dt = date.Day;

            string dtString = dt.ToString();
            string monthString = month.ToString();
        
        if (dt < 10) {
            dtString = '0' + dt.ToString();
        }
        if (month < 10) {
            monthString = '0' + month.ToString();
        }
        
        return(year+'-' + monthString + '-'+ dtString);
    }

    public string label = "";
    public string description = "";
    public bool external = false;
    public string technology = "";
    public bool database = false;
//    private string _id  = "";

}
}