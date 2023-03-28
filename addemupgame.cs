using System;


class Program
{
    static void Main(string[] args)
    {

        // Define input and output parameters with default values
        string inputFile = null;
        string outputFile = null;

        // Parse command line arguments
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--in":
                    inputFile = args[i + 1];
                    break;
                case "--out":
                    outputFile = args[i + 1];
                    break;
                default:
                    break;
            }
        }

        // Check if both input and output parameters are set
        if (inputFile == null || outputFile == null)
        {
            Console.WriteLine("Invalid arguments. Usage: addemupgame.exe --in input_file --out output_file");
            return;
        }

        // Proceed with the rest of the code using the inputFile and outputFile variables
        Console.WriteLine($"Input file: {inputFile}");
        Console.WriteLine($"Output file: {outputFile}");
        string inputDirectory = Path.GetDirectoryName(inputFile);
        string outputFilePath = Path.Combine(inputDirectory, outputFile);
        // Read all the lines from the file and store them in a list
        List<Player> players = null;
        try
        {
            players = File.ReadAllLines(inputFile).Select(p => new Player(p)).ToList();
            // Do something with players
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: Invalid input file. " + ex.Message);
            using (StreamWriter file = new StreamWriter(outputFilePath))
            {
                file.WriteLine("Exception: Invalid input file. " + ex.Message);
            }
        }

        // Calculate the points for each player
        if (players == null || players.Count == 0)
        {
            Console.WriteLine("No players found.");
        }
        else
        {
            foreach (Player player in players)
            {
                player.CalculatePoints();
            }
        }

        try
        {
            // Find the winner(s)
            List<Player> winners = players.OrderByDescending(p => p.Points).Where(p => p.Points == players.Max(pl => pl.Points)).ToList();
            string winnerString = string.Join(",", winners.Select(p => p.Name));

            // Check for tie based on suit score of highest card
            if (winners.Count > 1)
            {
                foreach (Player player in winners)
                {
                    player.CalculateSuitScore();
                    
                }

                winners = winners.OrderByDescending(p => p.SuitScore).Where(p => p.SuitScore == winners.Max(pl => pl.SuitScore)).ToList();
                winnerString = string.Join(",", winners.Select(p => p.Name));
                //Console.WriteLine(winners.Max(pl => pl.SuitScore));

                // Check for tie based on points if suit score is also tied
                if (winners.Count > 1 && winners.Max(p => p.SuitScore) == winners.Min(p => p.SuitScore))
                {
                    winners = winners.OrderByDescending(p => p.Points).Where(p => p.Points == winners.Max(pl => pl.Points)).ToList();
                    winnerString = string.Join(",", winners.Select(p => p.Name));
                }
            }

            // Add the suit value to the winner's total points
            int totalPoints = winners[0].Points + winners.Max(p => p.SuitScore);

            // Write the output to a file
            using (StreamWriter file = new StreamWriter(outputFilePath))
            {
                file.WriteLine($"{winnerString}:{totalPoints}");
            }

            Console.WriteLine($"Winner(s): {winnerString}, with {totalPoints} points.");

        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Argument is null. {ex.Message}");
            using (StreamWriter file = new StreamWriter(outputFilePath))
            {
                file.WriteLine($"Argument is null. {ex.Message}");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            using (StreamWriter file = new StreamWriter(outputFilePath))
            {
                file.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    class Player
    {
        public string Name { get; private set; }
        public string[] Cards { get; private set; }
        public int Points { get; private set; }
        public int SuitScore { get; private set; }

        public Player(string playerString)
        {
            string[] nameCards = playerString.Trim().Split(':');
            Name = nameCards[0];
            Cards = nameCards[1].ToUpper().Split(',');
        }

        public void CalculatePoints()
        {

            try {
                int[] cardValues = new int[Cards.Length];

                for (int i = 0; i < Cards.Length; i++)
                {
                    string card = Cards[i];
                    string value = card.Substring(0, card.Length - 1);
                    //value = value.ToUpper();

                    if (value == "A")
                    {
                        cardValues[i] = 11;
                    }
                    else if (value == "J")
                    {
                        cardValues[i] = 11;
                    }
                    else if (value == "Q")
                    {
                        cardValues[i] = 12;
                    }
                    else if (value == "K")
                    {
                        cardValues[i] = 13;
                    }
                    else
                    {
                        cardValues[i] = int.Parse(value);
                    }
                }

                Array.Sort(cardValues);
                Array.Reverse(cardValues);

                for (int i = 0; i < Math.Min(3, cardValues.Length); i++)
                {
                    Points += cardValues[i];
                }
            }
            catch(Exception) {
                Console.WriteLine("Exception: Invalid Input Format");
            }
            
        }

        public void CalculateSuitScore()
        {
            string maxCard = Cards.OrderByDescending(card => GetCardValue(card)).First();
            SuitScore = GetSuitValue(maxCard);
        }

        private int GetCardValue(string card)
        {
            string value = card.Substring(0, card.Length - 1);

            switch (value)
            {
                case "A":
                    return 11;
                case "K":
                    return 13;
                case "Q":
                    return 12;
                case "J":
                    return 11;
                default:
                    return int.Parse(value);
            }
        }
        private int GetSuitValue(string card)
        {
            string suit = card.Substring(card.Length - 1, 1);

            switch (suit)
            {
                case "D":
                    return 1;
                case "H":
                    return 2;
                case "S":
                    return 3;
                case "C":
                    return 4;
                default:
                    return 0;
            }
        }
    }
    
}