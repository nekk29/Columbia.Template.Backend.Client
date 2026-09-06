DECLARE @ClientId VARCHAR(200) = '__CLIENT_CODE__'

DECLARE @ColumnValue VARCHAR(MAX)

DECLARE @DataTable TABLE (
  [Id] INT IDENTITY(1, 1),
  [Name] VARCHAR(256)
);

INSERT INTO @DataTable([Name])
-- Endpoints
          SELECT 'ept:token'
UNION ALL SELECT 'ept:authorization'
UNION ALL SELECT 'ept:revocation'
UNION ALL SELECT 'ept:end_session'
-- GrantTypes
UNION ALL SELECT 'gt:password'
UNION ALL SELECT 'gt:authorization_code'
UNION ALL SELECT 'gt:client_credentials'
UNION ALL SELECT 'gt:refresh_token'
-- Scopes
UNION ALL SELECT 'scp:profile'
UNION ALL SELECT 'scp:email'
UNION ALL SELECT 'scp:roles'
UNION ALL SELECT 'scp:offline_access'
-- Scopes (Apis)
UNION ALL SELECT 'scp:' + @ClientId + '.apis'

SELECT
	@ColumnValue = '[' + STRING_AGG('"' + [Name] + '"', ',') + ']'
FROM @DataTable

UPDATE [dbo].[OpenIddictApplications] SET
	[Permissions] = @ColumnValue
WHERE [ClientId] = @ClientId
