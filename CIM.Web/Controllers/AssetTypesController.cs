using AutoMapper;
using CIM.Common;
using CIM.Model.Models;
using CIM.Service;
using CIM.Web.Infrastructure;
using CIM.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize(Roles = "System Admin,Manager")]
    public class AssetTypesController : BaseController
    {
        private IAssetTypeService _assetTypeService;
        private IAssetAttributeService _assetAttributeService;

        public AssetTypesController(IAssetTypeService assetTypeService, IAssetAttributeService assetAttributeService)
        {
            this._assetTypeService = assetTypeService;
            this._assetAttributeService = assetAttributeService;
        }

        // GET: AssetTypes
        public ActionResult Index(int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));
            int totalRow = 0;
            var assetTypesModel = _assetTypeService.GetAllPaging(out totalRow, page, pageSize);

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var assetTypeViewModel = Mapper.Map<IEnumerable<AssetType>, IEnumerable<AssetTypeViewModel>>(assetTypesModel);

            var paginationSet = new PaginationSet<AssetTypeViewModel>()
            {
                Items = assetTypeViewModel,
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
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));
            int totalRow = 0;

            var assetTypeModel = _assetTypeService.Search(searchString, out totalRow, page, pageSize);

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var assetTypeViewModels = Mapper.Map<IEnumerable<AssetType>, IEnumerable<AssetTypeViewModel>>(assetTypeModel);

            var paginationSet = new PaginationSet<AssetTypeViewModel>()
            {
                Items = assetTypeViewModels,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
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

        //GET: AssetTypes/Create
        public ActionResult Create()
        {
            List<String> listAssetType = _assetTypeService.GetAll().Select(at => at.Name).ToList();
            ViewBag.listAssetType = listAssetType;
            return View();
        }

        [HttpPost]
        //get AssetType and attribute
        public JsonResult Create(string jsonAssetAttribute, string nameAssetType)
        {
            string error = "false";
            if (!nameAssetType.Equals("null"))
            {
                var listAssetAttributes = new List<AssetTypeAttribute>();
                if (!jsonAssetAttribute.Equals("[]"))
                {
                    listAssetAttributes = JsonConvert.DeserializeObject<List<AssetTypeAttribute>>(jsonAssetAttribute);
                }
                string nameType = nameAssetType.Substring(1, nameAssetType.Length - 2);
                var listAllAssetType = _assetTypeService.GetAll().Where(x=>x.Name.ToLower().Trim().Equals(nameType.Trim().ToLower())).SingleOrDefault();
                if (listAllAssetType != null)
                {
                    error = "Name Asset Type already exists";
                }
                else
                {
                    AssetType assetType = new AssetType()
                    {
                        Name = nameType,
                        Active = true
                    };
                    bool checkAdd = _assetTypeService.Add(assetType, listAssetAttributes);
                    if (checkAdd)
                    {
                        SetAlert("Add Asset Type success", "success");
                    }
                    else
                    {
                        SetAlert("Add Asset Type error", "error");
                    }
                }

            }
            return Json(error);
        }

        [HttpPost]
        public JsonResult IsAssetType(string Name)
        {
            return Json(IsAssetTypeAvailable(Name));
        }

        public bool IsAssetTypeAvailable(string Name)
        {
            bool status = false;
            var listAssetType = _assetTypeService.GetActive();
            foreach (var assetType in listAssetType)
            {
                if (assetType.Name.ToUpper().Equals(Name.ToUpper()))
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

        public ActionResult Details(int id)
        {
            var assetTypeViewModel = _assetTypeService.GetById(id);

            if (assetTypeViewModel == null)
            {
                return HttpNotFound();
            }

            var listAttributes = _assetAttributeService.GetAssetAttributes(id);
            var assetAttributeViewModel = Mapper.Map<IEnumerable<AssetTypeAttribute>, IEnumerable<AssetTypeAttributeViewModel>>(listAttributes);
            ViewBag.AssetAttribute = assetAttributeViewModel;
            var viewModel = Mapper.Map<AssetType, AssetTypeViewModel>(assetTypeViewModel);

            return View(viewModel);
        }

        public ActionResult Delete(int id)
        {
            var assetTypeViewModel = _assetTypeService.GetById(id);

            if (assetTypeViewModel == null)
            {
                return HttpNotFound();
            }

            var listAttributes = _assetAttributeService.GetAssetAttributes(id);
            var assetAttributeViewModel = Mapper.Map<IEnumerable<AssetTypeAttribute>, IEnumerable<AssetTypeAttributeViewModel>>(listAttributes);
            ViewBag.AssetAttribute = assetAttributeViewModel;
            var viewModel = Mapper.Map<AssetType, AssetTypeViewModel>(assetTypeViewModel);

            return View(viewModel);
        }

        // POST: AssetTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            AssetType assetType = _assetTypeService.GetById(id);
            var listAttribute = _assetAttributeService.GetAssetAttributes(id);
            _assetTypeService.Delete(assetType, listAttribute);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            AssetType assetType = _assetTypeService.GetById(id);
            var assetTypeModel = Mapper.Map<AssetType, AssetTypeViewModel>(assetType);
            var listAttribute = _assetAttributeService.GetAssetAttributes(id);
            var assetTypeAttributeViewModels = Mapper.Map<IEnumerable<AssetTypeAttribute>, IEnumerable<AssetTypeAttributeViewModel>>(listAttribute);
            ViewBag.AssetAttribute = assetTypeAttributeViewModels;
            if (assetType == null)
            {
                return HttpNotFound();
            }
            return View(assetTypeModel);
        }

        [HttpPost]
        public JsonResult Edit(string jsonAssetAttributes, string assetTypes)
        {
            string error = "false";
            var listAssetAttributes = JsonConvert.DeserializeObject<List<AssetTypeAttribute>>(jsonAssetAttributes);
            var assetType = JsonConvert.DeserializeObject<AssetType>(assetTypes);
            
            var listAllAssetType = _assetTypeService.GetAll().Where(x => x.Name.ToLower().Trim().Equals(assetType.Name.Trim().ToLower())&&assetType.ID!=x.ID).SingleOrDefault();
            if (listAllAssetType != null)
            {
                error = "Name Asset Type already exists";
            }
            else
            {
                var checkUpdate = _assetTypeService.Update(assetType, listAssetAttributes);
                if (checkUpdate)
                {
                    SetAlert("Update Asset Type success", "success");
                }
                else
                {
                    SetAlert("Update Asset Type error", "error");
                }
            }
          
           
            return Json(error);
        }
    }
}