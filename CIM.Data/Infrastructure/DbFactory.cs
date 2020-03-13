namespace CIM.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private CIMDbContext dbContext;

        public CIMDbContext Init()
        {
            return dbContext ?? (dbContext = new CIMDbContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
            {
                dbContext.Dispose();
            }
        }
    }
}