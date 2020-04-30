using System;
using System.IO;
using AdventureWorks.Data.Models;
using AdventureWorks.Data.Repositories;
using Microsoft.SqlServer.Types;

namespace AdventureWorks.Services.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentFactory _factory;
        private readonly IDocumentRepository _repository;

        public DocumentService(
            IDocumentFactory factory,
            IDocumentRepository repository)
        {
            _factory = factory;
            _repository = repository;
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

                parentNode = document.DocumentNode;

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

        public Document Get(string fileName, SqlHierarchyId? node, Guid? id)
        {
            return _repository.Get(new DocumentSpecification
            {
                FileName = fileName,
                Node = node,
                Id = id
            });
        }
    }
}