using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Admin()
        {
            using (InsuranceEntities db = new InsuranceEntities())
            {
                var insurees = db.Insurees;

                var insureesVM = new List<Insuree>();

                foreach (var insuree in insurees)
                {
                    var insureeVM = new Insuree();
                    insureeVM.FirstName = insuree.FirstName;
                    insureeVM.LastName = insuree.LastName;
                    insureeVM.EmailAddress = insuree.EmailAddress;
                    insureeVM.Quote = insuree.Quote;
                    insureesVM.Add(insuree);
                }

                return View(insureesVM);
            }
        }

            // GET: Insuree/Details/5
            public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType")] Insuree insuree)
        {

            using (InsuranceEntities db = new InsuranceEntities())
            {
                var monthlyBase = 50;
                DateTime currentTime = DateTime.Now;
                var totalQuote = new List<Insuree>();

                if (currentTime.Year - insuree.DateOfBirth.Year > 100 || currentTime.Year - insuree.DateOfBirth.Year < 25)
                {
                    monthlyBase += 25;
                }

                if (currentTime.Year - insuree.DateOfBirth.Year < 18)
                {
                    monthlyBase += 75;
                }

                if (insuree.CarYear < 2000 || insuree.CarYear > 2015)
                {
                    monthlyBase += 25;
                }

                if (insuree.CarMake == "porsche")
                {
                    monthlyBase += 25;
                }

                if (insuree.CarMake == "porsche" || insuree.CarMake == "Porsche" || insuree.CarMake == "PORSCHE" && insuree.CarModel == "carrera" || insuree.CarModel == "CARRERA" || insuree.CarModel == "Carrera")
                {
                    monthlyBase += 50;
                }

                if (insuree.SpeedingTickets > 0)
                {
                    monthlyBase = insuree.SpeedingTickets * 10 + monthlyBase;
                }
                                
                if (insuree.DUI == true)
                {
                    var duiAmount = insuree.Quote / 4;
                    insuree.Quote += duiAmount;
                }

                if (insuree.CoverageType == true)
                {
                    var fullCoverage = insuree.Quote / 2;
                    insuree.Quote += fullCoverage;
                }

                totalQuote.Add(insuree);
                insuree.Quote = monthlyBase;
            }
            if (ModelState.IsValid)
            {
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
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
