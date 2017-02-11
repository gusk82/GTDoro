using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GTDoro.Models
{
    [NotMapped]
    public class SearchResults
    {
        public ICollection<Project> ProjectResults { get; set; }
        public ICollection<Task> TaskResults { get; set; }
        public ICollection<Action> ActionResults { get; set; }

        private string searchTerm;
        public string SearchTerm 
        {
            get { return searchTerm; }
            set
            {
                if (!String.IsNullOrEmpty(searchTerm) && 
                    value.ToLower() != searchTerm)
                {
                    throw new ArgumentException("A SearchResults instance cannot be provided if it has a different search term");
                }
                searchTerm = (!String.IsNullOrEmpty(value)) ?
                    value.ToLower() : String.Empty;
            }
        }
                
        public ICollection<object> GetLightResults(PomodoroContainerType type)
        {
            IEnumerable<PomodoroContainer> enumToGoThrough = null;
            List<object> lst = new List<object>();
            switch(type)
            {
                case PomodoroContainerType.Action:
                    enumToGoThrough = ActionResults;
                    break;
                case PomodoroContainerType.Task:
                    enumToGoThrough = TaskResults;
                    break;
                case PomodoroContainerType.Project:
                    enumToGoThrough = ProjectResults;
                    break;
            }
            foreach (PomodoroContainer result in enumToGoThrough)
            {
                lst.Add(new { ID = result.Ident, Name = result.PathItemName, ContainerType = result.Type });
            }
            return lst;
        
        }

        public ICollection<string> GetLightStringResults(PomodoroContainerType type)
        {
            IEnumerable<PomodoroContainer> enumToGoThrough = null;
            List<string> lst = new List<string>();
            switch (type)
            {
                case PomodoroContainerType.Action:
                    enumToGoThrough = ActionResults;
                    break;
                case PomodoroContainerType.Task:
                    enumToGoThrough = TaskResults;
                    break;
                case PomodoroContainerType.Project:
                    enumToGoThrough = ProjectResults;
                    break;
            }
            foreach (PomodoroContainer result in enumToGoThrough)
            {
                lst.Add(result.PathItemName);
            }
            return lst;
        }
    }
}