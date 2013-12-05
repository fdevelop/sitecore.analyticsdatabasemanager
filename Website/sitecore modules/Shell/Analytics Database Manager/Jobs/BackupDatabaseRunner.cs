// -----------------------------------------------------------------------
// <copyright file="BackupDatabaseRunner.cs" company="Sitecore A/S">
// Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the BackupDatabaseRunner type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Jobs
{
  using Sitecore.AnalyticsDatabaseManager.Logic;
  using Sitecore.Diagnostics;
  using Sitecore.Jobs;

  /// <summary>
  /// Represents the BackDatabase task.
  /// </summary>
  public class BackupDatabaseRunner : JobRunnerBase
  {
    /// <summary>
    /// Task parameter, which contains the path to the folder, where backup will be created.
    /// </summary>
    private readonly string pathTo;

    /// <summary>
    /// Initializes a new instance of the <see cref="BackupDatabaseRunner"/> class.
    /// </summary>
    /// <param name="pathTo">The path to backup.</param>
    public BackupDatabaseRunner(string pathTo)
    {
      this.pathTo = pathTo;
    }

    /// <summary>
    /// Run the logic.
    /// </summary>
    /// <param name="job">The job.</param>
    protected override void RunnerLogic(Job job)
    {
      Assert.ArgumentNotNull(job, "job");
      string backupResult = AnalyticsDatabaseService.BackupDatabase(this.pathTo);
      this.WriteDiagnosticInfo(job, "New backup was created at " + backupResult);
    }
  }
}
