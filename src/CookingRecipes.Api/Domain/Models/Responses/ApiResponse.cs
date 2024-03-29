using CookingRecipes.Api.Common;
using System.Text.Json.Serialization;

namespace CookingRecipes.Api.Domain.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string? ErrorMessage { get; set; }
        [JsonIgnore]
        public int StatusCode { get; set; }

        public ApiResponse(T data, bool success = true, string? errorMessage = null, int statusCode = HttpStatusCodes.Ok)
        {
            Success = success;
            Data = data;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }
    }
}
