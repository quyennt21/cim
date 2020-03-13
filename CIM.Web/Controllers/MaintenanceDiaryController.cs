using AutoMapper;
using CIM.Common;
using CIM.Model.Models;
using CIM.Service;
using CIM.Web.Infrastructure;
using CIM.Web.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize(Roles = "System Admin,Manager")]
    public class MaintenanceDiaryController : BaseController
    {
        private IMaintenanceDiaryService _maintenanceDiaryService;

        private IAssetService _assetService;

        public MaintenanceDiaryController(IMaintenanceDiaryService maintenanceDiaryService, IAssetService assetService)
        {
            this._maintenanceDiaryService = maintenanceDiaryService;
            this._assetService = assetService;
        }

        // GET: MaintenanceDiary
        public ActionResult Index(int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var maintenanceDiariesModel = _maintenanceDiaryService.GetAllPaging(out totalRow, page, pageSize, new string[] { "Asset" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var maintenanceDiaryViewModel = Mapper.Map<IEnumerable<MaintenanceDiary>, IEnumerable<MaintenanceDiaryViewModel>>(maintenanceDiariesModel);

            var paginationSet = new PaginationSet<MaintenanceDiaryViewModel>()
            {
                Items = maintenanceDiaryViewModel,
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

        public ActionResult Search(string assetSearch, string fromDate, string toDate, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var maintenanceDiariesModel = _maintenanceDiaryService.Search(assetSearch, fromDate, toDate, out totalRow, page, pageSize, new string[] { "Asset" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var maintenanceDiaryViewModel = Mapper.Map<IEnumerable<MaintenanceDiary>, IEnumerable<MaintenanceDiaryViewModel>>(maintenanceDiariesModel);

            var paginationSet = new PaginationSet<MaintenanceDiaryViewModel>()
            {
                Items = maintenanceDiaryViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            ViewBag.assetSearch = assetSearch;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;

            ViewBag.query = new
            {
                assetSearch = assetSearch,
                fromDate = fromDate,
                toDate = toDate,
                page = page
            };

            return View("Index", paginationSet);
        }

        // GET: MaintenanceDiary/Details/5
        public ActionResult Details(int id)
        {
            var maintenanceDiaryModel = _maintenanceDiaryService.GetById(id, new string[] { "Asset" });

            if (maintenanceDiaryModel == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<MaintenanceDiary, MaintenanceDiaryViewModel>(maintenanceDiaryModel);

            return View(viewModel);
        }

        // GET: MaintenanceDiary/Create
        public ActionResult Create(int assetId)
        {
            var viewModel = new MaintenanceDiaryViewModel();

            var assetModel = _assetService.GetById(assetId, new string[] { "Area" });

            if (assetModel == null)
            {
                return HttpNotFound();
            }

            viewModel.Asset = Mapper.Map<Asset, AssetViewModel>(assetModel);
            viewModel.MaintenanceDate = DateTime.Now;
            ViewBag.isAreaOrLocation = false;
            ViewBag.assetId = assetId;
            if (assetModel.AssetCode.StartsWith("A") || assetModel.AssetCode.StartsWith("L"))
            {
                // get all asset in area or location;
                var assetsModel = _assetService.GetAllByArea(assetModel.ID, assetModel.Area.ID, new string[] { "Area" });

                ViewBag.assetsViewModel = Mapper.Map<IEnumerable<Asset>, IEnumerable<AssetViewModel>>(assetsModel);
                if(assetsModel.Count() > 0)
                {
                    ViewBag.isAreaOrLocation = true;
                }
            }

            return View(viewModel);
        }

        // POST: MaintenanceDiary/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAll(FormCollection form)
        {
            try
            {
                ViewBag.allAssetSelect = form["assetCheckedList"];
                ViewBag.currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.returnUrl = form["returnUrl"];

                if (!string.IsNullOrEmpty(form["dateMaintenance"]))
                {
                    var dateMaintenance = form["dateMaintenance"];

                    var allAssetSelect = form["assetCheckedList"] != null ? form["assetCheckedList"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList() : new List<int>();
                   
                    foreach (int assetId in allAssetSelect)
                    {
                        var maintenanceDiary = new MaintenanceDiary()
                        {
                            MaintenanceDate = DateTime.Parse(dateMaintenance),
                            Description = form["description"],
                            AssetID = assetId,
                            Active = true
                        };

                        _maintenanceDiaryService.Add(maintenanceDiary);
                        _maintenanceDiaryService.SaveChanges();
                    }
                    SetAlert("Add Maintenance Diary success", "success");
                    return RedirectToAction("Index");
                }
            }catch(Exception e)
            {
                SetAlert("Add Maintenance Diary error", "error");
            }
            return View();
        }

        // POST: MaintenanceDiary/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MaintenanceDiaryViewModel viewModel)
        {
            var maintenanceDiary = new MaintenanceDiary()
            {
                MaintenanceDate = viewModel.MaintenanceDate,
                Description = viewModel.Description,
                AssetID = viewModel.Asset.ID,
                Active = true
            };

            _maintenanceDiaryService.Add(maintenanceDiary);
            _maintenanceDiaryService.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: MaintenanceDiary/Edit/5
        public ActionResult Edit(int id)
        {
            var maintenanceDiaryModel = _maintenanceDiaryService.GetById(id);

            if (maintenanceDiaryModel == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<MaintenanceDiary, MaintenanceDiaryViewModel>(maintenanceDiaryModel);

            return View(viewModel);
        }

        // POST: MaintenanceDiary/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MaintenanceDiaryViewModel viewModel)
        {
            try
            {
                // TODO: Add update logic here
                var maintenanceDiary = _maintenanceDiaryService.GetById(viewModel.ID, new string[] { "Asset" });
                maintenanceDiary.MaintenanceDate = viewModel.MaintenanceDate;
                maintenanceDiary.Description = viewModel.Description;
                maintenanceDiary.Active = viewModel.Active;
                _maintenanceDiaryService.Update(maintenanceDiary);
                _maintenanceDiaryService.SaveChanges();
                SetAlert("Update Maintenance Diary success", "success");
            }
            catch(Exception e)
            {
                SetAlert("Update Maintenance Diary error", "error");
            }
            return RedirectToAction("Index");
        }

        // GET: MaintenanceDiary/Export
        public ActionResult Export()
        {
            var exportList = _maintenanceDiaryService.GetAll(new string[] { "Asset" });

            if (exportList != null)
            {
                ExcelPackage excelPackage = new ExcelPackage();

                ExcelWorksheet Sheet = excelPackage.Workbook.Worksheets.Add("Maintenance Report");

                // build header sheet
                Sheet.Cells["A1"].Value = "Asset ID";
                Sheet.Cells["B1"].Value = "Asset Code";
                Sheet.Cells["C1"].Value = "Asset Name";
                Sheet.Cells["D1"].Value = "Maintenance Date";
                Sheet.Cells["E1"].Value = "Description";

                int row = 2;
                foreach (var item in exportList)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Asset.ID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Asset.AssetCode;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Asset.Name;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.MaintenanceDate.ToShortDateString();
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Description;
                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();

                byte[] bytesInStream;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    excelPackage.SaveAs(memoryStream);
                    bytesInStream = memoryStream.ToArray();
                }

                return File(bytesInStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Maintenance_Report_" + DateTime.Now + ".xlsx");
            }

            return new EmptyResult();
        }
    }
}