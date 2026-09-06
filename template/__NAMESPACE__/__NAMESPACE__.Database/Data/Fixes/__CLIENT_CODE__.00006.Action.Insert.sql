DECLARE @User NVARCHAR(256) = 'administrator';
DECLARE @Date DATETIME = GETDATE();

DECLARE @ApplicationCode VARCHAR(32) = '__CLIENT_CODE__';
DECLARE @ApplicationId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Applications] WHERE [Code] = @ApplicationCode);

DECLARE @ParentId UNIQUEIDENTIFIER;
DECLARE @ParentCode VARCHAR(64);

DECLARE @DataTable TABLE (
  [Id] INT IDENTITY(1, 1),
  [ParentCode] VARCHAR(64),
  [ModuleCode] VARCHAR(64),
  [Code] VARCHAR(64),
  [Name] VARCHAR(256),
  [Description] VARCHAR(1024)
);


INSERT INTO @DataTable([ParentCode], [ModuleCode], [Code], [Name], [Description])
-- SYSTEM ACTIONS
-- Settings
          SELECT NULL, 'settings', 'settings.edit', 'Edit Settings', 'Allows you to edit existing settings'
UNION ALL SELECT NULL, 'settings', 'settings.search', 'Search Settings', 'Allows you to search settings by criteria'
UNION ALL SELECT NULL, 'settings', 'settings.export', 'Export Settings', 'Allows you to export settings by criteria'


DECLARE @Index INT = 1
DECLARE @Count INT = (SELECT COUNT(1) FROM @DataTable)

WHILE @Index <= @Count
BEGIN
  SET @ParentId = NULL;

  SELECT TOP 1
    @ParentCode = [ParentCode]
  FROM @DataTable WHERE [Id] = @Index;

  SELECT TOP 1
    @ParentId = [ac].[Id]
  FROM [dbo].[Actions] [ac]
  INNER JOIN [dbo].[Modules] [m] ON [ac].[ModuleId] = [m].[Id]
  WHERE 1 = 1
    AND @ParentCode IS NOT NULL
    AND [ac].[Code] = @ParentCode
    AND [m].[ApplicationId] = @ApplicationId;

  INSERT INTO [dbo].[Actions] (
    [Id],
    [ParentActionId],
    [ModuleId],
    [Code],
    [Name],
    [Description],
    [CreationUser],
    [CreationDate],
    [UpdateUser],
    [UpdateDate],
    [IsActive]
  )
  SELECT
    NEWID(),
    @ParentId,
    [m].[Id],
    [dt].[Code],
    [dt].[Name],
    [dt].[Description],
    @User,
    @Date,
    @User,
    @Date,
    1
  FROM @DataTable [dt]
  INNER JOIN [dbo].[Modules] [m] ON [dt].[ModuleCode] = [m].[Code]
  WHERE 1 = 1
    AND [dt].[Id] = @Index
    AND [m].[Id] IS NOT NULL
    AND [m].[ApplicationId] = @ApplicationId
    AND NOT EXISTS (
      SELECT TOP 1 1
      FROM [dbo].[Actions] [ac]
      INNER JOIN [dbo].[Modules] [m] ON [ac].[ModuleId] = [m].[Id]
      WHERE 1 = 1
        AND [ac].[Code] = [dt].[Code]
        AND [m].[ApplicationId] = @ApplicationId
    );

  SET @Index = @Index + 1
END
