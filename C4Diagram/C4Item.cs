namespace C4Diagram
{
    public class C4Item
    {
        public List<C4Item> items = new ();

        public string itemType = "";
        public string label = "";
        public string description = "";
        public bool external = false;
        public string technology = "";
        public bool database = false;
        private string _id = "";

        public string id
        {
            get => _id; set
            {
                if (value.Length == 0)
                {
                    _id = label.Replace(' ', '_').Replace('-', '_');
                }
                else
                {
                    _id = value;
                }
            }
        }
    }
}