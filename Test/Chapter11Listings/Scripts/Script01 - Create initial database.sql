CREATE TABLE [dbo].[CustomerAndAddresses](
	[Id] int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(30) NOT NULL,
	[Address] nvarchar(30) NOT NULL,
 CONSTRAINT [PK_CustAndAdd] PRIMARY KEY CLUSTERED 
(
	Id ASC
)
) ON [PRIMARY]
GO

INSERT into [dbo].[CustomerAndAddresses](Name, Address) VALUES('John', 'His Street')
INSERT into [dbo].[CustomerAndAddresses](Name, Address) VALUES('Jane', 'Her''s Street')
GO

CREATE TABLE [dbo].[OtherTable](
	[Id] int IDENTITY(1,1) NOT NULL,
	[CustomerPK] int NOT NULL,
 CONSTRAINT [PK_OtherTable] PRIMARY KEY CLUSTERED 
(
	Id ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OtherTable]  WITH CHECK ADD CONSTRAINT [FK_dbo.Customer] FOREIGN KEY([CustomerPK])
REFERENCES [dbo].[CustomerAndAddresses] ([Id])
GO

INSERT INTO [dbo].[OtherTable]([CustomerPK]) 
SELECT TOP 1 Id
FROM [dbo].[CustomerAndAddresses]
GO
