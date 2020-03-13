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
    public class AssetLogController : Controller
    {
        private IAssetLogService _assetLogService;

        public AssetLogController(IAssetLogService assetLogService)
        {
            this._assetLogService = assetLogService;
        }

        // GET: AssetLog
        public ActionResult Index(int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var assetLogsModel = _assetLogService.GetAllPaging(out totalRow, page, pageSize, new string[] { "Asset", "ApplicationUser" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var assetLogViewModel = Mapper.Map<IEnumerable<AssetLog>, IEnumerable<AssetLogViewModel>>(assetLogsModel);

            var paginationSet = new PaginationSet<AssetLogViewModel>()
            {
                Items = assetLogViewModel,
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

        // GET: AssetLog
        public ActionResult Search(string assetSearch, string userSearch, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));

            int totalRow = 0;

            var assetLogsModel = _assetLogService.Search(assetSearch, userSearch, out totalRow, page, pageSize, new string[] { "Asset", "ApplicationUser" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var assetLogViewModel = Mapper.Map<IEnumerable<AssetLog>, IEnumerable<AssetLogViewModel>>(assetLogsModel);

            var paginationSet = new PaginationSet<AssetLogViewModel>()
            {
                Items = assetLogViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            ViewBag.assetSearch = assetSearch;
            ViewBag.userSearch = userSearch;

            ViewBag.query = new
            {
                userSearch = userSearch,
                assetSearch = assetSearch,
                page = page
            };

            return View("Index", paginationSet);
        }

        // GET: AssetLog/Details/5
        public ActionResult Details(int id)
        {
            var assetLogModel = _assetLogService.GetById(id, new string[] { "Asset", "ApplicationUser" });

            if (assetLogModel == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<AssetLog, AssetLogViewModel>(assetLogModel);

            return View(viewModel);
        }
    }
}