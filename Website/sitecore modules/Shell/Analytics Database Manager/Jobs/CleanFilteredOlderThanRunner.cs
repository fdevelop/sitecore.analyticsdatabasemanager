// -----------------------------------------------------------------------
// <copyright file="CleanFilteredOlderThanRunner.cs" company="Sitecore A/S">
// Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CleanFilteredOlderThanRunner type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Jobs
{
  using System;
  using Sitecore.AnalyticsDatabaseManager.Logic;
  using Sitecore.Diagnostics;
  using Sitecore.Jobs;

  /// <summary>
  /// Represents the logic of the CleanFiltered job.
  /// </summary>
  public class CleanFilteredOlderThanRunner : JobRunnerBase
  {
    /// <summary>
    /// Task parameter. All sessions older than this will be deleted.
    /// </summary>
    private readonly DateTime dateTime;

    /// <summary>
    /// Initializes a new instance of the CleanFilteredOlderThanRunner class. (for removing data 'older then')
    /// </summary>
    /// <param name="dateTime">Task parameter. All sessions older than this will be deleted.</param>
    public CleanFilteredOlderThanRunner(DateTime dateTime)
    {
      this.dateTime = dateTime;
    }

    /// <summary>
    /// Run the logic of CleanFiltered task.
    /// </summary>
    /// <param name="job">The job.</param>
    protected override void RunnerLogic(Job job)
    {
      Assert.ArgumentNotNull(job, "job");
      job.Status.Processed = AnalyticsDatabaseService.CleanCollectedDataOlderThen(this.dateTime);
      this.WriteDiagnosticInfo(job, string.Format("Visits removed: {0}", job.Status.Processed));
    }
  }
}
