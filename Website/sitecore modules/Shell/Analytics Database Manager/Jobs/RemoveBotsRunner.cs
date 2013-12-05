// -----------------------------------------------------------------------
// <copyright file="RemoveBotsRunner.cs" company="Sitecore A/S">
// Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the RemoveBotsRunner type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Jobs
{
  using Sitecore.AnalyticsDatabaseManager.Logic;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Represents the RemoveBotSessions task
  /// </summary>
  public class RemoveBotsRunner : JobRunnerBase
  {
    /// <summary>
    /// Runs the logic.
    /// </summary>
    /// <param name="job">The job.</param>
    protected override void RunnerLogic(Sitecore.Jobs.Job job)
    {
      Assert.ArgumentNotNull(job, "job");
      job.Status.Processed = AnalyticsDatabaseService.RemoveBotSessions();
      this.WriteDiagnosticInfo(job, string.Format("Visitors removed: {0}", job.Status.Processed));
    }
  }
}
