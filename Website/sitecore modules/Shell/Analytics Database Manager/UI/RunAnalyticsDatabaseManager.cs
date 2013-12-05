// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunAnalyticsDatabaseManager.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the runner command for Analytics Database Manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.UI
{
  using Sitecore.Analytics.Configuration;
  using Sitecore.Diagnostics;
  using Sitecore.Shell.Framework.Commands;
  using Sitecore.Text;
  using Sitecore.Web.UI.Sheer;

  /// <summary>
  ///   Defines the runner command for Analytics Database Manager
  /// </summary>
  public class RunAnalyticsDatabaseManager : Command
  {
    /// <summary>
    ///   Executes the command in the specified context
    /// </summary>
    /// <param name="context">The context</param>
    public override void Execute(CommandContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      SheerResponse.ShowModalDialog(new UrlString(UIUtil.GetUri("control:Sitecore.AnalyticsDatabaseManager")).ToString());
    }

    /// <summary>
    ///   Queries the state of the command
    /// </summary>
    /// <param name="context">The context</param>
    /// <returns>The command state</returns>
    public override CommandState QueryState(CommandContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      return (!AnalyticsSettings.Enabled) ? CommandState.Hidden : base.QueryState(context);
    }
  }
}
