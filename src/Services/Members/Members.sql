IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190520152139_TableMembersAddEmail')
BEGIN
    CREATE TABLE [Members] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(255) NOT NULL,
        [Email] nvarchar(255) NOT NULL,
        [Note] nvarchar(2048) NULL,
        [Active] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Members] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190520152139_TableMembersAddEmail')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190520152139_TableMembersAddEmail', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190521000736_TableMembersAddToastmastersId')
BEGIN
    ALTER TABLE [Members] ADD [Rank] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190521000736_TableMembersAddToastmastersId')
BEGIN
    ALTER TABLE [Members] ADD [ToastmastersId] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190521000736_TableMembersAddToastmastersId')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190521000736_TableMembersAddToastmastersId', N'2.2.4-servicing-10062');
END;

GO

