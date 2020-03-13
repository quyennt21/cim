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
    public class WarrantyController : BaseController
    {
        private IWarrantyService _warrantyService;
        private IAssetService _assetService;

        public WarrantyController(IWarrantyService warrantyService, IAssetService assetService)
        {
            this._warrantyService = warrantyService;
            this._assetService = assetService;
        }

        // GET: Warranty
        public ActionResult Index(int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var warrantiesModel = _warrantyService.GetAllPaging(out totalRow, page, pageSize, new string[] { "Asset" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var warrantyViewModel = Mapper.Map<IEnumerable<Warranty>, IEnumerable<WarrantyViewModel>>(warrantiesModel);

            var paginationSet = new PaginationSet<WarrantyViewModel>()
            {
                Items = warrantyViewModel,
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

        // GET: Warranty
        public ActionResult Search(string assetSearch, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var warrantiesModel = _warrantyService.Search(assetSearch, out totalRow, page, pageSize, new string[] { "Asset" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var warrantyViewModel = Mapper.Map<IEnumerable<Warranty>, IEnumerable<WarrantyViewModel>>(warrantiesModel);

            var paginationSet = new PaginationSet<WarrantyViewModel>()
            {
                Items = warrantyViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            ViewBag.assetSearch = assetSearch;

            ViewBag.query = new
            {
                assetSearch = assetSearch,
                page = page
            };

            return View("Index", paginationSet);
        }

        // GET: Warranty/Details/5
        public ActionResult Details(int id)
        {
            var warrantyModel = _warrantyService.GetById(id, new string[] { "Asset" });

            if (warrantyModel == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<Warranty, WarrantyViewModel>(warrantyModel);

            return View(viewModel);
        }

        // GET: Warranty/Create
        public ActionResult Create(int assetId)
        {
            var viewModel = new WarrantyViewModel();
            var assetModel = _assetService.GetById(assetId);

            if (assetModel == null)
            {
                return HttpNotFound();
            }
            viewModel.Asset = Mapper.Map<Asset, AssetViewModel>(assetModel);

            return View(viewModel);
        }

        // POST: Warranty/Create
        [HttpPost]
        public ActionResult Create(WarrantyViewModel viewModel)
        {
            try
            {
                int endYear =Convert.ToInt32(viewModel.EndDate.Year.ToString());
                int warrantyYear = Convert.ToInt32(viewModel.DateWarranty.Year.ToString());               
                if (String.IsNullOrEmpty(viewModel.Reason) || String.IsNullOrEmpty(viewModel.Result)||endYear<1970||warrantyYear<1970)
                {
                    return View(viewModel);
                }
                DateTime endD = Convert.ToDateTime(viewModel.EndDate.ToString());
                DateTime warrantyD  = Convert.ToDateTime(viewModel.DateWarranty.ToString());
                int compareDate = DateTime.Compare(endD, warrantyD);
                if (compareDate <= 0)
                {
                    ModelState.AddModelError("EndDate", "EndDate must have a date greater than DateWarranty");
                    return View(viewModel);
                }
                else
                {
                    var warranty = new Warranty()
                    {
                        Reason = viewModel.Reason.Trim(),
                        EndDate = viewModel.EndDate,
                        DateWarranty = viewModel.DateWarranty,
                        Result = viewModel.Result.Trim(),
                        AssetID = viewModel.Asset.ID,
                        Active = true
                    };

                    _warrantyService.Add(warranty);
                    _warrantyService.SaveChanges();
                    SetAlert("Add Warranty success", "success");
                }

            }
            catch(Exception e)
            {
                SetAlert("Add Warranty error", "error");
            }
            return RedirectToAction("Index");
        }

        //GET: Warranty/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    var warrantyViewModel = _warrantyService.GetById(id, new string[] { "Asset" });

        //    if (warrantyViewModel == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    var viewModel = Mapper.Map<Warranty, WarrantyViewModel>(warrantyViewModel);

        //    return View(viewModel);
        //}

        //POST: Warranty/Edit/5
        //[HttpPost]
        //public ActionResult Edit(WarrantyViewModel viewModel)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            xy ly loi
        //        }
        //    TODO: Add update logic here
        //        var warranty = _warrantyService.GetById(viewModel.ID, new string[] { "Asset" });
        //        warranty.EndDate = viewModel.EndDate;
        //        warranty.DateWarranty = viewModel.DateWarranty;
        //        warranty.Result = viewModel.Result;
        //        warranty.Reason = viewModel.Reason;
        //        warranty.Active = viewModel.Active;
        //        _warrantyService.Update(warranty);
        //        _warrantyService.SaveChanges();
        //        SetAlert("Update Warranty success", "success");
        //    }
        //    catch (Exception e)
        //    {
        //        SetAlert("Update Warranty success", "success");
        //    }
        //    return RedirectToAction("Index");
        //}

        //// GET: MaintenanceDiary/Export
        //public ActionResult Export()
        //{
        //    var exportList = _warrantyService.GetAll(new string[] { "Asset" });

        //    if (exportList != null)
        //    {
        //        ExcelPackage excelPackage = new ExcelPackage();

        //        ExcelWorksheet Sheet = excelPackage.Workbook.Worksheets.Add("Warranty Report");

        //        // build header sheet
        //        Sheet.Cells["A1"].Value = "Asset ID";
        //        Sheet.Cells["B1"].Value = "Asset Code";
        //        Sheet.Cells["C1"].Value = "Asset Name";
        //        Sheet.Cells["D1"].Value = "Date Warranty";
        //        Sheet.Cells["E1"].Value = "End Date";
        //        Sheet.Cells["F1"].Value = "Reason";
        //        Sheet.Cells["G1"].Value = "Result";
        //        int row = 2;
        //        foreach (var item in exportList)
        //        {
        //            Sheet.Cells[string.Format("A{0}", row)].Value = item.AssetID;
        //            Sheet.Cells[string.Format("B{0}", row)].Value = item.Asset.AssetCode;
        //            Sheet.Cells[string.Format("C{0}", row)].Value = item.Asset.Name;
        //            Sheet.Cells[string.Format("D{0}", row)].Value = item.DateWarranty.ToShortDateString();
        //            Sheet.Cells[string.Format("E{0}", row)].Value = item.EndDate.ToShortDateString();
        //            Sheet.Cells[string.Format("F{0}", row)].Value = item.Reason;
        //            Sheet.Cells[string.Format("G{0}", row)].Value = item.Result;
        //            row++;
        //        }

        //        Sheet.Cells["A:AZ"].AutoFitColumns();

        //        byte[] bytesInStream;

        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            excelPackage.SaveAs(memoryStream);
        //            bytesInStream = memoryStream.ToArray();
        //        }

        //        return File(bytesInStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Warranty_Report_" + DateTime.Now + ".xlsx");
        //    }

        //    return new EmptyResult();
        //}
    }
}