using System;
using System.Collections.Generic;
using System.Text;

namespace MessageData
{
    public class BookOrder
    {
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; }
    }
}
