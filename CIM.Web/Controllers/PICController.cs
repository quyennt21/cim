using AutoMapper;
using CIM.Common;
using CIM.Model.Models;
using CIM.Service;
using CIM.Web.Infrastructure;
using CIM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize(Roles = "System Admin,Manager")]
    public class PICController : BaseController
    {
        private IPICService _PICService;
        private IAreaService _areaService;
        private IAssetTypeService _assetTypeService;
        private ILocationService _locationService;

        private ApplicationUserManager applicationUserManager;

        public PICController(IPICService PICService, ILocationService locationService, IAreaService areaService, IAssetTypeService assetTypeService, ApplicationUserManager applicationUserManager)
        {
            this._PICService = PICService;
            this._locationService = locationService;
            this._areaService = areaService;
            this._assetTypeService = assetTypeService;
            this.applicationUserManager = applicationUserManager;
        }

        // GET: PIC
        public ActionResult Index(int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var PICsModel = _PICService.GetAllPaging(out totalRow, page, pageSize, new string[] { "Area", "ApplicationUser", "AssetType" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var PICViewModel = Mapper.Map<IEnumerable<PIC>, IEnumerable<PICViewModel>>(PICsModel);

            var locationModel = _locationService.GetAll();
            ViewBag.locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationModel);

            var assetTypeModel = _assetTypeService.GetAll();
            ViewBag.assetTypeViewModel = Mapper.Map<IEnumerable<AssetType>, IEnumerable<AssetTypeViewModel>>(assetTypeModel);

            var paginationSet = new PaginationSet<PICViewModel>()
            {
                Items = PICViewModel,
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

        // GET: PIC
        public ActionResult Search(int locationId, int assetTypeId, string userSearch, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var PICsModel = _PICService.Search(locationId, assetTypeId, userSearch, out totalRow, page, pageSize, new string[] { "Area", "ApplicationUser", "AssetType" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var PICViewModel = Mapper.Map<IEnumerable<PIC>, IEnumerable<PICViewModel>>(PICsModel);

            var paginationSet = new PaginationSet<PICViewModel>()
            {
                Items = PICViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            var locationModel = _locationService.GetAll();
            ViewBag.locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationModel);

            var assetTypeModel = _assetTypeService.GetAll();
            ViewBag.assetTypeViewModel = Mapper.Map<IEnumerable<AssetType>, IEnumerable<AssetTypeViewModel>>(assetTypeModel);

            ViewBag.locationId = locationId;
            ViewBag.assetTypeId = assetTypeId;
            ViewBag.userSearch = userSearch;

            ViewBag.query = new
            {
                locationId = locationId,
                assetTypeId = assetTypeId,
                userSearch = userSearch,
                page = page
            };

            return View("Index", paginationSet);
        }

        // GET: PIC/Details/5
        public ActionResult Details(int id)
        {
            var PICModel = _PICService.GetById(id, new string[] { "Area", "ApplicationUser", "AssetType" });

            if (PICModel == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<PIC, PICViewModel>(PICModel);

            return View(viewModel);
        }

        // GET: PIC/Create
        public ActionResult Create()
        {
            var locationModel = _locationService.GetAll();

            var assetTypeModel = _assetTypeService.GetAll();

            var userModel = applicationUserManager.Users.ToList();

            ViewBag.locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationModel);

            ViewBag.userViewModel = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(userModel);

            ViewBag.assetTypeViewModel = Mapper.Map<IEnumerable<AssetType>, IEnumerable<AssetTypeViewModel>>(assetTypeModel);

            var viewModel = new PICViewModel();
            viewModel.StartDate = DateTime.Now;
            viewModel.EndDate = DateTime.Now;

            return View(viewModel);
        }

        // POST: PIC/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PICViewModel viewModel)
        {

            try
            {
                DateTime endD = Convert.ToDateTime(viewModel.EndDate.ToString());
                DateTime startD = Convert.ToDateTime(viewModel.StartDate.ToString());
                int endYear = Convert.ToInt32(viewModel.EndDate.Year.ToString());
                int startYear = Convert.ToInt32(viewModel.StartDate.Year.ToString());
                int compareDate = DateTime.Compare(endD, startD);
                var locationModel = _locationService.GetAll();

                var assetTypeModel = _assetTypeService.GetAll();

                var userModel = applicationUserManager.Users.ToList();

                ViewBag.locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationModel);

                ViewBag.userViewModel = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(userModel);

                ViewBag.assetTypeViewModel = Mapper.Map<IEnumerable<AssetType>, IEnumerable<AssetTypeViewModel>>(assetTypeModel);
                if (endYear < 1970 || startYear < 1970)
                {
                    return View(viewModel);
                }
                if (compareDate <= 0)
                {
                    ModelState.AddModelError("EndDate", "EndDate must have a date greater than StartDate");
                    return View(viewModel);
                }

                var locationId = viewModel.Area.Location.ID;

                var listArea = _areaService.GetAllByLocationId(locationId);

                foreach (var area in listArea.ToList())
                {
                    //get exist pic
                    var pic = _PICService.GetByAreaIdAndAssetTypeId(area.ID, viewModel.AssetType.ID);

                    if (pic != null)
                    {
                        pic.ApplicationUserID = viewModel.ApplicationUser.Id;
                        pic.StartDate = viewModel.StartDate;
                        pic.EndDate = viewModel.EndDate;
                        pic.Active = true;
                        _PICService.Update(pic);
                    }
                    else {
                        //add new pic when no pic exist
                        pic = new PIC()
                        {
                            ApplicationUserID = viewModel.ApplicationUser.Id,
                            AreaID = area.ID,
                            AssetTypeID = viewModel.AssetType.ID,
                            StartDate = viewModel.StartDate,
                            EndDate = viewModel.EndDate,
                            Active = true
                        };

                        _PICService.Add(pic);
                    }  
                }
                _PICService.SaveChanges();
                SetAlert("Add PIC success", "success");
            }
            catch(Exception)
            {
                SetAlert("Add PIC error", "error");
            }
            return RedirectToAction("Index");
        }

        // GET: PIC/Edit/5
        public ActionResult Edit(int id, FormCollection form)
        {
            var picModel = _PICService.GetById(id, new string[] { "Area", "ApplicationUser", "AssetType" });

            var areaModel = _areaService.GetAll();

            var assetTypeModel = _assetTypeService.GetAll();

            var userModel = applicationUserManager.Users.ToList();

            ViewBag.areaViewModel = Mapper.Map<IEnumerable<Area>, IEnumerable<AreaViewModel>>(areaModel);

            ViewBag.userViewModel = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(userModel);

            ViewBag.assetTypeViewModel = Mapper.Map<IEnumerable<AssetType>, IEnumerable<AssetTypeViewModel>>(assetTypeModel);

            if (picModel == null)
            {
                return HttpNotFound();
            }
            var viewModel = Mapper.Map<PIC, PICViewModel>(picModel);

            return View(viewModel);
        }

        public ActionResult EditSelect(string selectListItem)
        {
            ViewBag.selectListItem = selectListItem;

            if (!string.IsNullOrEmpty(selectListItem))
            {
                var userModel = applicationUserManager.Users.ToList();

                ViewBag.userViewModel = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(userModel);
                return View("EditSelect");
            }
            return RedirectToAction("Index");
        }

        // POST: PIC/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSelect(FormCollection form)
        {
            try
            {
                ViewBag.selectListItem = form["selectListItem"];

                var userModel = applicationUserManager.Users.ToList();

                ViewBag.userViewModel = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(userModel);

                var selectListItem = form["selectListItem"] != null ? form["selectListItem"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList() : new List<int>();

                foreach (int picId in selectListItem)
                {
                    // TODO: Add update logic here
                    var pic = _PICService.GetById(picId);
                    pic.ApplicationUserID = Convert.ToInt32(form["userId"]);
                    string startD = form["startDate"].ToString();
                    string endD = form["endDate"].ToString();
                    if(!String.IsNullOrEmpty(startD) && !String.IsNullOrEmpty(endD))
                    {
                        DateTime startDate= Convert.ToDateTime(form["startDate"]);
                        DateTime endDate = Convert.ToDateTime(form["endDate"]);
                        int compareDate = DateTime.Compare(endDate, startDate);
                        if (compareDate <= 0)
                        {
                            ModelState.AddModelError("EndDate", "EndDate must have a date greater than StartDate");
                        }
                        return View();
                    }
                    if (String.IsNullOrEmpty(startD)|| String.IsNullOrEmpty(endD))
                    {
                        if (String.IsNullOrEmpty(startD))
                        {
                            ModelState.AddModelError("StartDate", "The StartDate field is required.");
                        }
                        if (String.IsNullOrEmpty(endD))
                        {
                            ModelState.AddModelError("EndDate", "The EndDate field is required.");
                        }
                        return View();
                    }
                    else
                    {
                        pic.StartDate = Convert.ToDateTime(form["startDate"]);
                        pic.EndDate = Convert.ToDateTime(form["endDate"]);
                        _PICService.Update(pic);
                        _PICService.SaveChanges();
                        SetAlert("Edit PIC Select success", "success");
                    }

                }               
            }
            catch(Exception)
            {
                SetAlert("Edit PIC Select error", "error");
            }
            return RedirectToAction("Index");
        }

        // POST: PIC/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PICViewModel viewModel)
        {
            try
            {
                DateTime endD = Convert.ToDateTime(viewModel.EndDate.ToString());
                DateTime startD = Convert.ToDateTime(viewModel.StartDate.ToString());
                int endYear = Convert.ToInt32(viewModel.EndDate.Year.ToString());
                int startYear = Convert.ToInt32(viewModel.StartDate.Year.ToString());
                int compareDate = DateTime.Compare(endD, startD);
                var areaModel = _areaService.GetAll();

                var assetTypeModel = _assetTypeService.GetAll();

                var userModel = applicationUserManager.Users.ToList();

                ViewBag.areaViewModel = Mapper.Map<IEnumerable<Area>, IEnumerable<AreaViewModel>>(areaModel);

                ViewBag.userViewModel = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(userModel);

                ViewBag.assetTypeViewModel = Mapper.Map<IEnumerable<AssetType>, IEnumerable<AssetTypeViewModel>>(assetTypeModel);
                if (endYear < 1970 || startYear < 1970)
                {
                    return View(viewModel);
                }
                if (compareDate <= 0)
                {
                    ModelState.AddModelError("EndDate", "EndDate must have a date greater than StartDate");
                    return View(viewModel);
                }
                // TODO: Add update logic here
                var pic = _PICService.GetById(viewModel.ID, new string[] { "Area", "ApplicationUser", "AssetType" });
                pic.AreaID = viewModel.Area.ID;
                pic.AssetTypeID = viewModel.AssetType.ID;
                pic.ApplicationUserID = viewModel.ApplicationUser.Id;
                pic.StartDate = viewModel.StartDate;
                pic.EndDate = viewModel.EndDate;
                pic.Active = viewModel.Active;
                _PICService.Update(pic);
                _PICService.SaveChanges();
                SetAlert("Edit PIC success", "success");
            }
            catch(Exception)
            {
                SetAlert("Edit PIC error", "error");
            }
            return RedirectToAction("Index");
        }
    }
}