using System;
using Microsoft.SqlServer.Types;

namespace AdventureWorks.Data.Repositories
{
    public class DocumentSpecification
    {
        public string FileName { get; set; }

        public int? DocumentLevel { get; set; }

        public SqlHierarchyId? ParentNode { get; set; }

        public SqlHierarchyId? Node { get; set; }

        public Guid? Id { get; set; }
    }
}