using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FGO_PersonalNPDamage {
    public class HandCombinations {
        const int uniqueCards = 15;
        public List<TurnBreakdown> TurnBreakdowns = new List<TurnBreakdown>();

        public HandCombinations() {
            ListTurnsRecursively(new TurnBreakdown());
        }

        void ListTurnsRecursively(TurnBreakdown turnBreakdown) {
            if (turnBreakdown.TotalCards == uniqueCards) {
                TurnBreakdowns.Add(turnBreakdown);
                return;
            }

            //For each card, decide which turn it'll be drawn on
            for (int turn = 1; turn <= 3; turn++) {
                var newPath = new TurnBreakdown(turnBreakdown);
                int cardID = newPath.TotalCards + 1;
                if (!newPath.CardsByTurn.ContainsKey(turn)) {
                    newPath.CardsByTurn.Add(turn, new List<int>() { cardID });
                    ListTurnsRecursively(newPath);
                }
                else if (newPath.CardsByTurn[turn].Count < 5) {
                    newPath.CardsByTurn[turn].Add(cardID);
                    ListTurnsRecursively(newPath);
                }
            }
        }

        public int HandsWith2CardsOnTurn3 {
            get {
                int retVal = 0;
                foreach (TurnBreakdown t in TurnBreakdowns) {
                    if (t.CardsByTurn[3].Contains(1) && t.CardsByTurn[3].Contains(2)) {
                        retVal++;
                    }
                }
                return retVal;
            }
        }
    }

    public class TurnBreakdown {

        //Unordered
        public Dictionary<int, List<int>> CardsByTurn = new Dictionary<int, List<int>>();

        public int TotalCards {
            get {
                int retVal = 0;
                foreach (var kvp in CardsByTurn) {
                    retVal += kvp.Value.Count;
                }
                return retVal;
            }
        }

        public TurnBreakdown() { }
        public TurnBreakdown(TurnBreakdown other) {
            CardsByTurn = new Dictionary<int, List<int>>();
            foreach (var kvp in other.CardsByTurn) {
                CardsByTurn.Add(kvp.Key, new List<int>(kvp.Value));
            }
        }

    }
}
