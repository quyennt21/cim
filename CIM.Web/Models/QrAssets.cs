using System.Collections.Generic;

namespace CIM.Model.Models
{
    public class QrAssets
    {
        public Asset asset { get; set; }
        public bool isChecked { get; set; }
    }

    public class QrAssetViewModel
    {
        public List<QrAssets> lstQr { get; set; }
    }
}