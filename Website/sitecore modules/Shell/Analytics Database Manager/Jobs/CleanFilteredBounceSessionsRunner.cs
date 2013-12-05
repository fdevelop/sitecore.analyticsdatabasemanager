// -----------------------------------------------------------------------
// <copyright file="CleanFilteredBounceSessionsRunner.cs" company="Sitecore A/S">
// Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CleanFilteredBounceSessions type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Jobs
{
  using Sitecore.AnalyticsDatabaseManager.Logic;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Represents the logic of CleanBounceSessionsRunner task.
  /// </summary>
  public class CleanFilteredBounceSessionsRunner : JobRunnerBase
  {
    /// <summary>
    /// Run the logic.
    /// </summary>
    /// <param name="job">The job.</param>
    protected override void RunnerLogic(Sitecore.Jobs.Job job)
    {
      Assert.ArgumentNotNull(job, "job");
      job.Status.Processed = AnalyticsDatabaseService.CleanBounceSessions();
      this.WriteDiagnosticInfo(job, string.Format("Visits removed: {0}", job.Status.Processed));
    }
  }
}
