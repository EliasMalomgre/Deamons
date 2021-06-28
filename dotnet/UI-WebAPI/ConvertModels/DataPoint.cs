namespace UI_WebAPI.ConvertModels
{
    public class DataPoint
    {
        public DataPoint(int y, string label, string colour)
        {
            Y = y;
            Label = label;
            Colour = colour;
        }

        public int Y { get; set; }
        public string Label { get; set; }
        public string Colour { get; set; }
    }
}