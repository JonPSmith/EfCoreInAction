-- I only copy old data that only exists in the [CustomerAndAddresses], but I put a negative -[Id] for the [Addresses] copy
INSERT INTO [dbo].[Customers] ([Name], [CustAndAddress]) 
SELECT T1.[Name], -T1.[Id]
FROM [dbo].[CustomerAndAddresses] AS T1
WHERE (SELECT COUNT(*) FROM [dbo].[Customers] AS T2 WHERE T2.[CustAndAddress] = T1.Id) = 0
GO

-- This copies the addresses, and uses the [CustAndAddress] field in the [Customers] to get the correct data
INSERT INTO [dbo].[Addresses] ([Address], [CustFK]) 
SELECT T1.Address, 
	(SELECT [Id] FROM [dbo].[Customers] AS TInner WHERE TInner.[CustAndAddress] = -T1.Id)
FROM [dbo].[CustomerAndAddresses] AS T1
WHERE EXISTS (SELECT * FROM [dbo].[Customers] AS T2 WHERE T1.Id = -T2.[CustAndAddress])
GO