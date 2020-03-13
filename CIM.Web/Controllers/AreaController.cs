using AutoMapper;
using CIM.Common;
using CIM.Model.Models;
using CIM.Service;
using CIM.Web.Infrastructure;
using CIM.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize(Roles = "System Admin,Manager")]
    public class AreaController : BaseController
    {
        private IAreaService _areaService;

        private ILocationService _locationService;

        public AreaController(IAreaService areaService, ILocationService locationService)
        {
            this._areaService = areaService;
            this._locationService = locationService;
        }

        // GET: Area
        public ActionResult Index(int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var areasModel = _areaService.GetAllPaging(out totalRow, page, pageSize, new string[] { "Location" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var areaViewModel = Mapper.Map<IEnumerable<Area>, IEnumerable<AreaViewModel>>(areasModel);

            var paginationSet = new PaginationSet<AreaViewModel>()
            {
                Items = areaViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            var locationModel = _locationService.GetAll();
            ViewBag.locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationModel);

            ViewBag.query = new
            {
                page = page
            };
            return View(paginationSet);
        }

        public ActionResult Search(string areaSearch, int locationId, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var areasModel = _areaService.Search(areaSearch, locationId, out totalRow, page, pageSize, new string[] { "Location" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var areaViewModel = Mapper.Map<IEnumerable<Area>, IEnumerable<AreaViewModel>>(areasModel);

            var paginationSet = new PaginationSet<AreaViewModel>()
            {
                Items = areaViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            var locationModel = _locationService.GetAll();

            ViewBag.locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationModel);
            ViewBag.areaSearch = areaSearch;

            ViewBag.query = new
            {
                locationId = locationId,
                areaSearch = areaSearch,
                page = page
            };

            return View("Index", paginationSet);
        }

        // GET: Area/Details/5
        public ActionResult Details(int id)
        {
            var areaModel = _areaService.GetById(id, new string[] { "Location" });

            if (areaModel == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<Area, AreaViewModel>(areaModel);

            return View(viewModel);
        }

        // GET: Area/Create
        public ActionResult Create()
        {
            var locationModel = _locationService.GetAll();

            ViewBag.locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationModel);

            var viewModel = new AreaViewModel();

            return View(viewModel);
        }

        // POST: Area/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AreaViewModel viewModel)
        {
            try
            {
                var locationModel = _locationService.GetAll();
                var validateName = _areaService.GetAreaDuplicateName(viewModel.ID, viewModel.Name);
                var validateCode = _areaService.GetAreaDuplicateCode(viewModel.ID, viewModel.AreaCode);
                if (validateName != null)
                {
                    ModelState.AddModelError("Name", "Name already exists");
                }
                if (validateCode != null)
                {
                    ModelState.AddModelError("AreaCode", "Code already exists");
                }
                if (!ModelState.IsValid)
                {
                    // xy ly loi
                    ViewBag.locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationModel);
                    return View(viewModel);
                }
                else
                {
                    var area = new Area()
                    {
                        AreaCode = viewModel.AreaCode.Trim(),
                        Name = viewModel.Name.Trim(),
                        Description = viewModel.Description,
                        LocationID = viewModel.LocationID,
                        Active = true
                    };

                    _areaService.Add(area);
                    _areaService.SaveChanges();
                    SetAlert("Add Area success", "success");
                }

               
            }
            catch(Exception e)
            {
                SetAlert("Add Area error", "error");
            }
            return RedirectToAction("Index");
        }

        // GET: Area/Edit/5
        public ActionResult Edit(int id)
        {
            var areaModel = _areaService.GetById(id, new string[] { "Location" });

            var locationModel = _locationService.GetAll();

            if (areaModel == null)
            {
                return HttpNotFound();
            }

            ViewBag.locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationModel);

            var viewModel = Mapper.Map<Area, AreaViewModel>(areaModel);

            return View(viewModel);
        }

        // POST: Area/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AreaViewModel viewModel)
        {
            try
            {
                var locationModel = _locationService.GetAll();
                ViewBag.locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationModel);
                var validateName = _areaService.GetAreaDuplicateName(viewModel.ID, viewModel.Name.Trim());
                var validateCode = _areaService.GetAreaDuplicateCode(viewModel.ID, viewModel.AreaCode.Trim());
                if (validateName != null)
                {
                    ModelState.AddModelError("Name", "Name already exists");
                }
                if (validateCode != null)
                {
                    ModelState.AddModelError("AreaCode", "Code already exists");
                }

                if (!ModelState.IsValid)
                {
                    // xy ly loi

                    return View(viewModel);
                }
                else
                {
                    var area = _areaService.GetById(viewModel.ID, new string[] { "Location" });
                    area.LocationID = viewModel.LocationID;
                    area.AreaCode = viewModel.AreaCode.Trim();
                    area.Name = viewModel.Name.Trim();
                    area.Description = viewModel.Description;
                    area.Active = viewModel.Active;
                    _areaService.Update(area);
                    _areaService.SaveChanges();
                    SetAlert("Update Area success", "success");
                }
                // TODO: Add update logic here
                
            }
            catch(Exception e)
            {
                SetAlert("Update Area error", "error");
            }
            return RedirectToAction("Index");
        }
    }
}