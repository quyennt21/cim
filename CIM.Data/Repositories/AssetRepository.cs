using CIM.Common;
using CIM.Data.Infrastructure;
using CIM.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace CIM.Data.Repositories
{
    public interface IAssetRepository : IRepository<Asset>
    {
        IEnumerable<Asset> listSearch(IEnumerable<Asset> assets, string searchString, string locationSearch, string typeSearch, string dateFrom, string dateTo);

        IEnumerable<Asset> listaAreaSearch(IEnumerable<Asset> assets, int areaID);

        void Add(Asset asset, List<AssetAttributeValue> assetAttributeValues);

        void Update(Asset asset, IEnumerable<AssetAttributeValue> assetAttributeValues);
    }

    public class AssetRepository : RepositoryBase<Asset>, IAssetRepository
    {
        public AssetRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public void Add(Asset asset, List<AssetAttributeValue> assetAttributeValues)
        {
            using (var context = new CIMDbContext())
            {
                using (DbContextTransaction dbTran = context.Database.BeginTransaction())
                {
                    try
                    {
                        asset.Image = Constants.DEFAULT_ASSET_IMAGE;
                        asset.CreatedAt = DateTime.Now;
                        asset.UpdatedAt = DateTime.Now;
                        asset.Active = true;
                        context.Assets.Add(asset);
                        context.SaveChanges();
                        if (assetAttributeValues.Count() != 0)
                        {
                            var id = asset.ID;
                            foreach (var data in assetAttributeValues)
                            {
                                data.AssetID = id;
                                data.Active = true;
                                context.AssetAttributeValues.Add(data);
                            }
                            context.SaveChanges();
                        }
                        dbTran.Commit();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                    }
                }
            }
        }

        public IEnumerable<Asset> listaAreaSearch(IEnumerable<Asset> assets, int areaID)
        {
            assets = assets.Where(an => an.AreaID == areaID);
            return assets;
        }

        public IEnumerable<Asset> listSearch(IEnumerable<Asset> assets, string searchString, string locationSearch, string typeSearch, string dateFrom, string dateTo)
        {
            if (locationSearch != null && !locationSearch.Equals("--All--"))
            {
                locationSearch = locationSearch.ToLower().Trim();
                assets = assets.Where(a => a.Area.Location.Name.ToLower().Trim().Equals(locationSearch));
            }

            if (typeSearch != null && !string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower().Trim();
                if (typeSearch.Equals("--All--"))
                {
                    assets = assets.Where(an => an.Name.ToLower().Contains(searchString) || an.AssetCode.ToLower().Contains(searchString) || an.Area.Name.ToLower().Contains(searchString));
                }
                else if (typeSearch.Equals("Asset Name"))
                {
                    assets = assets.Where(an => an.Name.ToLower().Contains(searchString));
                }
                else if (typeSearch.Equals("Asset Code"))
                {
                    assets = assets.Where(an => an.AssetCode.ToLower().Contains(searchString));
                }
                else if (typeSearch.Equals("Asset Type"))
                {
                    assets = assets.Where(an => an.AssetType.Name.ToLower().Contains(searchString));
                }
                else
                {
                    assets = assets.Where(an => an.Area.Name.ToLower().Contains(searchString));
                }
            }
            if (!String.IsNullOrEmpty(dateFrom) && String.IsNullOrEmpty(dateTo))
            {
                DateTime from = Convert.ToDateTime(dateFrom);
                assets = assets.Where(a => (from <= a.StartDate));
            }
            else if (String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
            {
                DateTime to = Convert.ToDateTime(dateTo);
                assets = assets.Where(a => (to >= a.StartDate));
            }
            else if (!String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
            {
                DateTime from = Convert.ToDateTime(dateFrom);
                DateTime to = Convert.ToDateTime(dateTo);
                assets = assets.Where(a => (from <= a.StartDate) && (to >= a.StartDate));
            }

            return assets.OrderByDescending(x => x.Active).ThenBy(x => x.CreatedAt);
        }

        public void Update(Asset asset, IEnumerable<AssetAttributeValue> assetAttributeValues)
        {
            using (var context = new CIMDbContext())
            {
                using (DbContextTransaction dbTran = context.Database.BeginTransaction())
                {
                    try
                    {
                        asset.UpdatedAt = DateTime.Now;
                        context.Entry(asset).State = EntityState.Modified;
                        context.SaveChanges();
                        foreach (var data in assetAttributeValues)
                        {
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
    }
}