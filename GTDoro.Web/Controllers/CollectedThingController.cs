using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GTDoro.Core.Models;
using GTDoro.Core.DAL;
using PagedList;
using GTDoro.Core.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using GTDoro.Web.Models.ActionFilters;

namespace GTDoro.Controllers
{
    [Authorize]
    public class CollectedThingController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();

        // GET: /CollectedThing/CreateLight
        public ActionResult CreateLight()
        {
            return View();
        }

        // POST: /CollectedThing/CreateLight
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLight([Bind(Include = "ID,Name")] CollectedThing collectedthing, string submitButton, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                //set owner
                UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                collectedthing.User = manager.FindById(User.Identity.GetUserId());

                //set creation date
                collectedthing.CreationDate = DateTime.UtcNow;

                db.CollectedThings.Add(collectedthing);
                db.SaveChanges();
            }
            UrlReferrer = UrlReferrer
                .Replace("?collecting=1", string.Empty)
                .Replace("&collecting=1", string.Empty);

            if (submitButton == "Save")
            {
                return Redirect(UrlReferrer);
            }
            //"Create more"
            return Redirect(UrlReferrer + (UrlReferrer.Contains("?") ? "&" : "?") + "collecting=1");            
        }

        // GET: /CollectedThing/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectedThing collectedthing = db.GetCollectedThingById(User, id.Value);
            if (collectedthing == null)
            {
                return HttpNotFound();
            }
            db.CollectedThings.Remove(collectedthing);
            db.SaveChanges();
            return RedirectToAction("Review", "Layout");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Private methods

        private void orderBySortParam(ref IQueryable<CollectedThing> collectedThings, string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    collectedThings = collectedThings.OrderByDescending(ct => ct.Name);
                    break;
                default: // name ascending
                    collectedThings = collectedThings.OrderBy(ct => ct.Name);
                    break;
            }
        }

        #endregion

        #region Disabled actions

        // GET: /CollectedThing/
        [DisabledActionFilter]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            //var tasks = db.Tasks.Include(t => t.Project);
            var collectedThings = db.GetMyCollectedThings(User);

            orderBySortParam(ref collectedThings, sortOrder);

            int pageSize = 25;
            int pageNumber = (page ?? 1);
            //return View(tasks.ToPagedList(pageNumber, pageSize));
            return View(collectedThings.ToPagedList(pageNumber, pageSize));
        }

        // GET: /CollectedThing/Details/5
        [DisabledActionFilter]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectedThing collectedthing = db.GetCollectedThingById(User, id.Value);
            if (collectedthing == null)
            {
                return HttpNotFound();
            }
            return View(collectedthing);
        }

        // GET: /CollectedThing/Create
        [DisabledActionFilter]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /CollectedThing/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisabledActionFilter]
        public ActionResult Create([Bind(Include = "ID,Name")] CollectedThing collectedthing)
        {
            if (ModelState.IsValid)
            {
                //set owner
                UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                collectedthing.User = manager.FindById(User.Identity.GetUserId());

                //set creation date
                collectedthing.CreationDate = DateTime.UtcNow;

                db.CollectedThings.Add(collectedthing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(collectedthing);
        }

        // GET: /CollectedThing/Edit/5
        [DisabledActionFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectedThing collectedthing = db.GetCollectedThingById(User, id.Value);
            if (collectedthing == null)
            {
                return HttpNotFound();
            }
            return View(collectedthing);
        }

        // POST: /CollectedThing/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisabledActionFilter]
        public ActionResult Edit([Bind(Include = "ID,Name,CreationDate")] CollectedThing collectedthing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collectedthing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = collectedthing.ID });
            }
            return View(collectedthing);
        }

        // POST: /CollectedThing/Delete/5
        [DisabledActionFilter]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, String UrlReferrer)
        {
            CollectedThing collectedthing = db.GetCollectedThingById(User, id);
            db.CollectedThings.Remove(collectedthing);
            db.SaveChanges();
            return Redirect(UrlReferrer);
        }
                

        #endregion
    }    
}
