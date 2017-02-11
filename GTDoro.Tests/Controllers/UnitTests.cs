using GTDoro.Controllers;
using GTDoro.DAL;
using GTDoro.Models;
using GTDoro.Models.Identity;
using Task = GTDoro.Models.Task;
using Action = GTDoro.Models.Action;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Threading;
using GTDoro.ViewModels;


namespace GTDoro.Tests
{
    [TestClass]
    public class UnitTest
    {
        private GTDoroContext db = new GTDoroContext();

        private string FULLNAME_TESTER = "Main Tester";
        private string EMAIL_ADDRESS_TESTER = "tester_123456@gtdoro.com";
        private string PASSWORD_TESTER = "Tester@123456";
        private string TIME_ZONE_TESTER = "GMT Standard Time";
        ApplicationUser appUser;
        IPrincipal user;

        private Project project;
        private Task task;
        private Action action;
        private CollectedThing collectedThing;

        WorkspaceController workspaceController;
        ProjectController projectController;
        TaskController taskController;
        ActionController actionController;
        CollectedThingController collectedThingController;

        private void CreateUser()
        {
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            appUser = new ApplicationUser { UserName = EMAIL_ADDRESS_TESTER, Email = EMAIL_ADDRESS_TESTER, FullName = FULLNAME_TESTER, TimeZoneId = TIME_ZONE_TESTER };
            var result = manager.Create(appUser, PASSWORD_TESTER);
            result = manager.SetLockoutEnabled(appUser.Id, false);
            result = manager.AddToRole(appUser.Id, "Admin");
            db.SaveChanges();

            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var genericIdentity = new GenericIdentity(EMAIL_ADDRESS_TESTER, "Forms");
            List<Claim> claims = new List<Claim>{
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", EMAIL_ADDRESS_TESTER), 
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", appUser.Id)
            };
            genericIdentity.AddClaims(claims);

            user = new GenericPrincipal(genericIdentity, new string[] { "Admin" });
        }

        private void DeleteUser()
        {
            ApplicationUser appUser = db.Users.Where(u => u.UserName == EMAIL_ADDRESS_TESTER).Include(u => u.Projects).FirstOrDefault();
            if (appUser != null)
            {
                db.CollectedThings.RemoveRange(db.CollectedThings.Where(ct => ct.User.Id == appUser.Id));
                db.Projects.RemoveRange(appUser.Projects);
                db.Users.Remove(appUser);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Inicializa los controladores, el usuario de prueba y la conexión con la BD
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            DeleteUser();
            CreateUser();

            workspaceController = new WorkspaceController();
            workspaceController.TestUser = user;

            projectController = new ProjectController();
            projectController.TestUser = user;

            taskController = new TaskController();
            taskController.TestUser = user;

            actionController = new ActionController();
            actionController.TestUser = user;

            collectedThingController = new CollectedThingController();
            collectedThingController.TestUser = user;
        }

        /// <summary>
        /// Elimina el usuario de prueba y desconecta la BD
        /// </summary>
        [TestCleanup] 
        public void Cleanup()
        {
            DeleteUser();
        }
        
        public Project CreateProject()
        {
            // Arrange

            //////////////////////////////////////////

            // Act
            Project project = new Project()
            {
                Code = "Test",
                Name = "Test Project"
            };
            ActionResult projectResult = workspaceController.CreateProject(project);

            //////////////////////////////////////////

            // Assert
            Assert.IsNotNull(projectResult);

            return project;
        }

        public Task CreateTask(int projectID)
        {
            // Arrange

            //////////////////////////////////////////

            // Act
            Task task = new Task()
            {
                Code = "Test",
                Name = "Test Task",
                ProjectID = projectID
            };
            ActionResult taskResult = projectController.CreateTask(task);

            //////////////////////////////////////////

            // Assert
            Assert.IsNotNull(taskResult);

            return task;
        }

        public Action CreateAction(int taskID)
        {
            // Arrange

            //////////////////////////////////////////

            // Act

            Action action = new Action()
            {
                Name = "Test Action",
                TaskID = taskID
            };
            ActionResult actionResult = taskController.CreateAction(action);

            //////////////////////////////////////////

            // Assert
            Assert.IsNotNull(actionResult);

            return action;
        }

        public CollectedThing CreateCollectedThing()
        {
            // Arrange

            //////////////////////////////////////////

            // Act

            CollectedThing collectedThing = new CollectedThing()
            {
                Name = "Test Collected Thing"
            };
            ActionResult actionResult = collectedThingController.CreateLight(collectedThing, string.Empty, string.Empty);

            //////////////////////////////////////////

            // Assert
            Assert.IsNotNull(actionResult);

            return collectedThing;
        }

        [TestMethod]
        public void DbItemCounts()
        {
            // Arrange

            int dbProjectCount = db.GetMyProjects(user).Count();
            int dbTaskCount = db.GetMyTasks(user).Count();
            int dbActionCount = db.GetMyActions(user).Count();

            //////////////////////////////////////////

            // Act

            //create project
            project = CreateProject();

            //create task
            task = CreateTask(project.ID);

            //create action
            action = CreateAction(task.ID);

            //////////////////////////////////////////

            // Assert

            Assert.AreEqual<int>(db.GetMyProjects(user).Count(), dbProjectCount + 1);
            Assert.AreEqual<int>(db.GetMyTasks(user).Count(), dbTaskCount + 1);
            Assert.AreEqual<int>(db.GetMyActions(user).Count(), dbActionCount + 1);
        }

        [TestMethod]
        public void DashboardActiveItemCounts()
        {
            // Arrange

            //dashboard (active) counts
            LayoutController lc = new LayoutController();
            lc.TestUser = user;
            DashboardViewModel dashboard = (DashboardViewModel)lc.Dashboard().ViewData.Model;

            int lProjectCount = dashboard.ActiveProjects.Count();
            int lTaskCount = dashboard.ActiveTasks.Count();
            int lActionCount = dashboard.ActiveActions.Count();

            //////////////////////////////////////////

            // Act

            //create project
            project = CreateProject();

            //create task
            task = CreateTask(project.ID);
            
            //create action
            action = CreateAction(task.ID);

            //////////////////////////////////////////

            //Assert

            Assert.AreEqual<int>(db.GetMyProjects(user).Count(), lProjectCount + 1);
            Assert.AreEqual<int>(db.GetMyTasks(user).Count(), lTaskCount + 1);
            Assert.AreEqual<int>(db.GetMyActions(user).Count(), lActionCount + 1);
        }

        [TestMethod]
        public void CreationDateTime()
        {   
            // Arrange

            DateTime dtBeforeCreatingProject = DateTime.Now;
            Thread.Sleep(500);

            //////////////////////////////////////////

            // Act

            //create project
            project = CreateProject();

            Thread.Sleep(500);
            DateTime dtAfterCreatingProject = DateTime.Now;
            Thread.Sleep(500);

            //create task
            task = CreateTask(project.ID);

            Thread.Sleep(500);
            DateTime dtAfterCreatingTask = DateTime.Now;
            Thread.Sleep(500);

            //create action
            action = CreateAction(task.ID);

            Thread.Sleep(500);
            DateTime dtAfterCreatingAction = DateTime.Now;

            //////////////////////////////////////////

            // Assert
            
            //project creation date
            Assert.IsTrue(project.CreationDate > dtBeforeCreatingProject);
            Assert.IsTrue(project.CreationDate < dtAfterCreatingProject);
            //task creation date
            Assert.IsTrue(task.CreationDate > dtAfterCreatingProject);
            Assert.IsTrue(task.CreationDate < dtAfterCreatingTask);
            //action creation date
            Assert.IsTrue(action.CreationDate > dtAfterCreatingTask);
            Assert.IsTrue(action.CreationDate < dtAfterCreatingAction);
        }
        
        [TestMethod]
        public void ProjectIndex()
        {
            // Arrange

            ProjectController projectController = new ProjectController();
            int projectDbCount = db.GetMyProjects(user).Count();

            //////////////////////////////////////////

            // Act

            projectController.TestUser = user;
            ViewResult result = projectController.Index(string.Empty, string.Empty, string.Empty, null);
            IEnumerable<Project> projects = result.Model as IEnumerable<Project>;

            //////////////////////////////////////////

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual<int>(projects.Count(), projectDbCount);
        }

        [TestMethod]
        public void TestStatusNew()
        {
            // Arrange 

            //////////////////////////////////////////

            // Act

            //create project
            project = CreateProject();
            //create task
            task = CreateTask(project.ID);
            //create action
            action = CreateAction(task.ID);

            //////////////////////////////////////////

            // Assert

            //project status
            Assert.IsTrue(project.Status == Status.Active);
            //task status
            Assert.IsTrue(task.Status == Status.Active);
            //action status
            Assert.IsTrue(action.Status == Status.Active);
        }


        [TestMethod]
        public void TestCreateCollectedThing()
        {
            // Arrange

            //dashboard (active) counts
            LayoutController lc = new LayoutController();
            lc.TestUser = user;
            ReviewViewModel review = (ReviewViewModel)lc.Review().ViewData.Model;

            int lCollectedThingsCount = review.CollectedThings.Items.Count();

            //////////////////////////////////////////

            // Act

            collectedThing = CreateCollectedThing();

            //////////////////////////////////////////

            // Assert

            review = (ReviewViewModel)lc.Review().ViewData.Model;
            Assert.AreEqual<int>(review.CollectedThings.Items.Count(), lCollectedThingsCount + 1);
        }
    }
}
