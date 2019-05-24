using Flurl;
using Flurl.Http;
using Meetings.Reminder.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meetings.Reminder
{
    public class MemberClient
    {
        private readonly string _baseUrl;

        public MemberClient(IConfiguration config)
        {
            _baseUrl = config["MemberServiceUri"];
        }

        public Task<List<Member>> GetAll()
        {
            return _baseUrl
                .AppendPathSegment("members")
                .GetJsonAsync<List<Member>>();
        }

        public Task<Member> Get(int id)
        {
            return _baseUrl
                .AppendPathSegment($"members/{id}")
                .GetJsonAsync<Member>();
        }
    }
}
