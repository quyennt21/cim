using CIM.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CIM.Service.Service
{
    public class QrAssetService
    {
        public QrAssetViewModel listViewModel(List<Asset> lst, List<QrAssets> listPrint)
        {
            QrAssetViewModel viewModel = new QrAssetViewModel();
            viewModel.lstQr = new List<QrAssets>();
            for (int i = 0; i < lst.Count; i++)
            {
                QrAssets qrAssets = new QrAssets();
                qrAssets.asset = lst[i];
                qrAssets.isChecked = false;
                viewModel.lstQr.Add(qrAssets);
            }
            try
            {
                for (int i = 0; i < viewModel.lstQr.Count; i++)
                {
                    for (int j = 0; j < listPrint.Count; j++)
                    {
                        if (viewModel.lstQr[i].asset.ID == listPrint[j].asset.ID)
                        {
                            viewModel.lstQr[i].isChecked = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }

            return viewModel;
        }

        public QrAssetViewModel getListChecked(QrAssetViewModel viewModel, List<QrAssets> listPrint)
        {
            QrAssetViewModel viewModelcked = new QrAssetViewModel();

            for (int i = 0; i < viewModel.lstQr.Count(); i++)
            {
                if (viewModel.lstQr[i].isChecked == true)
                {
                    if (!isExistinList(listPrint, viewModel.lstQr[i]))
                    {
                        listPrint.Add(viewModel.lstQr[i]);
                    }
                }
            }
            viewModelcked.lstQr = listPrint;

            return viewModelcked;
        }

        public QrAssetViewModel checkall(QrAssetViewModel viewModel, List<QrAssets> listPrint, bool value)
        {
            QrAssetViewModel viewModelcked = new QrAssetViewModel();

            for (int i = 0; i < viewModel.lstQr.Count(); i++)
            {
                viewModel.lstQr[i].isChecked = value;

                if (!isExistinList(listPrint, viewModel.lstQr[i]))
                {
                    listPrint.Add(viewModel.lstQr[i]);
                }
            }
            viewModelcked.lstQr = listPrint;

            return viewModelcked;
        }

        private bool isExistinList(List<QrAssets> listPrint, QrAssets item)
        {
            foreach (QrAssets a in listPrint)
            {
                if (a.asset.ID == item.asset.ID) return true;
            }
            return false;
        }

        public QrAssetViewModel getMyPage(int page, int size, QrAssetViewModel viewModel)
        {
            QrAssetViewModel view = new QrAssetViewModel();

            view.lstQr = new List<QrAssets>();
            int mathcell = (page + 1) * size;
            if (mathcell >= viewModel.lstQr.Count)
            {
                mathcell = viewModel.lstQr.Count;
            }
            for (int i = page * size; i < mathcell; i++)
            {
                view.lstQr.Add(viewModel.lstQr[i]);
            }

            return view;
        }
    }
}