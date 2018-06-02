using System;
using System.Threading;

namespace SiMarket
{
    class Program
    {
        static void Main()
        {
            Random rnd = new Random();
            bool running = true;

            Player igrok = Player.Current;
            Market Rblnok = Market.Current;
            Rblnok.BuyList.Initialize();
            Rblnok.SellList.Initialize();
            Rblnok.CompaniesList.Initialize();
            Rblnok.ReadCompaniesList();
            Rblnok.CreateFileWithTypesAndLocations();
            Rblnok.DayCounter = 1;

            igrok.Pocket.Initialize();
            igrok.History.Initialize();
            Rblnok.StartGame();            

            while (running)
            {
                Console.WriteLine("\nStarting day {0}...\n", Rblnok.DayCounter);

                igrok.DisplayInfo();

                igrok.DisplayHistory();

                Random RANDOM = new Random();

                int chance = RANDOM.Next(1, 11);
                if (chance <= 10) Rblnok.ReadEvent();

                Rblnok.CreateSellOfferings();
                Thread.Sleep(200);
                Rblnok.CreateBuyOfferings();
                Rblnok.SellList.SortUp();
                Rblnok.BuyList.SortDown();

                Rblnok.CombineEquals(Rblnok.SellList);
                Rblnok.CombineEquals(Rblnok.BuyList);


                Console.WriteLine("\nSell List: ");
                Rblnok.SellList.PrintList();
                Console.WriteLine("\nBuy List: ");
                Rblnok.BuyList.PrintList();
                
                Rblnok.CancelOfferings();

                Rblnok.ChooseAction();


                Rblnok.BuyList.RemoveEmptyEntries();
                Rblnok.RemoveOldOrders();

                igrok.Structurise();
                igrok.Pocket.RemoveEmptyEntries();
                igrok.DisplayHistory();

                Console.ReadKey();

                Rblnok.SellList.SortUp();
                Rblnok.BuyList.SortDown();

                Rblnok.SyncLists();
                Rblnok.RefreshCurrentEvents();
                Rblnok.DayCounter++;
            }
        }
    }
}
