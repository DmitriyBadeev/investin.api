using System.Collections.Generic;

namespace InvestIn.Finance.Services.DTO.Responses
{
    public class Description
    {
        public List<string> columns { get; set; }
        public List<List<string>> data { get; set; }
    }

    public class SearchResponse
    {
        public Description description { get; set; }
    }
}