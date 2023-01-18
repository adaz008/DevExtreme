using Codenames.Data;
using Codenames.Model;
using Codenames.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Codenames.Hubs
{
    [Authorize]
    public class GameHub : Hub<GameClient>
    {
        public static Game game=new Game();
        private CodenamesDbContext _context;
        public GameHub(CodenamesDbContext context)
        {
            _context = context;
        }
        private string? CurrentUserId => Context.UserIdentifier;
        public static GameTeam Red { get; } = new GameTeam { team = Team.RED };
        public static GameTeam Blue { get; } = new GameTeam { team = Team.BLUE };

        public class GameTeam
        {
            public Team team { get; set; }
            public List<Player> Players { get; } = new List<Player>();
        }
        public async Task UpdateState(int x, int y)
        {
            //Lobby.Messages.Add(messageInstance);
            await Clients.All.ReceiveNewState(game);
        }
        public async Task EnterTeamAsOperative(Team team)
        {
            var user = _context.DbUsers.Where(au => au.Id == CurrentUserId).FirstOrDefault();
            var player = new Player { User=user }; 
            if (team == Team.RED)
            {
                Red.Players.Add(player);
                await Clients.Group("Red").OperativeEntered(player.User);
                await Groups.AddToGroupAsync(Context.ConnectionId, "Red"); 
                await Clients.Caller.SetPlayers(Red.Players);
                await Clients.Caller.SetFields(game);
            }
            else if (team == Team.BLUE)
            {
                Blue.Players.Add(player);
                await Clients.Group("Blue").OperativeEntered(player.User);
                await Groups.AddToGroupAsync(Context.ConnectionId, "Blue");
                await Clients.Caller.SetPlayers(Blue.Players);
                await Clients.Caller.SetFields(game);
            }
            
           
            
        }
        public async Task EnterTeamAsSpymaster(Team team)
        {
            var player = new Player { User = new User { Id = CurrentUserId, Name = Context.User.Identity.Name } };
            if (team == Team.RED)
            {
                Red.Players.Add(player);
                await Clients.Group("Red").OperativeEntered(player.User); 
                await Groups.AddToGroupAsync(Context.ConnectionId, "Red");
                await Groups.AddToGroupAsync(Context.ConnectionId, "RedSpymaster");
                await Clients.Caller.SetPlayers(Red.Players);
                await Clients.Caller.SetFields(game);
            }
            else if (team == Team.BLUE)
            {
                Blue.Players.Add(player);
                await Clients.Group("Blue").OperativeEntered(player.User);
                await Groups.AddToGroupAsync(Context.ConnectionId, "Blue");
                await Groups.AddToGroupAsync(Context.ConnectionId, "BlueSpymaster");
                await Clients.Caller.SetPlayers(Blue.Players);
                await Clients.Caller.SetFields(game);
            }

        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var player = Red.Players.FirstOrDefault(p => p.User.Id == CurrentUserId);
            if (player != null)
            {
                Red.Players.Remove(player);
                // TODO: később a saját szobakezelés kapcsán is kezelni kell a kilépő klienseket
                if (player.Spymaster)
                {
                    await Clients.Group("Red").SpymasterLeft(CurrentUserId);
                }
                else await Clients.Group("Red").OperativeLeft(CurrentUserId);

            }
            else
            {
                player = Blue.Players.FirstOrDefault(p => p.User.Id == CurrentUserId);
                if (player != null)
                {
                    if (player.Spymaster)
                    {
                        await Clients.Group("Blue").SpymasterLeft(CurrentUserId);
                    }
                    else await Clients.Group("Blue").OperativeLeft(CurrentUserId);

                }
                else return;
            }              


            await base.OnDisconnectedAsync(exception);
        }
    }
}
