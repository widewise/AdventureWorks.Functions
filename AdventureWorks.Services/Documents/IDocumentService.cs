﻿using System;
using AdventureWorks.Data.Models;
using Microsoft.SqlServer.Types;

namespace AdventureWorks.Services.Documents
{
    public interface IDocumentService
    {
        Guid Create(DocumentCreateParameters parameters);

        Document Get(string fileName, string content, SqlHierarchyId? node, Guid? id);
    }
}