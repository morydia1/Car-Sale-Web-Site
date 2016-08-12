﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Car_Sale_Web_Site.Models;
using PagedList;

namespace Car_Sale_Web_Site.Controllers
{
    [ValidateInput(false)]
    public class PostCarsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult MyCars(int page = 1, int pageSize = 5)
        {
            List<PostCar> listCars = db.PostCar.Where(a => a.Author_UserName == User.Identity.Name).ToList();
            PagedList<PostCar> model = new PagedList<PostCar>(listCars, page, pageSize);
            return View(model);
        }

        // GET: PostCars
        public ActionResult Index(int page = 1,int pageSize=5)
        {
            List<PostCar> listCars = db.PostCar.ToList();
            PagedList<PostCar> model = new PagedList<PostCar>(listCars, page, pageSize);
            return View(model);
        }

        // GET: PostCars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostCar postCar = db.PostCar.Include(s => s.Files).SingleOrDefault(s => s.Id == id);
            if (postCar == null)
            {
                return HttpNotFound();
            }
            return View(postCar);
        }

        // GET: PostCars/Create
        [Authorize]

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CarModel,CarDescription,Town,Author_UserName,Price,Date,CategoryId,DoorId,Manufacturer,FuelId,GearId,YearId,HorsePower,ColorId,Climatronic,Climatic,Leather,ElWindows,ElMirrors,ElSeats,SeatsHeat,Audio,Retro,AllowWeels,DVDTV,Airbag,FourByFour,ABS,ESP,HallogenLights,NavigationSystem,SevenSeats,ASRTractionControl,Parktronic,Alarm,Imobilazer,CentralLocking,Insurance,Typetronic,Autopilot,TAXI,Computer,ServiceHistory,Tunning,BrandNew,SecondHand,Damaged")] PostCar model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var avatar = new File
                    {
                        FileName = System.IO.Path.GetFileName(upload.FileName),
                        FileType = FileType.Photo,
                        ContentType = upload.ContentType
                    };
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        avatar.Content = reader.ReadBytes(upload.ContentLength);
                    }
                    model.Files = new List<File> { avatar };
                }
                model.EditedDate = DateTime.Now;
                model.Author = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                model.Date = DateTime.Now;
                model.Author_UserName = User.Identity.Name;
                db.PostCar.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }


        // GET: PostCars/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostCar postCar = db.PostCar.Find(id);
            if (postCar == null)
            {
                return HttpNotFound();
            }
            var authors = db.Users.ToList();
            ViewBag.Authors = authors;
            return View(postCar);
        }

        // POST: PostCars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,CarModel,CarDescription,Town,Author_UserName,Price,Date,CategoryId,DoorId,Manufacturer,FuelId,GearId,YearId,HorsePower,ColorId,Climatronic,Climatic,Leather,ElWindows,ElMirrors,ElSeats,SeatsHeat,Audio,Retro,AllowWeels,DVDTV,Airbag,FourByFour,ABS,ESP,HallogenLights,NavigationSystem,SevenSeats,ASRTractionControl,Parktronic,Alarm,Imobilazer,CentralLocking,Insurance,Typetronic,Autopilot,TAXI,Computer,ServiceHistory,Tunning,BrandNew,SecondHand,Damaged")] PostCar postCar)
        {
            if (ModelState.IsValid)
            {
                postCar.AuthorId = db.PostCar.Find(postCar.Id).AuthorId;
                postCar.Author_UserName = User.Identity.Name;
                postCar.Date = db.PostCar.Find(postCar.Id).Date;
                postCar.EditedDate = DateTime.Now;
                var local = db.Set<PostCar>().Local.FirstOrDefault(f => f.Id == postCar.Id);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(postCar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyCars");
            }
            return View(postCar);
        }

        // GET: PostCars/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostCar postCar = db.PostCar.Find(id);
            if (postCar == null)
            {
                return HttpNotFound();
            }
            return View(postCar);
        }

        // POST: PostCars/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PostCar postCar = db.PostCar.Find(id);
            db.PostCar.Remove(postCar);
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
