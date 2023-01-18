using Codenames.Model;
using Codenames.Models;

namespace Codenames.Hubs

{
    public interface GameClient
    {
        Task OperativeEntered(User user);
        Task OperativeLeft(string userId);
        Task SpymasterEntered(User user);
        Task SpymasterLeft(string userId);
        Task ReceiveNewState(Game game);
        Task SetPlayers(List<Player> players);
        Task SetFields(Game game);
    }
}
