using Flurl;
using Flurl.Http;
using Frontend.MVC.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frontend.MVC.C
{
    public class MeetingClient
    {
        private readonly string _baseUrl;

        public MeetingClient(IConfiguration config)
        {
            _baseUrl = config["MeetingServiceUri"];
        }

        public Task<List<Meeting>> GetAll()
        {
            return _baseUrl
                .AppendPathSegment("v1/meetings")
                .GetJsonAsync<List<Meeting>>();
        }

        public Task<Meeting> Find(int id)
        {
            return _baseUrl
                .AppendPathSegment($"v1/meetings/{id}")
                .GetJsonAsync<Meeting>();
        }
    }
}
