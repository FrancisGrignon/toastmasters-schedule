using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Reminders.FunctionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reminders.FunctionApp
{
    public class MeetingClient
    {
        private readonly string _baseUrl;

        public MeetingClient(IConfiguration config)
        {
            _baseUrl = config["MeetingServiceUri"];
        }

        public Task<Meeting> GetUpcoming()
        {
            return _baseUrl
                .AppendPathSegment("v1/meetings/upcoming")
                .GetJsonAsync<Meeting>();
        }

        public Task<List<Meeting>> GetPlanning()
        {
            return _baseUrl
                .AppendPathSegment("v1/meetings/planning")
                .GetJsonAsync<List<Meeting>>();
        }
    }
}
