// -----------------------------------------------------------------------
// <copyright file="AnalyticsDatabaseHelper.cs" company="Sitecore A/S">
// Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the AnalyticsDatabaseHelper type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Logic
{
  using System.Configuration;
  using System.Data.SqlClient;
  using System.Linq;

  using Sitecore.Analytics;
  using Sitecore.Configuration;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Static class, which contains trivial Analytics database operations.
  /// </summary>
  public static class AnalyticsDatabaseHelper
  {
    /// <summary>
    /// Path to the item with query for Drop Constraints.
    /// </summary>
    private const string DropConstraintsQueryItem = "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/System/Drop Constraints";

    /// <summary>
    /// Path to the item with query for Create Constraints Cascade Delete.
    /// </summary>
    private const string CreateConstraintsCascadeDeleteQueryItem = "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/System/Add Constraints Cascade Delete";

    /// <summary>
    /// Path to the item with query for Create Constraints.
    /// </summary>
    private const string CreateConstraintsQueryItem = "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/System/Add Constraints";

    /// <summary>
    /// Path to the item with query for Removing Unused Global Sessions.
    /// </summary>
    private const string RemoveUnusedGlobalSessionsQueryItem = "/sitecore/system/Modules/Analytics Database Manager/SQL Commands/Remove Unused Visitors";

    /// <summary>
    /// Makes constraints in Analytics database to cascade delete.
    /// </summary>
    /// <exception cref="SqlSettingConstraintsException">Thrown when there is an error during the setting constraints.</exception>
    public static void MakeConstraintsCascade()
    {
      string dropConstraintsQuery = Util.GetSqlQueryFromItem(DropConstraintsQueryItem, Util.SqlProviderName);
      string createConstraintsDeleteCascadeQuery = Util.GetSqlQueryFromItem(CreateConstraintsCascadeDeleteQueryItem, Util.SqlProviderName);

      if (!ExecuteMultiNonQuery(dropConstraintsQuery))
      {
        Log.Warn("Failed to correctly drop constraints", typeof(AnalyticsDatabaseHelper));
      }

      if (!ExecuteMultiNonQuery(createConstraintsDeleteCascadeQuery))
      {
        throw new SqlSettingConstraintsException("Problem with adding/dropping constraints");
      }
    }

    /// <summary>
    /// Makes constraints in Analytics database without any actions.
    /// </summary>
    /// <exception cref="SqlSettingConstraintsException">Thrown when there is an error during the setting constraints.</exception>
    public static void MakeConstraintsStandard()
    {
      string dropConstraintsQuery = Util.GetSqlQueryFromItem(DropConstraintsQueryItem, Util.SqlProviderName);
      string createConstraintsQuery = Util.GetSqlQueryFromItem(CreateConstraintsQueryItem, Util.SqlProviderName);

      if (!ExecuteMultiNonQuery(dropConstraintsQuery))
      {
        Log.Warn("Failed to correctly drop constraints", typeof(AnalyticsDatabaseHelper));
      }

      if (!ExecuteMultiNonQuery(createConstraintsQuery))
      {
        throw new SqlSettingConstraintsException("Problem with adding/dropping constraints");
      }
    }

    /// <summary>
    /// Removes unused global sessions (global sessions without appropriate sessions in Session table).
    /// </summary>
    public static void RemoveUnusedGlobalSessions()
    {
      string getSqlQuery = Util.GetSqlQueryFromItem(RemoveUnusedGlobalSessionsQueryItem, Util.SqlProviderName);
      ExecuteNonQuery(getSqlQuery);
    }

    /// <summary>
    /// Gets the analytics connection string.
    /// </summary>
    /// <returns>Connection string for analytics database.</returns>
    internal static string GetAnalyticsConnectionString()
    {
      var databaseName = Settings.GetSetting("AnalyticsDatabaseManager.Database", "analytics");
      return ConfigurationManager.ConnectionStrings["analytics"].ConnectionString;
    }

    /// <summary>
    /// Gets the name of the analytics database.
    /// </summary>
    /// <returns>Database name.</returns>
    internal static string GetAnalyticsDatabaseName()
    {
      SqlConnectionStringBuilder sqlConnectionString = new SqlConnectionStringBuilder(GetAnalyticsConnectionString());
      return sqlConnectionString.InitialCatalog;
    }

    /// <summary>
    /// Executes the non query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>Number of affected entries.</returns>
    internal static int ExecuteNonQuery(string query)
    {
      using (SqlConnection sql = new SqlConnection(GetAnalyticsConnectionString()))
      {
        sql.Open();
        SqlCommand cmd = new SqlCommand(query, sql)
        {
          CommandTimeout = Settings.GetIntSetting("AnalyticsDatabaseManager.SqlTimeout", 1200)
        };

        return cmd.ExecuteNonQuery();
      }
    }

    /// <summary>
    /// Executes the non query, each single query separately.
    /// </summary>
    /// <param name="query">Long SQL query.</param>
    /// <returns>True, if the task performed successfully.</returns>
    internal static bool ExecuteMultiNonQuery(string query)
    {
      bool flagFailed = false;
      var queries = query.Split(';').Where(s => !string.IsNullOrEmpty(s));
      foreach (string s in queries)
      {
        try
        {
          ExecuteNonQuery(s);
        }
        catch (SqlException se)
        {
          Log.Error("Query could not be proceeded: " + s, se, new object());
          flagFailed = true;
        }
      }

      return !flagFailed;
    }
  }
}
