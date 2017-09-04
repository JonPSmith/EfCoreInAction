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

CREATE FUNCTION udf_AverageVotes (@bookId int)
RETURNS float
AS
BEGIN
DECLARE @result AS float
SELECT @result = AVG(CAST([NumStars] AS float)) 
    FROM dbo.Ch12Review AS r
    WHERE @bookId = r.Ch12BookId
RETURN @result
END
GO

ALTER TABLE dbo.Books
   DROP COLUMN AverageVotes
GO

ALTER TABLE dbo.Books
   ADD AverageVotes AS (dbo.udf_AverageVotes(Ch12BookId))
GO

