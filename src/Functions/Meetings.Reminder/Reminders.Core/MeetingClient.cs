using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Reminders.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reminders
{
    public class MeetingClient
    {
        private readonly string _baseUrl;

        public MeetingClient(IConfiguration config)
        {
            _baseUrl = config["MeetingServiceUri"];
        }

        public Task<List<Meeting>> GetUpcoming()
        {
            return _baseUrl
                .AppendPathSegment("v1/meetings/upcoming")
                .GetJsonAsync<List<Meeting>>();
        }

        public Task<List<Meeting>> GetMeetingPlanning()
        {
            return _baseUrl
                .AppendPathSegment("v1/meetings/planning")
                .GetJsonAsync<List<Meeting>>();
        }
    }
}
