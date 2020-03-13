using CIM.Data.Infrastructure;
using CIM.Model.Models;
using System.Data.Entity;
using System.Linq;

namespace CIM.Data.Repositories
{
    public interface IReportRepository : IRepository<Report>
    {
        ApplicationUser getNameUserReport(int id);
    }

    public class ReportRepository : RepositoryBase<Report>, IReportRepository
    {
        public ReportRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public ApplicationUser getNameUserReport(int id)
        {
            using (var context = new CIMDbContext())
            {
                using (DbContextTransaction dbTran = context.Database.BeginTransaction())
                {
                    ApplicationUser user = new ApplicationUser();
                    var pic = context.Assets
                    .Join(context.AssetTypes, ass => ass.AssetTypeID, at => at.ID, (ass, at) => new { ass, at })
                    .Join(context.Areas, assat => assat.ass.AreaID, ar => ar.ID, (assat, ar) => new { assat, ar })
                    .Join(context.PICs, assatar => assatar.assat.ass.AreaID, p => p.AreaID, (assatar, p) => new { assatar, p })
                    .Join(context.PICs, assatarp => assatarp.assatar.assat.ass.AssetTypeID, p1 => p1.AssetTypeID, (assatarp, p1) => new { assatarp, p1 })
                    .Select(picTemp => new
                    {
                        username = picTemp.p1.ApplicationUser.FullName,
                        email = picTemp.p1.ApplicationUser.Email,
                        assetID = picTemp.assatarp.assatar.assat.ass.ID
                    }).Where(n => n.assetID == id).FirstOrDefault();
                    if (pic != null)
                    {
                        user.FullName = pic.username;
                        user.Email = pic.email;

                        return user;
                    }
                    else
                    {
                        return context.Users.Where(a => a.Id == 2).SingleOrDefault();
                    }
                }
            }
        }
    }
}