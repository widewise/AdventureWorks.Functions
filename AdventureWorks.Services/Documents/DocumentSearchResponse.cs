using System;

namespace AdventureWorks.Services.Documents
{
    public class DocumentSearchResponse
    {
        public bool FolderFlag { get; set; }
        public string FileName { get; set; }
        public Guid rowguid { get; set; }
    }
}
