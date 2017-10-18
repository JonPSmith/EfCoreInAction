-- MySQL script file to add SQL code to improve performance
-- I have built this as an Idempotent Script, that is, it can be applied even if there isn't a change and it will ensure the database is up to date
-- NOTE: The GO is removed by the ExecuteScriptFileInTransaction

DROP FUNCTION IF EXISTS AuthorsStringUdf
GO

CREATE FUNCTION AuthorsStringUdf (bookId int) RETURNS VARCHAR(4000)
BEGIN
DECLARE Names VARCHAR(4000);
SELECT COALESCE(Names + ', ', '') + a.Name INTO Names
FROM Authors AS a, Books AS b, BookAuthor AS ba 
WHERE ba.BookId = bookId
      AND ba.AuthorId = a.AuthorId 
	  AND ba.BookId = b.BookId
ORDER BY ba.Order;
RETURN Names;
END 
GO

