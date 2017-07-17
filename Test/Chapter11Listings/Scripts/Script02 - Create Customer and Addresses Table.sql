ALTER TABLE [dbo].[CustomerAndAddresses]
ALTER COLUMN [Address] nvarchar(30) NULL
GO

CREATE VIEW [dbo].[Customers]
AS SELECT [Id], [Name] FROM [dbo].[CustomerAndAddresses]
GO

CREATE TABLE [dbo].[Addresses](
	[Id] int IDENTITY(1,1) NOT NULL,
	[Address] nvarchar(30) NOT NULL,
	[CustFK] int NOT NULL
 CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED 
(
	Id ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD CONSTRAINT [FK_dbo.Addresses.CustFK] FOREIGN KEY([CustFK])
REFERENCES [dbo].CustomerAndAddresses ([Id])
GO
