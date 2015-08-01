USE [master];
GO

CREATE DATABASE [Nature];
GO

USE [Nature];
GO

CREATE TABLE [dbo].[Acorn]
(
	[Id] INT CONSTRAINT PK_Acorn PRIMARY KEY IDENTITY(1, 1)  NOT NULL,
	[NutritionValue] INT NOT NULL
);
GO

INSERT INTO [dbo].[Acorn]
(
	[NutritionValue]
)
VALUES
(5),
(5),
(6),
(6),
(7);
GO
