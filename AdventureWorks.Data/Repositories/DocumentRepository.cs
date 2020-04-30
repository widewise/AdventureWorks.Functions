using System.Data;
using System.Text;
using AdventureWorks.Data.Models;
using Dapper;
using Microsoft.SqlServer.Types;

namespace AdventureWorks.Data.Repositories
{
    internal class DocumentRepository : IDocumentRepository
    {
        private const string GetSql =
            "SELECT [DocumentNode],[DocumentLevel],[Title],[Owner],[FolderFlag],[FileName],[FileExtension],[Revision],[ChangeNumber],[Status],[DocumentSummary],[Document] AS DocumentData,[rowguid] AS Id,[ModifiedDate] FROM [Production].[Document] WHERE 1=1";

        private const string GetNewDocumentNoteSql =
            "@ParentNode.GetDescendant((SELECT MAX([DocumentNode]) FROM [Production].[Document] WHERE [DocumentNode].GetAncestor(1) = @ParentNode), NULL)";

        private const string InsertSql =
            @"INSERT INTO [Production].[Document] ([DocumentNode],[Title],[Owner],[FolderFlag],[FileName],[FileExtension],[Revision],[ChangeNumber],[Status],[DocumentSummary],[Document],[rowguid],[ModifiedDate])
        VALUES (@DocumentNode,@Title,@Owner,@FolderFlag,@FileName,@FileExtension,@Revision,@ChangeNumber,@Status,@DocumentSummary,@DocumentData,@Id,@ModifiedDate)";

        private readonly IDbConnection _connection;

        public DocumentRepository(
            IDbConnection connection)
        {
            _connection = connection;
        }

        public Document Get(DocumentSpecification specification)
        {
            var queryBuilder = new StringBuilder(GetSql);

            if (string.IsNullOrEmpty(specification.FileName))
            {
                queryBuilder.Append($" AND [FileName] = @FileName");
            }

            if (specification.DocumentLevel.HasValue)
            {
                queryBuilder.Append($" AND [DocumentLevel] = @DocumentLevel");
            }

            if (specification.Node.HasValue)
            {
                queryBuilder.Append($" AND [DocumentNode] = @Node");
            }

            if (specification.Id.HasValue)
            {
                queryBuilder.Append($" AND [rowguid] = @Id");
            }

            var command = new CommandDefinition(
                commandText: queryBuilder.ToString(),
                new
                {
                    specification.FileName,
                    Node = specification.Node,
                    specification.Id
                });

            return _connection.QueryFirst<Document>(command);
        }

        public void Add(Document document)
        {
            var command = new CommandDefinition(
                commandText: InsertSql,
                new
                {
                    document.Id,
                    document.ModifiedDate,
                    document.DocumentNode,
                    document.Title,
                    document.FolderFlag,
                    document.FileName,
                    document.FileExtension,
                    document.Revision,
                    document.ChangeNumber,
                    document.Status,
                    document.DocumentSummary,
                    document.DocumentData
                });

            _connection.Execute(command);
        }

        public SqlHierarchyId GetNewDocumentNode(SqlHierarchyId parentNode)
        {
            var command = new CommandDefinition(
                commandText: GetNewDocumentNoteSql,
                new
                {
                    ParentNode = parentNode,
                });

            return _connection.QuerySingle<SqlHierarchyId>(command);
        }
    }
}