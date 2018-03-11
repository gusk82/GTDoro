using GTDoro.Controllers;
using GTDoro.Core.DAL;
using GTDoro.Core.Models;
using GTDoro.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GTDoro.Web.Controllers
{
    [Authorize]
    public class TimePeriodController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: /TimePeriod/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimePeriod pomodoro = db.GetTimePeriodById(User, id.Value);
            if (pomodoro == null)
            {
                return HttpNotFound();
            }
            db.TimePeriods.Remove(pomodoro);
            db.SaveChanges();
            return Redirect(Request.GetReferrerUrlOrCurrent());
        }


        // GET: /TimePeriod/ChangeAction/5?parent=1
        public ActionResult ChangeAction(int? id, int? parent)
        {
            if (id == null || parent.HasValue == false)
            {
                return Redirect(Request.GetReferrerUrlOrCurrent());
            }
            TimePeriod timePeriod = db.GetTimePeriodById(User, id.Value);
            if (timePeriod == null)
            {
                return HttpNotFound();
            }
            if (timePeriod.Status == TimePeriodStatus.Working)
            {
                //cannot perform change
                TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
                return Redirect(Request.GetReferrerUrlOrCurrent());
            }
            Activity action = db.GetActivityById(User, parent.Value);
            if (action == null)
            {
                return HttpNotFound();
            }

            timePeriod.ActivityID = parent.Value;
            db.Entry(timePeriod).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
            return Redirect(Request.GetReferrerUrlOrCurrent());
        }
    }
}