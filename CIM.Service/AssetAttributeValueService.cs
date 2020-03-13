using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System.Collections.Generic;

namespace CIM.Service
{
    public interface IAssetAttributeValueService
    {
        void Add(AssetAttributeValue assetAttributeValue);

        void Update(AssetAttributeValue assetAttributeValue);

        AssetAttributeValue GetById(int id, string[] includes = null);

        IEnumerable<AssetAttributeValue> GetByAssetId(int id, string[] includes = null);

        IEnumerable<AssetAttributeValue> GetAll(string[] includes = null);

        AssetAttributeValue SearchAttributeValue(int assetID, int attributeID);

        void SaveChanges();
    }

    public class AssetAttributeValueService : IAssetAttributeValueService
    {
        private IUnitOfWork _unitOfWork;
        private IAssetAttributeValueRepository _assetAttributeValueRepository;

        public AssetAttributeValueService(IUnitOfWork unitOfWork, IAssetAttributeValueRepository assetAttributeValueRepository)
        {
            this._unitOfWork = unitOfWork;
            this._assetAttributeValueRepository = assetAttributeValueRepository;
        }

        public void Add(AssetAttributeValue assetAttributeValue)
        {
            _assetAttributeValueRepository.Add(assetAttributeValue);
        }

        public IEnumerable<AssetAttributeValue> GetAll(string[] includes = null)
        {
            return _assetAttributeValueRepository.GetAll(includes);
        }

        public AssetAttributeValue GetById(int id, string[] includes = null)
        {
            return _assetAttributeValueRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public IEnumerable<AssetAttributeValue> GetByAssetId(int id, string[] includes = null)
        {
            return _assetAttributeValueRepository.GetByConditions(x => x.AssetID == id, includes);
        }

        public AssetAttributeValue SearchAttributeValue(int assetID, int attributeID)
        {
            AssetAttributeValue assetAttributeValue;

            assetAttributeValue = _assetAttributeValueRepository.GetSigleByConditions(l => l.AssetAttributeID.Equals(attributeID) && l.AssetID.Equals(assetID));

            return assetAttributeValue;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void Update(AssetAttributeValue assetAttributeValue)
        {
            _assetAttributeValueRepository.Update(assetAttributeValue);
        }
    }
}