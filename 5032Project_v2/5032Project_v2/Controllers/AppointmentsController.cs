using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using _5032Project_v2.Models;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Globalization;
using OfficeOpenXml;

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
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var appointments = db.Appointments.ToList();

                var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
                {
                    AppointmentId = a.AppointmentId,
                    DateTime = a.DateTime,
                    FormattedDateTime = FormatAppointmentTime(a.DateTime),
                    Status = a.Status,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    ClinicName = db.Clinics.FirstOrDefault(c => c.Id == a.ClinicId)?.ClinicName,
                    FeedbackRating = a.FeedbackRating
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
                    FormattedDateTime = FormatAppointmentTime(a.DateTime),
                    Status = a.Status,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    ClinicName = db.Clinics.FirstOrDefault(c => c.Id == a.ClinicId)?.ClinicName
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

            var appointment = db.Appointments.FirstOrDefault(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return HttpNotFound();
            }

            // Fetch the clinic name using the ClinicId from the appointment
            var clinicName = db.Clinics.Where(c => c.Id == appointment.ClinicId)
                                       .Select(c => c.ClinicName)
                                       .FirstOrDefault();
            var mriRecords = db.MRIRecords.Where(m => m.AppointmentId == appointment.AppointmentId).ToList();

            // Create a new ViewModel and populate it

            var viewModel = new AppointmentDetailsViewModel
            {
                Appointment = appointment,
                ClinicName = clinicName,
                MRIRecords = mriRecords
            };

            return View(viewModel);
        }

        public ActionResult ViewFeedback(int? id) {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var appointment = db.Appointments.FirstOrDefault(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return HttpNotFound();
            }

            // Fetch the clinic name using the ClinicId from the appointment
            var clinicName = db.Clinics.Where(c => c.Id == appointment.ClinicId)
                                       .Select(c => c.ClinicName)
                                       .FirstOrDefault();
            var mriRecords = db.MRIRecords.Where(m => m.AppointmentId == appointment.AppointmentId).ToList();

            // Create a new ViewModel and populate it

            var viewModel = new AppointmentDetailsViewModel
            {
                Appointment = appointment,
                ClinicName = clinicName,
                MRIRecords = mriRecords
            };

            return View(viewModel);
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
        public ActionResult Create([Bind(Include = "FirstName,LastName,DateTime,ClinicId")] Appointment appointment)
        {
            try
            {
                if (appointment.DateTime != null)
                {
                    DateTime parsedDate = DateTime.ParseExact(appointment.DateTime.ToString("dd/MM/yyyy HH:00"), "dd/MM/yyyy HH:00", CultureInfo.InvariantCulture);
                    appointment.DateTime = parsedDate;
                }
                // Check if a clinic was selected
                if (appointment.ClinicId == 0)
                {
                    ModelState.AddModelError("ClinicId", "A clinic must be selected.");
                }

                // Check if the selected clinic exists in the database
                if (!db.Clinics.Any(c => c.Id == appointment.ClinicId))
                {
                    ModelState.AddModelError("ClinicId", "The selected clinic does not exist.");
                }

                if (db.Appointments.Any(a => a.DateTime == appointment.DateTime && a.ClinicId == appointment.ClinicId))
                {
                    ModelState.AddModelError("DateTime", "This time slot is already booked for this clinic.");
                }


                // Set the UserId for the appointment
                appointment.UserId = User.Identity.GetUserId();

                if (ModelState.IsValid)
                {
                    db.Appointments.Add(appointment);
                    db.SaveChanges();

                    // Add a success message for the user
                    TempData["SuccessMessage"] = "Appointment created successfully!";
                    return RedirectToAction("Index");
                }

                // If ModelState is invalid, return the appointment model to the view
                // so that validation errors can be displayed to the user.
                return View(appointment);
            }
            catch (Exception ex)
            {
                // Log the exception details
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                // Replace the below line with a proper logging mechanism in your actual application
                System.Diagnostics.Debug.WriteLine(message);

                // Return an error message to the user
                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again.";
                return View("Error");  // Assuming you have an Error view
            }
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

            var mriRecords = db.MRIRecords.Where(m => m.AppointmentId == appointment.AppointmentId).ToList();
            var user = _context.Users.FirstOrDefault(u => u.Id == appointment.UserId);

            // Convert appointment to ViewModel if you are using one, and pass it to the view
            AppointmentViewModel viewModel = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                DateTime = appointment.DateTime,
                Status = appointment.Status,
                FirstName = appointment.FirstName,
                LastName = appointment.LastName,
                MRIRecords = mriRecords
            };

            return View(viewModel);
        }



        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AppointmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the original Appointment from the database.
                var appointmentToUpdate = db.Appointments.Find(viewModel.AppointmentId);
                if (appointmentToUpdate == null)
                {
                    // Handle not found situation
                    return HttpNotFound();
                }

                // Update the properties of the original Appointment object.
                appointmentToUpdate.DateTime = viewModel.DateTime;
                appointmentToUpdate.Status = viewModel.Status;
                // ... Map any other necessary properties

                // Tell Entity Framework the entity has been modified and save the changes.
                db.Entry(appointmentToUpdate).State = EntityState.Modified;
                db.SaveChanges();

                // Redirect to the Index action after updating.
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var appointment = db.Appointments.FirstOrDefault(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return HttpNotFound();
            }

            // Fetch the clinic name using the ClinicId from the appointment
            var clinicName = db.Clinics.Where(c => c.Id == appointment.ClinicId)
                                       .Select(c => c.ClinicName)
                                       .FirstOrDefault();
            var mriRecords = db.MRIRecords.Where(m => m.AppointmentId == appointment.AppointmentId).ToList();

            // Create a new ViewModel and populate it

            var viewModel = new AppointmentDetailsViewModel
            {
                Appointment = appointment,
                ClinicName = clinicName,
                MRIRecords = mriRecords
            };

            return View(viewModel);
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

        public ActionResult AddMRIRecord(int appointmentId)
        {
            var viewModel = new AddMRIRecordViewModel
            {
                AppointmentId = appointmentId
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SaveMRIRecord(AddMRIRecordViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(viewModel.MRIAttachment.FileName);
                    var path = Path.Combine(Server.MapPath("~/Records"), fileName);
                    viewModel.MRIAttachment.SaveAs(path);

                    var mriRecord = new MRIRecord
                    {
                        AppointmentId = viewModel.AppointmentId,
                        Result = viewModel.Result,
                        FilePath = path,
                        FileName = fileName
                    };

                    db.MRIRecords.Add(mriRecord);
                    db.SaveChanges();

                    return RedirectToAction("Index", "Appointments");
                }

                return View("AddMRIRecord", viewModel);
            }
            catch (Exception ex)
            {
                // Ideally log the error and then provide feedback to the user.
                ViewBag.ErrorMessage = "An error occurred while adding the MRI record.";
                return View("AddMRIRecord", viewModel);
            }
        }

        [HttpPost, ActionName("DeleteMRIRecord")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMRIRecordConfirmed(int id)
        {
            MRIRecord mriRecord = db.MRIRecords.Find(id);
            int appointmentId = mriRecord.AppointmentId;
            db.MRIRecords.Remove(mriRecord);
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = appointmentId });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SendEmail()
        {
            var appointments = db.Appointments.ToList();

            var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                DateTime = a.DateTime,
                Status = a.Status,
                FirstName = a.FirstName,
                LastName = a.LastName
                // add other properties if necessary
            }).ToList();

            return View(appointmentViewModels);
        }

        public async Task<ActionResult> SendEmails(List<int> appointmentIds)
        {
            try
            {
                if (!appointmentIds.Any())
                {
                    return Json(new { success = false, message = "No appointments selected." }, JsonRequestBehavior.AllowGet);
                }

                    foreach (int id in appointmentIds)
                {
                    var appointment = db.Appointments.Find(id);
                    if (appointment == null) continue;

                    var user = _context.Users.FirstOrDefault(u => u.Id == appointment.UserId);
                    if (user == null) continue;

                    var mriRecords = db.MRIRecords.Where(m => m.AppointmentId == appointment.AppointmentId).ToList();

                    await SendAppointmentEmail(user.Email, appointment, mriRecords);
                }

                return Json(new { success = true, message = "Emails sent successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception details
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                // Log this message using your logging mechanism
                return Json(new { success = false, message = "An error occurred while sending emails." }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task SendAppointmentEmail(string userEmail, Appointment appointment, List<MRIRecord> mriRecords)
        {
            var apiKey = "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"; // Store this securely! Don't hardcode it.
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("rainrhb@gmail.com", "MRI Mission Hub");
            var to = new EmailAddress(userEmail);
            var subject = $"Update on Your Appointment - {appointment.DateTime}";
            string statusDescription;

            switch (appointment.Status)
            {
                case 0:
                    statusDescription = "Waiting for approval";
                    break;
                case 1:
                    statusDescription = "Approved";
                    break;
                case 2:
                    statusDescription = "Waiting for the imaging";
                    break;
                case 3:
                    statusDescription = "Jobs has been completed";
                    break;
                default:
                    statusDescription = "Rejected";
                    break;
            }

            var plainTextContent = $"Status of your appointment on {appointment.DateTime}: {statusDescription}";
            var htmlContent = $"<strong>Status of your appointment on {appointment.DateTime}: {statusDescription}</strong>";


            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            // Attach MRI Records if they exist
            foreach (var record in mriRecords)
            {
                var bytes = System.IO.File.ReadAllBytes(record.FilePath);
                var fileContent = Convert.ToBase64String(bytes);
                msg.AddAttachment(record.FileName, fileContent);
            }

            var response = await client.SendEmailAsync(msg);
            // Optionally log the response or handle failures
        }

        [HttpPost]
        public ActionResult IsTimeSlotAvailable(DateTime dateTime, int clinicId)
        {
            bool isAvailable = !db.Appointments.Any(a => a.DateTime == dateTime && a.ClinicId == clinicId);
            return Json(isAvailable);
        }
        public static string FormatAppointmentTime(DateTime dateTime)
        {
            string datePart = dateTime.ToString("dd/MM/yyyy");
            string startTime = dateTime.ToString("h tt");
            string endTime = dateTime.AddHours(1).ToString("h tt");
            return $"{datePart} {startTime} to {endTime}";
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexUpcoming()
        {
            var appointments = db.Appointments.Where(a => a.Status == 0).ToList();

            var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                DateTime = a.DateTime,
                FormattedDateTime = FormatAppointmentTime(a.DateTime),
                Status = a.Status,
                FirstName = a.FirstName,
                LastName = a.LastName,
                ClinicName = db.Clinics.FirstOrDefault(c => c.Id == a.ClinicId)?.ClinicName
            }).ToList();

            return View(appointmentViewModels);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexProcessing()
        {
            var appointments = db.Appointments.Where(a => a.Status == 1 || a.Status == 2).ToList();
            var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                DateTime = a.DateTime,
                FormattedDateTime = FormatAppointmentTime(a.DateTime),
                Status = a.Status,
                FirstName = a.FirstName,
                LastName = a.LastName,
                ClinicName = db.Clinics.FirstOrDefault(c => c.Id == a.ClinicId)?.ClinicName
            }).ToList();

            return View(appointmentViewModels);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexFinished()
        {
            var appointments = db.Appointments.Where(a => a.Status == 3).ToList();
            var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                DateTime = a.DateTime,
                FormattedDateTime = FormatAppointmentTime(a.DateTime),
                Status = a.Status,
                FirstName = a.FirstName,
                LastName = a.LastName,
                ClinicName = db.Clinics.FirstOrDefault(c => c.Id == a.ClinicId)?.ClinicName
            }).ToList();

            return View(appointmentViewModels);
        }

        public ActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var appointments = db.Appointments.ToList();

            var userAppointmentViewModels = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                DateTime = a.DateTime,
                FormattedDateTime = FormatAppointmentTime(a.DateTime), // Ensure this method is present in the controller
                Status = a.Status,
                FirstName = a.FirstName,
                LastName = a.LastName,
                ClinicName = db.Clinics.FirstOrDefault(c => c.Id == a.ClinicId)?.ClinicName
            }).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Appointments");
                worksheet.Cells[1, 1].Value = "User Name";
                worksheet.Cells[1, 2].Value = "Date Time";
                worksheet.Cells[1, 3].Value = "Clinic Selected";
                worksheet.Cells[1, 4].Value = "Status";

                int rowNumber = 2;
                foreach (var appointment in userAppointmentViewModels)
                {
                    worksheet.Cells[rowNumber, 1].Value = appointment.FirstName + " " + appointment.LastName;
                    worksheet.Cells[rowNumber, 2].Value = appointment.FormattedDateTime;
                    worksheet.Cells[rowNumber, 3].Value = appointment.ClinicName;
                    worksheet.Cells[rowNumber, 4].Value = GetStatusText(appointment.Status);
                    rowNumber++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);

                string excelName = $"AppointmentsListExport.xlsx";

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        private string GetStatusText(int status)
        {
            switch (status)
            {
                case 0: return "Waiting for approval";
                case 1: return "Approved";
                case 2: return "Waiting for the imaging";
                case 3: return "Jobs has been completed";
                default: return "Rejected";
            }
        }
        [Authorize(Roles = "Patient")]
        public ActionResult LeavingFeedback(int id)
        {
            var appointment = db.Appointments.FirstOrDefault(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return HttpNotFound();
            }

            // Fetch the clinic name using the ClinicId from the appointment
            var clinicName = db.Clinics.Where(c => c.Id == appointment.ClinicId)
                                       .Select(c => c.ClinicName)
                                       .FirstOrDefault();
            var mriRecords = db.MRIRecords.Where(m => m.AppointmentId == appointment.AppointmentId).ToList();

            // Create a new ViewModel and populate it

            var viewModel = new AppointmentDetailsViewModel
            {
                Appointment = appointment,
                ClinicName = clinicName,
                MRIRecords = mriRecords
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Patient")]
        [HttpPost]
        public ActionResult SaveFeedback(int rating, int appointmentId, string feedbackComment)
        {
            try
            {
                var appointment = db.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
                if (appointment == null)
                {
                    return HttpNotFound("Appointment not found!");
                }

                appointment.FeedbackRating = rating;
                appointment.FeedbackComment = feedbackComment;
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                TempData["FeedbackMessage"] = "Thank you for your feedback!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["FeedbackMessage"] = "There was a problem saving your feedback. Please try again.";
                return RedirectToAction("Details", new { id = appointmentId });
            }
        }

        [HttpGet]
        public ActionResult GetClinics(string searchTerm = "")
        {
            // Assuming you have a DbContext called "db" and a Clinics DbSet
            var clinics = db.Clinics
                            .Where(c => c.ClinicName.Contains(searchTerm))
                            .Select(c => new
                            {
                                Id = c.Id,
                                ClinicName = c.ClinicName,
                                Latitude = c.Latitude,
                                Longitude = c.Longitude
                            })
                            .ToList();

            return Json(clinics, JsonRequestBehavior.AllowGet);
        }


    }
}
