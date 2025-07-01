-- Simple script để fix EF Migrations history
-- Chạy trong SQL Server Management Studio

USE ITMMS_DB;
GO

-- Tạo hoặc clear __EFMigrationsHistory table
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
BEGIN
    TRUNCATE TABLE [__EFMigrationsHistory];
    PRINT '__EFMigrationsHistory table cleared.'
END
ELSE
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory](
        [MigrationId] [nvarchar](150) NOT NULL,
        [ProductVersion] [nvarchar](32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
    PRINT '__EFMigrationsHistory table created.'
END

-- Insert tất cả migrations như đã applied
INSERT INTO [__EFMigrationsHistory] (MigrationId, ProductVersion) VALUES
('20250605093141_InitialCreate', '8.0.8'),
('20250612085834_AddTreatmentHistoryAndUserFeedback', '8.0.8'),
('20250612094258_AddUserProfileFields', '8.0.8'),
('20250623052143_InitialMigration', '8.0.8'),
('20250623060536_AddCheckoutFeatures', '8.0.8'),
('20250623085916_AddTreatmentServiceAndEnhancements', '8.0.8');

PRINT 'All migrations marked as applied.';

-- Verification
SELECT MigrationId, ProductVersion FROM [__EFMigrationsHistory] ORDER BY MigrationId; 