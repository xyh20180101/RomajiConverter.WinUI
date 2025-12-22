using System.Collections.ObjectModel;

namespace RomajiConverter.Core.Models
{
    public class ConvertedLine
    {
        public ConvertedLine()
        {
            Index = 0;
            Chinese = "";
            Japanese = "";
            Units = new ObservableCollection<ConvertedUnit>();
        }

        public ushort Index { get; set; }

        public string Chinese { get; set; }

        public string Japanese { get; set; }

        public ObservableCollection<ConvertedUnit> Units { get; set; }
    }
}