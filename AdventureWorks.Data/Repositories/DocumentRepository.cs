using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using AdventureWorks.Data.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Types;

namespace AdventureWorks.Data.Repositories
{
    internal class DocumentRepository : IDocumentRepository
    {
        private const string GetSql =
            "SELECT convert(nvarchar(20),[DocumentNode]) AS DocumentNode,[DocumentLevel],[Title],[Owner],[FolderFlag],[FileName],[FileExtension],[Revision],[ChangeNumber],[Status],[DocumentSummary],[Document] AS DocumentData,[rowguid] AS Id,[ModifiedDate] FROM [Production].[Document] WHERE 1=1";

        private const string GetNewDocumentNoteSql =
            @"DECLARE @DocumentNode hierarchyid
SET @DocumentNode = @ParentNode;
SELECT convert(nvarchar(20), @DocumentNode.GetDescendant((SELECT MAX([DocumentNode]) FROM [Production].[Document] WHERE [DocumentNode].GetAncestor(1) = @DocumentNode), NULL));";

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
            var parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(specification.FileName))
            {
                queryBuilder.Append($" AND LOWER([FileName]) = LOWER(@FileName)");
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

            var result = _connection.QueryFirstOrDefault<dynamic>(queryBuilder.ToString(),
                new
                {
                    specification.FileName,
                    ParentNode = specification.ParentNode.ToString(),
                    Node = specification.Node.ToString(),
                    specification.Id
                });

            if(result == null)
            {
                return null;
            }

            var document = new Document();
            document.DocumentNode = SqlHierarchyId.Parse(result.DocumentNode);
            document.DocumentLevel = Convert.ToInt32(result.DocumentLevel);
            document.Title = result.Title.ToString();
            document.Owner = Convert.ToInt32(result.Owner);
            document.FolderFlag = Convert.ToBoolean(result.FolderFlag);
            document.FileName = result.FileName.ToString();
            document.FileExtension = result.FileExtension.ToString();
            document.Revision = result.Revision.ToString();
            document.ChangeNumber = Convert.ToInt32(result.ChangeNumber);
            document.Status = Convert.ToInt32(result.Status);
            document.DocumentSummary = result.DocumentSummary.ToString();
            document.DocumentData = (byte[])result.DocumentData;
            document.Id = Guid.Parse(result.Id.ToString());
            document.ModifiedDate = Convert.ToDateTime(result.ModifiedDate);

            return document;
        }

        public void Add(Document document)
        {
            var nodeString = document.DocumentNode.ToString();
            var command = new CommandDefinition(
                commandText: InsertSql,
                new
                {
                    document.Id,
                    document.ModifiedDate,
                    DocumentNode = nodeString,
                    document.Owner,
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
                    ParentNode = parentNode.ToString(),
                });

            return SqlHierarchyId.Parse(_connection.QuerySingle<string>(command).ToString());
        }
    }
}