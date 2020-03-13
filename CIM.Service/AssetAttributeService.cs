using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System.Collections.Generic;

namespace CIM.Service
{
    public interface IAssetAttributeService
    {
        void Add(AssetTypeAttribute assetAttribute);

        void Update(AssetTypeAttribute assetAttribute);

        AssetTypeAttribute GetById(int id, string[] includes = null);

        IEnumerable<AssetTypeAttribute> GetAll(string[] includes = null);

        /// <summary>
        /// Get List Asset Attribute by AssetTypeID
        /// </summary>
        /// <param name="assetTypeID"></param>
        /// <returns></returns>
        IEnumerable<AssetTypeAttribute> GetAssetAttributes(int assetTypeID);

        AssetTypeAttribute GetAttributeByName(string name, int assetTypeID);

        void SaveChanges();
    }

    public class AssetAttributeService : IAssetAttributeService
    {
        private IUnitOfWork _unitOfWork;
        private IAssetAttributeRepository _assetAttributeRepository;

        public AssetAttributeService(IUnitOfWork unitOfWork, IAssetAttributeRepository assetAttributeRepository)
        {
            this._unitOfWork = unitOfWork;
            this._assetAttributeRepository = assetAttributeRepository;
        }

        public void Add(AssetTypeAttribute assetAttribute)
        {
            _assetAttributeRepository.Add(assetAttribute);
        }

        public IEnumerable<AssetTypeAttribute> GetAll(string[] includes = null)
        {
            return _assetAttributeRepository.GetAll(includes);
        }

        public IEnumerable<AssetTypeAttribute> GetAssetAttributes(int assetTypeID)
        {
            return _assetAttributeRepository.GetByConditions(at => at.AssetTypeID == assetTypeID && at.Active.Equals(true));
        }

        public AssetTypeAttribute GetAttributeByName(string name, int assetTypeID)
        {
            AssetTypeAttribute attribute = null;
            attribute = _assetAttributeRepository.GetSigleByConditions(a => a.Name.Equals(name) && a.AssetTypeID.Equals(assetTypeID));
            return attribute;
        }

        public AssetTypeAttribute GetById(int id, string[] includes = null)
        {
            return _assetAttributeRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void Update(AssetTypeAttribute assetAttribute)
        {
            _assetAttributeRepository.Update(assetAttribute);
        }
    }
}