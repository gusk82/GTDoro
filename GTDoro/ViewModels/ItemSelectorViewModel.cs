using GTDoro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTDoro.ViewModels
{
    public class ItemSelectorViewModel
    {
        public IEnumerable<Project> AvailableWork { get; set; }
        public string Title { get; set; }
        public ItemSelectorType SelectorType { get; set; }
        public PomodoroContainerType ItemType { get; set; }
        public int? CollectedThingID { get; set; }
        public string DefaultItemName { get; set; }
        public int? SourceItemID { get; set; }

        private ItemSelectorTarget target;
        public ItemSelectorTarget Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
                Initialize();
            }
        }

        public string ProjectController { get; private set; }
        public string ProjectAction { get; private set; }
        public string TaskController { get; private set; }
        public string TaskAction { get; private set; }
        public string ActionController { get; private set; }
        public string ActionAction { get; private set; }


        private void Initialize()
        {
            switch (Target)
            {
                case ItemSelectorTarget.Navigate:
                    ProjectController = "Project";
                    ProjectAction = "Details";
                    TaskController = "Task";
                    TaskAction = "Details";
                    ActionController = "Action";
                    ActionAction = "Details";
                    break;
                case ItemSelectorTarget.ChangeParentItem:
                    ProjectController = "Task";
                    ProjectAction = "ChangeProject";
                    TaskController = "Action";
                    TaskAction = "ChangeTask";
                    ActionController = "Pomodoro";
                    ActionAction = "ChangeAction";
                    break;
                case ItemSelectorTarget.CreateItemFromCollectedThing:
                    ProjectController = "Project";
                    ProjectAction = "CreateTask";
                    TaskController = "Task";
                    TaskAction = "CreateAction";
                    ActionController = string.Empty;
                    ActionAction = string.Empty;
                    break;
                case ItemSelectorTarget.SelectActiveAction:
                    ActionController = string.Empty;
                    ProjectAction = string.Empty;
                    TaskController = string.Empty;
                    TaskAction = string.Empty;
                    ActionController = "Action";
                    ActionAction = "Select";
                    break;
            }
        }

        public string FormController
        {
            get
            {
                switch(ItemType)
                {
                    case PomodoroContainerType.Project:
                        return ProjectController;
                    case PomodoroContainerType.Task:
                        return TaskController;
                    case PomodoroContainerType.Action:
                        return ActionController;
                }
                return string.Empty;
            }
        }
        
        public string FormAction
        {
            get
            {
                switch (ItemType)
                {
                    case PomodoroContainerType.Project:
                        return ProjectAction;
                    case PomodoroContainerType.Task:
                        return TaskAction;
                    case PomodoroContainerType.Action:
                        return ActionAction;
                }
                return string.Empty;
            }
        }

        public object GetRouteValues(int itemID)
        {
            if (Target == ItemSelectorTarget.CreateItemFromCollectedThing && CollectedThingID.HasValue)
            {
                //return new { id = itemID, ct = CollectedThingID.Value };
                return new { op = "c", id = itemID, ct = CollectedThingID.Value };
            }
            else if (Target == ItemSelectorTarget.CreateItemFromCollectedThing && !string.IsNullOrWhiteSpace(DefaultItemName))
            {
                return new { id = itemID, text = DefaultItemName };
            }
            else if (Target == ItemSelectorTarget.ChangeParentItem && SourceItemID.HasValue)
            {
                return new { id = SourceItemID.Value, parent = itemID };
            }
            return new { id = itemID };
        }

        public string ListCssClass
        {
            get
            {
                return SelectorType == ItemSelectorType.AccordionNav ? 
                    "menu" : 
                    "nav-dropdown menu";
            }
        }

        public bool IsSelectActionMode
        {
            get
            {
                return Target == ItemSelectorTarget.SelectActiveAction;
            }
        }

        public bool IsSelectTaskModeForActionMode
        {
            get
            {
                return Target == ItemSelectorTarget.CreateItemFromCollectedThing && 
                    this.ItemType == PomodoroContainerType.Task;
            }
        }
        
    }
}