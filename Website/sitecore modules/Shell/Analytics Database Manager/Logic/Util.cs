// -----------------------------------------------------------------------
// <copyright file="Util.cs" company="Sitecore A/S">
// Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the Util type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Logic
{
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Contains some useful methods for the program.
  /// </summary>
  public static class Util
  {
    /// <summary>
    /// Name of the field, which is the source for SQL queries.
    /// </summary>
    public const string SqlProviderName = "SqlServer";

    /// <summary>
    /// Gets the SQL query from item.
    /// </summary>
    /// <param name="pathToItem">The path to item.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>The SQL query.</returns>
    public static string GetSqlQueryFromItem(string pathToItem, string fieldName)
    {
      Database database = Factory.GetDatabase("master");
      Assert.IsNotNull(database, "master database");

      Item item = database.GetItem(pathToItem);
      return (item != null) ? item[fieldName] : string.Empty;
    }
  }
}
