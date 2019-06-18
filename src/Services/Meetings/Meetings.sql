IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190514022250_InitialMigration')
BEGIN
    CREATE TABLE [Roles] (
        [Id] int NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Note] nvarchar(max) NULL,
        [Order] int NOT NULL DEFAULT 100,
        [Active] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190514022250_InitialMigration')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190514022250_InitialMigration', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190514023415_MeetingsTable')
BEGIN
    CREATE TABLE [Meetings] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(255) NOT NULL,
        [Note] nvarchar(max) NULL,
        [Date] datetime2 NOT NULL,
        [Active] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Meetings] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190514023415_MeetingsTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190514023415_MeetingsTable', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190514024934_AttendeesTable')
BEGIN
    CREATE TABLE [Attendees] (
        [Id] int NOT NULL IDENTITY,
        [MeetingId] int NOT NULL,
        [RoleId] int NOT NULL,
        [MemberId] int NULL,
        [Order] int NOT NULL DEFAULT 100,
        [Active] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Attendees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Attendees_Meetings_MeetingId] FOREIGN KEY ([MeetingId]) REFERENCES [Meetings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Attendees_Roles_MeetingId] FOREIGN KEY ([MeetingId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190514024934_AttendeesTable')
BEGIN
    CREATE INDEX [IX_Attendees_MeetingId] ON [Attendees] ([MeetingId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190514024934_AttendeesTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190514024934_AttendeesTable', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190515032706_CreatedAt')
BEGIN
    ALTER TABLE [Roles] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000Z';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190515032706_CreatedAt')
BEGIN
    ALTER TABLE [Roles] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000Z';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190515032706_CreatedAt')
BEGIN
    ALTER TABLE [Meetings] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000Z';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190515032706_CreatedAt')
BEGIN
    ALTER TABLE [Meetings] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000Z';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190515032706_CreatedAt')
BEGIN
    ALTER TABLE [Attendees] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000Z';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190515032706_CreatedAt')
BEGIN
    ALTER TABLE [Attendees] ADD [Member] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190515032706_CreatedAt')
BEGIN
    ALTER TABLE [Attendees] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000Z';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190515032706_CreatedAt')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190515032706_CreatedAt', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190518004637_FixAttendeeRoleRelation')
BEGIN
    ALTER TABLE [Attendees] DROP CONSTRAINT [FK_Attendees_Roles_MeetingId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190518004637_FixAttendeeRoleRelation')
BEGIN
    CREATE INDEX [IX_Attendees_RoleId] ON [Attendees] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190518004637_FixAttendeeRoleRelation')
BEGIN
    ALTER TABLE [Attendees] ADD CONSTRAINT [FK_Attendees_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190518004637_FixAttendeeRoleRelation')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190518004637_FixAttendeeRoleRelation', N'2.2.4-servicing-10062');
END;

GO

