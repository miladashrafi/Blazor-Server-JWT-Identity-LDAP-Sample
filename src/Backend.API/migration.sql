IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [Disabled] bit NOT NULL,
    [Name] nvarchar(max) NULL,
    [Family] nvarchar(max) NULL,
    [RegisterDate] datetime2 NOT NULL,
    [RefreshToken] nvarchar(max) NULL,
    [RefreshTokenExpireTime] datetime2 NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] ON;
INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
VALUES (N'03B11179-8A33-4D3B-8092-463249F755A5', N'ABCD1254-FEE2-42D0-A96E-E2018C9161BF', N'user', N'USER'),
(N'CF0B61E1-3BB2-40D6-8E17-60CF475CE884', N'D5FC26A2-75FD-45F8-AB88-DF4D337C5910', N'admin', N'ADMIN');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Disabled', N'Email', N'EmailConfirmed', N'Family', N'LockoutEnabled', N'LockoutEnd', N'Name', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'RefreshToken', N'RefreshTokenExpireTime', N'RegisterDate', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
    SET IDENTITY_INSERT [AspNetUsers] ON;
INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [ConcurrencyStamp], [Disabled], [Email], [EmailConfirmed], [Family], [LockoutEnabled], [LockoutEnd], [Name], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [RefreshToken], [RefreshTokenExpireTime], [RegisterDate], [SecurityStamp], [TwoFactorEnabled], [UserName])
VALUES (N'c784d6e7-4424-4fe1-a1bb-b03c6a9a26cb', 0, N'2fe7f8d5-d321-4c77-884b-73ea438b1511', CAST(0 AS bit), N'milad.ashrafi@gmail.com', CAST(1 AS bit), N'Ashrafi (Admin)', CAST(1 AS bit), NULL, N'Milad', N'MILAD.ASHRAFI@GMAIL.COM', N'MILAD.ASHRAFI@GMAIL.COM', 'AQAAAAIAAYagAAAAEK1W3FMebsaQ5p6sqwXybnO6AdMcllqC99NBccKaS99FJZji0MmRjLfY4vMAR/ldRA==', N'09127372975', CAST(1 AS bit), NULL, '0001-01-01T00:00:00.0000000', '2023-08-05T21:28:39.0905955+03:30', N'LJNTPIYBD4KN2CFESBRMRL2YDQOXANQ4', CAST(0 AS bit), N'milad.ashrafi@gmail.com'),
(N'f0dccee8-a3e1-45f8-9bb7-f7e7decebd09', 0, N'6b263a8b-120f-4f48-a6bd-ad3a9c4c913d', CAST(0 AS bit), N'ashrafi.milad@gmail.com', CAST(1 AS bit), N'Ashrafi (user)', CAST(1 AS bit), NULL, N'Milad', N'ASHRAFI.MILAD@GMAIL.COM', N'ASHRAFI.MILAD@GMAIL.COM', 'AQAAAAIAAYagAAAAEK1W3FMebsaQ5p6sqwXybnO6AdMcllqC99NBccKaS99FJZji0MmRjLfY4vMAR/ldRA==', N'09127372975', CAST(1 AS bit), NULL, '0001-01-01T00:00:00.0000000', '2023-08-05T21:28:39.0906005+03:30', N'OHACRUB556PUCIJOKNPX6QMTHA5G77DG', CAST(0 AS bit), N'ashrafi.milad@gmail.com');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Disabled', N'Email', N'EmailConfirmed', N'Family', N'LockoutEnabled', N'LockoutEnd', N'Name', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'RefreshToken', N'RefreshTokenExpireTime', N'RegisterDate', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
    SET IDENTITY_INSERT [AspNetUsers] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClaimType', N'ClaimValue', N'RoleId') AND [object_id] = OBJECT_ID(N'[AspNetRoleClaims]'))
    SET IDENTITY_INSERT [AspNetRoleClaims] ON;
INSERT INTO [AspNetRoleClaims] ([Id], [ClaimType], [ClaimValue], [RoleId])
VALUES (1, N'Permission', N'Administrative.ViewUsers', N'03B11179-8A33-4D3B-8092-463249F755A5'),
(2, N'Permission', N'Administrative.ViewUsers', N'CF0B61E1-3BB2-40D6-8E17-60CF475CE884'),
(3, N'Permission', N'Administrative.ManageUsers', N'CF0B61E1-3BB2-40D6-8E17-60CF475CE884');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClaimType', N'ClaimValue', N'RoleId') AND [object_id] = OBJECT_ID(N'[AspNetRoleClaims]'))
    SET IDENTITY_INSERT [AspNetRoleClaims] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
    SET IDENTITY_INSERT [AspNetUserRoles] ON;
INSERT INTO [AspNetUserRoles] ([RoleId], [UserId])
VALUES (N'CF0B61E1-3BB2-40D6-8E17-60CF475CE884', N'c784d6e7-4424-4fe1-a1bb-b03c6a9a26cb'),
(N'03B11179-8A33-4D3B-8092-463249F755A5', N'f0dccee8-a3e1-45f8-9bb7-f7e7decebd09');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
    SET IDENTITY_INSERT [AspNetUserRoles] OFF;
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230805175839_InitializeDatabase', N'8.0.0-preview.6.23329.4');
GO

COMMIT;
GO

