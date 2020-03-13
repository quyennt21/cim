using AutoMapper;
using CIM.Common;
using CIM.Model.Models;
using CIM.Service;
using CIM.Web.Infrastructure;
using CIM.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize]
    public class MyReportController : Controller
    {
        private IReportService _reportService;
        private IAssetService _assetService;
        private IRateService _rateService;

        public MyReportController(IReportService reportService, IAssetService assetService, IRateService rateService)
        {
            this._reportService = reportService;
            this._assetService = assetService;
            this._rateService = rateService;
        }

        // GET: MyReport
        public ActionResult Index(int status = 0, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            string userReport = User.Identity.GetUserName();

            var reportModel = _reportService.GetAllByUserReport(userReport, null, out totalRow, page, pageSize, status, new string[] { "Asset", "Rate" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var reportViewModel = Mapper.Map<IEnumerable<Report>, IEnumerable<ReportViewModel>>(reportModel);
            var paginationSet = new PaginationSet<ReportViewModel>()
            {
                Items = reportViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            //list pageSize
            List<SelectListItem> listStatus = new List<SelectListItem>()
            {
                new SelectListItem { Text = "--All--", Value = "0" },
                new SelectListItem { Text = "Opening", Value = "1" },
                new SelectListItem { Text = "Processing", Value = "2" },
                new SelectListItem { Text = "Done", Value = "3" },
                new SelectListItem { Text = "Upwork", Value = "4" },
                new SelectListItem { Text = "Cancel", Value = "5" }
            };

            ViewBag.listStatus = listStatus;

            ViewBag.query = new
            {
                status = status,
                page = page
            };

            return View(paginationSet);
        }

        // GET: MyReport/Rate
        public ActionResult Rate(int reportId)
        {
            ViewBag.reportId = reportId;

            return View();
        }

        // POST: MyReport/Rate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rate(FormCollection form)
        {
            if (!string.IsNullOrEmpty(form["reportId"]))
            {
                if (Decimal.Parse(form["rating"]) <= 3)
                {
                    if (string.IsNullOrEmpty(form["comment"]))
                    {
                        ModelState.AddModelError("Comment", "The Comment field is required.");
                        return View();
                    }
                }
                Rate rate = new Rate
                {
                    ReportID = Int16.Parse(form["reportId"]),
                    Value = Decimal.Parse(form["rating"]),
                    Comment = form["comment"]
                };

                _rateService.Add(rate);
                _rateService.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}