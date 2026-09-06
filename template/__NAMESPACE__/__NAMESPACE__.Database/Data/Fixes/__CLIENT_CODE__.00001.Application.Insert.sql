DECLARE @User NVARCHAR(256) = 'administrator';
DECLARE @Date DATETIME = GETDATE();

DECLARE @DataTable TABLE (
  [Id] INT IDENTITY(1, 1),
  [Code] VARCHAR(32),
  [Name] VARCHAR(256),
  [LogoUri] VARCHAR(2048),
  [ApplicationUri] VARCHAR(256)
);


INSERT INTO @DataTable([Code], [Name], [LogoUri], [ApplicationUri])
-- Users
SELECT '__CLIENT_CODE__', '__CLIENT_DESCRIPTION__', 'http://localhost:5174/src/assets/images/logos/company.jpg', 'http://localhost:5174'


INSERT INTO [dbo].[Applications] (
  [Id],
  [Code],
  [Name],
  [LogoUri],
  [ApplicationUri],
  [CreationUser],
  [CreationDate],
  [UpdateUser],
  [UpdateDate],
  [IsActive]
)
SELECT
  NEWID(),
  [Code],
  [Name],
  [LogoUri],
  [ApplicationUri],
  @User,
  @Date,
  @User,
  @Date,
  1
FROM @DataTable [dt]
WHERE NOT EXISTS (
  SELECT TOP 1 1
  FROM [dbo].[Applications] [a]
  WHERE [a].[Code] = [dt].[Code]
);
