namespace FilesInBlazor.Shared
{
    public class FileUploadResponses
    {
        public IEnumerable<FileUploadResponse> Files { get; set; } = new List<FileUploadResponse>();
        public long? TotalSize => Files.Sum(f => f.Size);
        public int Count => Files.Count();
    }

    public class FileUploadResponse
    {
        public string? FileName { get; set; }
        public long? Size { get; set; }
        public string? Status { get; set; }
    }
}
