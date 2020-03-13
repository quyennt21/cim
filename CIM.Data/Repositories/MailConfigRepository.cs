using CIM.Data.Infrastructure;
using CIM.Model.Models;

namespace CIM.Data.Repositories
{
    public interface IMailConfigRepository : IRepository<MailConfig>
    {
    }

    public class MailConfigRepository : RepositoryBase<MailConfig>, IMailConfigRepository
    {
        public MailConfigRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}