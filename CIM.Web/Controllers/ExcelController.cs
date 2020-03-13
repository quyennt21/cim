using CIM.Model.Models;
using CIM.Service;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize(Roles = "System Admin,Manager")]
    public class ExcelController : Controller
    {
        private IAreaService areaService;
        private IAssetAttributeService attributeService;
        private IAssetAttributeValueService attributeValueService;
        private ILocationService locationService;
        private IAssetTypeService assetTypeService;
        private IAssetService assetService;
        private ICampusService campusService;
        private IAssetAttributeService assetAttributeService;
        private ApplicationUserManager _userManager;

        public ExcelController(IAreaService areaService, IAssetAttributeService attributeService,
            IAssetAttributeValueService attributeValueService, ILocationService locationService,
            IAssetTypeService assetTypeService, IAssetService assetService, ICampusService campusService,
            IAssetAttributeService assetAttributeService, ApplicationUserManager _userManager)
        {
            this.areaService = areaService;
            this.attributeService = attributeService;
            this.attributeValueService = attributeValueService;
            this.locationService = locationService;
            this.assetTypeService = assetTypeService;
            this.assetService = assetService;
            this.campusService = campusService;
            this.assetAttributeService = assetAttributeService;
            this._userManager = _userManager;
        }

        private ExcelService excelService;

        public ActionResult Download(string filename)
        {
            string path = Server.MapPath("~/Data/Excels/Templates/" + filename + ".xlsx");

            FileInfo file = new FileInfo(path);
            ExcelPackage excelPackage = new ExcelPackage(file);
            ExcelWorkbook excelWorkbook = excelPackage.Workbook;
            ExcelWorksheet excelWorksheet = excelWorkbook.Worksheets.First();
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xlsx");
                excelPackage.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            return View();
        }

        public ActionResult DownloadAsset(string assetType)
        {
            string path = Server.MapPath("~/Data/Excels/Templates/Asset.xlsx");

            FileInfo file = new FileInfo(path);
            ExcelPackage excelPackage = new ExcelPackage(file);
            ExcelWorkbook excelWorkbook = excelPackage.Workbook;
            ExcelWorksheet excelWorksheet = excelWorkbook.Worksheets.First();
            AssetType a = new AssetType();
            a = assetTypeService.GetAssetTypeByName(assetType);
            List<AssetTypeAttribute> list = assetAttributeService.
                GetAssetAttributes(a.ID).ToList<AssetTypeAttribute>();

            for (int i = 0; i < list.Count; i++)
            {
                excelWorksheet.Cells[2, i + 10].Value = list[i].Name;
                excelWorksheet.Cells[2, i + 10].Style.Font.Bold = true;
                excelWorksheet.Cells[2, i + 10].Style.WrapText = true;
                excelWorksheet.Cells[2, i + 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                excelWorksheet.Cells[2, i + 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                excelWorksheet.Cells[2, i + 10].Style.Fill.BackgroundColor.SetColor(Color.Aqua); ;
            }

            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + assetType + ".xlsx");
                excelPackage.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            return View();
        }

        public ActionResult Index()
        {
            //Session["current"] = assetType;

            return View();
        }

        public ActionResult ImportLocation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportLocation(HttpPostedFileBase excelfile)
        {
            excelService = new ExcelService(areaService, attributeService, attributeValueService, locationService, assetTypeService, assetService, campusService);

            if (excelfile == null)
            {
                ViewBag.Error = " Excel file is not exist.";
                return View();
            }

            if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
            {
                string filename = excelfile.FileName;
                string path = Server.MapPath("~/Data/Excels/" + filename);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                excelfile.SaveAs(path);
                FileInfo file = new FileInfo(path);

                var package = new ExcelPackage(file);
                ExcelWorksheet worksheetLocation = package.Workbook.Worksheets[1];
                if (excelService.ImportLoctionExcel(worksheetLocation))
                {
                    ViewBag.Error = "Import sucessful!";
                }
                else
                {
                    ViewBag.Error = " Format of excel is wrong/Campus is not exist.";
                }
            }
            else
            {
                ViewBag.Error = "Just accept excel file !.";
            }
            return View();
        }

        public ActionResult ImportArea()
        {
            //Session["current"] = assetType;

            return View();
        }

        [HttpPost]
        public ActionResult ImportArea(HttpPostedFileBase excelfile)
        {
            // getassetType = Session["current"].ToString();
            excelService = new ExcelService(areaService, attributeService, attributeValueService, locationService, assetTypeService, assetService, campusService);

            if (excelfile == null)
            {
                ViewBag.Error = "Excel file is not exist.";
                return View();
            }
            if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
            {
                string filename = excelfile.FileName;
                string path = Server.MapPath("~/Data/Excels/" + filename);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                excelfile.SaveAs(path);
                FileInfo file = new FileInfo(path);

                var package = new ExcelPackage(file);
                ExcelWorksheet worksheetArea = package.Workbook.Worksheets[1];
                if (excelService.ImportAreaExcel(worksheetArea))
                {
                    ViewBag.Error = "Import Thành Công.";
                }
                else
                {
                    ViewBag.Error = "Format Excel is wrong .";
                }
            }
            else
            {
                ViewBag.Error = "Just accept excel file .";
            }
            return View();
        }

        public ActionResult ImportAsset(string assetType)
        {
            //Session["current"] = assetType;
            SelectList cateList = new SelectList(assetTypeService.GetAll().Where(a => a.Active), "Name", "Name");
            ViewBag.listTypeSearch = cateList;

            return View();
        }

        [HttpPost]
        public ActionResult ImportAsset(HttpPostedFileBase excelfile)
        {
            SelectList cateList = new SelectList(assetTypeService.GetAll().Where(a => a.Active), "Name", "Name");

            ViewBag.listTypeSearch = cateList;

            excelService = new ExcelService(areaService, attributeService, attributeValueService, locationService, assetTypeService, assetService, campusService);

           
            if (excelfile == null)
            {
                ViewBag.Error = " file is null";
                return View();
            }
            ViewBag.FileName = "File:" + excelfile.FileName;
            if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
            {
                string filename = excelfile.FileName;
                string path = Server.MapPath("~/Data/Excels/" + filename);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                excelfile.SaveAs(path);
                FileInfo file = new FileInfo(path);

                var package = new ExcelPackage(file);
                ExcelWorksheet worksheetAsset = package.Workbook.Worksheets[1];

                string username = User.Identity.GetUserName();
                ApplicationUser user = _userManager.FindByName(username);

                if (user == null)
                {
                    ViewBag.Error = "User is not exsit in current database please import ";
                    return View();
                }

                Task<string> task = excelService.ImportAssetTaskAsync(worksheetAsset, user.Id);
                ViewBag.Error = "Status:" + task.Result;
            }
            else
            {
                ViewBag.Error = "Just accept excel file .";
            }
            return View();
        }
    }
}