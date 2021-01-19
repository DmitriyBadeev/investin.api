using System.Net;

namespace InvestIn.Finance.Services.DTO.Responses
{
    public class ApiResponse
    {
        public string JsonContent { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}