CREATE TABLE [dbo].[CustomerAndAddresses](
	[Id] int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(30) NOT NULL,
	[Address] nvarchar(30) NOT NULL,
 CONSTRAINT [PK_CustomerAndAddresses] PRIMARY KEY CLUSTERED 
(
	Id ASC
)
) ON [PRIMARY]
GO

INSERT into [dbo].[CustomerAndAddresses](Name, Address) VALUES('John', 'His Street')
INSERT into [dbo].[CustomerAndAddresses](Name, Address) VALUES('Jane', 'Her''s Street')
GO
