using System.Net;

namespace HRMS.Core.Utilities
{
    /// <summary>
    /// غلاف موحد لجميع استجابات النظام
    /// </summary>
    /// <typeparam name="T">نوع البيانات المرجعة</typeparam>
    public class Result<T>
    {
        public T Data { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public int StatusCode { get; set; }

        public Result()
        {
        }

        public Result(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
            Errors = null;
            StatusCode = 200;
        }

        public Result(string message)
        {
            Succeeded = false;
            Message = message;
            Errors = new List<string> { message };
            StatusCode = 400;
        }

        public static Result<T> Success(T data, string message = "تمت العملية بنجاح")
        {
            return new Result<T>(data, message) { StatusCode = 200 };
        }

        public static Result<T> Failure(string message, int statusCode = 400, List<string> errors = null)
        {
            return new Result<T>
            {
                Succeeded = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors ?? new List<string> { message }
            };
        }
    }
}
