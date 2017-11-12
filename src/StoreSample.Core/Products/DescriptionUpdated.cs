namespace StoreSample.Core.Products
{
    public class DescriptionUpdated : VersionedEvent
    {
        public string Description { get; set; }
    }
}
