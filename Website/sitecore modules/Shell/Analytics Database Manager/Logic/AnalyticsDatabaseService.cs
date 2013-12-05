// -----------------------------------------------------------------------
// <copyright file="AnalyticsDatabaseService.cs" company="Sitecore A/S">
// Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the AnalyticsDatabaseService type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Logic
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.IO;

  using Sitecore.Diagnostics;

  /// <summary>
  /// Contains the helpful methods to clean and optimize with Analytics database.
  /// </summary>
  public static class AnalyticsDatabaseService
  {
    /// <summary>
    /// Path to the item with query for Clean IP data.
    /// </summary>
    private const string CleanCollectedIpsQueryItem =
      "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/Clean GeoIP Lookup Data";

    /// <summary>
    /// Path to the item with query for Clean Helper Data.
    /// </summary>
    private const string CleanCollectedHelperDataQueryItem =
      "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/Clean Helper Data";

    /// <summary>
    /// Path to the item with query for Clean All.
    /// </summary>
    private const string CleanAllCollectedDataQueryItem =
      "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/Clean All";

    /// <summary>
    /// Path to the item with query for Clean Filtered Older Than.
    /// </summary>
    private const string CleanCollectedDataOlderThenQueryItem =
      "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/Clean Filtered Older Than";

    /// <summary>
    /// Path to the item with query for Clean Filtered Bounce Visits.
    /// </summary>
    private const string CleanBounceVisitsQueryItem =
      "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/Clean Filtered Bounce Visits";

    /// <summary>
    /// Path to the item with query for Clean Filtered Custom Rule
    /// </summary>
    private const string CleanCustomRuleQueryItem =
      "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/Clean Filtered Custom Rule";

    /// <summary>
    /// Path to the item with query for Rebuild Index
    /// </summary>
    private const string RebuildIndexQueryItem =
      "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/Rebuild Index";

    /// <summary>
    /// Path to the item with query for Rebuild Index Online
    /// </summary>
    private const string RebuildIndexOnlineQueryItem =
      "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/Rebuild Index Online";

    /// <summary>
    /// Path to the item with query for Remove Bots
    /// </summary>
    private const string RemoveBotVisitsQueryItem =
      "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/Remove Bots";

    /// <summary>
    /// Path to the item with query for Backup Database
    /// </summary>
    private const string BackupDatabaseQueryItem =
      "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/Backup Database";

    /// <summary>
    /// Clean all collected analytics data.
    /// </summary>
    /// <param name="cleanIps">Specifies, if the IP data should be deleted as well.</param>
    /// <param name="ipOwnersRemoved">Returns the number of removed IP Owners. If the cleanIps is false, returns 0.</param>
    /// <returns>Number of affected entries.</returns>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
      Justification = "Reviewed. Suppression is OK here.")]
    public static int CleanAllCollectedData(bool cleanIps, out int ipOwnersRemoved)
    {
      Log.Audit(Context.User, "Clean all task started with cleanIps={0}", cleanIps.ToString());
      string getSqlQuery = Util.GetSqlQueryFromItem(CleanAllCollectedDataQueryItem, Util.SqlProviderName);
      string getSqlQueryHelperPart = Util.GetSqlQueryFromItem(CleanCollectedHelperDataQueryItem, Util.SqlProviderName);
      string getSqlQueryIpsPart = Util.GetSqlQueryFromItem(CleanCollectedIpsQueryItem, Util.SqlProviderName);

      try
      {
        AnalyticsDatabaseHelper.MakeConstraintsCascade();
        int processed = AnalyticsDatabaseHelper.ExecuteNonQuery(getSqlQuery);
        ipOwnersRemoved = cleanIps ? AnalyticsDatabaseHelper.ExecuteNonQuery(getSqlQueryIpsPart) : 0;
        AnalyticsDatabaseHelper.ExecuteNonQuery(getSqlQueryHelperPart);

        return processed;
      }
      finally
      {
        AnalyticsDatabaseHelper.MakeConstraintsStandard();
      }
    }

    /// <summary>
    /// Cleans the collected data older then.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>Number of affected entries.</returns>
    public static int CleanCollectedDataOlderThen(DateTime dateTime)
    {
      Log.Audit(Context.User, "'Clean data older then' task started with parameter dateTime={0}", dateTime.ToString());
      string getSqlQuery = Util.GetSqlQueryFromItem(CleanCollectedDataOlderThenQueryItem, Util.SqlProviderName);
      string preparedQuery = string.Format(getSqlQuery, dateTime.ToString("yyyy-MM-dd HH:mm:ss"));

      return ApplySimpleCleanFilter(preparedQuery);
    }

    /// <summary>
    /// Cleans the bounce sessions.
    /// </summary>
    /// <returns>Number of affected entries.</returns>
    public static int CleanBounceSessions()
    {
      Log.Audit(Context.User, "'Clean bounce visitors' task started");
      string getSqlQuery = Util.GetSqlQueryFromItem(CleanBounceVisitsQueryItem, Util.SqlProviderName);
      string preparedQuery = getSqlQuery;

      return ApplySimpleCleanFilter(preparedQuery);
    }
    
    /// <summary>
    /// Cleans by the custom rule.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static int CleanCustomRule(string column, string value)
    {
      Log.Audit(Context.User, "'Clean by custom rule' task started with parameters column={0},value={1}", column, value);
      string getSqlQuery = Util.GetSqlQueryFromItem(CleanCustomRuleQueryItem, Util.SqlProviderName);

      string formattedValue;
      string valueType;
      ParseSqlType(value, out formattedValue, out valueType);

      string preparedQuery = string.Format(getSqlQuery, column, formattedValue, valueType);

      return ApplySimpleCleanFilter(preparedQuery);
    }

    /// <summary>
    /// Rebuilds the index.
    /// </summary>
    /// <param name="onlineMode">if set to <c>true</c> the task will be running in Online mode.</param>
    /// <returns>Number of affected entries (zero in this case).</returns>
    public static int RebuildIndex(bool onlineMode)
    {
      Log.Audit(Context.User, "'Rebuild index' task started with parameter onlineMode={0}", onlineMode.ToString());
      string getSqlQuery = Util.GetSqlQueryFromItem(
        onlineMode ? RebuildIndexOnlineQueryItem : RebuildIndexQueryItem, Util.SqlProviderName);

      return AnalyticsDatabaseHelper.ExecuteNonQuery(getSqlQuery);
    }

    /// <summary>
    /// Removes the BOTs sessions.
    /// </summary>
    /// <returns>Number of affected entries.</returns>
    public static int RemoveBotSessions()
    {
      Log.Audit(Context.User, "'RemoveBotSessions' task started");

      try
      {
        AnalyticsDatabaseHelper.MakeConstraintsCascade();
        string getSqlQuery = Util.GetSqlQueryFromItem(RemoveBotVisitsQueryItem, Util.SqlProviderName);
        int processed = AnalyticsDatabaseHelper.ExecuteNonQuery(getSqlQuery);
        return processed;
      }
      finally
      {
        AnalyticsDatabaseHelper.MakeConstraintsStandard();
      }
    }

    /// <summary>
    /// Backups current analytics database.
    /// </summary>
    /// <param name="pathToBackupFile">Path to the file, which will be created as a backup (directory must exist).</param>
    /// <returns>The full name of the created backup file.</returns>
    public static string BackupDatabase(string pathToBackupFile)
    {
      Assert.IsNotNull(Path.GetDirectoryName(pathToBackupFile), "Path could not be null");
      Assert.IsTrue(
        Directory.Exists(Path.GetDirectoryName(pathToBackupFile) ?? string.Empty),
        "Directory must exist: " + pathToBackupFile);
      Log.Audit(Context.User, "'BackupDatabase' task started with parameter pathToBackupFile={0}", pathToBackupFile);

      string getSqlQuery = Util.GetSqlQueryFromItem(BackupDatabaseQueryItem, Util.SqlProviderName);
      string preparedQuery = string.Format(
        getSqlQuery, AnalyticsDatabaseHelper.GetAnalyticsDatabaseName(), pathToBackupFile);

      AnalyticsDatabaseHelper.ExecuteNonQuery(preparedQuery);
      return pathToBackupFile;
    }

    /// <summary>
    /// Trivial logic for cleanup tasks with filter.
    /// </summary>
    /// <param name="query">Prepared query</param>
    /// <returns>Number of affected entries.</returns>
    private static int ApplySimpleCleanFilter(string query)
    {
      try
      {
        AnalyticsDatabaseHelper.MakeConstraintsCascade();
        int processed = AnalyticsDatabaseHelper.ExecuteNonQuery(query);
        AnalyticsDatabaseHelper.RemoveUnusedGlobalSessions();
        return processed;
      }
      finally
      {
        AnalyticsDatabaseHelper.MakeConstraintsStandard();
      }
    }

    /// <summary>
    /// Verifies that passed string could be converted to GUID.
    /// </summary>
    /// <param name="p">The input string.</param>
    /// <returns>True, if string could be parsed as GUID.</returns>
    private static bool IsGuid(string p)
    {
      try
      {
        Guid g = new Guid(p);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Parses the type of the SQL.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="formattedValue">The formatted value.</param>
    /// <param name="valueType">Type of the value.</param>
    private static void ParseSqlType(string value, out string formattedValue, out string valueType)
    {
      int formattedNumberValue;

      if (int.TryParse(value, out formattedNumberValue))
      {
        formattedValue = value;
        valueType = "int";
      }
      else if (IsGuid(value))
      {
        formattedValue = string.Format("'{0}'", value);
        valueType = "uniqueidentifier";
      }
      else
      {
        formattedValue = string.Format("'{0}'", value);
        valueType = "sql_variant";
      }
    }
  }
}