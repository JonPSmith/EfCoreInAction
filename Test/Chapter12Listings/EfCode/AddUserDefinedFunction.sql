IF OBJECT_ID('dbo.udf_AverageVotes', N'FN') IS NOT NULL 
   DROP FUNCTION dbo.udf_AverageVotes
GO

CREATE FUNCTION udf_AverageVotes (@id int)
RETURNS decimal  AS
BEGIN
DECLARE @result AS decimal
SELECT @result = AVG(NumStars) FROM dbo.Ch12Review AS r
     WHERE @id = r.FixSubOptimalSqlId
RETURN @result
END
GO