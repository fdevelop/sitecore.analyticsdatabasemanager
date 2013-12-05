// -----------------------------------------------------------------------
// <copyright file="CleanAllRunner.cs" company="Sitecore A/S">
//  Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CleanAllRunner type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Jobs
{
  using Sitecore.AnalyticsDatabaseManager.Logic;
  using Sitecore.Diagnostics;
  using Sitecore.Jobs;

  /// <summary>
  /// Represents the logic of the CleanAll job.
  /// </summary>
  public class CleanAllRunner : JobRunnerBase
  {
    /// <summary>
    /// Flag for Clean IPs setting.
    /// </summary>
    private readonly bool cleanIps;

    /// <summary>
    /// Initializes a new instance of the CleanAllRunner class.
    /// </summary>
    /// <param name="cleanIps">Determines, if the IP cleaning should be performed during the task processing.</param>
    public CleanAllRunner(bool cleanIps)
    {
      this.cleanIps = cleanIps;
    }

    /// <summary>
    /// Run the logic of CleanAll task.
    /// </summary>
    /// <param name="job">The job.</param>
    protected override void RunnerLogic(Job job)
    {
      Assert.ArgumentNotNull(job, "job");
      int ipOwnersRemoved;
      job.Status.Processed = AnalyticsDatabaseService.CleanAllCollectedData(this.cleanIps, out ipOwnersRemoved);
      this.WriteDiagnosticInfo(job, string.Format("Entries removed: {0}\r\nLocations removed: {1}\r\nThe data from helper tables have been removed as well.", job.Status.Processed, ipOwnersRemoved));
    }
  }
}
