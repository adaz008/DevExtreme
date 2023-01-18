using System.Reflection.Metadata;
using Codenames.Controllers;
using Codenames.Model;
using Codenames.Models;

namespace Codenames.Hubs
{
    public interface ChatClient
    {
        Task RoomCreated(Room room);
        Task RoomAbandoned(string roomName);
        Task UserEntered(User user);
        Task UserLeft(string userId);
        Task ReceiveMessage(string message);
        Task SetUsers(List<User> users);
        Task SetMessages(List<Message> messages);

        //GameHub
        Task OperativeEntered(User user, int team);
        Task OperativeLeft(string userId);
        Task SpymasterEntered(User user, int team);
        Task SpymasterLeft(string userId);
        Task ReceiveNewState(Game game);
        Task SetPlayers(List<Player> redplayers, List<Player> blueplayers);
        Task SetFields(Game game);

        Task StartGame();

        Task IsClueValid(bool valid, string clue, int tips);

        Task refresh(List<FieldDTO> fields, List<GameDTO> game);

        Task changeCurrentTeam();

        Task endGame(string winner);
    }
}
