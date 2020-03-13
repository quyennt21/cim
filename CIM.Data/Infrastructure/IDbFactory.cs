using System;

namespace CIM.Data.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        CIMDbContext Init();
    }
}