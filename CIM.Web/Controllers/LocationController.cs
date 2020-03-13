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
    public class LocationController : BaseController
    {
        private ILocationService _locationService;

        private ICampusService _campusService;

        public LocationController(ILocationService locationService, ICampusService campusService)
        {
            this._locationService = locationService;
            this._campusService = campusService;
        }

        // GET: Location
        public ActionResult Index(int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var locationsModel = _locationService.GetAllPaging(out totalRow, page, pageSize, new string[] { "Campus" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationsModel);

            var paginationSet = new PaginationSet<LocationViewModel>()
            {
                Items = locationViewModel,
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
        public ActionResult Search(string locationSearch, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var locationsModel = _locationService.Search(locationSearch, out totalRow, page, pageSize, new string[] { "Campus" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var locationViewModel = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationViewModel>>(locationsModel);

            var paginationSet = new PaginationSet<LocationViewModel>()
            {
                Items = locationViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            ViewBag.locationSearch = locationSearch;

            ViewBag.query = new
            {
                locationSearch = locationSearch,
                page = page
            };

            return View("Index", paginationSet);
        }

        // GET: Location/Details/5
        public ActionResult Details(int id)
        {
            var locationModel = _locationService.GetById(id, new string[] { "Campus" });

            if (locationModel == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<Location, LocationViewModel>(locationModel);

            return View(viewModel);
        }

        // GET: Location/Create
        public ActionResult Create()
        {
            var campusModel = _campusService.GetAll();

            ViewBag.campusViewModel = Mapper.Map<IEnumerable<Campus>, IEnumerable<CampusViewModel>>(campusModel);

            var viewModel = new LocationViewModel();

            return View(viewModel);
        }

        // POST: Location/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LocationViewModel viewModel)
        {
            try
            {
                 
                var validateName = _locationService.GetNameLocationDuplicate(viewModel.ID, viewModel.Name);
                var validateCode = _locationService.GetCodeLocationDuplicate(viewModel.ID, viewModel.LocationCode);
              
                 
                if (validateName != null)
                {
                    ModelState.AddModelError("Name", "Name already exists");
                     
                }
                if (validateCode != null)
                {
                    ModelState.AddModelError("LocationCode", "Code already exists");
                    
                }
                var campusModel = _campusService.GetAll();
                ViewBag.campusViewModel = Mapper.Map<IEnumerable<Campus>, IEnumerable<CampusViewModel>>(campusModel);
                if (!ModelState.IsValid)
                {
                    // xy ly loi

                    return View(viewModel);
                }
                if(validateCode!=null || validateName != null)
                {
                    return View(viewModel);
                }
                else
                {
                    var location = new Location()
                    {
                        LocationCode = viewModel.LocationCode.Trim(),
                        Name = viewModel.Name.Trim(),
                        Description = viewModel.Description,
                        CampusID = viewModel.CampusID,
                        Active = true
                    };

                    _locationService.Add(location);
                    _locationService.SaveChanges();
                    SetAlert("Add Location success", "success");
                }

                
            }
            catch(Exception e)
            {
                SetAlert("Add Location error", "error");
            }
            return RedirectToAction("Index");
        }

        // GET: Location/Edit/5
        public ActionResult Edit(int id)
        {
            var locationModel = _locationService.GetById(id, new string[] { "Campus" });

            var campusModel = _campusService.GetAll();

            if (locationModel == null)
            {
                return HttpNotFound();
            }

            ViewBag.campusViewModel = Mapper.Map<IEnumerable<Campus>, IEnumerable<CampusViewModel>>(campusModel);

            var viewModel = Mapper.Map<Location, LocationViewModel>(locationModel);

            return View(viewModel);
        }

        // POST: Location/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LocationViewModel viewModel)
        {
            try
            {
               
                var validateName = _locationService.GetNameLocationDuplicate(viewModel.ID, viewModel.Name.Trim());
                var validateCode = _locationService.GetCodeLocationDuplicate(viewModel.ID, viewModel.LocationCode.Trim());
                if (validateName != null)
                {
                    ModelState.AddModelError("Name", "Name already exists");
                }
                if (validateCode != null)
                {
                    ModelState.AddModelError("LocationCode", "Code already exists");
                }
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                if (!ModelState.IsValid)
                {
                    var campusModel = _campusService.GetAll();
                    ViewBag.campusViewModel = Mapper.Map<IEnumerable<Campus>, IEnumerable<CampusViewModel>>(campusModel);
                    // xy ly loi
                    return View(viewModel);
                }
                else
                {
                    // TODO: Add update logic here
                    var location = _locationService.GetById(viewModel.ID, new string[] { "Campus" });
                    location.CampusID = viewModel.CampusID;
                    location.LocationCode = viewModel.LocationCode.Trim();
                    location.Name = viewModel.Name.Trim();
                    location.Description = viewModel.Description;
                    location.Active = viewModel.Active;
                    _locationService.Update(location);
                    _locationService.SaveChanges();
                    SetAlert("Update Location success", "success");
                }
              
            }
            catch(Exception e)
            {
                SetAlert("Update Location error", "error");
            }
            return RedirectToAction("Index");
        }
    }
}