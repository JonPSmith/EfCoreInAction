CREATE FUNCTION udf_ActualPrice (@id int, @normalPrice decimal)
RETURNS decimal  AS
BEGIN
  DECLARE @result AS decimal
  SELECT @result = NewPrice FROM dbo.PriceOffers AS po
  WHERE po.Ch12BookId = @Id
  IF (@result IS NULL)
     SET @result = @normalPrice
  RETURN @result
END
GO

ALTER TABLE dbo.Books
   DROP COLUMN ActualPrice
GO

ALTER TABLE dbo.Books
   ADD ActualPrice AS (dbo.udf_ActualPrice([Ch12BookId], [Price]))
GO

CREATE FUNCTION udf_AverageVotes (@id int)
RETURNS decimal  AS
BEGIN
DECLARE @result AS decimal
SELECT @result = AVG(NumStars) FROM dbo.Ch12Review AS r
     WHERE @id = r.Ch12BookId
RETURN @result
END
GO

ALTER TABLE dbo.Books
   DROP COLUMN AverageVotes
GO

ALTER TABLE dbo.Books
   ADD AverageVotes AS (dbo.udf_AverageVotes(Ch12BookId))
GO

