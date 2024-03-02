https://learn.microsoft.com/en-us/sql/relational-databases/in-memory-oltp/transactions-with-memory-optimized-tables?view=sql-server-ver16

;WITH
  L0   AS (SELECT c FROM (SELECT 1 UNION ALL SELECT 1) AS D(c)), -- 2^1
  L1   AS (SELECT 1 AS c FROM L0 AS A CROSS JOIN L0 AS B),       -- 2^2
  L2   AS (SELECT 1 AS c FROM L1 AS A CROSS JOIN L1 AS B),       -- 2^4
  L3   AS (SELECT 1 AS c FROM L2 AS A CROSS JOIN L2 AS B),       -- 2^8
  L4   AS (SELECT 1 AS c FROM L3 AS A CROSS JOIN L3 AS B),       -- 2^16
  L5   AS (SELECT 1 AS c FROM L4 AS A CROSS JOIN L4 AS B),       -- 2^32
  Nums AS (SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS k FROM L5)
INSERT INTO dbo.test ([Name],[Data])
SELECT 'Name_' + cast (k AS varchar) AS a, 'Data_' + cast (k AS varchar)
FROM nums
WHERE k <= 10000


CREATE TABLE [dbo].[test](
	[Id] [bigint] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Data] [nvarchar](max) NULL,
	[Version] [int] NOT NULL DEFAULT (0),
 CONSTRAINT [PK_test] PRIMARY KEY NONCLUSTERED  HASH
	(
		[Id]
	) WITH (BUCKET_COUNT=1000000)
) WITH (MEMORY_OPTIMIZED=ON, DURABILITY = SCHEMA_ONLY);
GO

	
--ALTER DATABASE test_load_in_memory ADD FILE (
--    name='mem_file', filename='C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\test_load_in_memory_mem')
--    TO FILEGROUP IN_MEMORY;

ALTER DATABASE CURRENT SET MEMORY_OPTIMIZED_ELEVATE_TO_SNAPSHOT = ON

--CREATE TABLE [dbo].TblInMem
 
--(
 
--       [id] [int] NOT NULL,
 
--       [val1] [char](20) NULL,
 
--       [val2] [char](20) NOT NULL,
 
--        PRIMARY KEY NONCLUSTERED HASH
 
--(
 
--       [id],
 
--       [val2]
 
--)WITH (BUCKET_COUNT=1000000)
 
--)
 
--WITH (MEMORY_OPTIMIZED=ON, DURABILITY = SCHEMA_AND_DATA);