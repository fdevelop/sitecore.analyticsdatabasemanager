// -----------------------------------------------------------------------
// <copyright file="RebuildIndexRunner.cs" company="Sitecore A/S">
// Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the RebuildIndexRunner type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Jobs
{
  using Sitecore.AnalyticsDatabaseManager.Logic;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Represents the Rebuild Indexes task.
  /// </summary>
  public class RebuildIndexRunner : JobRunnerBase
  {
    /// <summary>
    /// Task parameter, which specifies if the task must be executed in Online mode.
    /// </summary>
    private readonly bool onlineMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="RebuildIndexRunner"/> class.
    /// </summary>
    /// <param name="onlineMode">if set to <c>true</c> the rebuild index task will be executed with ONLINE = ON parameter (available only for enterprise SQL Server).</param>
    public RebuildIndexRunner(bool onlineMode)
    {
      this.onlineMode = onlineMode;
    }

    /// <summary>
    /// Runs the logic.
    /// </summary>
    /// <param name="job">The job.</param>
    protected override void RunnerLogic(Sitecore.Jobs.Job job)
    {
      Assert.ArgumentNotNull(job, "job");
      job.Status.Processed = AnalyticsDatabaseService.RebuildIndex(this.onlineMode);
      this.WriteDiagnosticInfo(job, "Rebuild index is completed.");
    }
  }
}
