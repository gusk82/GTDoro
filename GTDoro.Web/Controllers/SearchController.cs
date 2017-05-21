using GTDoro.Core.DAL;
using GTDoro.Core.Models;
using GTDoro.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace GTDoro.Controllers
{
    [Authorize]
    public class SearchController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();

        private SearchResults SearchAll(string term, SearchResults currentResults = null)
        {
            if (currentResults == null)
            {
                currentResults = new SearchResults();
            }
            currentResults.SearchTerm = term;

            currentResults = SearchActions(term, currentResults);
            currentResults = SearchTasks(term, currentResults);
            currentResults = SearchProjects(term, currentResults);

            return currentResults;
        }

        private SearchResults SearchActions(string term = "", SearchResults currentResults = null, Status? status = null)
        {
            if (currentResults == null)
            {
                currentResults = new SearchResults();
            }
            currentResults.SearchTerm = term;

            currentResults.ActionResults = db.GetMyActions(User)
                .Where(a =>
                    a.Name.ToLower().Contains(currentResults.SearchTerm) ||
                    a.Description.ToLower().Contains(currentResults.SearchTerm))
                .Where(a =>
                    !status.HasValue || a.Status == status.Value)
                .ToList();

            return currentResults;
        }

        private SearchResults SearchTasks(string term = "", SearchResults currentResults = null, Status? status = null)
        {
            if (currentResults == null)
            {
                currentResults = new SearchResults();
            }
            currentResults.SearchTerm = term;

            currentResults.TaskResults = db.GetMyTasks(User)
                .Where(t =>
                    t.Code.ToLower().Contains(currentResults.SearchTerm) ||
                    t.Name.ToLower().Contains(currentResults.SearchTerm) ||
                    t.Description.ToLower().Contains(currentResults.SearchTerm))
                .Where(t =>
                    !status.HasValue || t.Status == status.Value)
                .ToList();

            return currentResults;
        }

        private SearchResults SearchProjects(string term = "", SearchResults currentResults = null, Status? status = null)
        {
            if (currentResults == null)
            {
                currentResults = new SearchResults();
            }
            currentResults.SearchTerm = term;

            currentResults.ProjectResults = db.GetMyProjects(User)
                .Where(p =>
                    p.Code.ToLower().Contains(currentResults.SearchTerm) ||
                    p.Name.ToLower().Contains(currentResults.SearchTerm) ||
                    p.Description.ToLower().Contains(currentResults.SearchTerm))
                .Where(p =>
                    !status.HasValue || p.Status == status.Value)
                .ToList();

            return currentResults;
        }

        //
        // GET: /Search/
        public ActionResult Index(string term)
        {
            return View(SearchAll(term));
        }

        [ChildActionOnly]
        public ActionResult Autocomplete(PomodoroContainerType itemType, ItemSelectorTarget target /*int? ActionID = null*/)
        {
            SearchResults results = null;
            SelectList selectList = null;
            switch(itemType)
            {
                case PomodoroContainerType.Project:
                    results = SearchProjects(String.Empty, null, null);
                    selectList = new SelectList(results.GetLightResults(PomodoroContainerType.Project), "ID", "Name");
                    break;
                case PomodoroContainerType.Task:
                    results = SearchTasks(String.Empty, null, null);
                    selectList = new SelectList(results.GetLightResults(PomodoroContainerType.Task), "ID", "Name");
                    break;
                case PomodoroContainerType.Action:
                    results = SearchActions(String.Empty, null, (target != ItemSelectorTarget.Navigate) ? (Status?)Status.Active : null);
                    selectList = new SelectList(results.GetLightResults(PomodoroContainerType.Action), "ID", "Name");
                    break;
            }
            if(target == ItemSelectorTarget.ChangeParentItem)
            {
                ViewBag.ElementID = "parent";
                ViewBag.parent = selectList;
            }
            else
            {
                ViewBag.ElementID = "id";
                ViewBag.id = selectList;
            }

            return PartialView("Autocomplete", results);
        }

        public ActionResult ItemNavSelector(PomodoroContainerType type, ItemSelectorTarget target,
            ItemSelectorType selectorType, int? collectedThingID, int? sourceItemID, string defaultItemName)
        {
            ItemSelectorViewModel viewModel = new ItemSelectorViewModel(){
                Target = target,
                ItemType = type,
                CollectedThingID = collectedThingID,
                SourceItemID = sourceItemID,
                SelectorType = selectorType,
                DefaultItemName = defaultItemName,
                AvailableWork = db.GetMyProjects(User)
                   .Include(p => p.Tasks.Select(t => t.Actions))
            };

            return PartialView("Panels/ItemSelector/_ItemNavSelector", viewModel);

        }

        public ActionResult ItemSelector(PomodoroContainerType type, ItemSelectorTarget target, 
            int? collectedThingID, int? sourceItemID, string defaultItemName)
        {
            string itemDesc = "Pomodoro";
            switch(type)
            {
                case PomodoroContainerType.Project:
                    itemDesc = "Task";
                    break;
                case PomodoroContainerType.Task:
                    itemDesc = "Action";
                    break;
            }
            string title = "Please select the ";
            switch(target)
            {
                case ItemSelectorTarget.ChangeParentItem:
                    title += "new " + type.ToString() + " for the current " + itemDesc;
                    break;
                case ItemSelectorTarget.CreateItemFromCollectedThing:
                    title += type.ToString() + " for the new " + itemDesc;
                    break;
                case ItemSelectorTarget.SelectActiveAction:
                    title += "new active Action to work";
                    break;
                case ItemSelectorTarget.Navigate:
                    title += "item to view its details";
                    break;
            }
            ItemSelectorViewModel viewModel = new ItemSelectorViewModel()
            {
                Target = target,
                Title = title,
                ItemType = type,
                CollectedThingID = collectedThingID,
                SourceItemID = sourceItemID,
                DefaultItemName = defaultItemName,
                AvailableWork = db.GetMyProjects(User)
                   .Include(p => p.Tasks.Select(t => t.Actions))
            };

            return PartialView("Panels/ItemSelector/_ItemSelector", viewModel);
        }
	}
}