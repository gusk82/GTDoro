using GTDoro.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace GTDoro.Core.Models
{
    /// <summary>
    /// Specify the rights for a user to interact with a project
    /// </summary>
    public enum PermissionType
    {
        Admin = 0,
        Work = 1,
        Read = 2
    }

    /// <summary>
    /// Useful for identifying the type in a generic container
    /// </summary>
    public enum PomodoroContainerType
    {
        Action = 0,
        Task = 1,
        Project = 2,
        Workspace = 3,
        Sprint = 4,
        Unspecified = 5
    }

    /// <summary>
    /// Enumeration for the status of Projects, Tasks and Actions
    /// </summary>
    public enum Status
    {
        [IconCssClass("fa-play-circle")]
        Active = 0, //default
        [IconCssClass("fa-times")]
        Cancelled = 1,
        [Display(Name = "On Hold")]
        [IconCssClass("fa-pause")]
        OnHold = 2,
        [IconCssClass("fa-check")]
        Completed = 3,
    }

    /// <summary>
    /// Enumeration for the project status, including calculated states
    /// </summary>
    public enum ProjectExtendedStatus
    {
        [IconCssClass("fa-file-o")]         Created = 0, //default

        [IconCssClass("fa-times")]   Cancelled = Status.Cancelled,

        [Display(Name = "On Hold")]
        [IconCssClass("fa-pause")]          OnHold = Status.OnHold,

        [IconCssClass("fa-check")]          Completed = Status.Completed,

        [Display(Name = "In Progress")]
        [IconCssClass("fa-play-circle")]    InProgress = 5
    }

    /// <summary>
    /// Enumeration for the task status, including calculated states
    /// </summary>
    public enum TaskExtendedStatus
    {
        [IconCssClass("fa-file-o")]
        Created = 0, //default

        [IconCssClass("fa-times")]
        Cancelled = Status.Cancelled,

        [Display(Name = "On Hold")]
        [IconCssClass("fa-pause")]
        OnHold = Status.OnHold,

        [IconCssClass("fa-check")]
        Completed = Status.Completed,
        
        [Display(Name = "In Progress")]
        [IconCssClass("fa-play-circle")]
        InProgress = 5,

        [Display(Name = "Project On Hold")]
        [IconCssClass("fa-pause")]          OnHoldInherited = 6,

        [Display(Name = "Project Completed")]
        [IconCssClass("fa-check")]          CompletedInherited = 7,

        [Display(Name = "Project Cancelled")]
        [IconCssClass("fa-times-circle")]   CancelledInherited = 8,

        [Display(Name = "Project Archived")]
        [IconCssClass("fa-paperclip")]      ArchivedInherited = 9
    }
    
    /// <summary>
    /// Enumeration for setting manually the action status, not including calculated states
    /// </summary>
    public enum ActionExtendedStatus
    {
        [IconCssClass("fa-file-o")]         Created = 0, //default

        [IconCssClass("fa-check")]          Completed = Status.Completed,

        [IconCssClass("fa-times")]          Cancelled = Status.Cancelled,

        [Display(Name = "In Progress")]
        [IconCssClass("fa-play-circle")]    InProgress = 3,

        [IconCssClass("fa-exclamation")]    Exceeded = 4,

        [Display(Name = "Container On Hold")]
        [IconCssClass("fa-pause")]          OnHoldInherited = 5,

        [Display(Name = "Container Completed")]
        [IconCssClass("fa-check")]          CompletedInherited = 6,

        [Display(Name = "Container Cancelled")]
        [IconCssClass("fa-times-circle")]   CancelledInherited = 7,

        [Display(Name = "Container Archived")]
        [IconCssClass("fa-paperclip")]      ArchivedInherited = 8
    }

    /// <summary>
    /// Enumeration for setting manually the pomodoro status, not including calculated states
    /// </summary>
    public enum LoggableItemCalculatedStatus
    {
        [IconCssClass("fa-calendar")]       Planified = PomodoroStatus.Planified, //default
        [IconCssClass("fa-play")]           Working = PomodoroStatus.Working,
        [IconCssClass("fa-times-circle")]   Cancelled = PomodoroStatus.Cancelled,
        [IconCssClass("fa-check")]          Completed = PomodoroStatus.Completed,
        [IconCssClass("fa-question")]       Unconfirmed = PomodoroStatus.Unconfirmed,
        [IconCssClass("fa-calendar-o")]     Expired = 6
    }
    /// <summary>
    /// Enumeration for the status of a Pomodoro
    /// </summary>
    public enum PomodoroStatus
    {
        Planified = 0, //default
        Working = 1,
        Cancelled = 2,
        Completed = 3,
        Unconfirmed = 4
    }
    
    /// <summary>
    /// Enumeration for the status of a TimePeriod
    /// </summary>
    public enum TimePeriodStatus
    {
        Planified = 0, //default
        Working = 1,
        Cancelled = 2,
        Completed = 3
    }

    /// <summary>
    /// Enumeration for the status of the user current work
    /// </summary>
    public enum WorkingStatus
    {
        NoActionSelected = 0,
        ActionSelected = 1,
        Working = 2,
        PendingConfirmation = 3,
        BreakTime = 4,
        Inconsistent = 5
    }

    /// <summary>
    /// Enumeration for a level in a discrete scale of values (5)
    /// </summary>
    public enum Level
    {
        Low = -1,
        Normal = 0,
        High = 1
    }

    /// <summary>
    /// Enumeration for a level in a discrete scale of values (5)
    /// </summary>
    public enum LevelExtended
    {
        [IconCssClass("fa-angle-double-down")]  [Display(Order=0)]  Lowest = -2,
        [IconCssClass("fa-angle-down")]         [Display(Order=1)]  Low = Level.Low,
        [IconCssClass("fa-angle-right")]        [Display(Order=2)]  Normal = Level.Normal,
        [IconCssClass("fa-angle-up")]           [Display(Order=3)]  High = Level.High,
        [IconCssClass("fa-angle-double-up")]    [Display(Order=4)]  Highest = 2
    }

    /// <summary>
    /// Warnings for calendar events
    /// </summary>
    public enum CalendarWarning
    {
        InvalidAction = 0,
        NonPositiveInterval = 1,
        PastTime = 2,
        Conflict = 3
    }

    /// <summary>
    /// Report types
    /// </summary>
    public enum ReportType
    {
        [Display(Name = "Work History")]
        WorkHistory = 0
    }

    /// <summary>
    /// Indicates the work percentage group according to the estimate
    /// </summary>
    public enum EffortGroup
    {
        [IconCssClass(""/*"bg-light-blue"*/)] Created = 0,
        [Display(Name = "In Progress")]
        [IconCssClass("bg-green")]      InProgress = 1,
        [IconCssClass("bg-yellow")]     Exceeded = 2,
        [IconCssClass("bg-red")]        VeryExceeded = 3,
        [IconCssClass("")]              Indeterminate = 4
    }

    /// <summary>
    /// Indicates the progress
    /// </summary>
    public enum ProgressGroup
    {
        [IconCssClass(""/*"bg-light-blue"*/)]
        Created = 0,
        [Display(Name = "In Progress")]
        [IconCssClass("bg-yellow")]
        InProgress = 1,
        [IconCssClass("bg-green")]
        Completed = 2
    }

    public enum DateInterval
    {
        None = 0,
        Monthly = 1,
        BiMonthly = 2,
        Quarterly = 3,
        EveryFourthMonth = 4,
        SemiAnnually = 6,
        Annually = 12
    }

    /// <summary>
    /// Available operations for the work items
    /// </summary>
    public enum Operation
    {
        [Display(Name = "Project Info")]
        [IconCssClass("fa-bullseye")]
        ProjectInfo,

        [Display(Name = "Task Info")]
        [IconCssClass("fa-dot-circle-o")]
        TaskInfo,

        [Display(Name = "Action Info")]
        [IconCssClass("fa-circle")]
        ActionInfo,

        [Display(Name = "Activity Info")]
        [IconCssClass("fa-circle")]
        ActivityInfo,

        [Display(Name = "Sprint Info")]
        [IconCssClass("fa-calendar-o")]
        SprintInfo,

        [Display(Name = "Sprint Comments")]
        [IconCssClass("fa-comments-o")]
        SprintComments,

        [Display(Name = "Edit Project")]
        [IconCssClass("fa-edit")]
        EditProject,

        [Display(Name = "Edit Task")]
        [IconCssClass("fa-edit")]
        EditTask,

        [Display(Name = "Edit Action")]
        [IconCssClass("fa-edit")]
        EditAction,

        [Display(Name = "Edit Activity")]
        [IconCssClass("fa-edit")]
        EditActivity,

        [Display(Name = "Edit Sprint")]
        [IconCssClass("fa-edit")]
        EditSprint,

        [Display(Name = "Delete Project")]
        [IconCssClass("fa-trash-o")]
        DeleteProject,

        [Display(Name = "Delete Task")]
        [IconCssClass("fa-trash-o")]
        DeleteTask,

        [Display(Name = "Delete Action")]
        [IconCssClass("fa-trash-o")]
        DeleteAction,

        [Display(Name = "Delete Activity")]
        [IconCssClass("fa-trash-o")]
        DeleteActivity,

        [Display(Name = "Delete Sprint")]
        [IconCssClass("fa-trash-o")]
        DeleteSprint,

        [IconCssClass("fa-file-o")]
        Create,

        [Display(Name = "Create Project")]
        //[IconCssClass("fa-bullseye")]
        [IconCssClass("fa-plus-circle")]
        CreateProject,

        [Display(Name = "Create Task")]
        //[IconCssClass("fa-dot-circle-o")]
        [IconCssClass("fa-plus-circle")]
        CreateTask,

        [Display(Name = "Create Action")]
        //[IconCssClass("fa-circle")]
        [IconCssClass("fa-plus-circle")]
        CreateAction,

        [Display(Name = "Create Activity")]
        //[IconCssClass("fa-circle")]
        [IconCssClass("fa-plus-circle")]
        CreateActivity,

        [Display(Name = "Create Sprint")]
        [IconCssClass("fa-plus-circle")]
        CreateSprint,

        [Display(Name = "Project List")]
        [IconCssClass("fa-sitemap")]
        ProjectList,

        [Display(Name = "Task List")]
        [IconCssClass("fa-sitemap")]
        TaskList,

        [Display(Name = "Action List")]
        [IconCssClass("fa-sitemap")]
        ActionList,

        [Display(Name = "Sprint List")]
        [IconCssClass("fa-sitemap")]
        SprintList,

        [Display(Name = "Work History")]
        [IconCssClass("fa-wrench")]
        WorkHistory,

        [Display(Name = "Work Division")]
        [IconCssClass("fa-adjust")]
        WorkDivision,

        [Display(Name = "Work Amount")]
        [IconCssClass("fa-bar-chart-o")]
        WorkAmount,

        [IconCssClass("fa-calendar")]
        Calendar
    }

    /// <summary>
    /// Different controls to select an item
    /// </summary>
    public enum ItemSelectorType
    {
        AccordionNav = 0,
        DropDownNav = 1,
        Autocomplete = 2
    }

    /// <summary>
    /// Item selector objective
    /// </summary>
    public enum ItemSelectorTarget
    {
        Navigate = 0,
        SelectActiveAction = 1,
        CreateItemFromCollectedThing = 2,
        ChangeParentItem = 3
    }

    /// <summary>
    /// Indicates the mode for the calendar view
    /// </summary>
    public enum CalendarViewMode
    {
        ViewNumber = 0,
        ViewNumberDetail = 1,
        ViewFullDetail = 2
    }

    /// <summary>
    /// Key Value Parameters
    /// </summary>
    public enum ParameterType
    {
        RegMessageSubject,
        RegMessageContent
    }

    /// <summary>
    /// Intended use for the filter bar
    /// </summary>
    public enum FilterBarMode
    {
        Filter = 0,
        MultipleEdit = 1
    }
}