using AdventureWorks.Data.Models;
using Microsoft.SqlServer.Types;

namespace AdventureWorks.Data.Repositories
{
    public interface IDocumentRepository
    {
        Document Get(DocumentSpecification specification);

        void Add(Document document);

        SqlHierarchyId GetNewDocumentNode(SqlHierarchyId parentNode);
    }
}