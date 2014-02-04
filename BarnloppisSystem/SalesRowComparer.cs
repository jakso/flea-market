using System.Collections.Generic;

namespace BarnloppisSystem
{
    public class SalesRowComparer : IComparer<SalesRow>
    {
        public int Compare(SalesRow salesrow1, SalesRow salesrow2)
        {
            int returnValue = 1;
            if (salesrow1 != null && salesrow2 != null)
            {
                returnValue = salesrow2.Timestamp.CompareTo(salesrow1.Timestamp);
            }
            return returnValue;
        }
    }
}