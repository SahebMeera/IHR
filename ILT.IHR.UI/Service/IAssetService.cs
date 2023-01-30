using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;

namespace ILT.IHR.UI.Service
{
    public interface IAssetService
    {
        Task<Response<IEnumerable<Asset>>> GetAssets();
        Task<Response<Asset>> GetAssetByIdAsync(int Id);
        Task<Response<Asset>> UpdateAsset(int Id, Asset updateObject);
        Task<Response<Asset>> SaveAsset(Asset obj);
        Task<Response<Asset>> DeleteAsset(int id);
    }
}

