using System;
using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System.Collections.Generic;
using System.Linq;
using CIM.Common;

namespace CIM.Service
{
    public interface IAssetService
    {
        void Add(Asset asset);
        bool Add(Asset asset, List<AssetAttributeValue> assetAttributeValue);
        void Update(Asset asset);
        void Update(Asset asset, IEnumerable<AssetAttributeValue> assetAttributeValues);
        Asset GetById(int id, string[] includes = null);

        IEnumerable<Asset> GetAll(string[] includes = null);
        IEnumerable<Asset> GetAllAssetIntoArea(int assetId, int areaId, string[] includes = null);

        IEnumerable<Asset> GetAllByAreaID(int areaID, string[] includes = null);
        IEnumerable<Asset> GetAllAssetArea(int areaId, string[] includes = null);

        IEnumerable<Asset> GetAllByArea(int assetId, int areaId, string[] includes = null);

        IEnumerable<Asset> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);
        IEnumerable<Asset> Search(string searchString, string locationSearch, string typeSearch, string dateFrom, string dateTo, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        List<String> ListTypeSearch();

        Asset GetAssetByCode(string code);

        void SaveChanges();

        IEnumerable<Asset> GetAllBySearch(string searchString, string locationSearch, string typeSearch, string dateFrom, string dateTo, string[] includes = null);
        string GetStringAssetCode(int type);
    }

    public class AssetService : IAssetService
    {
        private IUnitOfWork _unitOfWork;
        private IAssetRepository _assetRepository;

        public AssetService(IUnitOfWork unitOfWork, IAssetRepository assetRepository)
        {
            this._unitOfWork = unitOfWork;
            this._assetRepository = assetRepository;
        }

        public void Add(Asset asset)
        {
            _assetRepository.Add(asset);

        }

        public bool Add(Asset asset, List<AssetAttributeValue> assetAttributeValue)
        {
            try
            {
                _assetRepository.Add(asset, assetAttributeValue);
                return true;
            }catch(Exception e)
            {
                return false;
            }

        }

        public IEnumerable<Asset> GetAll(string[] includes = null)
        {
            return _assetRepository.GetAll(includes);
        }

        public IEnumerable<Asset> Search(string searchString, string locationSearch, string typeSearch, string dateFrom, string dateTo, out int totalRow,
            int page = 1, int pageSize = 10, string[] includes = null)
        {
            var assets = _assetRepository.GetAll(includes);
            assets = _assetRepository.listSearch(assets, searchString, locationSearch, typeSearch, dateFrom, dateTo);
            totalRow = assets.Count();
            return assets.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public Asset GetById(int id, string[] includes = null)
        {
            return _assetRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public List<string> ListTypeSearch()
        {
            List<String> listType = new List<string>();
            listType.Add("--All--");
            listType.Add("Asset Name");
            listType.Add("Asset Code");
            listType.Add("Asset Type");
            listType.Add("Name Area");

            return listType;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public Asset GetAssetByCode(string code)
        {
            Asset asset = null;
            asset = _assetRepository.GetSigleByConditions(j => j.AssetCode.Equals(code), new string[] { "Area", "AssetType", "ApplicationUser" });
            return asset;
        }

        public void Update(Asset asset)
        {
            _assetRepository.Update(asset);
        }

        public IEnumerable<Asset> GetAllByAreaID(int areaID, string[] includes = null)
        {
            var assets = _assetRepository.GetAll(includes);
            assets = _assetRepository.listaAreaSearch(assets, areaID);
            return assets;
        }


        public void Update(Asset asset, IEnumerable<AssetAttributeValue> assetAttributeValues)
        {
            _assetRepository.Update(asset, assetAttributeValues);
        }

        public IEnumerable<Asset> GetAllBySearch(string searchString, string locationSearch, string typeSearch, string dateFrom, string dateTo, string[] includes = null)
        {
            var assets = _assetRepository.GetAll(includes);
            assets = _assetRepository.listSearch(assets, searchString, locationSearch, typeSearch, dateFrom, dateTo);
            return assets;
        }

        public IEnumerable<Asset> GetAllByArea(int assetId, int areaId, string[] includes = null)
        {
            return _assetRepository.GetByConditions(x => x.AreaID == areaId && x.ID != assetId, includes);
        }

        public IEnumerable<Asset> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var query = _assetRepository.GetAll(includes);

            totalRow = query.Count();

            return query.OrderByDescending(x => x.Active).ThenBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IEnumerable<Asset> GetAllAssetIntoArea(int assetId, int areaId, string[] includes = null)
        {
            return _assetRepository.GetByConditions(x => x.ID != assetId && x.AreaID == areaId, includes);
        }
        public IEnumerable<Asset> GetAllAssetArea(int areaId, string[] includes = null)
        {
            return _assetRepository.GetByConditions(x => x.AreaID == areaId, includes);
        }

        public string GetStringAssetCode(int type)
        {
            string assetCode = "";
            int indexCode = 0;
            string code = "";
            Asset asset = new Asset();
            if (type == 1)
            {
                assetCode = "GEN";
                asset = _assetRepository.GetByConditions(a => a.AssetCode.Contains("GEN")).OrderBy(a => a.ID).LastOrDefault();
                if (asset == null)
                {
                    indexCode = 1;
                }
                else
                {
                    string test = asset.AssetCode.Substring(3, asset.AssetCode.Length - 3);
                    indexCode = Convert.ToInt32(asset.AssetCode.Substring(3, asset.AssetCode.Length - 3)) + 1;
                }
            }
            else if (type == 2)
            {
                assetCode = "A";
                asset = _assetRepository.GetByConditions(a => a.AssetCode.StartsWith("A")).OrderBy(a => a.ID).LastOrDefault();
                if (asset == null)
                {
                    indexCode = 1;
                }
                else
                {
                    indexCode = Convert.ToInt32(asset.AssetCode.Substring(1, asset.AssetCode.Length - 1)) + 1;
                }

            }
            else
            {
                assetCode = "L";
                asset = _assetRepository.GetByConditions(a => a.AssetCode.StartsWith("L")).OrderBy(a => a.ID).LastOrDefault();
                if (asset == null)
                {
                    indexCode = 1;
                }
                else
                {
                    indexCode = Convert.ToInt32(asset.AssetCode.Substring(1, asset.AssetCode.Length - 1)) + 1;
                }
            }
            code = indexCode.ToString();
            code = code.PadLeft(6, '0');
            assetCode += code;
            return assetCode;
        }
    }
}