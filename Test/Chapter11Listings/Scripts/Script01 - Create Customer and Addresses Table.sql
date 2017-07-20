CREATE TABLE [dbo].[Customers](
	[Id] int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(30) NOT NULL,
	[CustAndAddress] int NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	Id ASC
)
) ON [PRIMARY]
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

CREATE UNIQUE INDEX [IX_Addresses_CustFK] 
   ON [Addresses] ([CustFK]);
GO


ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD CONSTRAINT [FK_dbo.Addresses.CustFK] FOREIGN KEY([CustFK])
REFERENCES [dbo].Customers ([Id]) ON DELETE CASCADE
GO

-- This is the interim update of the data, which updates both the old and new tables
-- If you need to update or delete the CustomerAndAddresses table then you need to write other stored procs
CREATE PROCEDURE InterimCustomerAndAddressUpdate   
    @Name nvarchar(30),   
    @Address nvarchar(30)   
AS   
    SET NOCOUNT ON; 
	DECLARE @CustAndAddrId int 
    DECLARE @CustId int 
	INSERT [dbo].[CustomerAndAddresses](Name, Address) VALUES(@Name, @Address)
	SELECT @CustAndAddrId = Scope_Identity()
	INSERT [dbo].[Customers](Name, CustAndAddress) VALUES(@Name, @CustAndAddrId)
	SELECT @CustId = Scope_Identity()
  	INSERT [dbo].[Addresses](Address, CustFK) VALUES(@Address, @CustId) 
GO 

-- This is a test that the stored proc updates all three tables
EXEC InterimCustomerAndAddressUpdate N'Mid-migrate name', N'Mid-migrate address'
