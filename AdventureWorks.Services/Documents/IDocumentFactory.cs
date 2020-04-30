using AdventureWorks.Data.Models;
using Microsoft.SqlServer.Types;

namespace AdventureWorks.Services.Documents
{
    public interface IDocumentFactory
    {
        Document CreateFileDocument(
            string fullFileName,
            int documentLevel,
            SqlHierarchyId parentNode,
            string documentSummary,
            byte[] documentData);

        Document CreateDirectoryDocument(
            string directoryName,
            int documentLevel,
            SqlHierarchyId parentNode);
    }
}