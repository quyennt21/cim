using CIM.Model.Models;
using OfficeOpenXml;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace CIM.Service
{
    public class ExcelService
    {
        private IAreaService _areaService;
        private IAssetAttributeService _attributeService;
        private IAssetAttributeValueService _attributeValueService;
        private ILocationService _locationService;
        private IAssetTypeService _assetTypeService;
        private IAssetService _assetService;
        private ICampusService _campusService;

        public ExcelService(IAreaService areaService, IAssetAttributeService attributeService,
            IAssetAttributeValueService attributeValueService, ILocationService locationService,
            IAssetTypeService assetTypeService, IAssetService assetService, ICampusService campusService)
        {
            this._areaService = areaService;
            this._attributeService = attributeService;
            this._attributeValueService = attributeValueService;
            this._locationService = locationService;
            this._assetTypeService = assetTypeService;
            this._assetService = assetService;
            this._campusService = campusService;
        }

        public bool ImportLoctionExcel(ExcelWorksheet locationSheet)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 60, 0)))
            {
                try
                {
                    var colCount = locationSheet.Dimension.End.Column;
                    var rowCount = locationSheet.Dimension.End.Row;
                    for (int i = 2; i <= rowCount; i++)
                    {
                        Location location;
                        string name = locationSheet.Cells[i, 1].Value.ToString();

                        if (_locationService.GetLocationByName(name) != null)
                        {
                            location = _locationService.GetLocationByName(name);
                            location.Name = locationSheet.Cells[i, 1].Value.ToString();
                            location.LocationCode = locationSheet.Cells[i, 2].Value.ToString();
                            location.Description = locationSheet.Cells[i, 3].Value.ToString();
                            string campusName = locationSheet.Cells[i, 4].Value.ToString();
                            location.CampusID = _campusService.GetCampusCode(campusName).ID;
                            location.Active = true;
                            _locationService.Update(location);
                            _locationService.SaveChanges();
                        }
                        else
                        {
                            location = new Location();
                            location.Name = locationSheet.Cells[i, 1].Value.ToString();
                            location.LocationCode = locationSheet.Cells[i, 2].Value.ToString();
                            location.Description = locationSheet.Cells[i, 3].Value.ToString();
                            string campusName = locationSheet.Cells[i, 4].Value.ToString();
                            location.CampusID = _campusService.GetCampusCode(campusName).ID;
                            location.Active = true;
                            _locationService.Add(location);
                            _locationService.SaveChanges();
                        }
                    }
                    scope.Complete();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public bool ImportAreaExcel(ExcelWorksheet areaSheet)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 60, 0)))
            {
                try
                {
                    var rowCount = areaSheet.Dimension.End.Row;
                    for (int i = 2; i <= rowCount; i++)
                    {
                        string areaCode = areaSheet.Cells[i, 1].Value.ToString();
                        string areaName = areaSheet.Cells[i, 2].Value.ToString();
                        string areaDescription = areaSheet.Cells[i, 3].Value.ToString();
                        string LocationName = areaSheet.Cells[i, 4].Value.ToString();
                        Location location = _locationService.GetLocationByName(LocationName);
                        Area area;

                        if (_areaService.GetAreaByCode(areaCode) != null)
                        {
                            area = _areaService.GetAreaByCode(areaCode);
                            area.AreaCode = areaCode;
                            area.Name = areaName;
                            area.LocationID = location.ID;
                            area.Description = areaDescription;
                            area.Active = true;
                            _areaService.Update(area);
                            _areaService.SaveChanges();
                        }
                        else
                        {
                            area = new Area();
                            area.AreaCode = areaCode;
                            area.Name = areaName;
                            area.LocationID = location.ID;
                            area.Description = areaDescription;
                            area.Active = true;
                            _areaService.Add(area);
                            _areaService.SaveChanges();
                        }
                    }
                    scope.Complete();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public async Task<string> ImportAssetTaskAsync(ExcelWorksheet worksheet, int userID)
        {
            string result = await ImportAssetExcelAsync(worksheet, userID);
            return result;
        }

        public async Task<string> ImportAssetExcelAsync(ExcelWorksheet worksheet, int userID)
        {
            string result = "Import fail " + worksheet.Name.Trim();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                               new System.TimeSpan(0, 60, 0)))
            {
                try
                {
                    var colCount = worksheet.Dimension.End.Column;
                    var rowCount = worksheet.Dimension.End.Row;
                    string locationCode = worksheet.Cells[1, 3].Value.ToString().Trim();

                    Location location = _locationService.GetLocationByCode(locationCode);
                    if (location == null)
                    {
                        result = "Do not exist current location code: " + locationCode + ". Please check again or create new!";

                        return result;
                    }

                    for (int i = 3; i <= rowCount; i++)
                    {
                        result = "Error at cell " + i;
                        string areaCode = worksheet.Cells[i, 2].Value.ToString();

                        Area area = _areaService.GetAreaByCode(areaCode);
                        if (area == null)
                        {
                            result = result + " in area collum do not exist current area code:" + areaCode + ". Please check again or create new!";
                            return result;
                        }
                        string assetname = worksheet.Cells[i, 3].Value.ToString();
                        string assetTypeName = worksheet.Cells[i, 4].Value.ToString();
                        string description = worksheet.Cells[i, 8].Value.ToString();
                        bool active = false;
                        if (worksheet.Cells[i, 9].Value.ToString().Equals("1"))
                        {
                            active = true;
                        }

                        string assetCode = worksheet.Cells[i, 7].Value.ToString();
                        int quantity = Int32.Parse(worksheet.Cells[i, 5].Value.ToString().Trim());
                        DateTime startDate = DateTime.Now;
                        try
                        {
                            startDate = Convert.ToDateTime(worksheet.Cells[i, 6].Value.ToString());
                        }
                        catch (Exception e)
                        {
                            //set default date
                            startDate = Convert.ToDateTime("1900-01-01");
                        }
                        AssetType assetTypes = _assetTypeService.GetAssetTypeByName(assetTypeName);
                        if (assetTypes == null)
                        {
                            result = result + "in Assettype collum do not exist current type import: " + assetTypeName;
                            return result;
                        }

                        // check asset is exist or not / if exist update asset
                        Asset asset = _assetService.GetAssetByCode(assetCode);
                        if (asset == null)
                        {
                            Asset newAsset = new Asset();
                            newAsset.Name = assetname;
                            newAsset.AssetCode = assetCode;
                            newAsset.Description = description;
                            newAsset.Quantity = quantity;
                            newAsset.StartDate = startDate;
                            newAsset.AssetTypeID = assetTypes.ID;
                            newAsset.ApplicationUserID = userID;
                            newAsset.AreaID = area.ID;
                            newAsset.Active = active;
                            _assetService.Add(newAsset);
                            asset = newAsset;
                            _assetService.SaveChanges();
                        }
                        else
                        {
                            asset.AssetCode = assetCode;
                            asset.Name = assetname;
                            asset.Description = description;
                            asset.Quantity = quantity;
                            asset.StartDate = startDate;
                            asset.AssetTypeID = assetTypes.ID;
                            asset.AreaID = area.ID;
                            asset.Active = active;
                            _assetService.Update(asset);
                            _assetService.SaveChanges();
                        }

                        for (int j = 10; j <= colCount; j++)
                        {
                            string attributeName = worksheet.Cells[2, j].Value.ToString().Trim();
                            AssetTypeAttribute assetAttribute = _attributeService.GetAttributeByName(attributeName, assetTypes.ID);
                            string value = "";
                            try
                            {
                                if (worksheet.Cells[i, j].Value == null)
                                {
                                    value = "N/A";
                                }
                                value = worksheet.Cells[i, j].Value.ToString().Trim();
                            }
                            catch (Exception e)
                            {
                                value = "N/A";
                            }

                            AssetAttributeValue insertValue = _attributeValueService.SearchAttributeValue(asset.ID, assetAttribute.ID);
                            if (insertValue != null)
                            {
                                insertValue.Value = value;
                                _attributeValueService.Update(insertValue);
                            }
                            else
                            {
                                insertValue = new AssetAttributeValue();
                                insertValue.Value = value;
                                insertValue.AssetID = asset.ID;
                                insertValue.AssetAttributeID = assetAttribute.ID;
                                insertValue.Active = true;
                                _attributeValueService.Add(insertValue);
                            }
                        }
                        result = result + " in Attribute value table.";
                    }

                    _attributeValueService.SaveChanges();
                    result = "Error in Commplete transison.";
                    scope.Complete();
                    result = "Import successfull!";
                    return result;
                }
                catch (Exception)
                {
                    return result;
                }
            }
        }
    }
}