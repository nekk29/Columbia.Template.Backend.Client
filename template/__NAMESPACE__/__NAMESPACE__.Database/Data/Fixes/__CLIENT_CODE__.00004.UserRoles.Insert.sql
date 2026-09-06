DECLARE @User NVARCHAR(256) = 'administrator';

DECLARE @ApplicationCode VARCHAR(32) = '__CLIENT_CODE__';
DECLARE @ApplicationId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Applications] WHERE [Code] = @ApplicationCode);

DECLARE @RoleName NVARCHAR(256);
DECLARE @RoleId UNIQUEIDENTIFIER;

DECLARE @UserName NVARCHAR(256);
DECLARE @UserId UNIQUEIDENTIFIER;

DECLARE @DataTable TABLE (
  [Id] INT IDENTITY(1, 1),
  [RoleName] VARCHAR(256),
  [UserName] VARCHAR(256)
);


INSERT INTO @DataTable([RoleName], [UserName])
SELECT 'Admin __CLIENT_CODE__', '__CLIENT_CODE__.systemadmin@yourdomain.com'


DECLARE @Index INT = 1
DECLARE @Count INT = (SELECT COUNT(1) FROM @DataTable)

WHILE @Index <= @Count
BEGIN
  SELECT
    @RoleName = [RoleName],
    @UserName = [UserName]
  FROM @DataTable WHERE [Id] = @Index;

  SET @RoleId = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = @RoleName AND [ApplicationId] = @ApplicationId);
  SET @UserId = (SELECT TOP 1 [Id] FROM [dbo].[Users] WHERE [UserName] = @UserName);

  IF (@RoleId IS NOT NULL AND @UserId IS NOT NULL)
  BEGIN
    INSERT INTO [dbo].[UserRoles] (
      [UserId],
      [RoleId]
    ) 
    SELECT
      @UserId,
      @RoleId
    WHERE NOT EXISTS (
      SELECT TOP 1 1
      FROM [dbo].[UserRoles] [ur]
      WHERE 1 = 1
        AND [ur].[UserId] = @UserId
        AND [ur].[RoleId] = @RoleId
    );
  END

  SET @Index = @Index + 1
END
