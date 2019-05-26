using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Reminders.FunctionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reminders.FunctionApp
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
