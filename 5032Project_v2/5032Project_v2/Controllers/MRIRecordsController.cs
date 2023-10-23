using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _5032Project_v2.Models;

namespace _5032Project_v2.Controllers
{
    public class MRIRecordsController : Controller
    {
        private AppointmentModel db = new AppointmentModel();

        // GET: MRIRecords
        public ActionResult Index()
        {
            var mRIRecords = db.MRIRecords;
            return View(mRIRecords.ToList());
        }

        // GET: MRIRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MRIRecord mRIRecord = db.MRIRecords.Find(id);
            if (mRIRecord == null)
            {
                return HttpNotFound();
            }
            return View(mRIRecord);
        }

        // GET: MRIRecords/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MRIRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AppointmentId,Result")] MRIRecord mRIRecord)
        {
            if (ModelState.IsValid)
            {
                db.MRIRecords.Add(mRIRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mRIRecord);
        }

        // GET: MRIRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MRIRecord mRIRecord = db.MRIRecords.Find(id);
            if (mRIRecord == null)
            {
                return HttpNotFound();
            }
            return View(mRIRecord);
        }

        // POST: MRIRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AppointmentId,Result")] MRIRecord mRIRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mRIRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mRIRecord);
        }

        // GET: MRIRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MRIRecord mRIRecord = db.MRIRecords.Find(id);
            if (mRIRecord == null)
            {
                return HttpNotFound();
            }
            return View(mRIRecord);
        }

        // POST: MRIRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MRIRecord mRIRecord = db.MRIRecords.Find(id);
            db.MRIRecords.Remove(mRIRecord);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
