using AutoMapper;
using CIM.Common;
using CIM.Model.Models;
using CIM.Service;
using CIM.Web.Infrastructure;
using CIM.Web.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize(Roles = "System Admin, Manager")]
    public class CampusController : BaseController
    {
        private ICampusService _campusService;

        public CampusController(ICampusService campusService)
        {
            this._campusService = campusService;
        }

        // GET: Campus
        public ActionResult Index(int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var campusModel = _campusService.GetAllPaging(out totalRow, page, pageSize);

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var campusViewModel = Mapper.Map<IEnumerable<Campus>, IEnumerable<CampusViewModel>>(campusModel);

            var paginationSet = new PaginationSet<CampusViewModel>()
            {
                Items = campusViewModel,
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

        public ActionResult Search(string campusSearch, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var campusModel = _campusService.Search(campusSearch, out totalRow, page, pageSize);

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var campusViewModel = Mapper.Map<IEnumerable<Campus>, IEnumerable<CampusViewModel>>(campusModel);

            var paginationSet = new PaginationSet<CampusViewModel>()
            {
                Items = campusViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("PageSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            ViewBag.campusSearch = campusSearch;

            ViewBag.query = new
            {
                campusSearch = campusSearch,
                page = page
            };

            return View("Index", paginationSet);
        }

        // GET: Campus/Details/5
        public ActionResult Details(int id)
        {
            var campusModel = _campusService.GetById(id);

            if (campusModel == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<Campus, CampusViewModel>(campusModel);

            return View(viewModel);
        }

        // GET: Campus/Create
        public ActionResult Create()
        {
            var viewModel = new CampusViewModel();

            return View(viewModel);
        }

        // POST: Campus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CampusViewModel viewModel)
        {
            try
            {
                string MatchPhonePattern = @"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$";
                Regex rx = new Regex(MatchPhonePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var validateName = _campusService.GetCampusDuplicate(viewModel.ID, viewModel.Name);
                if (validateName != null)
                {
                    ModelState.AddModelError("Name", "Campus Name already exists");
                    return View(viewModel);
                }
                else
                {
                   try
                    {
                        if (!rx.IsMatch(viewModel.Telephone.Trim()))
                        {
                            ModelState.AddModelError("Telephone", "Telephone must numbers");
                            return View(viewModel);
                        }

                        var campus = new Campus()
                        {
                            Name = viewModel.Name.Trim(),
                            Address = viewModel.Address,
                            Telephone = viewModel.Telephone,
                            Active = true
                        };

                        _campusService.Add(campus);
                        _campusService.SaveChanges();
                        SetAlert("Add Campus success", "success");
                    } catch(Exception e)
                    {
                        return View(viewModel);
                    }
                   
                   
                }
              
                if (!ModelState.IsValid)
                {
                    // xy ly loi
                    return View(viewModel);

                }
               
            }
            catch(Exception e)
            {
                SetAlert("Add Campus error", "error");
            }
            return RedirectToAction("Index");
        }

        // GET: Campus/Edit/5
        public ActionResult Edit(int id)
        {
            var campusModel = _campusService.GetById(id);

            if (campusModel == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<Campus, CampusViewModel>(campusModel);

            return View(viewModel);
        }

        // POST: Campus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CampusViewModel viewModel)
        {
            try
            {
                var validateName = _campusService.GetCampusDuplicate(viewModel.ID, viewModel.Name.Trim());
                string MatchPhonePattern = @"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$";
                Regex rx = new Regex(MatchPhonePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (!rx.IsMatch(viewModel.Telephone.Trim()))
                {
                    ModelState.AddModelError("Telephone", "Telephone must numbers");
                    return View(viewModel);
                }
                if (validateName != null)
                {
                    ModelState.AddModelError("Name", "Campus Name already exists");
                }
                if (ModelState.IsValid)
                {
                  
                    // TODO: Add update logic here
                    var campus = _campusService.GetById(viewModel.ID);
                    campus.Name = viewModel.Name.Trim();
                    campus.Address = viewModel.Address;
                    campus.Telephone = viewModel.Telephone.Trim();
                    campus.Active = viewModel.Active;
                    _campusService.Update(campus);
                    _campusService.SaveChanges();
                    SetAlert("Update Campus success", "success");
                    return RedirectToAction("Index");
                }
            }catch(Exception e)
            {
                SetAlert("Update Campus error", "error");
            }
            return View(viewModel);
        }
    }
}