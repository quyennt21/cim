using AutoMapper;
using CIM.Common;
using CIM.Model.Models;
using CIM.Service;
using CIM.Web.Infrastructure;
using CIM.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize(Roles = "System Admin,Manager,Technician")]
    public class ReportController : BaseController
    {
        private IReportService _reportService;
        private IAssetService _assetService;
        private IMailConfigService _mailConfigService;
        private IMaintenanceDiaryService _maintenanceDiaryService;
        private IAssetLogService _assetLogService;
        private ApplicationUserManager applicationUserManager;

        //need using enum define status report!!!
        private enum StatusReport
        {
            [Description("Processing")]
            Processing = 2,
            [Description("Done")]
            Done = 3,
            [Description("Upwork")]
            Upwork = 4,
            [Description("Cancel")]
            Cancel = 5
        }

        public ReportController(IReportService reportService, IAssetService assetService, IMailConfigService mailConfigService, IMaintenanceDiaryService maintenanceDiaryService, IAssetLogService assetLogService, ApplicationUserManager applicationUserManager)
        {
            this._reportService = reportService;
            this._assetService = assetService;
            this._mailConfigService = mailConfigService;
            this._maintenanceDiaryService = maintenanceDiaryService;
            this._assetLogService = assetLogService;
            this.applicationUserManager = applicationUserManager;
        }

        // GET: Report
        public ActionResult Index(string searchString, int page = 1, int statusSearch = 0)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;
            var reportModel = _reportService.GetAllPaging(searchString, out totalRow, page, pageSize, statusSearch, new string[] { "Asset", "Rate", "Asset.Area", "Asset.Area.Location" });
            if (User.IsInRole("Technician"))
            {
                string user = User.Identity.GetUserName();
                reportModel = _reportService.GetAllPagingTechnician(user, searchString, out totalRow, page, pageSize, statusSearch, new string[] { "Asset", "Rate", "Asset.Area", "Asset.Area.Location" });
            }
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
            List<SelectListItem> listStatus = new List<SelectListItem>();
            listStatus.Add(new SelectListItem { Text = "--All--", Value = "0" });
            listStatus.Add(new SelectListItem { Text = "Opening", Value = "1" });
            listStatus.Add(new SelectListItem { Text = "Processing", Value = "2" });
            listStatus.Add(new SelectListItem { Text = "Done", Value = "3" });
            listStatus.Add(new SelectListItem { Text = "Upwork", Value = "4" });
            listStatus.Add(new SelectListItem { Text = "Cancel", Value = "5" });
            // Set ViewBag

            ViewBag.listStatus = listStatus;
            ViewBag.searchString = searchString;
            ViewBag.statusSearch = statusSearch;
            ViewBag.query = new
            {
                searchString = searchString,
                statusSearch = statusSearch,
                page = page
            };
            return View(paginationSet);
        }

        [AllowAnonymous]
        // GET: Reports/Create
        public ActionResult Create()
        {
            //ViewBag.assetID = asset.ID;
            ViewBag.isCreateSuccess = false;
            return View();
        }

        // POST: Reports/Create
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReportViewModel viewModel, HttpPostedFileBase file, string assetID)
        {
            string[] listAssetID = assetID.Split(',');
            List<string> listUserName = new List<string>();
            List<string> listMail = new List<string>();

            for (int i = 0; i < listAssetID.Length; i++)
            {
                int idAsset = Int32.Parse(listAssetID[i]);
                string name = _reportService.getNameUserReport(idAsset).FullName;
                string mail = _reportService.getNameUserReport(idAsset).Email;
                listUserName.Add(name);
                listMail.Add(mail);
            }
            List<String> listMailRecever = new List<string>();
            listMailRecever.AddRange(listMail.Distinct());
            var mailConfig = _mailConfigService.GetAll().FirstOrDefault();
            var listMailConfig = _mailConfigService.GetAll();

            string assetReport = _assetService.GetById(Int32.Parse(assetID)).Name;
            string areaAssetReport = _assetService.GetById(Int32.Parse(assetID), new string[] { "Area" }).Area.Name;
            string locationAssetReport = _assetService.GetById(Int32.Parse(assetID), new string[] { "Area", "Area.Location" }).Area.Location.Name;
            string userReport = User.Identity.GetUserName();

            var pathTemplate = HttpRuntime.AppDomainAppPath;
            string body = MailHelper.createContent(pathTemplate, "templates//", "TemplateEmail",
               new object[] { userReport, assetReport, locationAssetReport, areaAssetReport, viewModel.Comment, DateTime.Now });

            foreach (var mail in listMailConfig)
            {
                if (mail.DateSend != DateTime.Now.Date)
                {
                    mail.Count = 0;
                    mail.DateSend = DateTime.Now.Date;
                }
                if (mail.Count + listMailRecever.Count <= 2000)
                {
                    mail.Count += listMailRecever.Count;
                    foreach (var recever in listMailRecever)
                    {
                        MailHelper.SendMail(mailConfig.EmailAddress, mailConfig.Password, mailConfig.Host,
                            mailConfig.Port, mailConfig.EnabledSSL, recever, "Report asset", "cim", body);
                    }
                    break;
                }
                _mailConfigService.Update(mail);
            }
           
            //process image upload
            //set path default
            string fileName = "default.png";
            string path = Path.Combine(Server.MapPath("~/Data/Reports/"), fileName);
            try
            {
                if (file != null)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    if (string.Equals(fileExtension, ".jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(fileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                           || string.Equals(fileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                           || string.Equals(fileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        path = Path.Combine(Server.MapPath("~/Data/Reports/"), fileName);
                    }
                };
                file.SaveAs(path);
            }
            catch
            {
            }

            if (ModelState.IsValid)
            {
                var userName = User.Identity.GetUserName();
                if (string.IsNullOrEmpty(userName))
                {
                    userName = "Anonymous";
                }

                for (int i = 0; i < listAssetID.Length; i++)
                {
                    var report = new Report()
                    {
                        AssetID = Int32.Parse(listAssetID[i]),
                        Picture = fileName,
                        RequestManager = listUserName[i],
                        status = 1,
                        ReportAt = DateTime.Now,
                        Active = true,
                        Comment = "- " + viewModel.Comment,
                        CreatedAt = DateTime.Now,
                        UserReport = userName
                    };
                    _reportService.Add(report);
                    _reportService.SaveChanges();
                }
                ViewBag.isCreateSuccess = true;
                return View();
            }
            return RedirectToAction("Index", "MyReport");
        }

        // GET: Report/Details/5
        public ActionResult Details(int id)
        {
            var reportModel = _reportService.GetById(id, new string[] { "Asset" });

            if (reportModel == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<Report, ReportViewModel>(reportModel);

            return View(viewModel);
        }

        // GET: Report/Edit/5
        public ActionResult Edit(string idA)
        {
            if (idA == null)
            {
                return RedirectToAction("ListReportArea", "Report", new { id = 0 });
            }
            int id = 0;
            try
            {
                id = Convert.ToInt32(idA);
            }
            catch (Exception e)
            {
                id = Convert.ToInt32(idA.Substring(1, idA.Length - 1));
                ViewBag.checkReportArea = "true";
            }
            var reportModel = _reportService.GetById(id, new string[] { "Asset" });
            var viewModel = Mapper.Map<Report, ReportViewModel>(reportModel);

            if (reportModel == null)
            {
                return RedirectToAction("Details", "Asset", new { id = id });
            }
            else
            {
                viewModel.Comment = "";
            }

            List<SelectListItem> listStatus = new List<SelectListItem>();
            listStatus.Add(new SelectListItem { Text = "Processing", Value = "2" });
            listStatus.Add(new SelectListItem { Text = "Done", Value = "3" });
            listStatus.Add(new SelectListItem { Text = "Upwork", Value = "4" });
            listStatus.Add(new SelectListItem { Text = "Cancel", Value = "5" });
            // Set vào ViewBag
            ViewBag.listStatus = listStatus;
            return View(viewModel);
        }

        // POST: Report/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReportViewModel viewModel, string alternativeMaterials, string reportArea)
        {
            // TODO: Add update logic here
            var report = _reportService.GetById(viewModel.ID, new string[] { "Asset", "Asset.Area" });
            report.status = viewModel.status;
            if (viewModel.Comment != null)
            {
                report.Comment = report.Comment + "\n -> " + User.Identity.GetUserName() + ": " + viewModel.Comment;
            }
            report.UpdatedAt = DateTime.Now;
            _reportService.Update(report);
            _reportService.SaveChanges();

            if (viewModel.status == 3)
            {
                MaintenanceDiary maintenanceDiary = new MaintenanceDiary
                {
                    AssetID = report.Asset.ID,
                    MaintenanceDate = DateTime.Now,
                    Active = true,
                    Description = alternativeMaterials
                };
                _maintenanceDiaryService.Add(maintenanceDiary);
                _maintenanceDiaryService.SaveChanges();
            }

            AssetLog assetLog = new AssetLog
            {
                AssetID = report.Asset.ID,
                Active = true,
                CreatedAt = DateTime.Now,
                Message = "Report ID: " + report.ID + " at asset code: " + report.Asset.AssetCode + " Status: " + Enum.GetName(typeof(StatusReport), report.status),
                ApplicationUserID = applicationUserManager.FindByName(User.Identity.GetUserName()).Id
            };

            _assetLogService.Add(assetLog);
            _assetLogService.SaveChanges();

            if (reportArea.Equals("true"))
            {
                var listAssetInArea = _assetService.GetAllByAreaID(report.Asset.Area.ID);
                int idAssetArea = 0;
                foreach (var asset in listAssetInArea)
                {
                    if (asset.AssetCode.StartsWith("A"))
                    {
                        idAssetArea = asset.ID;
                        break;
                    }
                }
                return Redirect("~/Report/ListReportArea/" + idAssetArea);
            }
            else
            {
                SetAlert("Update Report success", "success");
                return RedirectToAction("Index");
            }
        }

        // GET: Report
        [AllowAnonymous]
        public ActionResult ListReport(int id)
        {
            Asset temp = _assetService.GetById(id);
            string assetCode = temp.AssetCode;
            List<Asset> assetModel = _assetService.GetAll().Where(a => a.Active).ToList();

            if (assetCode.StartsWith("A"))
            {
                assetModel = _assetService.GetAllAssetIntoArea(temp.ID, temp.AreaID).ToList();
                temp.Name = "Thiết bị khác";
                assetModel.Add(temp);

                List<int> listID = new List<int>();
                List<int> listIDReport = new List<int>();
                foreach (var a in assetModel)
                {
                    var report = _reportService.GetReportByConditions(a.ID, new string[] { "Asset" });
                    if (report != null)
                    {
                        listID.Add(a.ID);
                    }
                    else
                    {
                        listIDReport.Add(a.ID);
                    }
                }
                ViewBag.listIDReport = listIDReport;
                ViewBag.listAssetID = listID;
            }
            else
            {
                TempData["assetID"] = temp.ID;
                return RedirectToAction("Create");
            }

            var assetViewModel = Mapper.Map<IEnumerable<Asset>, IEnumerable<AssetViewModel>>(assetModel);

            //list pageSize
            List<SelectListItem> listStatus = new List<SelectListItem>();
            listStatus.Add(new SelectListItem { Text = "--All--", Value = "0" });
            listStatus.Add(new SelectListItem { Text = "Opening", Value = "1" });
            listStatus.Add(new SelectListItem { Text = "Processing", Value = "2" });
            listStatus.Add(new SelectListItem { Text = "Done", Value = "3" });
            listStatus.Add(new SelectListItem { Text = "Upwork", Value = "4" });
            listStatus.Add(new SelectListItem { Text = "Cancel", Value = "5" });
            // Set ViewBag
            ViewBag.listStatus = listStatus;
            ViewBag.isCreateSuccess = false;

            return View(assetViewModel);
        }

        [AllowAnonymous]
        public ActionResult ListReportArea(int id)
        {
            IEnumerable<ReportViewModel> reportModel = new List<ReportViewModel>();
            if (id != 0)
            {
                Asset asset = _assetService.GetById(id);
                asset.Name = "Thiết bị khác";
                List<Asset> assetModel = _assetService.GetAllAssetIntoArea(asset.ID, asset.AreaID).ToList();
                assetModel.Add(asset);
                var listAssetID = assetModel.Select(x => x.ID);
                List<Report> listReport = new List<Report>();
                foreach (var a in listAssetID)
                {
                    var report = _reportService.GetReportByConditions(a, new string[] { "Asset" });
                    if (report != null)
                    {
                        listReport.Add(report);
                    }
                }
                reportModel = Mapper.Map<IEnumerable<Report>, IEnumerable<ReportViewModel>>(listReport);
            }
            return View(reportModel);
        }

        // POST: Reports/Create
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListReport(ReportViewModel viewModel, HttpPostedFileBase file, FormCollection formval)
        {
            string listReport = formval["skill"];
            string comment = formval["Comment"];
            if (listReport == null)
            {
                listReport = viewModel.ID.ToString();
            }
            string[] listAssetID = listReport.Split(',');
            List<string> listUserName = new List<string>();
            List<string> listMail = new List<string>();

            string assetReport = "";
            string areaAssetReport = _assetService.GetById(viewModel.ID, new string[] { "Area" }).Area.Name;
            string locationAssetReport = _assetService.GetById(viewModel.ID, new string[] { "Area", "Area.Location" }).Area.Location.Name;
            string img = viewModel.Picture;

            var userReport = User.Identity.GetUserName();
            if (string.IsNullOrEmpty(userReport))
            {
                userReport = "Anonymous";
            }
            for (int i = 0; i < listAssetID.Length; i++)
            {
                int idAsset = Int32.Parse(listAssetID[i]);
                assetReport += _assetService.GetById(idAsset).Name + "; ";
                string name = _reportService.getNameUserReport(idAsset).FullName;
                listUserName.Add(name);
                string mail = _reportService.getNameUserReport(idAsset).Email;
                listMail.Add(mail);
            }

            List<String> listMailRecever = new List<string>();
            listMailRecever.AddRange(listMail.Distinct());
            var mailConfig = _mailConfigService.GetAll().FirstOrDefault();
            var listMailConfig = _mailConfigService.GetAll();

            var pathTemplate = HttpRuntime.AppDomainAppPath;

            string body = MailHelper.createContent(pathTemplate, "templates//", "TemplateEmail",
                new object[] { userReport, assetReport, locationAssetReport, areaAssetReport, viewModel.Comment, DateTime.Now });
            foreach (var mail in listMailConfig)
            {
                if (mail.DateSend != DateTime.Now.Date)
                {
                    mail.Count = 0;
                    mail.DateSend = DateTime.Now.Date;
                }
                if (mail.Count + listMailRecever.Count <= 2000)
                {
                    mail.Count += listMailRecever.Count;
                    foreach (var recever in listMailRecever)
                    {
                        MailHelper.SendMail(mailConfig.EmailAddress, mailConfig.Password, mailConfig.Host,
                            mailConfig.Port, mailConfig.EnabledSSL, recever, "Report asset", "cim", body);
                    }
                    break;
                }
                _mailConfigService.Update(mail);
            }

            //process upload image
            //set path default
            string fileName = "default.png";
            string path = Path.Combine(Server.MapPath("~/Data/Reports/"), fileName);
            try
            {
                if (file != null)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    if (string.Equals(fileExtension, ".jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(fileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                           || string.Equals(fileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                           || string.Equals(fileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        path = Path.Combine(Server.MapPath("~/Data/Reports/"), fileName);
                    }
                };

                file.SaveAs(path);
            }
            catch
            {
            }
            if (ModelState.IsValid)
            {
                var userName = User.Identity.GetUserName();
                if (string.IsNullOrEmpty(userName))
                {
                    userName = "Anonymous";
                }
                for (int i = 0; i < listAssetID.Length; i++)
                {
                    var report = new Report()
                    {
                        AssetID = Int32.Parse(listAssetID[i]),
                        Picture = fileName,
                        RequestManager = listUserName[i],
                        status = 1,
                        ReportAt = DateTime.Now,
                        Active = true,
                        Comment = "- " + viewModel.Comment,
                        CreatedAt = DateTime.Now,
                        UserReport = userName
                    };
                    _reportService.Add(report);
                    _reportService.SaveChanges();
                }
                ViewBag.isCreateSuccess = true;
                return View();
            }
            return RedirectToAction("Index", "MyReport");
        }

        public ActionResult EditSelect(string selectListItem)
        {
            ViewBag.selectListItem = selectListItem;

            if (!string.IsNullOrEmpty(selectListItem))
            {
                var userModel = applicationUserManager.Users.ToList();

                ViewBag.userViewModel = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(userModel);
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSelect(FormCollection form, string noteForward)
        {
            ViewBag.selectListItem = form["selectListItem"];

            var userModel = applicationUserManager.Users.ToList();

            ViewBag.userViewModel = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(userModel);
            var idUser = Convert.ToInt32(form["userId"]);

            //get mail recever
            string mailRecever = "";
            foreach (var user in userModel)
            {
                if (user.Id == idUser)
                {
                    mailRecever = user.Email;
                    break;
                }
            }

            var selectListItem = form["selectListItem"] != null ? form["selectListItem"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList() : new List<int>();
            var listReportForward = new List<Report>();
            foreach (int idReport in selectListItem)
            {
                var reportForward = _reportService.GetById(idReport, new string[] { "Asset", "Asset.Area" });
                reportForward.Comment += "\n -> Forward to: " + mailRecever;
                listReportForward.Add(reportForward);
                _reportService.Update(reportForward);
            }
            _reportService.SaveChanges();

            //content mail
            string content = "";
            int i = 0;
            listReportForward.OrderBy(r => r.Asset.Area.Name);
            foreach (var report in listReportForward)
            {
                i++;
                string comment = "";
                if (report.Comment.StartsWith("-"))
                {
                    comment = report.Comment.Substring(1, report.Comment.Length - 1);
                }
                else
                {
                    comment = report.Comment;
                }
                string[] listComment = comment.Split('-');
                content += i + ". <b>Area: </b>" + report.Asset.Area.Name +
                    "<br/><b>Asset: </b>" + report.Asset.Name +
                    "<br/><b>Comment: </b>" + listComment[0] + "<br/><p>";
            }

            //send mail forward
            var mailConfig = _mailConfigService.GetAll().FirstOrDefault();
            var listMailConfig = _mailConfigService.GetAll();
            var pathTemplate = HttpRuntime.AppDomainAppPath;
            if (!noteForward.Equals(""))
            {
                noteForward = "<b>Note: </b>" + noteForward;
            }
            string body = MailHelper.createContent(pathTemplate, "templates//", "TemplateMailForward",
                new object[] { content, noteForward, DateTime.Now });
            foreach (var mail in listMailConfig)
            {
                if (mail.DateSend != DateTime.Now.Date)
                {
                    mail.Count = 0;
                    mail.DateSend = DateTime.Now.Date;
                }
                if (mail.Count < 2000)
                {
                    mail.Count += 1;
                    MailHelper.SendMail(mailConfig.EmailAddress, mailConfig.Password, mailConfig.Host,
                        mailConfig.Port, mailConfig.EnabledSSL, mailRecever, "Report asset", "cim", body);
                    break;
                }
                _mailConfigService.Update(mail);
            }
            SetAlert("Forward Report success", "success");
            return RedirectToAction("Index");
        }
    }
}