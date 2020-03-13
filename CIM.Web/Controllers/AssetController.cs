using AutoMapper;
using CIM.Common;
using CIM.Model.Models;
using CIM.Service;
using CIM.Web.Infrastructure;
using CIM.Web.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize(Roles = "System Admin,Manager")]
    public class AssetController : BaseController
    {
        private IAssetService _assetService;
        private IAssetTypeService _assetTypeService;
        private IAreaService _areaService;
        private IAssetAttributeService _assetAttributeService;
        private IAssetAttributeValueService _assetAttributeValueService;
        private IReportService _reportService;
        private IAssetLogService _assetLogService;
        private ILocationService _locationService;
        private ApplicationUserManager applicationUserManager;

        public AssetController(IAssetService assetService, IAssetTypeService assetTypeService, IAreaService areaService, ILocationService locationService,
            IAssetAttributeService assetAttributeService, IAssetAttributeValueService assetAttributeValueService, IReportService reportService, IAssetLogService assetLogService, ApplicationUserManager applicationUserManager)
        {
            this._assetAttributeService = assetAttributeService;
            this._assetTypeService = assetTypeService;
            this._areaService = areaService;
            this._assetService = assetService;
            this._assetAttributeValueService = assetAttributeValueService;
            this._reportService = reportService;
            this._assetLogService = assetLogService;
            this.applicationUserManager = applicationUserManager;
            this._locationService = locationService;
        }

        // GET: Asset
        public ActionResult Index(int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));
            int totalRow = 0;
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);


            var paginationSet = new PaginationSet<AssetViewModel>()
            {
                Items = null,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };
            SelectList typeSearch = new SelectList(_assetService.ListTypeSearch());
            ViewBag.typeSearch = typeSearch;
            List<String> listLocation = new List<string>();
            listLocation.Add("--All--");
            listLocation.AddRange(_locationService.GetAll().Select(l => l.Name).ToList());
            SelectList locationSearch = new SelectList(listLocation);
            ViewBag.locationSearch = locationSearch;
            ViewBag.query = new
            {
                page = page
            };
            TempData.Remove("checkScreenArea");
            TempData.Remove("idArea");
            return View(paginationSet);
        }

        // GET: Search Location
        public ActionResult Search(string typeSearch, string locationSearch, string dateFrom, string dateTo, string searchString, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));
            int totalRow = 0;

            var assetModel = _assetService.Search(searchString, locationSearch, typeSearch, dateFrom, dateTo, out totalRow, page, pageSize, new string[] { "Area", "AssetType", "Area.Location", "ApplicationUser" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var assetViewModel = Mapper.Map<IEnumerable<Asset>, IEnumerable<AssetViewModel>>(assetModel);

            var paginationSet = new PaginationSet<AssetViewModel>()
            {
                Items = assetViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };
            SelectList listSearch = new SelectList(_assetService.ListTypeSearch());
            ViewBag.typeSearch = listSearch;

            List<String> listLocation = new List<string>();
            listLocation.Add("--All--");
            listLocation.AddRange(_locationService.GetAll().Select(l => l.Name).ToList());
            SelectList locationS = new SelectList(listLocation);
            ViewBag.locationSearch = locationS;

            ViewBag.searchString = searchString;
            ViewBag.dateFrom = dateFrom;
            ViewBag.dateTo = dateTo;
            ViewBag.query = new
            {
                locationSearch = locationSearch,
                typeSearch = typeSearch,
                searchString = searchString,
                dateFrom = dateFrom,
                dateTo = dateTo,
                page = page
            };

            return View("Index", paginationSet);
        }

        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var assetViewModel = _assetService.GetById(id, new string[] { "Area", "AssetType", "Area.Location", "ApplicationUser" });
            var report = _reportService.GetReportByConditions(id, new string[] { "Asset" });
            if (report != null)
            {
                ViewBag.reportID = report.ID;
            }
            if (assetViewModel == null)
            {
                return HttpNotFound();
            }

            var listAssetAttributeValue = _assetAttributeValueService.GetByAssetId(id, new string[] { "AssetTypeAttribute" });

            var assetAttributeValueViewModel = Mapper.Map<IEnumerable<AssetAttributeValue>, IEnumerable<AssetAttributeValueViewModel>>(listAssetAttributeValue);
            ViewBag.AssetAttributeValueViewModel = assetAttributeValueViewModel;
            AssetViewModel viewModel = Mapper.Map<Asset, AssetViewModel>(assetViewModel);
            if (viewModel.Image == null)
            {
                viewModel.Image = Common.Constants.DEFAULT_ASSET_IMAGE;
            }
            return View(viewModel);
        }

        public ActionResult Create(int? id)
        {
            if (id != null)
            {
                ViewBag.IdArea = id;
            }
            var areas = _areaService.GetAll();
            var assetTypes = _assetTypeService.GetActive();
            var assetTypeViewModels = Mapper.Map<IEnumerable<AssetType>, IEnumerable<AssetTypeViewModel>>(assetTypes);
            var areaViewModels = Mapper.Map<IEnumerable<Area>, IEnumerable<AreaViewModel>>(areas);
            ViewBag.AreaID = new SelectList(areaViewModels, "ID", "Name");
            ViewBag.AssetTypeID = new SelectList(assetTypeViewModels, "ID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(AssetViewModel assetViewModel, int? idArea, int SelectedOption)
        {
            var validateMail = _assetService.GetAll(new string[] { "Area", "Area.Location" }).FirstOrDefault
                    (x => x.Name == assetViewModel.Name && x.Area.ID == assetViewModel.AreaID && x.ID != assetViewModel.ID);
            if (validateMail != null)
            {
                ModelState.AddModelError("Name", "Name already exists in Area");
            }

            var listAttribute = _assetAttributeService.GetAssetAttributes(assetViewModel.AssetTypeID);
            string assetTypeCode = "";
            assetTypeCode = _assetService.GetStringAssetCode(SelectedOption);
            if (listAttribute.Count() > 0)
            {
                if (ModelState.IsValid)
                {
                    assetViewModel.Active = true;
                    assetViewModel.CreatedAt = DateTime.Now;
                    assetViewModel.UpdatedAt = DateTime.Now;
                    assetViewModel.AssetCode = assetTypeCode;
                    TempData["idArea"] = idArea;
                    return RedirectToAction("CreateAttributeValue", assetViewModel);
                }
                var areas = _areaService.GetAll();
                var assetTypes = _assetTypeService.GetAll();
                ViewBag.AreaID = new SelectList(areas, "ID", "AreaCode");
                ViewBag.AssetTypeID = new SelectList(assetTypes, "ID", "Name");
                return View(assetViewModel);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Asset asset = new Asset()
                    {
                        AssetTypeID = assetViewModel.AssetTypeID,
                        AreaID = assetViewModel.AreaID,
                        Name = assetViewModel.Name,
                        AssetCode = assetTypeCode,
                        StartDate = assetViewModel.StartDate,
                        Quantity = assetViewModel.Quantity,
                        Description = assetViewModel.Description,
                        ApplicationUserID = Convert.ToInt32(User.Identity.GetUserId()),
                        Active = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };
                    var checkAdd=_assetService.Add(asset, new List<AssetAttributeValue>());
                    if (checkAdd)
                    {
                        SetAlert("Add Asset success", "success");
                    }
                    else
                    {
                        SetAlert("Add Asset error", "error");
                    }
                    if (idArea != null)
                    {
                        return RedirectToAction("AssetArea", "Asset", new { id = idArea });
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                var areas = _areaService.GetAll();
                var assetTypes = _assetTypeService.GetAll();
                ViewBag.AreaID = new SelectList(areas, "ID", "AreaCode");
                ViewBag.AssetTypeID = new SelectList(assetTypes, "ID", "Name");
                return View(assetViewModel);
            }
        }

        public ActionResult CreateAttributeValue(AssetViewModel assetViewModel)
        {
            var listAttribute = _assetAttributeService.GetAssetAttributes(assetViewModel.AssetTypeID);
            ViewBag.AssetAttribute = listAttribute;
            ViewBag.Asset = JsonConvert.SerializeObject(assetViewModel);
            if (TempData["idArea"] != null)
            {
                ViewBag.IdArea = TempData["idArea"];
            }
            return View();
        }

        [HttpPost]
        public JsonResult CreateAttributeValue(string jsonAssetAttribute, string asset)
        {
            try
            {
                String assetString = HttpUtility.HtmlDecode(asset);
                List<AssetAttributeValue> assetAttributeValue = JsonConvert.DeserializeObject<List<AssetAttributeValue>>(jsonAssetAttribute);
                Asset assetViewModel = JsonConvert.DeserializeObject<Asset>(assetString);
                assetViewModel.ApplicationUserID = Convert.ToInt32(User.Identity.GetUserId());               
                var checkAdd = _assetService.Add(assetViewModel, assetAttributeValue);
                if (checkAdd)
                {
                    SetAlert("Add Asset success", "success");
                }
                else
                {
                    SetAlert("Add Asset error", "error");
                }
            }
            catch (Exception e)
            {
                return Json("Error");
            }
            return Json("");
        }

        public ActionResult Delete(int id)
        {
            var assetViewModel = _assetService.GetById(id);

            if (assetViewModel == null)
            {
                return HttpNotFound();
            }
            var viewModel = Mapper.Map<Asset, AssetViewModel>(assetViewModel);

            return View(viewModel);
        }

        // POST: AssetTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Asset asset = _assetService.GetById(id);
            asset.Active = false;
            var listAttributeValue = _assetAttributeValueService.GetByAssetId(id, new string[] { "AssetTypeAttribute" });
            foreach (var attribute in listAttributeValue)
            {
                attribute.Active = false;
            }
            _assetService.Update(asset);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var assetViewModel = _assetService.GetById(id, new string[] { "Area", "AssetType", "Area.Location", "ApplicationUser" });

            if (assetViewModel == null)
            {
                return HttpNotFound();
            }
            var listAssetAttributeValue = _assetAttributeValueService.GetByAssetId(id, new string[] { "AssetTypeAttribute", "Asset" });
            var assetAttributeValueViewModel = Mapper.Map<IEnumerable<AssetAttributeValue>, IEnumerable<AssetAttributeValueViewModel>>(listAssetAttributeValue);
            var areas = _areaService.GetAll();
            var assetTypes = _assetTypeService.GetAll();
            ViewBag.AssetAttributeValueViewModel = assetAttributeValueViewModel;

            ViewBag.AreaViewModel = Mapper.Map<IEnumerable<Area>, IEnumerable<AreaViewModel>>(areas);
            ViewBag.AssetTypeViewModel = Mapper.Map<IEnumerable<AssetType>, IEnumerable<AssetTypeViewModel>>(assetTypes);
            var viewModel = Mapper.Map<Asset, AssetViewModel>(assetViewModel);
            //if (TempData["checkScreenArea"] != null)
            //{
            //    ViewBag.checkScreenArea = TempData["checkScreenArea"];
            //}
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Edit(string jsonAssetAttribute, string asset)
        {
            string error = "false";
            string errorQuantity = "false";
            try
            {
                TempData.Remove("checkScreenArea");
                String assetString = HttpUtility.HtmlDecode(asset);
                List<AssetAttributeValue> assetAttributeValue = JsonConvert.DeserializeObject<List<AssetAttributeValue>>(jsonAssetAttribute);
                Asset assetViewModel = JsonConvert.DeserializeObject<Asset>(assetString);
                //assetViewModel.ApplicationUserID = Convert.ToInt32(User.Identity.GetUserId());
               
                var validateName = _assetService.GetAll(new string[] { "Area", "Area.Location" }).FirstOrDefault
                    (x => x.Name == assetViewModel.Name && x.Area.ID == assetViewModel.AreaID && x.ID != assetViewModel.ID);
                if (validateName != null)
                {
                    error = "Name Asset already exists in Area";

                }
                else
                {
                        _assetService.Update(assetViewModel, assetAttributeValue);
                        SetAlert("Update Asset success", "success");
                        AssetLog assetLog = new AssetLog
                        {
                            AssetID = assetViewModel.ID,
                            Active = true,
                            CreatedAt = DateTime.Now,
                            Message = "Asset Updated",
                            ApplicationUserID = applicationUserManager.FindByName(User.Identity.GetUserName()).Id
                        };

                        _assetLogService.Add(assetLog);
                        _assetLogService.SaveChanges();

                }
                // update asset log
               
            }
            catch (Exception e)
            {
                SetAlert("Update Asset error", "error");
            }
            return Json(error);
        }

        public ActionResult AssetArea(int id)
        {
            IEnumerable<AssetViewModel> listAssetModel = new List<AssetViewModel>();
            if (id != 0)
            {
                Asset asset = _assetService.GetById(id);
                List<Asset> assetModel = _assetService.GetAllAssetArea(asset.AreaID, new string[] { "Area", "AssetType", "Area.Location", "ApplicationUser" }).ToList();
                listAssetModel = Mapper.Map<IEnumerable<Asset>, IEnumerable<AssetViewModel>>(assetModel);
            }
            TempData["checkScreenArea"] = id;
            return View(listAssetModel);
        }

        public ActionResult EditAttributeValue(AssetViewModel assetViewModel)
        {
            var listAssetAttributeValue = _assetAttributeValueService.GetByAssetId(assetViewModel.ID, new string[] { "AssetTypeAttribute", "Asset" });
            var assetAttributeValueViewModel = Mapper.Map<IEnumerable<AssetAttributeValue>, IEnumerable<AssetAttributeValueViewModel>>(listAssetAttributeValue);
            ViewBag.Asset = JsonConvert.SerializeObject(assetViewModel);
            ViewBag.AssetAttributeValueViewModel = assetAttributeValueViewModel;
            return View();
        }

        [HttpPost]
        public JsonResult EditAttributeValue(string jsonAssetAttribute, string asset)
        {
            try
            {
                String assetString = HttpUtility.HtmlDecode(asset);
                List<AssetAttributeValue> assetAttributeValue = JsonConvert.DeserializeObject<List<AssetAttributeValue>>(jsonAssetAttribute);
                Asset assetViewModel = JsonConvert.DeserializeObject<Asset>(assetString);
                //                assetViewModel.ApplicationUserID = Convert.ToInt32(User.Identity.GetUserId());
                _assetService.Update(assetViewModel, assetAttributeValue);
            }
            catch (Exception e)
            {
                return Json("Error");
            }
            return Json("");
        }
    }
}