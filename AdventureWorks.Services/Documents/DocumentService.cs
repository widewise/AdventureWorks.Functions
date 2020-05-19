using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorks.Data.Models;
using AdventureWorks.Data.Repositories;
using Microsoft.Azure.Search;
using Microsoft.SqlServer.Types;

namespace AdventureWorks.Services.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentFactory _factory;
        private readonly IDocumentRepository _repository;
        private readonly SearchIndexSettings _searchIndexSettings;

        public DocumentService(
            IDocumentFactory factory,
            IDocumentRepository repository,
            SearchIndexSettings searchIndexSettings)
        {
            _factory = factory;
            _repository = repository;
            _searchIndexSettings = searchIndexSettings;
        }

        public Guid Create(DocumentCreateParameters parameters)
        {
            var directoryPath = Path.GetDirectoryName(parameters.FileName);
            var documentLevel = CreateDirectory(directoryPath, out var parentNode);

            var existingDocument = _repository.Get(new DocumentSpecification
            {
                ParentNode = parentNode,
                FileName = Path.GetFileName(parameters.FileName)
            });

            if (existingDocument != null)
            {
                throw new Exception($"File '{parameters.FileName}' already exists.");
            }

            var document = _factory.CreateFileDocument(parameters.FileName, documentLevel, parentNode,
                parameters.DocumentSummary, parameters.DocumentData);

            _repository.Add(document);

            return document.Id;
        }

        private int CreateDirectory(
            string directoryPath,
            out SqlHierarchyId parentNode)
        {
            parentNode = SqlHierarchyId.GetRoot();
            var documentLevel = 0;

            if (string.IsNullOrEmpty(directoryPath))
            {
                return documentLevel;
            }

            var directoryParts = directoryPath.Split(Path.DirectorySeparatorChar);
            var newDirectory = false;
            foreach (var directoryPart in directoryParts)
            {
                var document = CreateDirectory(directoryPart, documentLevel, parentNode, ref newDirectory);

                parentNode = document.DocumentNode ?? SqlHierarchyId.GetRoot();

                documentLevel++;
            }

            return documentLevel;
        }

        private Document CreateDirectory(
            string directoryPart,
            int documentLevel,
            SqlHierarchyId parentNode,
            ref bool newDirectory)
        {
            if (newDirectory)
            {
                var existingDocument = _repository.Get(new DocumentSpecification
                {
                    ParentNode = parentNode,
                    FileName = directoryPart
                });

                if (existingDocument != null)
                {
                    return existingDocument;
                }
            }

            var document = _factory.CreateDirectoryDocument(directoryPart, documentLevel, parentNode);

            _repository.Add(document);

            newDirectory = true;

            return document;
        }

        public Document Get(string fileName, string content, SqlHierarchyId? node, Guid? id)
        {
            if (!id.HasValue && !string.IsNullOrEmpty(content))
            {
                var searchFileName = SearchInFileContent(content);
                if (!string.IsNullOrEmpty(searchFileName))
                {
                    return _repository.Get(new DocumentSpecification
                    {
                        FileName = searchFileName
                    });
                }
            }

            if (!id.HasValue && !string.IsNullOrEmpty(fileName))
            {
                var searchId = SearchDocument(fileName);
                if (searchId.HasValue)
                {
                    return _repository.Get(new DocumentSpecification
                    {
                        Id = searchId
                    });
                }
            }

            return _repository.Get(new DocumentSpecification
            {
                FileName = fileName,
                Node = node,
                Id = id
            });
        }

        private string SearchInFileContent(string content)
        {
            using (SearchIndexClient searchClient = new SearchIndexClient(
                _searchIndexSettings.ServiceName,
                _searchIndexSettings.BlobIndexName,
                new SearchCredentials(_searchIndexSettings.ServiceApiKey)))
            {
                var results = searchClient.Documents.Search<FileSearchResponse>(content);

                return results.Results.Select(x => x.Document.metadata_storage_name).FirstOrDefault();
            }
        }

        private Guid? SearchDocument(string fileName)
        {
            using (SearchIndexClient searchClient = new SearchIndexClient(
                _searchIndexSettings.ServiceName,
                _searchIndexSettings.SqlIndexName,
                new SearchCredentials(_searchIndexSettings.ServiceApiKey)))
            {
                var results = searchClient.Documents.Search<DocumentSearchResponse>(fileName);

                return results.Results.Select(x => x.Document.rowguid).FirstOrDefault();
            }
        }
    }
}