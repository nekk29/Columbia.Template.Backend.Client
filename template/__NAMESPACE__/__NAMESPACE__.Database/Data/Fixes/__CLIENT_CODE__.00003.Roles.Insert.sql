DECLARE @User NVARCHAR(256) = 'administrator';
DECLARE @Date DATETIME = GETDATE();

DECLARE @ApplicationCode VARCHAR(32) = '__CLIENT_CODE__';
DECLARE @ApplicationId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Applications] WHERE [Code] = @ApplicationCode);

DECLARE @DataTable TABLE (
  [Id] INT IDENTITY(1, 1),
  [Name] VARCHAR(256)
);


INSERT INTO @DataTable([Name])
SELECT 'Admin __CLIENT_CODE__'


INSERT INTO [dbo].[Roles] (
  [Id],
  [ApplicationId],
  [Name],
  [NormalizedName],
  [ConcurrencyStamp],
  [CreationUser],
  [CreationDate],
  [UpdateUser],
  [UpdateDate],
  [IsActive]
)
SELECT
  NEWID(),
  @ApplicationId,
  [dt].[Name],
  UPPER([dt].[Name]),
  NEWID(),
  @User,
  @Date,
  @User,
  @Date,
  1
FROM @DataTable [dt]
WHERE 1 = 1
  AND @ApplicationId IS NOT NULL
  AND NOT EXISTS (
    SELECT TOP 1 1
    FROM [dbo].[Roles] 
    WHERE 1 = 1
      AND [Name] = [dt].[Name]
      AND [ApplicationId] = @ApplicationId
  );
