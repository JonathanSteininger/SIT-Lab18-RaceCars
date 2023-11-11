using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingGame
{
    public class Player
    {

        private static int _counter;

        private int _playerID;
        private string _name;
        private List<Medal> _medals;
        private int _totalRaces;

        public int PlayerID { get { return _playerID; } }
        public string Name { get { return _name; } set { _name = value; } }
        public IList<Medal> Medals { get {  return _medals.AsReadOnly(); } }
        public int TotalRaces { get { return _totalRaces; } }

        public Player() : this("Default") { }
        public Player(string name) : this(name, new List<Medal>(), 0){ } 
        public Player(string name, List<Medal> medals, int totalRaces) : this(_counter++, name, medals, totalRaces) { }
        public Player(int playerID, string name, List<Medal> medals, int totalRaces) => (_playerID, _name, _medals, _totalRaces) = (playerID, name, medals, totalRaces);


        public void AddMedal(Medal medal) => _medals.Add(medal);
        public void AddRace() => _totalRaces++;


    }

    public enum Medal
    {
        Gold,
        Silver,
        Bronze,
    }
}
