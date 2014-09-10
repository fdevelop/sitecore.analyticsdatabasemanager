/* Remove visits
 * Script for Sitecore DMS 2.0 / 6.6 - 7.2.
 * 
 * Usage: go to PLACEHOLDER and replace condition with your own.
 * Default condition: remove visits finished before the {date}.
 * Note: it is always possible to execute script on limited scope of the records (e.g. 1000).
 *       To achieve this, replace 'select' statement with 'select top 1000' below.
 *
 * Script x2: Processes analytics data faster, but with less control on results.
 */

-- fill the temporary table with required IDs
IF OBJECT_ID('tempdb..#visitorsMatched') IS NOT NULL
BEGIN
    drop table #visitorsMatched
END

create table #visitorsMatched (id uniqueidentifier, cmd int);

insert into #visitorsMatched
select VisitorId, 0 FROM Visitors 
/* PLACEHOLDER! */
WHERE
	(select top 1 EndDateTime from Visits where Visits.VisitorId = Visitors.VisitorId order by EndDateTime desc) < CAST('2012-05-01 00:00' AS datetime);

-- go through...
BEGIN TRAN CleanOlderThen;

DELETE FROM Profiles WHERE VisitorId in (select id from #visitorsMatched)
DELETE FROM AutomationStates WHERE VisitorId in (select id from #visitorsMatched)
DELETE FROM VisitorTags WHERE VisitorId in (select id from #visitorsMatched)
DELETE FROM PageEvents WHERE VisitorId in (select id from #visitorsMatched)
DELETE FROM Pages WHERE VisitorId in (select id from #visitorsMatched)
DELETE FROM Visits WHERE VisitorId in (select id from #visitorsMatched)
DELETE FROM Visitors WHERE VisitorId in (select id from #visitorsMatched)

COMMIT TRAN CleanOlderThen;

-- finish
drop table #visitorsMatched