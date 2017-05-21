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
using Microsoft.AspNet.Identity;
using GTDoro.Core.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using GTDoro.Web.Models.ActionFilters;

namespace GTDoro.Controllers
{
    [Authorize]
    public class TagController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();

        // GET: /Task/
        [DisabledActionFilter]
        public ViewResult Index()
        {          
            var tags = db.GetMyTags(User).ToList();

            return View(tags);
        }

        // GET: /Tag/Details/5
        [DisabledActionFilter]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tag tag = db.GetTagById(User, id.Value);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View(tag);
        }

        // GET: /Tag/Create
        [DisabledActionFilter]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Tag/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisabledActionFilter]
        public ActionResult Create([Bind(Include="ID,Code,Name,Description")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                //set owner
                UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                tag.User = manager.FindById(User.Identity.GetUserId());

                //set creation date
                tag.CreationDate = DateTime.UtcNow;

                db.Tags.Add(tag);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tag);
        }

        // GET: /Tag/Edit/5
        [DisabledActionFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tag tag = db.GetTagById(User, id.Value);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View(tag);
        }

        // POST: /Tag/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisabledActionFilter]
        public ActionResult Edit([Bind(Include="ID,Code,Name,Description")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tag).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = tag.ID });
            }
            return View(tag);
        }

        // GET: /Tag/Delete/5
        [DisabledActionFilter]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tag tag = db.GetTagById(User, id.Value);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View(tag);
        }

        // POST: /Tag/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [DisabledActionFilter]
        public ActionResult DeleteConfirmed(int id, String UrlReferrer)
        {
            Tag tag = db.GetTagById(User, id);
            db.Tags.Remove(tag);
            db.SaveChanges();
            return Redirect(UrlReferrer);
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

        private void orderBySortParam(ref IQueryable<Tag> tags, string sortOrder)
        {
            switch (sortOrder)
            {
                case "code_desc":
                    tags = tags.OrderByDescending(c => c.Code);
                    break;
                case "name":
                    tags = tags.OrderBy(c => c.Name);
                    break;
                case "name_desc":
                    tags = tags.OrderByDescending(c => c.Name);
                    break;
                case "task":
                    tags = tags.OrderBy(c => c.Tasks.Count);
                    break;
                case "task_desc":
                    tags = tags.OrderByDescending(c => c.Tasks.Count);
                    break;
                default: // code ascending
                    tags = tags.OrderBy(c => c.Code);
                    break;
            }
        }

        #endregion
    }
}
