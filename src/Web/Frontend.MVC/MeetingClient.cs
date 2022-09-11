using Flurl;
using Flurl.Http;
using Frontend.MVC.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frontend.MVC
{
    public class MeetingClient
    {
        public const string API_KEY_HEADER_NAME = "x-api-key";

        private readonly string _baseUrl;
        private readonly string _apiKey;

        public MeetingClient(IConfiguration config)
        {
            _baseUrl = config["MeetingServiceUri"];
            _apiKey = config["MeetingServiceApiKey"];
        }

        public Task<List<Meeting>> GetAll()
        {
            return _baseUrl
                .AppendPathSegment("v1/meetings")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .GetJsonAsync<List<Meeting>>();
        }

        public Task<Meeting> Get(int id)
        {
            return _baseUrl
                .AppendPathSegment($"v1/meetings/{id}")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .GetJsonAsync<Meeting>();
        }

        public Task<List<Meeting>> GetPlanning()
        {
            return _baseUrl
                .AppendPathSegment($"v1/meetings/planning")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .GetJsonAsync<List<Meeting>>();
        }

        public async Task<Attendee> GetAttendee(int meetingId, int attendeeId)
        {
            return await _baseUrl
                .AppendPathSegment($"v1/meetings/{meetingId}/attendees/{attendeeId}")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .GetJsonAsync<Attendee>();
        }

        public async Task<IFlurlResponse> Create(Meeting meeting)
        {
            return await _baseUrl
                .AppendPathSegment($"v1/meetings")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .PostJsonAsync(meeting);
        }

        public async Task<IFlurlResponse> Delete(Meeting meeting)
        {
            return await _baseUrl
                .AppendPathSegment($"v1/meetings/{meeting.Id}")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .DeleteAsync();
        }

        public async Task<IFlurlResponse> Update(Meeting meeting)
        {
            return await _baseUrl
                .AppendPathSegment($"v1/meetings/{meeting.Id}")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .PutJsonAsync(meeting);
        }

        public async Task<IFlurlResponse> UpdateAttendee(int meetingId, Attendee attendee)
        {
            return await _baseUrl
                .AppendPathSegment($"v1/meetings/{meetingId}/attendees/{attendee.Id}")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .PutJsonAsync(attendee);
        }

        public Task<List<Role>> GetRoles()
        {
            return _baseUrl
                .AppendPathSegment($"v1/roles")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .GetJsonAsync<List<Role>>();
        }
    }
}
