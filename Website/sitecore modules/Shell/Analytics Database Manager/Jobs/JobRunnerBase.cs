// -----------------------------------------------------------------------
// <copyright file="JobRunnerBase.cs" company="Sitecore A/S">
// Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the JobRunnerBase type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.Jobs
{
  using System;
  using System.Data.SqlClient;

  using Sitecore.AnalyticsDatabaseManager.Logic;
  using Sitecore.Diagnostics;
  using Sitecore.Jobs;

  /// <summary>
  /// Represents a base class for all task runners.
  /// </summary>
  public abstract class JobRunnerBase
  {
    /// <summary>
    /// Message for the exception 'Online mode could be executed on SQL server Enterprise'.
    /// </summary>
    private const string SqlServerOnlyEnterpriseMessage = "Online index operations can only be performed in Enterprise edition of SQL Server";

    /// <summary>
    /// Message for the timeout exception.
    /// </summary>
    private const string SqlTimeoutMessage = "Timeout expired";

    /// <summary>
    /// Logic to handle sub-messages from SqlException inside Sitecore Job.
    /// </summary>
    /// <param name="se">The SQL Exception.</param>
    /// <param name="job">The job.</param>
    internal static void HandleSqlException(SqlException se, Job job)
    {
      if (se.Message.Contains(SqlTimeoutMessage))
      {
        job.Status.Messages.Add("Timeout was reached while running an SQL command on Analytics database. Please increase the value of 'AnalyticsDatabaseManager.SqlTimeout' and try again.");
      }
      else if (se.Message.Contains(SqlServerOnlyEnterpriseMessage))
      {
        job.Status.Messages.Add(SqlServerOnlyEnterpriseMessage);
      }
      else
      {
        job.Status.Messages.Add("An exception occurred during running the SQL command. Please check the log file for details.");
        job.Status.Messages.Add("Short info: " + se.Message);
      }

      Log.Error("SQL error during the running of the task.", se, job);
    }

    /// <summary>
    /// Runs this instance.
    /// </summary>
    protected void Run()
    {
      Job job = Context.Job;
      if (job != null)
      {
        try
        {
          job.Status.Processed = 0L;
          this.RunnerLogic(job);
        }
        catch (SqlSettingConstraintsException)
        {
          job.Status.Failed = true;
          job.Status.Messages.Add("There was problem with setting/removing constraints. Please re-run the task.");
        }
        catch (SqlException se)
        {
          job.Status.Failed = true;
          HandleSqlException(se, job);
        }
        catch (Exception exception)
        {
          job.Status.Failed = true;
          job.Status.Messages.Add("Unexpected exception during running of the Analytics Database Manager task. Please check the log files for more details.");
          Log.Error("Unexpected exception during running of the Analytics Database Manager task.", exception, this);
        }

        job.Status.State = JobState.Finished;
      }
    }

    /// <summary>
    /// Writes the diagnostic info into log file and result screen.
    /// </summary>
    /// <param name="job">The job.</param>
    /// <param name="message">The message.</param>
    protected virtual void WriteDiagnosticInfo(Job job, string message)
    {
      job.Status.Messages.Add(message);
      Log.Audit(message, this);
    }

    /// <summary>
    /// Runners the logic.
    /// </summary>
    /// <param name="job">The job.</param>
    protected abstract void RunnerLogic(Job job);
  }
}
