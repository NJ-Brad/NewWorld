namespace FlowDiagram
{
    public class FlowchartItem {
        public List<FlowchartItem> items = new();

    public string itemType = "";
    public string label = "";
    public string description = "";
        public bool HasConnections = false;

    private string _id = "";

        public string Id { get => _id; set { if (value.Length == 0) {
                    _id = this.label.Replace(' ', '_').Replace('-', '_');
                }
                else {
                    _id = value;
                }
                //    _id = value; 
            }
        }
    }
}