using CIM.Data.Infrastructure;
using CIM.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace CIM.Data.Repositories
{
    public interface IAssetTypeRepository : IRepository<AssetType>
    {
        bool Add(AssetType assetType, List<AssetTypeAttribute> listAssetAttributes);

        bool Update(AssetType assetType, IEnumerable<AssetTypeAttribute> listAssetAttributes);

        void Delete(AssetType assetType, IEnumerable<AssetTypeAttribute> listAssetAttributes);
    }

    public class AssetTypeRepository : RepositoryBase<AssetType>, IAssetTypeRepository
    {
        public AssetTypeRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        /// <summary>
        /// Add assetType and AssetAttribute
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="listAssetAttributes"></param>
        public bool Add(AssetType assetType, List<AssetTypeAttribute> listAssetAttributes)
        {
            using (var context = new CIMDbContext())
            {
                using (DbContextTransaction dbTran = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.AssetTypes.Add(assetType);
                        context.SaveChanges();
                        if (listAssetAttributes.Count > 0)
                        {
                            var id = assetType.ID;
                            foreach (var data in listAssetAttributes)
                            {
                                data.AssetTypeID = id;
                                data.Active = true;
                                context.AssetAttributes.Add(data);
                            }
                            context.SaveChanges();
                        }
                        dbTran.Commit();
                        return true;
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                        return false;
                    }
                }
            }
        }

        public void Delete(AssetType assetType, IEnumerable<AssetTypeAttribute> listAssetAttributes)
        {
            using (var context = new CIMDbContext())
            {
                using (DbContextTransaction dbTran = context.Database.BeginTransaction())
                {
                    try
                    {
                        assetType.Active = false;
                        assetType.UpdatedAt = DateTime.Now;
                        context.Entry(assetType).State = EntityState.Modified;
                        context.SaveChanges();

                        foreach (var data in listAssetAttributes)
                        {
                            data.Active = false;
                            data.UpdatedAt = DateTime.Now;
                            context.Entry(data).State = EntityState.Modified;
                        }

                        context.SaveChanges();
                        dbTran.Commit();
                        
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
         
                    }
                }
            }
        }

        public bool Update(AssetType assetType, IEnumerable<AssetTypeAttribute> listAssetAttributes)
        {
            using (var context = new CIMDbContext())
            {
                using (DbContextTransaction dbTran = context.Database.BeginTransaction())
                {
                    try
                    {
                        assetType.UpdatedAt = DateTime.Now;
                        context.Entry(assetType).State = EntityState.Modified;
                        context.SaveChanges();
                        foreach (var data in listAssetAttributes)
                        {
                            data.AssetTypeID = assetType.ID;
                            data.UpdatedAt = DateTime.Now;
                            if (data.ID == 0)
                            {
                                context.AssetAttributes.Add(data);
                            }
                            else
                            {
                                context.Entry(data).State = EntityState.Modified;
                            }
                        }
                        context.SaveChanges();

                        context.SaveChanges();
                        dbTran.Commit();
                        return true;
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}