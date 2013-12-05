// -----------------------------------------------------------------------
// <copyright file="SqlSettingConstraintsException.cs" company="Sitecore A/S">
// Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SqlSettingConstraintsException type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Logic
{
  using System;

  /// <summary>
  /// Exception for error on operation with SQL constraints.
  /// </summary>
  public class SqlSettingConstraintsException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlSettingConstraintsException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public SqlSettingConstraintsException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlSettingConstraintsException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public SqlSettingConstraintsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
