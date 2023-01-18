using Codenames.Controllers;
using Codenames.Data;
using Codenames.Model;
using Codenames.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Codenames.Hubs
{
    [Authorize]
    public class ChatHub : Hub<ChatClient>
    {
        public const string LobbyRoomName = "ChattRLobby";
        //GameHub
        public static Game game = new Game();
        private CodenamesDbContext _context;

        //GameHub
        public static GameTeam Red { get; } = new GameTeam { team = Team.RED };
        public static GameTeam Blue { get; } = new GameTeam { team = Team.BLUE };

        //GameHub
        public class GameTeam
        {
            public Team team { get; set; }
            public List<Player> Players { get; } = new List<Player>();
        }
        public ChatHub(CodenamesDbContext context)
        {
            _context = context;
        }
        public static HubRoom Lobby { get; } = new HubRoom
        {
            Name = LobbyRoomName
        };
        public class HubRoom
        {
            public string? Name { get; set; }
            public string? CreatorId { get; set; }
            public string? Passkey { get; set; }
            public List<Message> Messages { get; } = new List<Message>();
            public List<User> Users { get; } = new List<User>();
        }
        // Ahhoz hogy működjön a Context.UserIdentifier,
        // külön kellene implementálni az IUserIdProvider interfészt
        // https://stackoverflow.com/a/63059742/1406798
        //private string CurrentUserId => Context.User.FindFirst("sub").Value;
        private string? CurrentUserId => Context.UserIdentifier;

         // TODO: a szobakezelést érdemes a beépített Group mechanizmus segítségével kezelni, de az
         // kizárólag a klienseknek történő válaszok küldésére használható. A Group ID alapján
         // automatikusan "létrejön", ha egy felhasználó belép, és "megszűnik", ha az utolsó is kilép.
         // Ezért szükséges egy saját adatstruktúrában is eltárolnunk a szobákat, hogy a felhasználók
         // adatait és a korábbi üzeneteket meg tudjuk jegyezni. A ChattRHub nem singleton, minden
         // kéréshez egy ChattRHub objektum példányosodik. A legegyszerűbb megoldás egy statikus
         // objektumban tárolni itt az adatokat, de ez éles környezetben nem lenne optimális, helyette
         // egy singleton service-ben kellene az adatokat kezelnünk. A laboron a statikus megoldás
         // teljesen megfelel, de legyünk tisztában a "static smell" jelenséggel; állapotot megosztani
         // explicit érdemes, tehát függőséginjektálással, nem "láthatatlan" statikus függőségekkel.
        public async Task EnterLobby()
        {
            var user = new User { Id = CurrentUserId, Name = Context.User.Identity.Name };
            Lobby.Users.Add(user);
            // Megvizsgálhatjuk a Client objekumot: ezen keresztül érjük el a hívó klienst (Caller),
            // adott klienseket tudunk megszólítani pl. ConnectionId vagy UserIdentifier alapján, vagy
            // használhatjuk a beépített csoport (Group) mechanizmust felhasználói csoportok kezelésére.
            await Clients.Group(LobbyRoomName)
            // A Client típusunk a fent megadott típusparaméter, ezeket a függvényeket tudjuk
            // meghívni a kliense(ke)n.
            .UserEntered(user);
            await Groups.AddToGroupAsync(Context.ConnectionId, LobbyRoomName);
            await Clients.Caller.SetUsers(Lobby.Users);
            await Clients.Caller.SetMessages(Lobby.Messages);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = Lobby.Users.FirstOrDefault(u => u.Id == CurrentUserId);
                


            //GameHub
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
                    Blue.Players.Remove(player);
                    if (player.Spymaster)
                    {
                        await Clients.Group("Blue").SpymasterLeft(CurrentUserId);
                    }
                    else await Clients.Group("Blue").OperativeLeft(CurrentUserId);

                }
            }

            await Clients.All.SetPlayers(Red.Players, Blue.Players);

            if (user == null)
                return;
            Lobby.Users.Remove(user);
            // TODO: később a saját szobakezelés kapcsán is kezelni kell a kilépő klienseket
            await Clients.Group(LobbyRoomName).UserLeft(CurrentUserId);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string user, string message)
        {
            var messageInstance = new Message
            {
                //SenderId = user.Id,
                //SenderName = Context.User.Identity.Name,
                SenderName = user,
                Text = message,
                PostedDate = DateTimeOffset.Now
            };
            Lobby.Messages.Add(messageInstance);

            var u = _context.DbUsers.Where(au => au.Id == CurrentUserId).FirstOrDefault();
            
            
            await Clients.All.ReceiveMessage(u.UserName + " says "+message);
        }

        //GameHub
        public async Task UpdateState(int x, int y)
        {
            //Lobby.Messages.Add(messageInstance);
            await Clients.All.ReceiveNewState(game);
        }
        public async Task EnterTeamAsOperative(int team)
        {
            var user = _context.DbUsers.Where(au => au.Id == CurrentUserId).FirstOrDefault();
            Player player = null;
            foreach(var item in Red.Players)
            {
                if(item.Name == user.UserName)
                    player = item;
            }
            foreach(var item in Blue.Players)
            {
                if(item.Name == user.UserName)
                    player=item;
            }
            if (player == null)
            {
                player = new Player();
                player.Name = user.UserName;
                player.User = user;
                player.Spymaster = false;
            }
 
            if (team == 0)
            {
                Red.Players.Add(player);
                if (Blue.Players.Contains(player))
                {
                    Blue.Players.Remove(player);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Blue");
                }
                await Clients.Caller.OperativeEntered(player.User, 0);
                await Groups.AddToGroupAsync(Context.ConnectionId, "Red");
                
                
                
            }
            else if (team == 1)
            {
                Blue.Players.Add(player);
                if(Red.Players.Contains(player))
                {
                    Red.Players.Remove(player);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Red");
                }
                await Clients.Caller.OperativeEntered(player.User, 1);
                await Groups.AddToGroupAsync(Context.ConnectionId, "Blue");
                
                
                
            }
            await Clients.All.SetPlayers(Red.Players, Blue.Players);



        }
        public async Task EnterTeamAsSpymaster(int team)
        {
            var user = _context.DbUsers.Where(au => au.Id == CurrentUserId).FirstOrDefault();
            Player player = null;
            foreach (var item in Red.Players)
            {
                if (item.Name == user.UserName)
                    player = item;
            }
            foreach (var item in Blue.Players)
            {
                if (item.Name == user.UserName)
                    player = item;
            }
            if (player == null)
            {
                player = new Player();
                player.Name = user.UserName;
                player.User = user;
            }
            player.Spymaster = true;

            if (team == 0)
            {
                if(!Red.Players.Contains(player))
                    Red.Players.Add(player);
                if (Blue.Players.Contains(player))
                {
                    Blue.Players.Remove(player);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Blue");
                    if (player.Spymaster)
                    {
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "BlueSpymaster");
                    }
                }

                await Clients.All.SpymasterEntered(player.User, 0); 
                await Clients.Caller.OperativeEntered(player.User, 0);
                await Groups.AddToGroupAsync(Context.ConnectionId, "Red");
                await Groups.AddToGroupAsync(Context.ConnectionId, "RedSpymaster");
            }
            else if (team == 1)
            {

                if (!Blue.Players.Contains(player))
                    Blue.Players.Add(player);
                if (Red.Players.Contains(player))
                {
                    Red.Players.Remove(player);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Red");
                    if (player.Spymaster)
                    {
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "RedSpymaster");
                    }
                }

                await Clients.All.SpymasterEntered(player.User, 1);
                await Clients.Caller.OperativeEntered(player.User, 1);
                await Groups.AddToGroupAsync(Context.ConnectionId, "Blue");
                await Groups.AddToGroupAsync(Context.ConnectionId, "BlueSpymaster");
            }

            await Clients.All.SetPlayers(Red.Players, Blue.Players);

        }

        public async Task StartGame()
        {

            int[] indexes = new int[25];
            Random vel=new Random(); 

            for (int i = 0; i < 25; i++)
            {
                indexes[i] = vel.Next(0, 401);
                for (int j = 0; j < i; j++)
                {
                    if (indexes[i] == indexes[j])
                    {
                        i--;
                        break;
                    }
                }
            }
            List<Field> fields = new List<Field>();

            var words = _context.Words.Where(t => indexes.Contains(t.Id));
            Field f;
            foreach (Word? q in words)
            {
                f = new Field();
                f.Word = new Word();
                f.Word.WordItem = q.WordItem;
                Console.WriteLine(f.Word.WordItem);

                fields.Add(f);
            }
            for(int i = 0; i < fields.Count; i++)
            {
                fields[i].X = i % 5;
                fields[i].Y = i / 5;
                Console.WriteLine(fields[i].Color);
            }
            game.fieldsList = fields;
            game.setUp();
            List<GameDTO> lista = new List<GameDTO>();
            lista.Add(gameToGameDTO(game));
            await Clients.All.refresh(fieldToFieldDTO(game.fieldsList), lista);
            await Clients.All.StartGame();
            

        }

        public async Task IsClueValidInv(string clue, int tips) 
        {
            bool value = game.isClueValid(clue, tips);

            await Clients.All.IsClueValid(value, clue, tips);
        }

        public async Task FieldClicked(int x, int y, int team)
        {
            int value = game.fieldClicked(x, y, (Team)(team+1));
            List<GameDTO> lista = new List<GameDTO> { gameToGameDTO(game) };

            switch (value)
            {
                //Piros nyert
                case 1:
                    await Clients.All.refresh(fieldToFieldDTO(game.fieldsList), lista);
                    await Clients.All.endGame("Red");
                    SaveStats("Red");
                    Red.Players.Clear();
                    Blue.Players.Clear();
                    game = new Game();
                    break;
                //Kék nyert
                case 2:
                    await Clients.All.refresh(fieldToFieldDTO(game.fieldsList), lista);
                    await Clients.All.endGame("Blue");
                    SaveStats("Blue");
                    Red.Players.Clear();
                    Blue.Players.Clear();
                    game = new Game();
                    break;
                //Csapatváltás
                case 3:
                    await Clients.All.refresh(fieldToFieldDTO(game.fieldsList), lista);
                    await Clients.All.changeCurrentTeam();
                    break;
                //Frissítés
                case 4: 
                    await Clients.All.refresh(fieldToFieldDTO(game.fieldsList), lista);
                    break;
            }
        }
        public void SaveStats(string winner)
        {
            foreach (var player in Red.Players)
            {
                var piros = _context.DbUsers.Where(au => au.Id == player.User.Id).FirstOrDefault();
                if (winner == "Red")
                {
                    if (player.Spymaster)
                        piros.WinsSm++;
                    piros.Wins++;
                }
                if (player.Spymaster)
                    piros.GamesSm++;
                piros.Games++;
            }

            foreach (var player in Blue.Players)
            {
                var kek = _context.DbUsers.Where(au => au.Id == player.User.Id).FirstOrDefault();
                if (winner == "Blue")
                {
                    if (player.Spymaster)
                        kek.WinsSm++;
                    kek.Wins++;
                }
                if (player.Spymaster)
                    kek.GamesSm++;
                kek.Games++;
            }
            _context.SaveChanges();
        }
        private List<FieldDTO> fieldToFieldDTO(List<Field> list)
        {
            FieldDTO fieldDTO;
            List<FieldDTO> fields = new List<FieldDTO>();
            for (int i = 0; i < list.Count; i++) 
            {
                fieldDTO=new FieldDTO();
                fieldDTO.Word = list[i].Word.WordItem;
                fieldDTO.Color = (int)list[i].Color;
                fieldDTO.FieldPicked = list[i].Picked;
                fieldDTO.X=list[i].X;
                fieldDTO.Y=list[i].Y;
                fields.Add(fieldDTO);
            }
            return fields;

        }
        private GameDTO gameToGameDTO(Game game)
        {
            GameDTO gameDTO = new GameDTO();
            gameDTO.remainingTips = game.remainingTips;
            gameDTO.CurentTeam = (int)game.CurentTeam -1;
            gameDTO.RedPoints=game.RedPoints;
            gameDTO.BluePoints=game.BluePoints;
            return gameDTO;
        }
    }
}
