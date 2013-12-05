namespace Sitecore.AnalyticsDatabaseManager.Jobs
{
  using System;

  using Sitecore.AnalyticsDatabaseManager.Logic;
  using Sitecore.Diagnostics;
  using Sitecore.Jobs;

  /// <summary>
  /// Represents the logic of CleanFilteredCustomRuleRunner task.
  /// </summary>
  public class CleanFilteredCustomRuleRunner : JobRunnerBase
  {
    /// <summary>
    /// Task parameter. Specifies column of database table to consider.
    /// </summary>
    private readonly string tableColumn;

    /// <summary>
    /// Task parameter. Specifies the value of the column in database table to consider.
    /// </summary>
    private readonly string tableColumnValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="CleanFilteredCustomRuleRunner"/> class.
    /// </summary>
    /// <param name="tableColumn">The table column.</param>
    /// <param name="tableColumnValue">The table column value.</param>
    public CleanFilteredCustomRuleRunner(string tableColumn, string tableColumnValue)
    {
      this.tableColumn = tableColumn;
      this.tableColumnValue = tableColumnValue;
    }

    /// <summary>
    /// Runs the logic.
    /// </summary>
    /// <param name="job">The job.</param>
    protected override void RunnerLogic(Job job)
    {
      Assert.ArgumentNotNull(job, "job");
      job.Status.Processed = AnalyticsDatabaseService.CleanCustomRule(this.tableColumn, this.tableColumnValue);
      this.WriteDiagnosticInfo(job, string.Format("Visits removed: {0}", job.Status.Processed));
    }
  }
}
