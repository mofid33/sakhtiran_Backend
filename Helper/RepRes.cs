namespace MarketPlace.API.Helper
{
    public class RepRes<T>
    {
        public RepRes(string message, bool result, T data)
        {
            Message = message;
            Result = result;
            Data = data;
        }

        public string Message { get; set; }
        public bool Result { get; set; }
        public T Data { get; set; }
    }
}