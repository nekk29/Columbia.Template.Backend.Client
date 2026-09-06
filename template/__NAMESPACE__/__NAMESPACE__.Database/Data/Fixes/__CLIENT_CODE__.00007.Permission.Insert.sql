DECLARE @User NVARCHAR(256) = 'administrator';
DECLARE @Date DATETIME = GETDATE();

DECLARE @ApplicationCode VARCHAR(32) = '__CLIENT_CODE__';
DECLARE @ApplicationId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Applications] WHERE [Code] = @ApplicationCode);

DECLARE @DataTable TABLE (
  [Id] INT IDENTITY(1, 1),
  [RoleName] NVARCHAR(256),
  [ActionCode] NVARCHAR(64)
);


INSERT INTO @DataTable([RoleName], [ActionCode])
-- Admin __CLIENT_CODE__
SELECT
  'Admin __CLIENT_CODE__',
  [ac].[Code]
FROM [dbo].[Actions] [ac]
INNER JOIN [dbo].[Modules] [m] ON [ac].[ModuleId] = [m].[Id]
WHERE 1 = 1
  AND [m].[ApplicationId] = @ApplicationId;


INSERT INTO [dbo].[Permissions] (
  [Id],
  [RoleId],
  [ActionId],
  [CreationUser],
  [CreationDate],
  [UpdateUser],
  [UpdateDate],
  [IsActive]
)
SELECT
  NEWID(),
  [r].[Id],
  [a].[Id],
  @User,
  @Date,
  @User,
  @Date,
  1
FROM @DataTable [dt]
INNER JOIN [dbo].[Roles] [r] ON [dt].[RoleName] = [r].[Name]
INNER JOIN [dbo].[Actions] [a] ON [dt].[ActionCode] = [a].[Code]
INNER JOIN [dbo].[Modules] [m] ON [a].[ModuleId] = [m].[Id]
WHERE 1 = 1
  AND [r].[Id] IS NOT NULL
  AND [a].[Id] IS NOT NULL
  AND [m].[ApplicationId] = @ApplicationId
  AND NOT EXISTS (
    SELECT 1 FROM [dbo].[Permissions] [p]
    WHERE 1 = 1
      AND [p].[RoleId] = [r].[Id]
      AND [p].[ActionId] = [a].[Id]
  )
ORDER BY
  [dt].[RoleName],
  [dt].[ActionCode];
