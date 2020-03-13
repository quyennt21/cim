using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;

namespace CIM.Service
{
    public interface IRateService
    {
        void Add(Rate rate);

        void SaveChanges();
    }

    public class RateService : IRateService
    {
        private IUnitOfWork _unitOfWork;
        private IRateRepository _rateRepository;

        public RateService(IUnitOfWork unitOfWork, IRateRepository rateRepository)
        {
            this._unitOfWork = unitOfWork;
            this._rateRepository = rateRepository;
        }

        public void Add(Rate rate)
        {
            _rateRepository.Add(rate);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}