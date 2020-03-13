using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIM.UnitTest.Repositories
{
    [TestClass]
    public class AssetRepostitoryTest
    {
        private IDbFactory _dbFactory;
        private IUnitOfWork _unitOfWork;
        private IAssetRepository _assetRepository;

        [TestInitialize]
        public void Initialize()
        {
            _dbFactory = new DbFactory();
            _unitOfWork = new UnitOfWork(_dbFactory);
            _assetRepository = new AssetRepository(_dbFactory);
        }

        [TestMethod]
        public void AssetRepositoryAdd()
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
            var actual = _assetRepository.Add(assetTest);
            _unitOfWork.Commit();

            var expected = "UTN1";

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.AssetCode);
        }

    }
}
