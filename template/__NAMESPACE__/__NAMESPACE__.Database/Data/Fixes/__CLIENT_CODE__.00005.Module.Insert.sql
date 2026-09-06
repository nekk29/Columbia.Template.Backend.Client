DECLARE @User NVARCHAR(256) = 'administrator';
DECLARE @Date DATETIME = GETDATE();

DECLARE @ApplicationCode VARCHAR(32) = '__CLIENT_CODE__';
DECLARE @ApplicationId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Applications] WHERE [Code] = @ApplicationCode);

DECLARE @DataTable TABLE (
  [Id] INT IDENTITY(1, 1),
  [Code] VARCHAR(64),
  [Name] VARCHAR(256),
  [Description] VARCHAR(1024)
);


INSERT INTO @DataTable([Code], [Name], [Description])
-- SYSTEM MODULES
          SELECT 'settings', 'Settings', 'Settings management module'


INSERT INTO [dbo].[Modules] (
  [Id],
  [ApplicationId],
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
  @ApplicationId,
  [Code],
  [Name],
  [Description],
  @User,
  @Date,
  @User,
  @Date,
  1
FROM @DataTable [dt]
WHERE 1 = 1
  AND @ApplicationId IS NOT NULL
  AND NOT EXISTS (
    SELECT TOP 1 1 FROM [dbo].[Modules] [m]
    INNER JOIN [dbo].[Applications] [a] ON [m].[ApplicationId] = [a].[Id]
    WHERE 1 = 1
      AND [m].[ApplicationId] = @ApplicationId
      AND [m].[Code] = [dt].[Code]
  );
