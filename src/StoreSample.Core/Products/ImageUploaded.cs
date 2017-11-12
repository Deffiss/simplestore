namespace StoreSample.Core.Products
{
    public class ImageUploaded : VersionedEvent
    {
        public string ImagePath { get; set; }
    }
}
