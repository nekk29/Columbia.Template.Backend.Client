DECLARE @UserName VARCHAR(64) = 'system.account'
DECLARE @DateTime DATETIME = GETDATE()

DECLARE @ApplicationCode VARCHAR(20) = '__CLIENT_CODE__'
DECLARE @ApplicationName VARCHAR(20) = NULL
DECLARE @ApplicationId UNIQUEIDENTIFIER

SELECT TOP 1
  @ApplicationId    = [Id],
  @ApplicationName  = [Name]
FROM [dbo].[Applications]
WHERE [Code] = @ApplicationCode

DECLARE @ClientId VARCHAR(200) = '__CLIENT_CODE__'

IF @ApplicationId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [dbo].[OpenIddictApplications] WHERE [ClientId] = @ClientId)
BEGIN
  INSERT INTO [dbo].[OpenIddictApplications] (
    [Id],
    [ApplicationType],
    [ClientId],
    [ClientSecret],
    [ClientType],
    [ConcurrencyToken],
    [ConsentType],
    [DisplayName],
    [DisplayNames],
    [JsonWebKeySet],
    [Permissions],
    [PostLogoutRedirectUris],
    [Properties],
    [RedirectUris],
    [Requirements],
    [Settings]
  ) VALUES (
    NEWID(),          --[Id],
    'Web',            --[ApplicationType],
    @ClientId,        --[ClientId],
    NULL,             --[ClientSecret],
    'public',         --[ClientType],
    NEWID(),          --[ConcurrencyToken],
    NULL,             --[ConsentType],
    @ApplicationName, --[DisplayName],
    NULL,             --[DisplayNames],
    NULL,             --[JsonWebKeySet],
    NULL,             --[Permissions],
    NULL,             --[PostLogoutRedirectUris],
    NULL,             --[Properties],
    NULL,             --[RedirectUris],
    NULL,             --[Requirements],
    NULL              --[Settings]
  );

  UPDATE [dbo].[Applications] SET
    [ClientId] = @ClientId
  WHERE [Code] = @ApplicationCode;
END
