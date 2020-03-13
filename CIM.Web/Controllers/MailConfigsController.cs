using AutoMapper;
using CIM.Common;
using CIM.Data;
using CIM.Model.Models;
using CIM.Service;
using CIM.Web.Infrastructure;
using CIM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize(Roles = "System Admin, Manager")]
    public class MailConfigsController : BaseController
    {
        private CIMDbContext db = new CIMDbContext();
        private IMailConfigService _mailConfigService;

        public MailConfigsController(IMailConfigService mailConfigService)
        {
            this._mailConfigService = mailConfigService;
        }

        // GET: MailConfigs
        public ActionResult Index(int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));
            int totalRow = 0;
            var mailConfigModel = _mailConfigService.GetAllPaging(out totalRow, page, pageSize);

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var mailConfigViewModels = Mapper.Map<IEnumerable<MailConfig>, IEnumerable<MailConfigViewModel>>(mailConfigModel);

            var paginationSet = new PaginationSet<MailConfigViewModel>()
            {
                Items = mailConfigViewModels,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };
            ViewBag.query = new
            {
                page = page
            };
            return View(paginationSet);
        }

        // GET: Search Location
        public ActionResult Search(string searchString, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("MaxSize"));
            int totalRow = 0;

            var mailConfigModel = _mailConfigService.Search(searchString, out totalRow, page, pageSize);

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var mailConfigViewModels = Mapper.Map<IEnumerable<MailConfig>, IEnumerable<MailConfigViewModel>>(mailConfigModel);

            var paginationSet = new PaginationSet<MailConfigViewModel>()
            {
                Items = mailConfigViewModels,
                MaxPage = int.Parse(ConfigHelper.GetKey("PageSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            ViewBag.searchString = searchString;

            ViewBag.query = new
            {
                searchString = searchString,
                page = page
            };

            return View("Index", paginationSet);
        }

        // GET: MailConfigs/Details/5
        public ActionResult Details(int id)
        {
            var mailConfigModel = _mailConfigService.GetById(id);

            if (mailConfigModel == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<MailConfig, MailConfigViewModel>(mailConfigModel);

            return View(viewModel);
        }

        // GET: MailConfigs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MailConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "ID,EmailAddress,Password,Port,Host,EnabledSSL,TemplateID,CreatedAt,UpdatedAt,Active")] MailConfig mailConfig)
        {
            try
            {
                string password=MailHelper.Encrypt(mailConfig.Password);

                var validateMail = _mailConfigService.GetAll().FirstOrDefault
                                    (x => x.EmailAddress == mailConfig.EmailAddress && x.ID != mailConfig.ID);
                if (validateMail != null)
                {
                    ModelState.AddModelError("EmailAddress", "Email Address already exists");
                }
                if (ModelState.IsValid)
                {
                    mailConfig.Password = password;
                    mailConfig.DateSend = DateTime.Now;
                    mailConfig.UpdatedAt = DateTime.Now;
                    mailConfig.CreatedAt = DateTime.Now;
                    mailConfig.EmailAddress = mailConfig.EmailAddress.ToLower();
                    db.MailConfig.Add(mailConfig);
                    db.SaveChanges();
                    SetAlert("Add Mail Config success", "success");
                    return RedirectToAction("Index");
                }
            }catch(Exception e)
            {
                SetAlert("Add Mail Config error", "error");
            }
            return View();
        }

        // GET: MailConfigs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailConfig mailConfig = db.MailConfig.Find(id);
            mailConfig.Password = MailHelper.Decrypt(mailConfig.Password);
            if (mailConfig == null)
            {
                return HttpNotFound();
            }
            return View(mailConfig);
        }

        // POST: MailConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EmailAddress,Password,Port,Host,EnabledSSL,TemplateID,CreatedAt,UpdatedAt,Active")] MailConfig mailConfig)
        {
            try
            {
                var validateName = _mailConfigService.GetAll().FirstOrDefault
                        (x => x.EmailAddress == mailConfig.EmailAddress && x.ID != mailConfig.ID);
                if (validateName != null)
                {
                    ModelState.AddModelError("EmailAddress", "Email Address already exists");
                }
                if (ModelState.IsValid)
                {
                    string password = MailHelper.Encrypt(mailConfig.Password);
                    mailConfig.Password = password;
                    mailConfig.UpdatedAt = DateTime.Now;
                    db.Entry(mailConfig).State = EntityState.Modified;
                    db.SaveChanges();
                    SetAlert("Update Mail Config success", "success");
                    return RedirectToAction("Index");
                }
            }catch(Exception e)
            {
                SetAlert("Update Mail Config error", "error");
            }
            return View(mailConfig);
        }

        // GET: MailConfigs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailConfig mailConfig = db.MailConfig.Find(id);
            if (mailConfig == null)
            {
                return HttpNotFound();
            }
            return View(mailConfig);
        }

        // POST: MailConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MailConfig mailConfig = db.MailConfig.Find(id);
            db.MailConfig.Remove(mailConfig);
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