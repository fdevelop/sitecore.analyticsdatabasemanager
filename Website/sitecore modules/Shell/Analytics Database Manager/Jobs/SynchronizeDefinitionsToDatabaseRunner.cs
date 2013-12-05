// -----------------------------------------------------------------------
// <copyright file="SynchronizeDefinitionsToDatabaseRunner.cs" company="Sitecore A/S">
// Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SynchronizeDefinitionsToDatabaseRunner type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Jobs
{
  using System;
  using System.Reflection;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Represents the logic of synchronize definitions task.
  /// </summary>
  public class SynchronizeDefinitionsToDatabaseRunner : JobRunnerBase
  {
    /// <summary>
    /// Path to the campaign items root.
    /// </summary>
    private const string CampaignItemsRoot = "/sitecore/system/Marketing Center/Campaigns";

    /// <summary>
    /// Path to the multivariable test items root.
    /// </summary>
    private const string MultivariableTestItemsRoot = "/sitecore/system/Marketing Center/Test Lab";
    
    /// <summary>
    /// Path to the page event items root.
    /// </summary>
    private const string PageEventItemsRoot = "/sitecore/system/Settings/Analytics/Page Events";

    /// <summary>
    /// Path to the goals item root.
    /// </summary>
    private const string GoalsItemsRoot = "/sitecore/system/Marketing Center/Goals";

    /// <summary>
    /// Path to the automations item root.
    /// </summary>
    private const string AutomationsItemsRoot = "/sitecore/system/Marketing Center/Engagement Plans";

    /// <summary>
    /// Result string for the task.
    /// </summary>
    private const string ReportResultString = "{0} success, {1} failed.";

    /// <summary>
    /// Method, which executes saveAnalyticsEntity logic.
    /// </summary>
    private readonly Func<Item, object> saveAnalyticsEntityMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizeDefinitionsToDatabaseRunner"/> class.
    /// </summary>
    public SynchronizeDefinitionsToDatabaseRunner()
    {
      this.saveAnalyticsEntityMethod = item => Sitecore.Analytics.Tracker.DeployDefinition(item);
    }

    /// <summary>
    /// Represents the processing logic on the item.
    /// </summary>
    /// <param name="item">The item.</param>
    private delegate void ProcessItem(Item item);

    /// <summary>
    /// Run the logic of the task.
    /// </summary>
    /// <param name="job">The job.</param>
    protected override void RunnerLogic(Sitecore.Jobs.Job job)
    {
      Log.Audit(Context.User, "'SynchronizeDefinitionsToDatabase' task started");

      int success, failed;
      Database masterdb = Factory.GetDatabase("master");

      this.RunMethodOnItems(masterdb.GetItem(AutomationsItemsRoot), this.SaveAnalyticsEntityExt, out success, out failed);
      this.WriteDiagnosticInfo(job, string.Format("Engagement Plans: " + ReportResultString, success, failed));

      this.RunMethodOnItems(masterdb.GetItem(CampaignItemsRoot), this.SaveAnalyticsEntityExt, out success, out failed);
      this.WriteDiagnosticInfo(job, string.Format("Campaign items: " + ReportResultString, success, failed));

      this.RunMethodOnItems(masterdb.GetItem(MultivariableTestItemsRoot), this.SaveAnalyticsEntityExt, out success, out failed);
      this.WriteDiagnosticInfo(job, string.Format("MV items: " + ReportResultString, success, failed));

      this.RunMethodOnItems(masterdb.GetItem(PageEventItemsRoot), this.SaveAnalyticsEntityExt, out success, out failed);
      this.WriteDiagnosticInfo(job, string.Format("PageEvent items: " + ReportResultString, success, failed));

      this.RunMethodOnItems(masterdb.GetItem(GoalsItemsRoot), this.SaveAnalyticsEntityExt, out success, out failed);
      this.WriteDiagnosticInfo(job, string.Format("Goal items: " + ReportResultString, success, failed));
    }

    /// <summary>
    /// Run a specified method for all sub items of the specified item.
    /// Returns count of successful and failed attempts.
    /// </summary>
    /// <param name="rootItem">The item.</param>
    /// <param name="pi">The method.</param>
    /// <param name="counterSuccess">Number of successful items.</param>
    /// <param name="counterFail">Number of failed items.</param>
    private void RunMethodOnItems(Item rootItem, ProcessItem pi, out int counterSuccess, out int counterFail)
    {
      Assert.IsNotNull(rootItem, "Root item could not be null");
      Assert.IsNotNull(pi, "Processing method was not specified");

      counterSuccess = 0;
      counterFail = 0;

      if (!rootItem.HasChildren)
      {
        return;
      }

      foreach (Item subItem in rootItem.Children)
      {
        try
        {
          pi(subItem);
          counterSuccess++;

          int subCntSuccess, subCntFail;
          this.RunMethodOnItems(subItem, pi, out subCntSuccess, out subCntFail);
          counterSuccess += subCntSuccess;
          counterFail += subCntFail;
        }
        catch (Exception e)
        {
          Log.Error("SynchronizeDefinitionsToDatabase: Error during the processing of item " + subItem.ID, e, this);
          counterFail++;
        }
      }
    }

    /// <summary>
    /// Saves definition item from Master database to Analytics database.
    /// </summary>
    /// <param name="item">The item.</param>
    private void SaveAnalyticsEntityExt(Item item)
    {
      this.saveAnalyticsEntityMethod(item);
    }
  }
}
