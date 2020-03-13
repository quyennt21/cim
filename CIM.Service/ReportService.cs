using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CIM.Service
{
    public interface IReportService
    {
        void Add(Report report);

        void Update(Report report);

        Report GetById(int id, string[] includes = null);

        Report GetByAssetId(int id, string[] includes = null);

        Report GetReportByConditions(int id, string[] includes = null);

        IEnumerable<Report> GetAll(string[] includes = null);

        IEnumerable<Report> GetAllByUserReport(string userReport, string searchString, out int totalRow, int page = 1, int pageSize = 10, int statusSearch = 0, string[] includes = null);

        IEnumerable<Report> GetAllPaging(string searchString, out int totalRow, int page = 1, int pageSize = 10, int statusSearch = 0, string[] includes = null);
        IEnumerable<Report> GetAllPagingTechnician(string user,string searchString, out int totalRow, int page = 1, int pageSize = 10, int statusSearch = 0, string[] includes = null);

        ApplicationUser getNameUserReport(int id);

        void SaveChanges();
    }

    public class ReportService : IReportService
    {
        private IUnitOfWork _unitOfWork;
        private IReportRepository _reportRepository;

        public ReportService(IUnitOfWork unitOfWork, IReportRepository reportRepository)
        {
            this._unitOfWork = unitOfWork;
            this._reportRepository = reportRepository;
        }

        public void Add(Report report)
        {
            _reportRepository.Add(report);
        }

        public IEnumerable<Report> GetAll(string[] includes = null)
        {
            return _reportRepository.GetAll(includes);
        }

        public IEnumerable<Report> GetAllByUserReport(string userReport, string searchString, out int totalRow, int page = 1, int pageSize = 10, int statusSearch = 0, string[] includes = null)
        {
            var query = _reportRepository.GetByConditions(at => at.UserReport.Equals(userReport), includes);

            if (statusSearch == 0)
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    query = _reportRepository.GetByConditions(at => at.Active && at.Asset.AssetCode.Contains(searchString.Trim()) && at.UserReport.Equals(userReport), includes);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    query = _reportRepository.GetByConditions(at => at.Active && at.Asset.AssetCode.Contains(searchString.Trim()) && at.status.Equals(statusSearch) && at.UserReport.Equals(userReport), includes);
                }
                else
                {
                    query = _reportRepository.GetByConditions(at => at.Active && at.status.Equals(statusSearch) && at.UserReport.Equals(userReport), includes);
                }
            }
            totalRow = query.Count();
            return query.OrderByDescending(at => at.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IEnumerable<Report> GetAllPaging(string searchString, out int totalRow, int page = 1, int pageSize = 10, int statusSearch = 0, string[] includes = null)
        {
            var query = _reportRepository.GetByConditions(at => at.Active, includes);
            if (statusSearch == 0)
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    query = _reportRepository.GetByConditions(at => at.Active && at.Asset.Name.ToUpper().Contains(searchString.ToUpper().Trim()), includes);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    query = _reportRepository.GetByConditions(at => at.Active && at.Asset.Name.ToUpper().Contains(searchString.ToUpper().Trim()) && at.status.Equals(statusSearch), includes);
                }
                else
                {
                    query = _reportRepository.GetByConditions(at => at.Active && at.status.Equals(statusSearch), includes);
                }
            }
            totalRow = query.Count();
            return query.OrderBy(at => at.status).ThenByDescending(or => or.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IEnumerable<Report> GetAllPagingTechnician(string user,string searchString, out int totalRow, int page = 1, int pageSize = 10, int statusSearch = 0, string[] includes = null)
        {
            var query = _reportRepository.GetByConditions(at => at.Active, includes);
            List<Report> listReport = new List<Report>();
            foreach (var r in query)
            {
                string[] comment = r.Comment.Split(new char[] { '-','>' });
                if(comment[comment.Length-1].Contains("Forward to: "+ user))
                {
                    listReport.Add(r);
                }
            }
            query = listReport.AsEnumerable();
            if (statusSearch == 0)
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    query = query.Where(at => at.Active && at.Asset.Name.ToUpper().Contains(searchString.ToUpper().Trim()));
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    query = query.Where(at => at.Active && at.Asset.Name.ToUpper().Contains(searchString.ToUpper().Trim()) && at.status.Equals(statusSearch));
                }
                else
                {
                    query = query.Where(at => at.Active && at.status.Equals(statusSearch));
                }
            }
            totalRow = query.Count();
            return query.OrderBy(at => at.status).ThenByDescending(or => or.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public Report GetByAssetId(int id, string[] includes = null)
        {
            return _reportRepository.GetByConditions(x => x.AssetID == id, includes).OrderByDescending(x => x.ID).FirstOrDefault();
        }

        public Report GetById(int id, string[] includes = null)
        {
            return _reportRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public ApplicationUser getNameUserReport(int id)
        {
            return _reportRepository.getNameUserReport(id);
        }

        public Report GetReportByConditions(int id, string[] includes = null)
        {
            var reportByID = _reportRepository.GetByConditions(x => x.AssetID == id, includes).OrderByDescending(x => x.ID).FirstOrDefault();
            if (reportByID != null && reportByID.status != 3 && reportByID.status != 5)
            {
                return reportByID;
            }
            return null;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void Update(Report report)
        {
            _reportRepository.Update(report);
        }
    }
}