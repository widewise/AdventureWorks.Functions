using System;
using System.IO;
using AdventureWorks.Data.Models;
using AdventureWorks.Data.Repositories;
using Microsoft.SqlServer.Types;

namespace AdventureWorks.Services.Documents
{
    public class DocumentFactory : IDocumentFactory
    {
        private readonly IDocumentRepository _repository;

        public DocumentFactory(
            IDocumentRepository repository)
        {
            _repository = repository;
        }

        public Document CreateFileDocument(
            string fullFileName,
            int documentLevel,
            SqlHierarchyId parentNode,
            string documentSummary,
            byte[] documentData)
        {
            var fileName = Path.GetFileName(fullFileName);
            var title = Path.GetFileNameWithoutExtension(fullFileName);
            var fileExtension = Path.GetExtension(fullFileName);

            var documentId = Guid.NewGuid();
            try
            {
                var documentNode = _repository.GetNewDocumentNode(parentNode);

                return new Document
                {
                    Id = documentId,
                    ModifiedDate = DateTime.UtcNow,
                    DocumentNode = documentNode,
                    DocumentLevel = documentLevel,
                    Title = title,
                    Owner = 220,
                    FolderFlag = documentLevel > 0,
                    FileName = fileName,
                    FileExtension = fileExtension,
                    Revision = "0",
                    ChangeNumber = 0,
                    Status = 2,
                    DocumentSummary = documentSummary,
                    DocumentData = documentData
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public Document CreateDirectoryDocument(
            string directoryName,
            int documentLevel,
            SqlHierarchyId parentNode)
        {
            var newDocumentNode = _repository.GetNewDocumentNode(parentNode);
            return new Document
            {
                Id = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                DocumentNode = newDocumentNode,
                DocumentLevel = documentLevel,
                Title = directoryName,
                Owner = 220,
                FolderFlag = true,
                FileName = directoryName,
                FileExtension = string.Empty,
                Revision = "0",
                ChangeNumber = 0,
                Status = 2
            };
        }
    }
}