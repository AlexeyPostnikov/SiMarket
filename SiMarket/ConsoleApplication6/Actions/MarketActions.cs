using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SiMarket
{
    class MarketActions : FileWorks
    {
        public void ChooseAction()
        {

            Console.WriteLine("\nChoose company:");
            Console.Write('$');
            string companyChoice = Console.ReadLine();
            if (companyChoice == "save" || companyChoice == "Save")
            {
                Market.Current.Save();
                Market.Current.ChooseAction();
            }
            else if (companyChoice == "load" || companyChoice == "Load")
            {
                Market.Current.Load();
                Market.Current.ChooseAction();
            }
            else if (companyChoice == "")
            {
                return;

            }
            else
            {
                if (Market.Current.CompaniesList.FindNumber(companyChoice) == -1)
                {
                    while (companyChoice != "")
                    {
                        Console.WriteLine("Screw you! Enter existing company name");
                        Console.Write("$");
                        companyChoice = Console.ReadLine();
                        if (Market.Current.CompaniesList.FindNumber(companyChoice) != -1)
                        {
                            break;
                        }
                    }
                    if (companyChoice == "")
                    {
                        return;
                    }

                    Market.Current.AskPlayer(companyChoice);
                }
                else
                {
                    int number = Market.Current.CompaniesList.FindNumber(companyChoice);
                    if (number != -1)
                    {
                        Console.WriteLine("Company spezialization: {0}\nCompany location: {1}", Market.Current.CompaniesList.List[number].CompanyType, Market.Current.CompaniesList.List[number].Location);
                        Market.Current.AskPlayer(companyChoice);
                    }
                }
            }
        }        

        public void AskPlayer(string companyChoice)
        {
            decimal priceChoice;
            int amountChoice;
            string command = "";
            Console.WriteLine("Want to buy? (y/n)");
            Console.Write("$");
            command = Console.ReadLine().ToLower();
            while (command == "y")
            {
                Market.Current.SellList.SortUp();
                if (Market.Current.SellList.FindNumber(companyChoice) >= 0)
                {
                    Market.Current.SellList.PrintList(companyChoice);
                }
                else
                {
                    Console.WriteLine("Nobody sells share packets in this company.");
                }
                Console.WriteLine("Enter amount and price");
                Console.Write("$");
                string Choice = Console.ReadLine();
                String[] substrings = Choice.Split(' ');
                if (substrings.Length > 1)
                {
                    Int32.TryParse(substrings[0], out amountChoice);
                    decimal.TryParse(substrings[1], NumberStyles.Any, new CultureInfo("en-US"), out priceChoice);
                    while (priceChoice <= 0)
                    {
                        Console.WriteLine("Enter price again(it must be decimal and not negative and more than zero)");
                        decimal.TryParse(Console.ReadLine(), out priceChoice);

                    }
                    while (amountChoice <= 0)
                    {
                        Console.WriteLine("Enter amount again(it must be integer and not negative and more than zero)");
                        int.TryParse(Console.ReadLine(), out amountChoice);
                    }
                    if (Player.Current.Money >= priceChoice * amountChoice)
                    {
                        Market.Current.GenerateBuyOffering(companyChoice, priceChoice, amountChoice, Player.Current.Name);
                        Player.Current.Money -= amountChoice * priceChoice;
                    }
                    command = " ";
                }
            }
            Player.Current.Structurise();
            Console.WriteLine("Want to sell? (y/n)");
            command = Console.ReadLine().ToLower();
            while (command == "y")
            {
                Market.Current.BuyList.SortDown();
                if (Market.Current.BuyList.FindNumber(companyChoice) >= 0)
                {
                    Market.Current.BuyList.PrintList(companyChoice);
                }
                else
                {
                    Console.WriteLine("Nobody wants to buy share packets in this company.");
                }
                Console.WriteLine("Enter amount and price");
                string Choice = Console.ReadLine();
                String[] substrings = Choice.Split(' ');
                if (substrings.Length > 1)
                {
                    Int32.TryParse(substrings[0], out amountChoice);
                    decimal.TryParse(substrings[1], NumberStyles.Any, new CultureInfo("en-US"), out priceChoice);
                    while (priceChoice <= 0)
                    {
                        Console.WriteLine("Enter price again(it must be positive decimal)");
                        decimal.TryParse(Console.ReadLine(), out priceChoice);

                    }
                    while (amountChoice <= 0)
                    {
                        Console.WriteLine("Enter amount again(it must be positive integer)");
                        int.TryParse(Console.ReadLine(), out amountChoice);
                    }
                    int number = Player.Current.Pocket.FindNumber(companyChoice);
                    if (number >= 0 && Player.Current.Pocket.List[number].CurrentQuantity >= amountChoice)
                    {
                        Market.Current.GenerateSellOffering(companyChoice, priceChoice, amountChoice, Player.Current.Name);
                        Player.Current.Pocket.List[Player.Current.Pocket.FindNumber(companyChoice)].CurrentQuantity -= amountChoice;
                        Player.Current.Pocket.RemoveEmptyEntries();
                    }
                    else
                    {
                        Console.WriteLine("You don't have enough shares of this company!");
                    }
                    command = " ";
                }
            }
        }

        public String[] RemoveDuplicates(String[] str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                for (int j = i + 1; j < str.Length; j++)
                {
                    if (str[i] == str[j])
                    {
                        for (int counter_shift = j; counter_shift < str.Length - 1; counter_shift++)
                        {
                            str[counter_shift] = str[counter_shift + 1];
                        }
                        if (str[i] == str[j])
                        {
                            j--;
                        }
                        Array.Resize(ref str, str.Length - 1);
                    }
                }
            }
            return str;
        }

        public void ReadEvent()
        {
            Event evnt = new Event();
            Random rand = new Random();
            int number = rand.Next(20, 21);
            string findevent = "[Event";
            findevent += number;
            findevent += "]";
            Market.Current.FindandAddEvent(evnt, findevent);
        }

        public void CancelOfferings()
        {
            if (Player.Current.History.List.Count == 0)
            {
                return;
            }
            Console.WriteLine("Do you want to cancel your offer/request?");
            Console.Write('$');
            string command = Console.ReadLine();
            ListActions Temp = new ListActions();
            Temp.Initialize();
            if (command.ToLower() == "y")
            {
                int j = 1;
                for (int i = 0; i < Player.Current.History.List.Count; i++)
                {
                    if (Player.Current.History.List[i].Owner == "REQUEST" || Player.Current.History.List[i].Owner == "OFFER")
                    {
                        Temp.List.Add(Player.Current.History.List[i]);
                        Console.WriteLine("{0} {1} {2} {3}", j, Player.Current.History.List[i].CompanyName, Player.Current.History.List[i].CurrentQuantity, Player.Current.History.List[i].Price);
                        Player.Current.History.List.RemoveAt(i);
                        i--;
                        j++;
                    }
                }
                if (j != 1)
                {
                    int number;
                    Console.WriteLine("Enter number of one that you want to cancel:");
                    Int32.TryParse(Console.ReadLine(), out number);
                    number--;
                    if (number > 0 && Temp.List.Count >= number && Temp.List[number].Owner == "REQUEST")
                    {
                        Player.Current.Money += Temp.List[number].Price * Temp.List[number].CurrentQuantity;
                        int pointToDelete = Market.Current.BuyList.FindNumber(Temp.List[number].CompanyName, Temp.List[number].Price, Temp.List[number].CurrentQuantity);
                        Market.Current.BuyList.List.RemoveAt(pointToDelete);
                        Temp.List.RemoveAt(number);
                        foreach (var packet in Temp.List)
                        {
                            Player.Current.History.List.Add(packet);
                        }
                    }
                    else if (number > 0 && Temp.List.Count >= number && Temp.List[number].Owner == "OFFER")
                    {
                        SharePacket pack = new SharePacket
                        {
                            Price = Temp.List[number].Price,
                            CurrentQuantity = Temp.List[number].CurrentQuantity,
                            CompanyName = Temp.List[number].CompanyName
                        };
                        Player.Current.Pocket.List.Add(pack);
                        int pointToDelete = Market.Current.SellList.FindNumber(Temp.List[number].CompanyName, Temp.List[number].Price, Temp.List[number].CurrentQuantity);
                        Market.Current.SellList.List.RemoveAt(pointToDelete);
                        Temp.List.RemoveAt(number);
                        foreach (var packet in Temp.List)
                        {
                            Player.Current.History.List.Add(packet);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("You don't have any requests or offers");
                }
            }
        }
    }
}
