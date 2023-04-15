using MarketPlace.API.Data.Enums;
using MarketPlace.API.Services.IService;

namespace MarketPlace.API.Helper
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public T Result { get; set; }
        public string Message { get; set; }
        public ApiResponse(ResponseStatusEnum status, T result, string message)
        {
            this.Result = result;
            this.Status = (int)status;
            this.Message = message;
        }
    }
}