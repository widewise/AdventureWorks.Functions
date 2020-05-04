using System;
using Microsoft.SqlServer.Types;

namespace AdventureWorks.Data.Models
{
    public class Document
    {
        public Guid Id { get; set; }

        public DateTime ModifiedDate { get; set; }

        public SqlHierarchyId? DocumentNode { get; set; }

        public int DocumentLevel { get; set; }

        public string Title { get; set; }

        public int Owner { get; set; }

        public bool FolderFlag { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string Revision { get; set; }

        public int ChangeNumber { get; set; }
        public int Status { get; set; }

        public string DocumentSummary { get; set; }

        public byte[] DocumentData { get; set; }
    }
}
