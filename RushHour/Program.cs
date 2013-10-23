using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RushHour {
    class Program {
        static bool outputMode;
        static int targetX, targetY;
        static void Main(string[] args) {
            #region Parse input
            outputMode = Console.ReadLine() == "0" ? false : true;
            int height = int.Parse(Console.ReadLine());
            targetX = int.Parse(Console.ReadLine());
            targetY = int.Parse(Console.ReadLine());
            var initialConfig = new string[height];
            for (int i = 0; i < height; i++) initialConfig[i] = Console.ReadLine();
            #endregion

        }
    }
}

/*
 * Concept plan:
 * Een soort Breadth First Search op de moves die vanuit de huidige positie kan worden gedaan, zie ook:
 * http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.151.126&rep=rep1&type=pdf
 * Dus per iteratie:
 * Haal een configuratie uit een concurrent queue (priority queue om hem meer breadth first search te maken?)
 * Voor elke auto die niet als laatste is verplaatst, genereer een permutatie van de configuratie voor elke move dat je met de auto kunt maken
 * Genereer een hash van elke permutatie, controleer of de hash al bestaat in de concurrent dictionary.
 * - Bestaat hij niet? Dan creeer je eerst een nieuwe node in de tree met die configuratie, voeg je de verwijzing toe aan de dictionary onder de hash
 * Vervolgens pak je de node van de huidige configuratie, voeg je een connectie toe naar de node van de gecreeerde permutatie.
 * Als de hash niet bestond, voeg de nieuwe configuratie toe aan de concurrent queue.
 * 
 * Iteraties kunnen parallel worden gedaan, zolang de queue niet leeg is.
 * 
 * Benodigdheden:
 * - Map structuur
 * - Parser van map
 * - Permutatie van map generator 
 * - Concurrent boom, d.w.z. boom waar je kunt traversen, nodes aanmaken en connecties maken. Connecties maken moet concurrent werken.
 * 
 * Optioneel:
 * - Concurrent priority queue maken
 * - Alternatief voor concurrent dictionary bedenken
 */
