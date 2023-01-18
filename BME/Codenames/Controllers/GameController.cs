using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Codenames.Data;
using Codenames.Model;
using Microsoft.AspNetCore.Authorization;

namespace Codenames.Controllers
{
    [Authorize]
    [ApiController]
    [Route("game")]
    public class GameController : ControllerBase
    {
        private CodenamesDbContext _context;
        Random vel = new Random();


        public GameController(CodenamesDbContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "generateFields")]
        public IActionResult generateFields()
        {
            
            int[] indexes = new int[25];
            
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
            List<FieldDTO> fieldDTOs = new List<FieldDTO>();
            
            var words = _context.Words.Where(t => indexes.Contains(t.Id));
            FieldDTO f;
            foreach (Word? q in words)
            {
                f = new FieldDTO();
                f.Word = q.WordItem;

                fieldDTOs.Add(f);
            }
            return Ok(fieldDTOs);
        }

        //[HttpGet("{email}",Name = "stats")]
        [HttpGet]
        [Route("user/{email}", Name = "stats")]
        public IActionResult getStatByUser(String email)
        {
            var user = _context.DbUsers.Where(u => u.Email == email).FirstOrDefault();
            List<Stats> stats = new List<Stats>();

            if (user == null)
                return NotFound();

            Stats stat = new Stats();
            stat.GamesPlayed = user.Games;
            stat.GamesWon = user.Wins;
            stat.GamesPlayedAsSpymaster = user.GamesSm;
            stat.GamesWonAsSpymaster = user.WinsSm;
            stat.Name = user.UserName;

            stats.Add(stat);

            return Ok(stats);
        }

        [HttpGet]
        [Route("topPlayers")]
        //[HttpGet(Name = "topPlayers")]
        public IActionResult getTopStats()
        {
            var q = _context.DbUsers.OrderByDescending(u => u.Wins).Take(10).ToList();
            List<Stats> stats = new List<Stats>();

            if (q == null)
                return NotFound();

            foreach (User? item in q)
            {
                Stats stat = new Stats();
                stat.GamesPlayed = item.Games;
                stat.GamesWon = item.Wins;
                stat.GamesPlayedAsSpymaster = item.GamesSm;
                stat.GamesWonAsSpymaster = item.WinsSm;
                stat.Name = item.UserName;

                stats.Add(stat);
            }

            return Ok(stats);
        }
        
    }


    public partial class GameDTO
    {
        public int BluePoints { get; set; }
        public int RedPoints { get; set; }

        public int remainingTips { get; set; }

        public int CurentTeam { get; set; }

    }
    public partial class FieldDTO
    {
        public string? Word { get; set; }

        public int Color { get; set; }

        public bool FieldPicked { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

    }

    public partial class PlayerDTO
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long Id { get; set; }

        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string? Name { get; set; }

        [Newtonsoft.Json.JsonProperty("team", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public PlayerTeam Team { get; set; }

        [Newtonsoft.Json.JsonProperty("role", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public PlayerRole Role { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class UserDTO
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long Id { get; set; }

        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string? Name { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class Stats
    {
        [Newtonsoft.Json.JsonProperty("gamesPlayed", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long GamesPlayed { get; set; }

        [Newtonsoft.Json.JsonProperty("gamesWon", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long GamesWon { get; set; }

        [Newtonsoft.Json.JsonProperty("gamesWonAsSpymaster", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long GamesWonAsSpymaster { get; set; }

        public long GamesPlayedAsSpymaster { get; set; }

        public string Name { get; set; }   

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }
   

   

    

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v11.0.0.0))")]
    public enum FieldState
    {

        [System.Runtime.Serialization.EnumMember(Value = @"red")]
        Red = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"blue")]
        Blue = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"black")]
        Black = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"white")]
        White = 3,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v11.0.0.0))")]
    public enum PlayerTeam
    {

        [System.Runtime.Serialization.EnumMember(Value = @"red")]
        Red = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"blue")]
        Blue = 1,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v11.0.0.0))")]
    public enum PlayerRole
    {

        [System.Runtime.Serialization.EnumMember(Value = @"spymaster")]
        Spymaster = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"operative")]
        Operative = 1,

    }
}
