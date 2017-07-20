ALTER TABLE [dbo].[Customers]
  DROP COLUMN [CustAndAddress]
GO

DROP TABLE [dbo].[CustomerAndAddresses]
GO

DROP PROCEDURE InterimCustomerAndAddressUpdate
GO