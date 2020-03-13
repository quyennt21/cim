using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using CIM.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CIM.UnitTest.Services
{
    [TestClass]
    public class AreaServiceTest
    {
        private IDbFactory _dbFactory;
        private IUnitOfWork _unitOfWork;
        private IAreaRepository _areaRepository;
        private IAreaService _areaService;

        [TestInitialize]
        public void Initialize()
        {
            _dbFactory = new DbFactory();
            _unitOfWork = new UnitOfWork(_dbFactory);
            _areaRepository = new AreaRepository(_dbFactory);
            _areaService = new AreaService(_unitOfWork, _areaRepository);
        }

        [TestMethod]
        public void AreaService_Add()
        {
            Area area = new Area
            {
                LocationID = 1,
                Name = "Area test",
                AreaCode = "UT_Area_service",
                Description = "tet add area",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Active = true
            };
            
            _areaService.Add(area);
            _areaService.SaveChanges();
            Assert.IsNotNull(_areaService.GetAreaByCode("UT_Area_service"));
        }

        [TestMethod]
        public void AreaService_GetAreaByCode()
        {
            var actual = _areaService.GetAreaByCode("UT_Area_service");

            var expected = "UT_Area_service";

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.AreaCode);
        }

    }
}