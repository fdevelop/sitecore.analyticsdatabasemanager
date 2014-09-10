/* Remove visits
 * Script for Sitecore DMS 2.0 / 6.6 - 7.2.
 * 
 * Usage: go to PLACEHOLDER and replace condition with your own.
 * Default condition: remove visits finished before the {date}.
 * Note: it is always possible to execute script on limited scope of the records (e.g. 1000).
 *       To achieve this, replace first occurence of 'insert into ... select ...' statement with 'insert into ... select top 1000 ...' below.
 *
 * Script x1: Processes analytics visits one-by-one with detailed report for every visit.
 */

-- fill the temporary table with required IDs
IF OBJECT_ID('tempdb..#visitorsMatched') IS NOT NULL
BEGIN
    drop table #visitorsMatched
END

create table #visitorsMatched (id uniqueidentifier, cmd int);

-- create pocedure for processing visitors
IF OBJECT_ID ( 'admProcessVisitor', 'P' ) IS NOT NULL 
    DROP PROCEDURE admProcessVisitor;
GO

CREATE PROCEDURE admProcessVisitor @id uniqueidentifier
AS
  BEGIN TRAN CleanOlderThen;

	DELETE FROM Profiles WHERE VisitorId = @id
	DELETE FROM AutomationStates WHERE VisitorId = @id
	DELETE FROM VisitorTags WHERE VisitorId = @id
	DELETE FROM PageEvents WHERE VisitorId = @id
	DELETE FROM Pages WHERE VisitorId = @id
	DELETE FROM Visits WHERE VisitorId = @id
	DELETE FROM Visitors WHERE VisitorId = @id
	UPDATE #visitorsMatched SET cmd = 1 WHERE id = @id
	
	COMMIT TRAN CleanOlderThen;
GO

insert into #visitorsMatched
select VisitorId, 0 FROM Visitors 
/* PLACEHOLDER! */
WHERE
	(select top 1 EndDateTime from Visits where Visits.VisitorId = Visitors.VisitorId order by EndDateTime desc) < CAST('2012-05-01 00:00' AS datetime);

-- go through...
DECLARE @lid uniqueidentifier
DECLARE visitorsCursor CURSOR FOR  
SELECT id
FROM #visitorsMatched

OPEN visitorsCursor
FETCH NEXT FROM visitorsCursor INTO @lid

WHILE @@FETCH_STATUS = 0   
BEGIN   
  BEGIN TRY
    EXEC admProcessVisitor @id = @lid
  END TRY
  BEGIN CATCH
    UPDATE #visitorsMatched SET cmd = 2 WHERE id = @lid
  END CATCH
  PRINT CAST(@lid as varchar(50)) + ' processed'
  FETCH NEXT FROM visitorsCursor INTO @lid
END

CLOSE visitorsCursor   
DEALLOCATE visitorsCursor

select count(*) as 'Success' from #visitorsMatched WHERE cmd = 1
select count(*) as 'Failed' from #visitorsMatched WHERE cmd > 1

-- finish
drop table #visitorsMatched