using System;

namespace ColdTranslation.Model
{
    [Serializable]
    public class LastRow
    {
        public string Sheet { get; set; }
        public int Row { get; set; }

        public LastRow() { }
        public LastRow(string sheet, int row)
        {
            Sheet = sheet;
            Row = row;

        }
    }
}