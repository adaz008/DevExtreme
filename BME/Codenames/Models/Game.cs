using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Codenames.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Codenames.Model
{

    public class Game
    {
        public int Id { get; set; }
        public ICollection<Field> Fields { get; set; } = null!;
        public ICollection<Player> Players { get; set; } = null!;
        public int BluePoints { get; set; }
        public int RedPoints { get; set; }
        public Team CurentTeam { get; set; }
        public int? Time { get; set; }

        public List<Field> fieldsList = new List<Field>();

        public int remainingTips = 0;

        public bool setUp()
        {
            //Kezdő játékos generálás
            Random random = new Random();
            int first = random.Next(1, 3);
            int second = first == 1 ? 2 : 1;

            CurentTeam = (Team)first;

            // 1 piros 2 kék 3 fehér 4 fekete


            //Kezdő csapat kártyáinak színe
            for (int i = 0; i < 9; i++)
            {
                int index = random.Next(0, 25);
                if (fieldsList.ElementAt(index).Color != 0)
                    i--;
                else
                    fieldsList.ElementAt(index).Color = (Color)first;
            }

            //Második csapat kártyáinak színe
            for (int i = 0; i < 8; i++)
            {
                int index = random.Next(0, 25);
                if (fieldsList.ElementAt(index).Color != 0)
                    i--;
                else
                    fieldsList.ElementAt(index).Color = (Color)second;
            }

            //Semelyik csapat kártyáinak színe
            for (int i = 0; i < 7; i++)
            {
                int index = random.Next(0, 25);
                if (fieldsList.ElementAt(index).Color != 0)
                    i--;
                else
                    fieldsList.ElementAt(index).Color = (Color)3;
            }

            //Vesztő kártya színe
            for (int i = 0; i < 25; i++)
                if (fieldsList.ElementAt(i).Color == 0)
                    fieldsList.ElementAt(i).Color = (Color)4;

            //Igaz ha piros kezd, hamis ha kék
            return first == 1 ? true : false;
        }

        public bool isClueValid(string clue, int tips)
        {
            // Szó helyesség vizsgálat
            for (int i = 0; i < 25; i++)
                if (clue.Contains(fieldsList.ElementAt(i).Word.WordItem))
                    return false;
            //Tipp szám vizsgálat
            int currentMaxTips = 0;
            for (int i = 0; i < 25; i++)
                if (!fieldsList.ElementAt(i).Picked && (int)fieldsList.ElementAt(i).Color == (int)CurentTeam)
                    currentMaxTips++;

            if (tips > currentMaxTips)
                return false;
            remainingTips = tips;
            return true;
        }

        public int fieldClicked(int x, int y, Team team)
        {
            // return 1 Piros nyert
            // return 2 Kék nyert
            // return 3 Csapatváltás
            // return 4 Frissítés

            int temp = -1;
            for (int i = 0; i < 25; i++)
                if (fieldsList.ElementAt(i).X == x && fieldsList.ElementAt(i).Y == y)
                {
                    temp = i;
                    fieldsList.ElementAt(i).Picked = true;
                }
            //Saját kártya tipp
            if ((int)fieldsList.ElementAt(temp).Color == (int)CurentTeam)
            {
                if (CurentTeam == Team.RED)
                    RedPoints++;
                else if (CurentTeam == Team.BLUE)
                    BluePoints++;
                remainingTips--;
            }
            //Senki földje tipp
            else if ((int)fieldsList.ElementAt(temp).Color == 3)
                remainingTips = 0;
            //Fekete kártya tipp
            else if ((int)fieldsList.ElementAt(temp).Color == 4) 
                return CurentTeam == Team.RED ? (int)Color.BLUE : (int)Color.RED;
            //Ellenfél kártya tipp
            else
            {
                //Rossz tippnél ellenfél kap pontot
                if (CurentTeam == Team.RED)
                    BluePoints++;
                else if (CurentTeam == Team.BLUE)
                    RedPoints++;
                remainingTips = 0;
            }

            if (checkEndGame() != (int)Color.WHITE)
                return checkEndGame();
            else if (remainingTips == 0)
            {
                //Csapatváltás
                CurentTeam = CurentTeam == Team.RED ? Team.BLUE : Team.RED;
                return 3;
            }
            
            return 4;
        }

        private int checkEndGame()
        {
            bool endGame = true;

            //Piros csapatnak nincs már kártyája amit ki kell találni
            for (int i = 0; i < 25; i++)
                if (!fieldsList.ElementAt(i).Picked && fieldsList.ElementAt(i).Color == Color.RED)
                    endGame = false;
            //Nyert a piros
            if (endGame)
                return (int)Color.RED;

            //Kék csapatnak nincs már kártyája amit ki kell találni
            if (!endGame)
            {
                endGame = true;
                for (int i = 0; i < 25; i++)
                    if (!fieldsList.ElementAt(i).Picked && fieldsList.ElementAt(i).Color == Color.BLUE)
                        endGame = false;
            }

            //Nyert a kék
            if (endGame)
                return (int)Color.BLUE;

            return (int)Color.WHITE;
        }

    }
}
