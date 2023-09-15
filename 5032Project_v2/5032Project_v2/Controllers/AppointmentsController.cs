using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _5032Project_v2.Models;
using Microsoft.AspNet.Identity;

namespace _5032Project_v2.Controllers
{
    public class AppointmentsController : Controller
    {
        private AppointmentModel db = new AppointmentModel();

        private ApplicationDbContext _context;

        public AppointmentsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Appointments
        [Authorize]
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var appointments = db.Appointments.ToList();

                var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
                {
                    AppointmentId = a.AppointmentId,
                    DateTime = a.DateTime,
                    Status = a.Status,
                    FirstName = _context.Users.FirstOrDefault(u => u.Id == a.UserId)?.FirstName,
                    LastName = _context.Users.FirstOrDefault(u => u.Id == a.UserId)?.LastName
                    // add other properties if necessary
                }).ToList();

                return View(appointmentViewModels);
            }
            else
            {
                string currentUserId = User.Identity.GetUserId();
                var userAppointments = db.Appointments.Where(a => a.UserId == currentUserId).ToList();

                var userAppointmentViewModels = userAppointments.Select(a => new AppointmentViewModel
                {
                    AppointmentId = a.AppointmentId,
                    DateTime = a.DateTime,
                    Status = a.Status,
                    FirstName = _context.Users.FirstOrDefault(u => u.Id == a.UserId)?.FirstName,
                    LastName = _context.Users.FirstOrDefault(u => u.Id == a.UserId)?.LastName
                    // add other properties if necessary
                }).ToList();

                return View(userAppointmentViewModels);
            }
        }


        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "DateTime")] Appointment appointment)
        {

            appointment.UserId = User.Identity.GetUserId();
            ModelState.Clear();
            TryValidateModel(appointment);
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentId,UserId,DateTime,Status")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
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
