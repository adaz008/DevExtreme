using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Codenames.Hubs
{
    public class IdBasedUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {

            return connection.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
