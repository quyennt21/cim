using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CIM.Service
{
    public interface IAssetTypeService
    {
        void Add(AssetType assetType);

        void Update(AssetType assetType);

        AssetType GetById(int id, string[] includes = null);

        IEnumerable<AssetType> GetActive(string[] includes = null);

        IEnumerable<AssetType> GetAll(string[] includes = null);

        IEnumerable<AssetType> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        IEnumerable<AssetType> Search(string searchString, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        bool Add(AssetType assetType, List<AssetTypeAttribute> assetTypeAttribute);

        bool Update(AssetType assetType, IEnumerable<AssetTypeAttribute> assetTypeAttributes);

        void Delete(AssetType assetType, IEnumerable<AssetTypeAttribute> assetTypeAttributes);

        AssetType GetAssetTypeByName(string Name);

        void SaveChanges();
    }

    public class AssetTypeService : IAssetTypeService
    {
        private IUnitOfWork _unitOfWork;
        private IAssetTypeRepository _assetTypeRepository;
        private IAssetAttributeRepository _assetAttributeRepository;

        public AssetTypeService(IUnitOfWork unitOfWork, IAssetTypeRepository assetTypeRepository)
        {
            this._unitOfWork = unitOfWork;
            this._assetTypeRepository = assetTypeRepository;
        }

        public void Add(AssetType assetType)
        {
            _assetTypeRepository.Add(assetType);
        }

        public IEnumerable<AssetType> GetAll(string[] includes = null)
        {
            return _assetTypeRepository.GetAll(includes);
        }

        public IEnumerable<AssetType> Search(string searchString, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var query = _assetTypeRepository.GetByConditions(at => at.Active, includes);
            if (!String.IsNullOrEmpty(searchString))
            {
                query = _assetTypeRepository.GetByConditions(at => at.Active && at.Name.ToLower().Contains(searchString.ToLower().Trim()), includes);
            }
            totalRow = query.Count();
            return query.OrderBy(at => at.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        //        private string ConvertToUnSign(string input)
        //        {
        //            input = input.Trim();
        //            for (int i = 0x20; i < 0x30; i++)
        //            {
        //                input = input.Replace(((char)i).ToString(), " ");
        //            }
        //            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
        //            string str = input.Normalize(NormalizationForm.FormD);
        //            string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
        //            while (str2.IndexOf("?") >= 0)
        //            {
        //                str2 = str2.Remove(str2.IndexOf("?"), 1);
        //            }
        //            return str2;
        //        }

        public bool Add(AssetType assetType, List<AssetTypeAttribute> listAssetAttributes)
        {
            return _assetTypeRepository.Add(assetType, listAssetAttributes);
        }

        public bool Update(AssetType assetType, IEnumerable<AssetTypeAttribute> assetTypeAttributes)
        {
            return _assetTypeRepository.Update(assetType, assetTypeAttributes);
        }

        public AssetType GetById(int id, string[] includes = null)
        {
            return _assetTypeRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void Update(AssetType assetType)
        {
            _assetTypeRepository.Update(assetType);
        }

        public AssetType GetAssetTypeByName(string Name)
        {
            AssetType assetType = null;

            assetType = _assetTypeRepository.GetSigleByConditions(a => a.Name.Equals(Name));

            return assetType;
        }

        public IEnumerable<AssetType> GetActive(string[] includes = null)
        {
            return _assetTypeRepository.GetByConditions(at => at.Active, includes);
        }

        public void Delete(AssetType assetType, IEnumerable<AssetTypeAttribute> assetTypeAttributes)
        {
            _assetTypeRepository.Delete(assetType, assetTypeAttributes);
        }

        public IEnumerable<AssetType> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var query = _assetTypeRepository.GetByConditions(x => x.Active, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}