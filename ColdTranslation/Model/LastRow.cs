using System;

namespace ColdTranslation.Model
{
    [Serializable]
    public struct LastRow
    {
        public string Sheet { get; set; }
        public int Row { get; set; }
    }
}