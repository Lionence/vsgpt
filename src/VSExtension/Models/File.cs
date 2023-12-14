namespace Lionence.VSGPT.Models
{
    public sealed class File
    {
        public string Id { get; set; }
        public int Bytes { get; set; }
        public long CreatedAt { get; set; }
        public string Filename { get; set; }
        public string Purpose { get; set; }
    }
}
