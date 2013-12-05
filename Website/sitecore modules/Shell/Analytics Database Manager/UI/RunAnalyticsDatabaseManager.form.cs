// -----------------------------------------------------------------------
// <copyright file="RunAnalyticsDatabaseManager.form.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the RunAnalyticsDatabaseManagerForm type.
// </summary>
// -----------------------------------------------------------------------

namespace Sitecore.AnalyticsDatabaseManager.UI
{
  using System;
  using System.IO;
  using System.Text;

  using Sitecore.AnalyticsDatabaseManager.Jobs;
  using Sitecore.Configuration;
  using Sitecore.Diagnostics;
  using Sitecore.Jobs;
  using Sitecore.Web.UI.HtmlControls;
  using Sitecore.Web.UI.Pages;
  using Sitecore.Web.UI.Sheer;
  using Sitecore.Web.UI.XmlControls;

  /// <summary>
  /// Defines the wizard form of the Analytics Database Manager
  /// </summary>
  public class RunAnalyticsDatabaseManagerForm : WizardForm
  {
    /// <summary>
    /// Bot description string.
    /// </summary>
    private const string BotDescString = "Remove {1} ({0})";

    /// <summary>
    /// Registry path.
    /// </summary>
    private const string RegistryPath = "/Current_User/ADM/";

    /// <summary>
    /// State message, that appears in the last page before executing the task.
    /// </summary>
    private const string StateMessage = "You have selected '{0}' task{1}";

    /// <summary>
    /// Addition to the state message, that appears in the last page before executing the task.
    /// </summary>
    private const string StateMessageWithParams = " with parameters: <br/>";

    /// <summary>
    /// Gets or sets the ready state message control.
    /// </summary>
    protected Literal ReadyStateMessage { get; set; }

    /// <summary>
    /// Gets or sets the actions page.
    /// </summary>
    protected XmlControl Actions { get; set; }

    /// <summary>
    /// Gets or sets the radio button control for CleanAll task.
    /// </summary>
    protected Radiobutton CleanAllButton { get; set; }

    /// <summary>
    /// Gets or sets the radio button control for CleanFiltered task.
    /// </summary>
    protected Radiobutton CleanFilteredButton { get; set; }

    /// <summary>
    /// Gets or sets the rebuild index button.
    /// </summary>
    protected Radiobutton RebuildIndexButton { get; set; }

    /// <summary>
    /// Gets or sets the remove bots button.
    /// </summary>
    protected Radiobutton RemoveBotsButton { get; set; }

    /// <summary>
    /// Gets or sets the backup database button.
    /// </summary>
    protected Radiobutton BackupDatabaseButton { get; set; }

    /// <summary>
    /// Gets or sets the sync definitions button.
    /// </summary>
    protected Radiobutton SyncDefinitionsButton { get; set; }

    /// <summary>
    /// Gets or sets the options page.
    /// </summary>
    protected XmlControl Options { get; set; }

    /// <summary>
    /// Gets or sets the border control for CleanAll task.
    /// </summary>
    protected Border CleanAllOptions { get; set; }

    /// <summary>
    /// Gets or sets the border control for CleanFiltered task.
    /// </summary>
    protected Border CleanFilteredOptions { get; set; }

    /// <summary>
    /// Gets or sets the rebuild index options panel.
    /// </summary>
    protected Border RebuildIndexOptions { get; set; }

    /// <summary>
    /// Gets or sets the backup database options.
    /// </summary>
    protected Border BackupDatabaseOptions { get; set; }

    /// <summary>
    /// Gets or sets the check box control for CleanAll task.
    /// </summary>
    protected Checkbox CleanAllCleanIpData { get; set; }

    /// <summary>
    /// Gets or sets the radio button for 'older than' filter in CleanFiltered task.
    /// </summary>
    protected Radiobutton CleanFilteredOlderThanButton { get; set; }

    /// <summary>
    /// Gets or sets the dateTimePicker control for CleanFiltered task.
    /// </summary>
    protected DateTimePicker CleanFilteredOlderThanDateTime { get; set; }

    /// <summary>
    /// Gets or sets the clean filtered bounce button.
    /// </summary>
    /// <value>
    /// The clean filtered bounce button.
    /// </value>
    protected Radiobutton CleanFilteredBounceButton { get; set; }

    /// <summary>
    /// Gets or sets the clean by custom button.
    /// </summary>
    /// <value>
    /// The clean by custom button.
    /// </value>
    protected Radiobutton CleanFilteredCustomButton { get; set; }

    /// <summary>
    /// Gets or sets the clean filtered custom column edit.
    /// </summary>
    /// <value>
    /// The clean filtered custom column edit.
    /// </value>
    protected Combobox CleanFilteredCustomColumnEdit { get; set; }

    /// <summary>
    /// Gets or sets the clean filtered custom value edit.
    /// </summary>
    /// <value>
    /// The clean filtered custom value edit.
    /// </value>
    protected Edit CleanFilteredCustomValueEdit { get; set; }

    /// <summary>
    /// Gets or sets the backup database location edit.
    /// </summary>
    protected Edit BackupDatabaseLocationEdit { get; set; }

    /// <summary>
    /// Gets or sets the rebuild index live checkbox.
    /// </summary>
    protected Checkbox RebuildIndexLive { get; set; }

    /// <summary>
    /// Gets or sets the control for error text
    /// </summary>
    protected Memo ErrorText { get; set; }

    /// <summary>
    /// Gets or sets the control for result text
    /// </summary>
    protected Memo ResultText { get; set; }

    /// <summary>
    /// Raises the load event.
    /// </summary>
    /// <param name="e">The EventArgs instance containing the event data.</param>
    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      base.OnLoad(e);
      if (!Context.ClientPage.IsEvent)
      {
        this.CleanAllButton.Checked = true;
        this.CleanFilteredOlderThanButton.Checked = true;
        this.CleanAllCleanIpData.Checked = false;
        this.CleanFilteredOlderThanDateTime.Value = Registry.GetString(
          RegistryPath + "CleanFilteredOlderThanDateTime",
          Registry.GetString(RegistryPath + "CleanFilteredOlderThanDateTime", DateTime.Now.ToString("yyyyMMddT") + "000000"));
        this.CleanFilteredCustomColumnEdit.Value = Registry.GetString(RegistryPath + "CleanFilteredCustomColumnEdit", "TrafficType");
        this.CleanFilteredCustomValueEdit.Value = Registry.GetString(RegistryPath + "CleanFilteredCustomValueEdit", string.Empty);
        this.BackupDatabaseLocationEdit.Value = this.GenerateBackupFileName();
      }
    }
    
    /// <summary>
    /// Overrides base method to set the correct button labels and states in the wizard.
    /// </summary>
    /// <param name="page">The page that has been entered.</param>
    /// <param name="oldPage">The page that was left.</param>
    protected override void ActivePageChanged(string page, string oldPage)
    {
      Assert.ArgumentNotNull(page, "page");
      Assert.ArgumentNotNull(oldPage, "oldPage");
      this.NextButton.Header = "Next >";
      if (page == "Ready")
      {
        this.NextButton.Header = "Start";
        this.PrintTaskParameters();
      }

      base.ActivePageChanged(page, oldPage);
      if (page == "Running")
      {
        this.NextButton.Disabled = true;
        this.BackButton.Disabled = true;
        this.CancelButton.Disabled = true;
        Context.ClientPage.ClientResponse.Timer("StartRunning", 10);
      }
    }

    /// <summary>
    /// Overrides base method to set the correct button labels and states in the wizard.
    /// </summary>
    /// <param name="page">The page that is being left.</param>
    /// <param name="newPage">The new page that is being entered.</param>
    /// <returns>True, if the change is allowed, otherwise false.</returns>
    protected override bool ActivePageChanging(string page, ref string newPage)
    {
      Assert.ArgumentNotNull(page, "page");
      Assert.ArgumentNotNull(newPage, "newPage");

      this.PageChangingValidation(page, ref newPage);

      if ((page == "Retry") && (newPage == "Running"))
      {
        newPage = "Ready";
        this.NextButton.Disabled = false;
        this.BackButton.Disabled = false;
        this.CancelButton.Disabled = false;
      }

      if (newPage == "Options")
      {
        this.CleanAllOptions.Visible = this.CleanAllButton.Checked;
        this.CleanFilteredOptions.Visible = this.CleanFilteredButton.Checked;
        this.RebuildIndexOptions.Visible = this.RebuildIndexButton.Checked;
        this.BackupDatabaseOptions.Visible = this.BackupDatabaseButton.Checked;

        this.SkipOptionsPageIfNeeded(page, ref newPage);
      }

      return base.ActivePageChanging(page, ref newPage);
    }

    /// <summary>
    /// Check the Job status and make the appropriate changes.
    /// </summary>
    protected void CheckStatus()
    {
      Job job = JobManager.GetJob(Handle.Parse((Context.ClientPage.ServerProperties["handle"] as string) ?? string.Empty));
      if (job.Status.Failed)
      {
        this.Active = "Retry";
        this.NextButton.Disabled = true;
        this.BackButton.Disabled = false;
        this.CancelButton.Disabled = false;
        this.ErrorText.Value = StringUtil.StringCollectionToString(job.Status.Messages, "\r\n");
      }
      else
      {
        if (job.IsDone)
        {
          this.Active = "LastPage";
          this.BackButton.Disabled = true;
          this.ResultText.Value = StringUtil.StringCollectionToString(job.Status.Messages, "\r\n");
        }
        else
        {
          string status = (job.Status.State == JobState.Running) ? "Processing..." : "Queued.";
          Context.ClientPage.ClientResponse.SetInnerHtml("Status", status);
          Context.ClientPage.ClientResponse.Timer("CheckStatus", 500);
        }
      }
    }

    /// <summary>
    /// Starts the selected job.
    /// </summary>
    protected void StartRunning()
    {
      string[] taskRunnerParam;
      object runner = this.SelectJobRunner(out taskRunnerParam);
      JobOptions options = new JobOptions("Analytics Database Manager", "Analytics", Client.Site.Name, runner, "Run")
      {
        AfterLife = TimeSpan.FromMinutes(1.0),
        ContextUser = Context.User
      };
      var job = JobManager.Start(options);
      Context.ClientPage.ServerProperties["handle"] = job.Handle.ToString();
      Context.ClientPage.ClientResponse.Timer("CheckStatus", 1000);
    }

    /// <summary>
    /// Executes the validation actions during changing the active page in the Wizard.
    /// </summary>
    /// <param name="page">The page that is being left.</param>
    /// <param name="newPage">The new page that is being entered.</param>
    private void PageChangingValidation(string page, ref string newPage)
    {
      if (page != "Options")
      {
        return;
      }

      if (this.CleanFilteredCustomButton.Checked && newPage == "Ready"
          && string.IsNullOrEmpty(this.CleanFilteredCustomColumnEdit.Value))
      {
        SheerResponse.ShowError("Please, select a column to use filtering by custom rule!", string.Empty);
        newPage = page;
      }
      else if (this.BackupDatabaseButton.Checked && newPage == "Ready"
               && !Directory.Exists(Path.GetDirectoryName(this.BackupDatabaseLocationEdit.Value) ?? string.Empty))
      {
        SheerResponse.ShowError("Directory of the specified backup file does not exist!", string.Empty);
        newPage = page;
      }
    }

    /// <summary>
    /// Skips the options page if needed.
    /// </summary>
    /// <param name="page">The page that is being left.</param>
    /// <param name="newPage">The new page that is being entered.</param>
    private void SkipOptionsPageIfNeeded(string page, ref string newPage)
    {
      if (this.RemoveBotsButton.Checked || this.SyncDefinitionsButton.Checked)
      {
        newPage = (page == "Ready") ? "Actions" : "Ready";
      }
    }

    /// <summary>
    /// Prints the task parameters.
    /// </summary>
    private void PrintTaskParameters()
    {
      string[] taskRunnerParam;
      this.SelectJobRunner(out taskRunnerParam);

      StringBuilder stateParams = new StringBuilder();
      if (taskRunnerParam.Length > 1)
      {
        stateParams.Append(StateMessageWithParams);
        stateParams.Append("<ul>");
        for (int i = 1; i < taskRunnerParam.Length; i++)
        {
          stateParams.Append("<li style='width: 400px; word-wrap: break-word;'>").Append(taskRunnerParam[i]).Append("</li>");
        }

        stateParams.Append("</ul>");
      }
      else
      {
        stateParams.Append(".");
      }

      this.ReadyStateMessage.Text = string.Format(StateMessage, taskRunnerParam[0], stateParams);
    }

    /// <summary>
    /// Selects the job runner.
    /// </summary>
    /// <param name="param">Return the parameters of the selected runner.</param>
    /// <returns>Job runner object.</returns>
    private object SelectJobRunner(out string[] param)
    {
      if (this.CleanAllButton.Checked)
      {
        return this.SelectCleanAllJob(out param);
      }
      
      if (this.CleanFilteredButton.Checked)
      {
        if (this.CleanFilteredOlderThanButton.Checked)
        {
          return this.SelectCleanFilteredOlderThan(out param);
        }
        
        if (this.CleanFilteredBounceButton.Checked)
        {
          return this.SelectCleanFilteredBounce(out param);
        }
        
        if (this.CleanFilteredCustomButton.Checked)
        {
          return this.SelectCleanFilteredCustom(out param);
        }
      }
      
      if (this.RebuildIndexButton.Checked)
      {
        return this.SelectRebuildIndex(out param);
      }
      
      if (this.RemoveBotsButton.Checked)
      {
        return this.SelectRemoveBots(out param);
      }
      
      if (this.BackupDatabaseButton.Checked)
      {
        return this.SelectBackupDatabase(out param);
      }
      
      if (this.SyncDefinitionsButton.Checked)
      {
        return this.SelectSyncDefinitions(out param);
      }

      param = new string[] { };
      return null;
    }

    /// <summary>
    /// Selects the sync definitions.
    /// </summary>
    /// <param name="param">The parameters of the specified Job Runner.</param>
    /// <returns>The job runner.</returns>
    private object SelectSyncDefinitions(out string[] param)
    {
      param = new[]
      {
        this.SyncDefinitionsButton.Header,
        "Automations",
        "Goals and Page Events",
        "Campaigns",
        "Multivariate Tests and Variables"
      };
      return new SynchronizeDefinitionsToDatabaseRunner();
    }

    /// <summary>
    /// Selects the backup database.
    /// </summary>
    /// <param name="param">The parameters of the specified Job Runner.</param>
    /// <returns>The job runner.</returns>
    private object SelectBackupDatabase(out string[] param)
    {
      string pathTo = this.BackupDatabaseLocationEdit.Value;
      param = new[] { this.BackupDatabaseButton.Header, "Backup to: " + pathTo };
      Registry.SetString(RegistryPath + "BackupDatabaseLocationEdit", Path.GetDirectoryName(this.BackupDatabaseLocationEdit.Value));
      return new BackupDatabaseRunner(pathTo);
    }

    /// <summary>
    /// Selects the remove bots.
    /// </summary>
    /// <param name="param">The parameters of the specified Job Runner.</param>
    /// <returns>The job runner.</returns>
    private object SelectRemoveBots(out string[] param)
    {
      param = new[]
      {
        this.RemoveBotsButton.Header, 
        string.Format(BotDescString, 900, "Bot - Feed Reader"),
        string.Format(BotDescString, 910, "Bot - Search Engine"),
        string.Format(BotDescString, 920, "Bot - Unidentified"),
        string.Format(BotDescString, 925, "Bot - Autodetected"),
        string.Format(BotDescString, 930, "Bot - Malicious")
      };
      return new RemoveBotsRunner();
    }

    /// <summary>
    /// Selects the index of the rebuild.
    /// </summary>
    /// <param name="param">The parameters of the specified Job Runner.</param>
    /// <returns>The job runner.</returns>
    private object SelectRebuildIndex(out string[] param)
    {
      bool onlineMode = this.RebuildIndexLive.Checked;
      param = new[] { this.RebuildIndexButton.Header, "Online mode: " + onlineMode };
      return new RebuildIndexRunner(onlineMode);
    }
    
    private object SelectCleanFilteredCustom(out string[] param)
    {
      if (string.IsNullOrEmpty(this.CleanFilteredCustomColumnEdit.Value))
      {
        throw new InvalidCastException("Incorrect custom column was specified");
      }

      string column = this.CleanFilteredCustomColumnEdit.Value;
      string value = this.CleanFilteredCustomValueEdit.Value;

      Registry.SetString(RegistryPath + "CleanFilteredCustomColumnEdit", column);
      Registry.SetString(RegistryPath + "CleanFilteredCustomValueEdit", value);
      param = new[]
        {
          this.CleanFilteredButton.Header,
          "Filter: " + this.CleanFilteredCustomButton.Header, 
          string.Format("Remove visits, where '{0}' = '{1}'", column, value)
        };
      return new CleanFilteredCustomRuleRunner(column, value);
    }

    /// <summary>
    /// Selects the clean filtered bounce.
    /// </summary>
    /// <param name="param">The parameters of the specified Job Runner.</param>
    /// <returns>The job runner.</returns>
    private object SelectCleanFilteredBounce(out string[] param)
    {
      param = new[] { this.CleanFilteredButton.Header, "Filter: " + this.CleanFilteredBounceButton.Header };
      return new CleanFilteredBounceSessionsRunner();
    }

    /// <summary>
    /// Selects the clean filtered older than.
    /// </summary>
    /// <param name="param">The parameters of the specified Job Runner.</param>
    /// <returns>The job runner.</returns>
    private object SelectCleanFilteredOlderThan(out string[] param)
    {
      DateTime dateTime = DateUtil.ParseDateTime(this.CleanFilteredOlderThanDateTime.Value, DateTime.MinValue);
      Registry.SetString(RegistryPath + "CleanFilteredOlderThanDateTime", this.CleanFilteredOlderThanDateTime.Value);
      param = new[] { this.CleanFilteredButton.Header, "Filter: " + this.CleanFilteredOlderThanButton.Header, "DateTime: " + dateTime };
      return new CleanFilteredOlderThanRunner(dateTime);
    }

    /// <summary>
    /// Selects the clean all job.
    /// </summary>
    /// <param name="param">The parameters of the specified Job Runner.</param>
    /// <returns>The job runner.</returns>
    private object SelectCleanAllJob(out string[] param)
    {
      bool cleanIps = this.CleanAllCleanIpData.Checked;
      param = new[] { this.CleanAllButton.Header, "Clean GeoIP Lookup data: " + cleanIps };
      return new CleanAllRunner(cleanIps);
    }

    /// <summary>
    /// Generates the full file name to the backup file for BackupDatabaseLocationEdit.
    /// </summary>
    /// <returns>The generated file name.</returns>
    private string GenerateBackupFileName()
    {
      string dateString = DateTime.Now.ToString("yyMMdd");
      string pathTo = Registry.GetString(RegistryPath + "BackupDatabaseLocationEdit", Settings.DataFolder);
      string generatedFileName = Settings.GetSetting("AnalyticsDatabaseManager.BackupFileName", "Analytics{0}.bak");

      if (!pathTo.EndsWith(Path.DirectorySeparatorChar.ToString()))
      {
        pathTo += Path.DirectorySeparatorChar.ToString();
      }

      return pathTo + string.Format(generatedFileName, dateString);
    }
  }
}
