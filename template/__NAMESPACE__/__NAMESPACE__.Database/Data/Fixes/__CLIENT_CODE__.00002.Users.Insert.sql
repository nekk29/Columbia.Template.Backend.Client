DECLARE @User NVARCHAR(256) = 'administrator';
DECLARE @Date DATETIME = GETDATE();

DECLARE @DataTable TABLE (
  [Id] INT IDENTITY(1, 1),
  [FirstName] VARCHAR(100),
  [LastName] VARCHAR(100),
  [UserName] VARCHAR(256),
  [Email] VARCHAR(256),
  [PasswordHash] NVARCHAR(MAX),
  [SecurityStamp] NVARCHAR(MAX)
);


INSERT INTO @DataTable([FirstName], [LastName], [UserName], [Email], [PasswordHash], [SecurityStamp])
VALUES (
  'System',
  'Admin',
  '__CLIENT_CODE__.systemadmin@yourdomain.com',
  '__CLIENT_CODE__.systemadmin@yourdomain.com',
  '[PASSWORD_HASH]',
  '[SECURITY_STAMP]'
);


INSERT INTO [dbo].[Users] (
  [Id],
  [FirstName],
  [LastName],
  [UserName],
  [NormalizedUserName],
  [Email],
  [NormalizedEmail],
  [EmailConfirmed],
  [PasswordHash],
  [SecurityStamp],
  [ConcurrencyStamp],
  [PhoneNumber],
  [PhoneNumberConfirmed],
  [TwoFactorEnabled],
  [LockoutEnd],
  [LockoutEnabled],
  [AccessFailedCount],
  [CreationUser],
  [CreationDate],
  [UpdateUser],
  [UpdateDate],
  [IsActive]
)
SELECT
  NEWID(),
  [dt].[FirstName],
  [dt].[LastName],
  [dt].[UserName],
  UPPER([dt].[UserName]),
  [dt].[Email],
  UPPER([dt].[Email]),
  1,
  [dt].[PasswordHash],
  [dt].[SecurityStamp],
  NEWID(),
  NULL,
  0,
  0,
  NULL,
  1,
  0,
  @User,
  @Date,
  @User,
  @Date,
  1
FROM @DataTable [dt]
WHERE NOT EXISTS (
  SELECT TOP 1 1
  FROM [dbo].[Users]
  WHERE 1 = 1
    AND [UserName] = [dt].[UserName]
    AND [Email] = [dt].[Email]
);
