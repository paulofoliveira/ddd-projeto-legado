using System.Collections.Generic;

namespace Acl
{
    internal class DeliveryBubble
    {
        public int DeliveryID { get; set; }
        public decimal? CostEstimate { get; set; }
        public string DestinationStreet { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationState { get; set; }
        public string DestinationZipCode { get; set; }

        public List<ProductLineBubble> Lines { get; set; } = new List<ProductLineBubble>();
    }
}
