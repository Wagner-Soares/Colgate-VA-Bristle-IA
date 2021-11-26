
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/22/2021 23:26:15
-- Generated from EDMX file: E:\calgate-frontend\Bristle\Database\ColgateSkelta4.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ColgateSkelta];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AnalyzeSetBristleAnalysisResultSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BristleAnalysisResultSet] DROP CONSTRAINT [FK_AnalyzeSetBristleAnalysisResultSet];
GO
IF OBJECT_ID(N'[dbo].[FK_AnalyzeSetBrushAnalysisResultSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BrushAnalysisResultSet] DROP CONSTRAINT [FK_AnalyzeSetBrushAnalysisResultSet];
GO
IF OBJECT_ID(N'[dbo].[FK_AnalyzeSetTuffAnalysisResultSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TuffAnalysisResultSet] DROP CONSTRAINT [FK_AnalyzeSetTuffAnalysisResultSet];
GO
IF OBJECT_ID(N'[dbo].[FK_DatasetSetSample_ASet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample_ASet] DROP CONSTRAINT [FK_DatasetSetSample_ASet];
GO
IF OBJECT_ID(N'[dbo].[FK_ModelsDatasets]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DatasetsSet] DROP CONSTRAINT [FK_ModelsDatasets];
GO
IF OBJECT_ID(N'[dbo].[FK_ModelsValidationDatasets]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ValidationDatasets] DROP CONSTRAINT [FK_ModelsValidationDatasets];
GO
IF OBJECT_ID(N'[dbo].[FK_RegistrationWaitingSetTuftTempSetSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TuftTempSetSet] DROP CONSTRAINT [FK_RegistrationWaitingSetTuftTempSetSet];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_ASetTuftSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TuftSet] DROP CONSTRAINT [FK_Sample_ASetTuftSet];
GO
IF OBJECT_ID(N'[dbo].[FK_TuftSetBristleSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BristleSet] DROP CONSTRAINT [FK_TuftSetBristleSet];
GO
IF OBJECT_ID(N'[dbo].[FK_TuftSetImageSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ImageSet] DROP CONSTRAINT [FK_TuftSetImageSet];
GO
IF OBJECT_ID(N'[dbo].[FK_TuftTempSetSetBristleTempSetSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BristleTempSetSet] DROP CONSTRAINT [FK_TuftTempSetSetBristleTempSetSet];
GO
IF OBJECT_ID(N'[dbo].[FK_TuftTempSetSetImageTempSetSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ImageTempSetSet] DROP CONSTRAINT [FK_TuftTempSetSetImageTempSetSet];
GO
IF OBJECT_ID(N'[dbo].[FK_ValidationDatasetVsample_ASet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Vsample_ASet] DROP CONSTRAINT [FK_ValidationDatasetVsample_ASet];
GO
IF OBJECT_ID(N'[dbo].[FK_Vsample_ASetVtuftSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VtuftSets] DROP CONSTRAINT [FK_Vsample_ASetVtuftSet];
GO
IF OBJECT_ID(N'[dbo].[FK_VtuftSetVbristleSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VbristleSets] DROP CONSTRAINT [FK_VtuftSetVbristleSet];
GO
IF OBJECT_ID(N'[dbo].[FK_VtuftSetVimageSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VimageSets] DROP CONSTRAINT [FK_VtuftSetVimageSet];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AI_Sample_log]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AI_Sample_log];
GO
IF OBJECT_ID(N'[dbo].[AnalyzeSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AnalyzeSet];
GO
IF OBJECT_ID(N'[dbo].[BristleAnalysisResultSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BristleAnalysisResultSet];
GO
IF OBJECT_ID(N'[dbo].[BristleSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BristleSet];
GO
IF OBJECT_ID(N'[dbo].[BristleTempSetSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BristleTempSetSet];
GO
IF OBJECT_ID(N'[dbo].[BrushAnalysisResultSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BrushAnalysisResultSet];
GO
IF OBJECT_ID(N'[dbo].[DatasetSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DatasetSet];
GO
IF OBJECT_ID(N'[dbo].[DatasetsSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DatasetsSet];
GO
IF OBJECT_ID(N'[dbo].[GeneralSettings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GeneralSettings];
GO
IF OBJECT_ID(N'[dbo].[ImageSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ImageSet];
GO
IF OBJECT_ID(N'[dbo].[ImageTempSetSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ImageTempSetSet];
GO
IF OBJECT_ID(N'[dbo].[ModelsSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ModelsSet];
GO
IF OBJECT_ID(N'[dbo].[RegistrationWaitingSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegistrationWaitingSet];
GO
IF OBJECT_ID(N'[dbo].[Sample_ASet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sample_ASet];
GO
IF OBJECT_ID(N'[dbo].[TuffAnalysisResultSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TuffAnalysisResultSet];
GO
IF OBJECT_ID(N'[dbo].[TuftSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TuftSet];
GO
IF OBJECT_ID(N'[dbo].[TuftTempSetSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TuftTempSetSet];
GO
IF OBJECT_ID(N'[dbo].[UserSystems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSystems];
GO
IF OBJECT_ID(N'[dbo].[ValidationDatasets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ValidationDatasets];
GO
IF OBJECT_ID(N'[dbo].[ValidationDatasetSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ValidationDatasetSet];
GO
IF OBJECT_ID(N'[dbo].[VbristleSets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VbristleSets];
GO
IF OBJECT_ID(N'[dbo].[VimageSets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VimageSets];
GO
IF OBJECT_ID(N'[dbo].[Vsample_ASet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Vsample_ASet];
GO
IF OBJECT_ID(N'[dbo].[VtuftSets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VtuftSets];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AnalyzeSet'
CREATE TABLE [dbo].[AnalyzeSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [iSKU_id] int  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [Equipament] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'BristleAnalysisResultSet'
CREATE TABLE [dbo].[BristleAnalysisResultSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DefectClassification] nvarchar(max)  NOT NULL,
    [DefectIdentified] nvarchar(max)  NOT NULL,
    [Probability] int  NOT NULL,
    [SelectedManual] bit  NOT NULL,
    [X] int  NOT NULL,
    [Y] int  NOT NULL,
    [Width] int  NOT NULL,
    [Height] int  NOT NULL,
    [Position] nvarchar(max)  NOT NULL,
    [AnalyzeSet_Id] int  NOT NULL
);
GO

-- Creating table 'BristleSet'
CREATE TABLE [dbo].[BristleSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Classification] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [X] int  NOT NULL,
    [Y] int  NOT NULL,
    [Width] int  NOT NULL,
    [Height] int  NOT NULL,
    [Probability] float  NOT NULL,
    [TuftSet_Id] int  NOT NULL
);
GO

-- Creating table 'BristleTempSetSet'
CREATE TABLE [dbo].[BristleTempSetSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Classification] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [X] int  NOT NULL,
    [Y] int  NOT NULL,
    [Width] int  NOT NULL,
    [Height] int  NOT NULL,
    [Probability] float  NOT NULL,
    [TuftTempSetSet_Id] int  NOT NULL
);
GO

-- Creating table 'BrushAnalysisResultSet'
CREATE TABLE [dbo].[BrushAnalysisResultSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TotalBristles] int  NOT NULL,
    [TotalGoodBristles] int  NOT NULL,
    [TotalBristlesAnalyzed] int  NOT NULL,
    [AnalysisResult] nvarchar(max)  NOT NULL,
    [Hybrid] bit  NOT NULL,
    [Signaling_Id] int  NOT NULL,
    [AnalyzeSet_Id] int  NOT NULL
);
GO

-- Creating table 'DatasetSet'
CREATE TABLE [dbo].[DatasetSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Historic] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ImageSet'
CREATE TABLE [dbo].[ImageSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Path] nvarchar(max)  NOT NULL,
    [TuftSet_Id] int  NOT NULL
);
GO

-- Creating table 'ImageTempSetSet'
CREATE TABLE [dbo].[ImageTempSetSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Path] nvarchar(max)  NOT NULL,
    [TuftTempSetSet_Id] int  NOT NULL
);
GO

-- Creating table 'RegistrationWaitingSet'
CREATE TABLE [dbo].[RegistrationWaitingSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DataSet_id] nvarchar(max)  NOT NULL,
    [Sample_id] nvarchar(max)  NOT NULL,
    [AnalyzeSet_id] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Sample_ASet'
CREATE TABLE [dbo].[Sample_ASet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [SKU_Id] int  NOT NULL,
    [DatasetSet_Id] int  NOT NULL
);
GO

-- Creating table 'TuffAnalysisResultSet'
CREATE TABLE [dbo].[TuffAnalysisResultSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Position] nvarchar(max)  NOT NULL,
    [TotalBristlesFoundNN] int  NOT NULL,
    [TotalBristleFoundManual] int  NOT NULL,
    [SelectedManual] bit  NOT NULL,
    [Probability] nvarchar(max)  NOT NULL,
    [AnalyzeSet_Id] int  NOT NULL
);
GO

-- Creating table 'TuftSet'
CREATE TABLE [dbo].[TuftSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Position] nvarchar(max)  NOT NULL,
    [Sample_ASet_Id] int  NOT NULL
);
GO

-- Creating table 'TuftTempSetSet'
CREATE TABLE [dbo].[TuftTempSetSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Position] nvarchar(max)  NOT NULL,
    [RegistrationWaitingSet_Id] int  NOT NULL
);
GO

-- Creating table 'ModelsSet'
CREATE TABLE [dbo].[ModelsSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PerformanceType1] float  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [PerformanceType2] float  NOT NULL,
    [PerformanceType3] float  NOT NULL,
    [PerformanceNone] float  NOT NULL,
    [PerformanceLocalization] float  NOT NULL
);
GO

-- Creating table 'DatasetsSet'
CREATE TABLE [dbo].[DatasetsSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Dataset_id] int  NOT NULL,
    [Models_Id] int  NOT NULL
);
GO

-- Creating table 'ValidationDatasetSet'
CREATE TABLE [dbo].[ValidationDatasetSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Historic] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Vsample_ASet'
CREATE TABLE [dbo].[Vsample_ASet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [SKU_Id] int  NOT NULL,
    [ValidationDataset_Id] int  NOT NULL
);
GO

-- Creating table 'VtuftSets'
CREATE TABLE [dbo].[VtuftSets] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Position] nvarchar(max)  NOT NULL,
    [Vsample_ASet_Id] int  NOT NULL
);
GO

-- Creating table 'VbristleSets'
CREATE TABLE [dbo].[VbristleSets] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Classification] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [X] int  NOT NULL,
    [Y] int  NOT NULL,
    [Width] int  NOT NULL,
    [Height] int  NOT NULL,
    [Probability] float  NOT NULL,
    [VtuftSet_Id] int  NOT NULL
);
GO

-- Creating table 'VimageSets'
CREATE TABLE [dbo].[VimageSets] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Path] nvarchar(max)  NOT NULL,
    [VtuftSet_Id] int  NOT NULL
);
GO

-- Creating table 'ValidationDatasets'
CREATE TABLE [dbo].[ValidationDatasets] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ValidationDataset_id] int  NOT NULL,
    [Model_Id] int  NOT NULL
);
GO

-- Creating table 'UserSystems'
CREATE TABLE [dbo].[UserSystems] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Salt] nvarchar(max)  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Key] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'GeneralSettings'
CREATE TABLE [dbo].[GeneralSettings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Prefix] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'AI_Sample_log'
CREATE TABLE [dbo].[AI_Sample_log] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [iStatus_id] int  NOT NULL,
    [bActive] bit  NULL,
    [iShift] int  NOT NULL,
    [iTest_id] int  NOT NULL,
    [sEquipament] nvarchar(200)  NULL,
    [sArea] nvarchar(200)  NULL,
    [sBatchLote] nvarchar(200)  NULL,
    [dtSample] datetime  NULL,
    [fResult] float  NOT NULL,
    [sOperator] nvarchar(200)  NOT NULL,
    [dtPublished_at] datetime  NOT NULL,
    [sComments] nvarchar(200)  NOT NULL,
    [sCreated_by] nvarchar(200)  NOT NULL,
    [dtCreated_at] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'AnalyzeSet'
ALTER TABLE [dbo].[AnalyzeSet]
ADD CONSTRAINT [PK_AnalyzeSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BristleAnalysisResultSet'
ALTER TABLE [dbo].[BristleAnalysisResultSet]
ADD CONSTRAINT [PK_BristleAnalysisResultSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BristleSet'
ALTER TABLE [dbo].[BristleSet]
ADD CONSTRAINT [PK_BristleSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BristleTempSetSet'
ALTER TABLE [dbo].[BristleTempSetSet]
ADD CONSTRAINT [PK_BristleTempSetSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BrushAnalysisResultSet'
ALTER TABLE [dbo].[BrushAnalysisResultSet]
ADD CONSTRAINT [PK_BrushAnalysisResultSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DatasetSet'
ALTER TABLE [dbo].[DatasetSet]
ADD CONSTRAINT [PK_DatasetSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ImageSet'
ALTER TABLE [dbo].[ImageSet]
ADD CONSTRAINT [PK_ImageSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ImageTempSetSet'
ALTER TABLE [dbo].[ImageTempSetSet]
ADD CONSTRAINT [PK_ImageTempSetSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RegistrationWaitingSet'
ALTER TABLE [dbo].[RegistrationWaitingSet]
ADD CONSTRAINT [PK_RegistrationWaitingSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Sample_ASet'
ALTER TABLE [dbo].[Sample_ASet]
ADD CONSTRAINT [PK_Sample_ASet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TuffAnalysisResultSet'
ALTER TABLE [dbo].[TuffAnalysisResultSet]
ADD CONSTRAINT [PK_TuffAnalysisResultSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TuftSet'
ALTER TABLE [dbo].[TuftSet]
ADD CONSTRAINT [PK_TuftSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TuftTempSetSet'
ALTER TABLE [dbo].[TuftTempSetSet]
ADD CONSTRAINT [PK_TuftTempSetSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ModelsSet'
ALTER TABLE [dbo].[ModelsSet]
ADD CONSTRAINT [PK_ModelsSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DatasetsSet'
ALTER TABLE [dbo].[DatasetsSet]
ADD CONSTRAINT [PK_DatasetsSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ValidationDatasetSet'
ALTER TABLE [dbo].[ValidationDatasetSet]
ADD CONSTRAINT [PK_ValidationDatasetSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Vsample_ASet'
ALTER TABLE [dbo].[Vsample_ASet]
ADD CONSTRAINT [PK_Vsample_ASet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VtuftSets'
ALTER TABLE [dbo].[VtuftSets]
ADD CONSTRAINT [PK_VtuftSets]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VbristleSets'
ALTER TABLE [dbo].[VbristleSets]
ADD CONSTRAINT [PK_VbristleSets]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VimageSets'
ALTER TABLE [dbo].[VimageSets]
ADD CONSTRAINT [PK_VimageSets]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ValidationDatasets'
ALTER TABLE [dbo].[ValidationDatasets]
ADD CONSTRAINT [PK_ValidationDatasets]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserSystems'
ALTER TABLE [dbo].[UserSystems]
ADD CONSTRAINT [PK_UserSystems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GeneralSettings'
ALTER TABLE [dbo].[GeneralSettings]
ADD CONSTRAINT [PK_GeneralSettings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AI_Sample_log'
ALTER TABLE [dbo].[AI_Sample_log]
ADD CONSTRAINT [PK_AI_Sample_log]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Models_Id] in table 'DatasetsSet'
ALTER TABLE [dbo].[DatasetsSet]
ADD CONSTRAINT [FK_ModelsDatasets]
    FOREIGN KEY ([Models_Id])
    REFERENCES [dbo].[ModelsSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ModelsDatasets'
CREATE INDEX [IX_FK_ModelsDatasets]
ON [dbo].[DatasetsSet]
    ([Models_Id]);
GO

-- Creating foreign key on [DatasetSet_Id] in table 'Sample_ASet'
ALTER TABLE [dbo].[Sample_ASet]
ADD CONSTRAINT [FK_DatasetSetSample_ASet]
    FOREIGN KEY ([DatasetSet_Id])
    REFERENCES [dbo].[DatasetSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DatasetSetSample_ASet'
CREATE INDEX [IX_FK_DatasetSetSample_ASet]
ON [dbo].[Sample_ASet]
    ([DatasetSet_Id]);
GO

-- Creating foreign key on [AnalyzeSet_Id] in table 'BristleAnalysisResultSet'
ALTER TABLE [dbo].[BristleAnalysisResultSet]
ADD CONSTRAINT [FK_AnalyzeSetBristleAnalysisResultSet]
    FOREIGN KEY ([AnalyzeSet_Id])
    REFERENCES [dbo].[AnalyzeSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AnalyzeSetBristleAnalysisResultSet'
CREATE INDEX [IX_FK_AnalyzeSetBristleAnalysisResultSet]
ON [dbo].[BristleAnalysisResultSet]
    ([AnalyzeSet_Id]);
GO

-- Creating foreign key on [AnalyzeSet_Id] in table 'BrushAnalysisResultSet'
ALTER TABLE [dbo].[BrushAnalysisResultSet]
ADD CONSTRAINT [FK_AnalyzeSetBrushAnalysisResultSet]
    FOREIGN KEY ([AnalyzeSet_Id])
    REFERENCES [dbo].[AnalyzeSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AnalyzeSetBrushAnalysisResultSet'
CREATE INDEX [IX_FK_AnalyzeSetBrushAnalysisResultSet]
ON [dbo].[BrushAnalysisResultSet]
    ([AnalyzeSet_Id]);
GO

-- Creating foreign key on [AnalyzeSet_Id] in table 'TuffAnalysisResultSet'
ALTER TABLE [dbo].[TuffAnalysisResultSet]
ADD CONSTRAINT [FK_AnalyzeSetTuffAnalysisResultSet]
    FOREIGN KEY ([AnalyzeSet_Id])
    REFERENCES [dbo].[AnalyzeSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AnalyzeSetTuffAnalysisResultSet'
CREATE INDEX [IX_FK_AnalyzeSetTuffAnalysisResultSet]
ON [dbo].[TuffAnalysisResultSet]
    ([AnalyzeSet_Id]);
GO

-- Creating foreign key on [TuftTempSetSet_Id] in table 'BristleTempSetSet'
ALTER TABLE [dbo].[BristleTempSetSet]
ADD CONSTRAINT [FK_TuftTempSetSetBristleTempSetSet]
    FOREIGN KEY ([TuftTempSetSet_Id])
    REFERENCES [dbo].[TuftTempSetSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TuftTempSetSetBristleTempSetSet'
CREATE INDEX [IX_FK_TuftTempSetSetBristleTempSetSet]
ON [dbo].[BristleTempSetSet]
    ([TuftTempSetSet_Id]);
GO

-- Creating foreign key on [TuftTempSetSet_Id] in table 'ImageTempSetSet'
ALTER TABLE [dbo].[ImageTempSetSet]
ADD CONSTRAINT [FK_TuftTempSetSetImageTempSetSet]
    FOREIGN KEY ([TuftTempSetSet_Id])
    REFERENCES [dbo].[TuftTempSetSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TuftTempSetSetImageTempSetSet'
CREATE INDEX [IX_FK_TuftTempSetSetImageTempSetSet]
ON [dbo].[ImageTempSetSet]
    ([TuftTempSetSet_Id]);
GO

-- Creating foreign key on [RegistrationWaitingSet_Id] in table 'TuftTempSetSet'
ALTER TABLE [dbo].[TuftTempSetSet]
ADD CONSTRAINT [FK_RegistrationWaitingSetTuftTempSetSet]
    FOREIGN KEY ([RegistrationWaitingSet_Id])
    REFERENCES [dbo].[RegistrationWaitingSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RegistrationWaitingSetTuftTempSetSet'
CREATE INDEX [IX_FK_RegistrationWaitingSetTuftTempSetSet]
ON [dbo].[TuftTempSetSet]
    ([RegistrationWaitingSet_Id]);
GO

-- Creating foreign key on [TuftSet_Id] in table 'ImageSet'
ALTER TABLE [dbo].[ImageSet]
ADD CONSTRAINT [FK_TuftSetImageSet]
    FOREIGN KEY ([TuftSet_Id])
    REFERENCES [dbo].[TuftSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TuftSetImageSet'
CREATE INDEX [IX_FK_TuftSetImageSet]
ON [dbo].[ImageSet]
    ([TuftSet_Id]);
GO

-- Creating foreign key on [TuftSet_Id] in table 'BristleSet'
ALTER TABLE [dbo].[BristleSet]
ADD CONSTRAINT [FK_TuftSetBristleSet]
    FOREIGN KEY ([TuftSet_Id])
    REFERENCES [dbo].[TuftSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TuftSetBristleSet'
CREATE INDEX [IX_FK_TuftSetBristleSet]
ON [dbo].[BristleSet]
    ([TuftSet_Id]);
GO

-- Creating foreign key on [Sample_ASet_Id] in table 'TuftSet'
ALTER TABLE [dbo].[TuftSet]
ADD CONSTRAINT [FK_Sample_ASetTuftSet]
    FOREIGN KEY ([Sample_ASet_Id])
    REFERENCES [dbo].[Sample_ASet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_ASetTuftSet'
CREATE INDEX [IX_FK_Sample_ASetTuftSet]
ON [dbo].[TuftSet]
    ([Sample_ASet_Id]);
GO

-- Creating foreign key on [ValidationDataset_Id] in table 'Vsample_ASet'
ALTER TABLE [dbo].[Vsample_ASet]
ADD CONSTRAINT [FK_ValidationDatasetVsample_ASet]
    FOREIGN KEY ([ValidationDataset_Id])
    REFERENCES [dbo].[ValidationDatasetSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ValidationDatasetVsample_ASet'
CREATE INDEX [IX_FK_ValidationDatasetVsample_ASet]
ON [dbo].[Vsample_ASet]
    ([ValidationDataset_Id]);
GO

-- Creating foreign key on [Vsample_ASet_Id] in table 'VtuftSets'
ALTER TABLE [dbo].[VtuftSets]
ADD CONSTRAINT [FK_Vsample_ASetVtuftSet]
    FOREIGN KEY ([Vsample_ASet_Id])
    REFERENCES [dbo].[Vsample_ASet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Vsample_ASetVtuftSet'
CREATE INDEX [IX_FK_Vsample_ASetVtuftSet]
ON [dbo].[VtuftSets]
    ([Vsample_ASet_Id]);
GO

-- Creating foreign key on [VtuftSet_Id] in table 'VbristleSets'
ALTER TABLE [dbo].[VbristleSets]
ADD CONSTRAINT [FK_VtuftSetVbristleSet]
    FOREIGN KEY ([VtuftSet_Id])
    REFERENCES [dbo].[VtuftSets]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_VtuftSetVbristleSet'
CREATE INDEX [IX_FK_VtuftSetVbristleSet]
ON [dbo].[VbristleSets]
    ([VtuftSet_Id]);
GO

-- Creating foreign key on [VtuftSet_Id] in table 'VimageSets'
ALTER TABLE [dbo].[VimageSets]
ADD CONSTRAINT [FK_VtuftSetVimageSet]
    FOREIGN KEY ([VtuftSet_Id])
    REFERENCES [dbo].[VtuftSets]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_VtuftSetVimageSet'
CREATE INDEX [IX_FK_VtuftSetVimageSet]
ON [dbo].[VimageSets]
    ([VtuftSet_Id]);
GO

-- Creating foreign key on [Model_Id] in table 'ValidationDatasets'
ALTER TABLE [dbo].[ValidationDatasets]
ADD CONSTRAINT [FK_ModelsValidationDatasets]
    FOREIGN KEY ([Model_Id])
    REFERENCES [dbo].[ModelsSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ModelsValidationDatasets'
CREATE INDEX [IX_FK_ModelsValidationDatasets]
ON [dbo].[ValidationDatasets]
    ([Model_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------