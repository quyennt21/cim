using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CIM.UnitTest.Repositories
{
    [TestClass]
    public class AreaRepositoryTest
    {
        private IDbFactory _dbFactory;
        private IUnitOfWork _unitOfWork;
        private IAreaRepository _areaRepository;

        [TestInitialize]
        public void Initialize()
        {
            _dbFactory = new DbFactory();
            _unitOfWork = new UnitOfWork(_dbFactory);
            _areaRepository = new AreaRepository(_dbFactory);
        }

        [TestMethod]
        public void AreaRepository_Add()
        {
            Area areaTest = new Area
            {
                Name = "Area test",
                AreaCode = "A_UT",
                Description = "Add area unit test",
                LocationID = 1
            };
            var actual = _areaRepository.Add(areaTest);
            _unitOfWork.Commit();

            var expected = "A_UT";

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.AreaCode);
        }

        [TestMethod]
        public void AreaRepository_Update()
        {
            Expression<Func<Area, bool>> condition = x => x.ID == 839;

            //get area id = 839
            var area = _areaRepository.GetSigleByConditions(condition);

            var expected = area.Name + "_Update_test";
            area.Name = expected;

            _areaRepository.Update(area);
            _unitOfWork.Commit();

            var actual = _areaRepository.GetSigleByConditions(condition);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Name);
        }

        [TestMethod]
        public void AreaRepository_GetSingleByConditions()
        {
            // get area with id = 1
            Expression<Func<Area, bool>> condition = x => x.ID == 1;
            var actual = _areaRepository.GetSigleByConditions(condition);

            var expected = 1;

            Assert.AreEqual(expected, actual.ID);
        }

        [TestMethod]
        public void AreaRepository_GetByConditions()
        {
            // get area has condition is active and location is beta with location id = 2
            Expression<Func<Area, bool>> conditions = x => x.Active == true && x.LocationID == 2;
            var actual = _areaRepository.GetByConditions(conditions).ToList();

            var expected = 76;

            Assert.AreEqual(expected, actual.Count());
        }

        [TestMethod]
        public void AreaRepository_GetAll()
        {
            var actual = _areaRepository.GetAll().ToList();

            var expected = 839;

            Assert.AreEqual(expected, actual.Count());
        }
    }
}