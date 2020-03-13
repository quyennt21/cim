using CIM.Common;
using CIM.Model.Models;
using CIM.Service;
using CIM.Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize(Roles = ("System Admin, Manager"))]
    public class QRController : Controller
    {
        // GET: QrManager
        private IAssetService _assetService;

        private QrAssetService qrAssetService = new QrAssetService();
        private ILocationService _locationService;

        public QRController(IAssetService assetService, ILocationService locationService)
        {
            _locationService = locationService;
            _assetService = assetService;
        }

        public ActionResult Index(string searchString, string locationSearch, string typeSearch, string dateFrom, string dateTo, int page = 1)
        {
            List<QrAssets> listPrint;
            listPrint = (List<QrAssets>)Session["var"];
            if (listPrint == null)
            {
                listPrint = new List<QrAssets>();
            }
            Session["var"] = null;
            Session["all"] = null;

            int pageSize = 13;
            int totalRow = 0;
            List<String> listLocation = new List<string>();
            listLocation.Add("--All--");
            listLocation.AddRange(_locationService.GetAll().Select(l => l.Name).ToList());
            SelectList locationS = new SelectList(listLocation);
            ViewBag.locationSearch = locationS;
      
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);
            QrAssetViewModel viewModel = new QrAssetViewModel();
            QrAssetViewModel allViewModel = new QrAssetViewModel();
            viewModel.lstQr = new List<QrAssets>();
            SelectList cateList = new SelectList(_assetService.ListTypeSearch());
            ViewBag.listTypeSearch = cateList;
            ViewBag.totalPage = 0;
            ViewBag.totalRow = 0;
            ViewBag.searchString = searchString;
            ViewBag.searchType = typeSearch;
            ViewBag.dateFrom = dateFrom;
            ViewBag.dateTo = dateTo;
            ViewBag.stringlocationsearch = locationSearch;
            Session["var"] = listPrint;
            Session["all"] = allViewModel.lstQr;
            return View(viewModel);
        }

        public ActionResult Search(string searchString, string locationSearch, string typeSearch, string dateFrom, string dateTo, int page = 1)
        {
            List<QrAssets> listPrint;
            listPrint = (List<QrAssets>)Session["var"];
            if (listPrint == null)
            {
                listPrint = new List<QrAssets>();
            }
            Session["var"] = null;
            Session["all"] = null;

            int pageSize = 13;
            int totalRow = 0;
            List<String> listLocation = new List<string>();
            listLocation.Add("--All--");
            listLocation.AddRange(_locationService.GetAll().Select(l => l.Name).ToList());
            SelectList locationS = new SelectList(listLocation);
            ViewBag.locationSearch = locationS;
            var assetModel = _assetService.Search(searchString, locationSearch, typeSearch, dateFrom,
                dateTo, out totalRow, page, pageSize, new string[]
                { "Area", "AssetType", "Area.Location", "ApplicationUser" });

            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);
            QrAssetViewModel viewModel = new QrAssetViewModel();
            QrAssetViewModel allViewModel = new QrAssetViewModel();

            var allAssetBySearch = _assetService.GetAllBySearch(searchString, locationSearch,
                typeSearch, dateFrom, dateTo, new string[]
                { "Area", "AssetType", "Area.Location", "ApplicationUser" });

            allViewModel = qrAssetService.listViewModel(allAssetBySearch.ToList<Asset>(), null);
            viewModel = qrAssetService.listViewModel(assetModel.ToList<Asset>(), listPrint);

            SelectList cateList = new SelectList(_assetService.ListTypeSearch());
            ViewBag.listTypeSearch = cateList;
            ViewBag.totalPage = totalPage;
            ViewBag.totalRow = totalRow;
            ViewBag.searchString = searchString;
            ViewBag.searchType = typeSearch;
            ViewBag.dateFrom = dateFrom;
            ViewBag.dateTo = dateTo;
            ViewBag.stringlocationsearch = locationSearch;
            Session["stringlocationsearch"] = locationSearch;
            Session["typeSearch"] = typeSearch;
            Session["searchString"] = searchString;
            Session["page"] = page;
            Session["var"] = listPrint;
            Session["all"] = allViewModel.lstQr;
            return View("Index",viewModel);
        }

        public ActionResult Generator(string url)
        {
            string domain = Request.Url.Scheme + System.Uri.SchemeDelimiter
                  + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

            if (String.IsNullOrEmpty(url))
            {
                url = "";
            }

            domain = domain + "/Asset/Details/" + url;
            var bitmapBytes = QRHelper.Generator(domain);

            return File(bitmapBytes, "image/jpeg"); //Return as file result
        }

        [HttpPost]
        public ActionResult PrintQr(QrAssetViewModel viewModel)
        {
            List<QrAssets> listPrint = Session["var"] as List<QrAssets>;
            if (listPrint == null)
            {
                listPrint = new List<QrAssets>();
            }

            if (viewModel.lstQr != null)
            {
                QrAssetViewModel mViewModel = qrAssetService.getListChecked(viewModel, listPrint);
                Session["var"] = null;
                Session["var"] = mViewModel.lstQr;

                return RedirectToAction("PrintQr");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Resetlist()
        {
            Session["var"] = null;
            Session["all"] = null;
            return RedirectToAction("Search");
        }

        public ActionResult PrintQr(int page = 1, string status = "notall")
        {
            ViewBag.status = status;
            int pageSize = 24;
            ViewBag.searchString = Session["searchString"] as string;
            ViewBag.typeSearch = Session["typeSearch"] as string;
            ViewBag.locationsearch = Session["stringlocationsearch"] as string;
            int mpage = 1;
            Int32.TryParse(Session["page"] as string, out mpage);
                if (mpage <= 0)
                    {
                        mpage = 1;
                    }
            ViewBag.page = mpage;
           QrAssetViewModel allViewModel = new QrAssetViewModel();
            List<QrAssets> listPrint;
            if (status.Equals("all"))
            {
                listPrint = Session["all"] as List<QrAssets>;
            }
            else
            {
                listPrint = Session["var"] as List<QrAssets>;
            }

            if (listPrint == null)
            {
                listPrint = new List<QrAssets>();
            }

            if (allViewModel == null)
            {
                return RedirectToAction("Index");
            }
            allViewModel.lstQr = listPrint;
            if (allViewModel.lstQr != null)
            {
                int totalPage = (int)Math.Ceiling((double)(allViewModel.lstQr.Count() + 1) / pageSize);
                ViewBag.totalpage = totalPage;
                ViewBag.totalRow = allViewModel.lstQr.Count() + 1;
                QrAssetViewModel myView = qrAssetService.getMyPage(page - 1, pageSize, allViewModel);
                return View(myView);
            }

            return RedirectToAction("Index");
        }
    }
}