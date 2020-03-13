using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using CIM.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIM.UnitTest.Services
{
    [TestClass]
    public class AssetServiceTest
    {
        IAssetService _assetService;
        IUnitOfWork _unitOfWork;
        IAssetRepository _assetRepository;
        IDbFactory _dbFactory;
        [TestInitialize]
        public void Initialize()
        {
            _dbFactory = new DbFactory();
            _unitOfWork = new UnitOfWork(_dbFactory);
            _assetRepository = new AssetRepository(_dbFactory);
            _assetService = new AssetService(_unitOfWork,_assetRepository);
        }
        [TestMethod]
        public void AddAssetService()
        {
            Asset assetTest = new Asset
            {
                AssetTypeID = 1,
                AreaID = 1,
                Name = "Unit test Name",
                AssetCode = "UTN1",
                StartDate = DateTime.Now,
                Quantity = 1,
                Description = "Test",
                ApplicationUserID = 1,
                Active = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            var checkAdd=_assetService.Add(assetTest,new List<AssetAttributeValue>());
            Assert.AreEqual(true, checkAdd);
        }

        [TestMethod]
        public void UpdateAssetService()
        {
            var asset = _assetService.GetAll().FirstOrDefault();
            if (asset != null)
            {
                asset.Name = "Unit test update";
                _assetService.Update(asset);
                _assetService.SaveChanges();
            }
            Assert.AreEqual("Unit test update", asset.Name);
        }
        [TestMethod]
        public void GetAllAssetService()
        {
            var listAsset = _assetService.GetAll().ToList();
            var expected = 376;
            Assert.AreEqual(expected, listAsset.Count);
        }
        [TestMethod]
        public void GetTypeSearchService()
        {
            var listTypeSearch = _assetService.ListTypeSearch();
            var expected = 5;
            Assert.AreEqual(expected, listTypeSearch.Count);
        }
        [TestMethod]
        public void GetAllAssetByIDService()
        {
            int idArea = 1;
            var listAll = _assetService.GetAllByAreaID(idArea).ToList();
            var expected = 36;
            Assert.AreEqual(expected, listAll.Count);
        }
        [TestMethod]
        public void GetAllByAreaService()
        {
            int idArea = 1;
            int assetID = 1;
            var listAll = _assetService.GetAllByArea(assetID,idArea).ToList();
            var expected = 35;
            Assert.AreEqual(expected, listAll.Count);
        }
        [TestMethod]
        public void GetAllAssetAreaService()
        {
            int idArea = 2;
            var listAll = _assetService.GetAllAssetArea(idArea).ToList();
            var expected = 9;
            Assert.AreEqual(expected, listAll.Count);
        }
        [TestMethod]
        public void GetAllAssetIntoAreaService()
        {
            int idArea = 1;
            int assetID = 1;
            var listAll = _assetService.GetAllAssetIntoArea(assetID, idArea).ToList();
            var expected = 34;
            Assert.AreEqual(expected, listAll.Count);
        }
        [TestMethod]
        public void GetSearchService()
        {
            var listAll = _assetService.GetAllBySearch("","--All--","--All--",null,null).ToList();
            var expected = 376;
            Assert.AreEqual(expected, listAll.Count);
        }
        [TestMethod]
        public void GetAssetByCodeService()
        {
            string assetCode = "A202";
            var asset = _assetService.GetAssetByCode(assetCode);
            var expected = "202";
            Assert.AreEqual(expected, asset.Name);
        }

    }
}
