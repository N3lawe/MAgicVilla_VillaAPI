using MagicVilla_Web.Models;

namespace MagicVilla_Web.Service.IService
{
    public interface IBaseService
    {
        APIResponse responseModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apiRequest);
    }
}
