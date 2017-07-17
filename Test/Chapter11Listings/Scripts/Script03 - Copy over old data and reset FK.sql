INSERT INTO [dbo].[Addresses] ([Address], [CustFK]) 
SELECT T1.Address, T1.Id
FROM [dbo].[CustomerAndAddresses] AS T1
WHERE (SELECT COUNT(*) FROM [dbo].[Addresses] AS T2 WHERE T2.CustFK = T1.Id) = 0
GO

ALTER TABLE [dbo].[CustomerAndAddresses]
  DROP COLUMN [Address]
GO

DROP VIEW [dbo].[Customers]
GO

ALTER TABLE [dbo].[Addresses] DROP CONSTRAINT [FK_dbo.Addresses.CustFK]
GO

sp_rename 'CustomerAndAddresses', 'Customers'
GO

ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD CONSTRAINT [FK_dbo.Addresses.CustFK] FOREIGN KEY([CustFK])
REFERENCES [dbo].[Customers] ([Id])
GO