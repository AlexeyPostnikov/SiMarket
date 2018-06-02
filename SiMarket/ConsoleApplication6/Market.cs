using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace SiMarket
{

    class Market : MarketActions
    {
        private Market() { }
        private static readonly Market current = new Market();
        public static Market Current
        {
            get
            {
                return current;
            }
        }

        private int dayCounter;
        public string difficult;
        public bool showTolltips = false;

        private ListActions sellList = new ListActions();
        private ListActions buyList = new ListActions();
        private ListActions companiesList = new ListActions();
        private List<Event> currentEvents = new List<Event>();

        public int DayCounter
        {
            get
            {
                return dayCounter;
            }
            set
            {
                dayCounter = value;
            }
        }

        public string Difficult
        {
            get
            {
                return difficult;
            }
            set
            {
                difficult = value;
            }
        }

        public bool ShowTooltips
        {
            get
            {
                return showTolltips;
            }
            set
            {
                showTolltips = value;
            }
        }

        public ListActions SellList
        {
            get
            {
                return sellList;
            }
        }

        public ListActions BuyList
        {
            get
            {
                return buyList;
            }
        }

        public ListActions CompaniesList
        {
            get
            {
                return companiesList;
            }
        }

        public List<Event> CurrentEvents
        {
            get
            {
                return currentEvents;
            }
        }

        public void RefreshCurrentEvents()
        {
            for (int i = 0; i < CurrentEvents.Count; i++)
            {
                if (CurrentEvents[i].Duration > 0)
                {
                    CurrentEvents[i].Duration = CurrentEvents[i].Duration - 1;
                }

                if (CurrentEvents[i].Duration == 0)
                {
                    Console.WriteLine(CurrentEvents[i].EndMessage);
                    Console.ReadKey();
                    CurrentEvents.RemoveAt(i);
                    i--;
                }
            }
        }

        public decimal CheckReferences(SharePacket cmp)//!!!Обрабоать невозможность создания 2 одинаковых инвентов + перейти к новому формату ивентов!!!
        {
            decimal coeff = 1;
            for (int i = 0; i < CurrentEvents.Count; i++)
            {
                String[] substrings1 = CurrentEvents[i].CompanyType.Split(new char[] { ',' });
                String[] substrings2 = CurrentEvents[i].Influence.Split(new char[] { ',' });
                String[] substrings3 = CurrentEvents[i].Location.Split(new char[] { ',' });
                string companyName = CurrentEvents[i].CompanyName;


                coeff *= EventTypeComparer(cmp, substrings2, substrings1, substrings3, companyName);
                
            }
            
            return coeff;
        }

        public decimal EventTypeComparer(SharePacket cmp, string[] influences, string[] companyTypes, string[] locations, string companyName)
        {
            decimal eventInfluence;
            decimal.TryParse(influences[0], NumberStyles.Any, new CultureInfo("en-US"), out eventInfluence);
            decimal coef = 1;
            int i = 0;
            int j = 0;
                        
            if (eventInfluence>0)
            {
                if (companyName != null && cmp.CompanyName == companyName)
                {
                    coef *= eventInfluence;
                    return coef;
                }
                while (i < locations.Count())
                {
                    if (locations[i] == "All" || locations[i] == cmp.Location)
                    {
                        while (j < companyTypes.Count())
                        {
                            if (companyTypes[j] == "All" || companyTypes[j] == cmp.CompanyType )
                            {
                                coef *= eventInfluence;
                            }
                            j++;
                        }
                    }

                    i++;
                }
            }
            else 
            {
                Console.WriteLine("Influence was corrupt");               
            }          
            return coef;
        }

        public void CreateSellOfferings()
        {
            Random rnd = new Random();
            foreach (SharePacket cmp in CompaniesList.List)
            {
                decimal coeff = CheckReferences(cmp);
                if (coeff > 4) coeff = 4;
                if (coeff < 0.25m) coeff = 0.25m;
                decimal border = 20 * coeff;
               // Console.WriteLine("{0} {1} ", cmp.CompanyName, border);
                decimal chance = rnd.Next(1, 101);
                while (chance <= border)
                {
                    int amount = -1;
                    if (cmp.CurrentQuantity > 0)
                    {
                        if (cmp.CurrentQuantity > 1)
                        {
                            int currentQuantity = cmp.CurrentQuantity - 1;
                            int quantity = cmp.Quantity / 4;
                            if (currentQuantity < quantity)
                            {
                                amount = rnd.Next(1, currentQuantity);
                            }
                            else
                            {
                                amount = rnd.Next(1, quantity);
                            }
                        }
                        else
                        {
                            amount = 1;
                        }
                        if (SellList.FindNumber(cmp.CompanyName) > 0)
                        {
                            decimal price = SellList.List[SellList.FindNumber(cmp.CompanyName)].Price * rnd.Next(80, 105) / 100m;
                            GenerateSellOffering(cmp.CompanyName, Math.Round(price, 2), amount, null);
                        }
                        else
                        {
                            GenerateSellOffering(cmp.CompanyName, Math.Round(cmp.Price * rnd.Next(900, 1100) / 1000m, 2), amount, null);
                        }
                        cmp.CurrentQuantity = cmp.CurrentQuantity - amount;
                        Thread.Sleep(3);
                        chance = rnd.Next(1, 101);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void CreateBuyOfferings()
        {
            Random rnd = new Random();
            foreach (SharePacket cmp in CompaniesList.List)
            {
                decimal coeff = 1 / CheckReferences(cmp);
                if (coeff > 4) coeff = 4;
                if (coeff < 0.25m) coeff = 0.25m;  
                decimal border = 20 * coeff;
                decimal chance = rnd.Next(1, 101);
                while (chance <= border)
                {
                    int amount = -1;
                    int quantity = cmp.Quantity / 4;
                    amount = rnd.Next(1, quantity + 1);
                    if (BuyList.FindNumber(cmp.CompanyName) > 0)
                    {
                        decimal price = BuyList.List[BuyList.FindNumber(cmp.CompanyName)].Price * rnd.Next(95, 120) / 100m;
                        GenerateBuyOffering(cmp.CompanyName, Math.Round(price, 2), amount, null);

                    }
                    else
                    {
                        GenerateBuyOffering(cmp.CompanyName, Math.Round((cmp.Price * rnd.Next(900, 1100) / 1000m), 2), amount, null);
                    }
                    Thread.Sleep(3);
                    chance = rnd.Next(1, 101);
                }
            }
        }

        public void GenerateSellOffering(string name, decimal price, int quantity, string owner)
        {
            SharePacket SP = new SharePacket
            {
                CompanyName = name,
                Price = price,
                CurrentQuantity = quantity,
                Owner = owner
            };
            SellList.List.Add(SP);
            if (owner != null)
            {
                SharePacket SP2 = new SharePacket
                {
                    CompanyName = name,
                    Price = price,
                    CurrentQuantity = quantity,
                    Owner = "OFFER"
                };
                Player.Current.History.List.Add(SP2);
            }
        }

        public void GenerateBuyOffering(string name, decimal price, int quantity, string owner)
        {
            SharePacket SP = new SharePacket
            {
                CompanyName = name,
                Price = price,
                CurrentQuantity = quantity,
                Owner = owner
            };
            BuyList.List.Add(SP);
            if (owner != null)
            {
                SharePacket SP2 = new SharePacket
                {
                    CompanyName = name,
                    Price = price,
                    CurrentQuantity = quantity,
                    Owner = "REQUEST"
                };
                Player.Current.History.List.Add(SP2);
            }
        }

        public void SyncLists()
        {
            if (SellList.List.Count != 0 && BuyList.List.Count != 0)
            {
                List<string> companies = SellList.ReturnAllCompanies();
                foreach (var companyName in companies)
                {
                    var buy = BuyList.GetPackets(companyName);
                    var sell = SellList.GetPackets(companyName);
                    MakeDeals(buy, sell);
                }
            }
        }

        public void MakeDeals(List<SharePacket> buy, List<SharePacket> sell)// DODELAT
        {
            int i = 0;
            int j = 0;
            while (i < buy.Count && j < sell.Count)
            {
                if (buy[i].Owner != null && buy[i].Owner == sell[j].Owner)
                {
                    i++;
                    continue;
                }
                if (buy[i].Price < sell[j].Price)
                {
                    break;
                }
                var qty = Math.Min(buy[i].CurrentQuantity, sell[j].CurrentQuantity);

                buy[i].CurrentQuantity -= qty;
                sell[j].CurrentQuantity -= qty;
                if (buy[i].Owner == Player.Current.Name)
                {
                    var price = Math.Min(buy[i].Price, sell[j].Price);
                    OnBuy(buy[i], qty, price);
                }
                else if (sell[j].Owner == Player.Current.Name)
                {
                    var price = Math.Min(buy[i].Price, sell[j].Price);
                    OnSell(sell[j], qty, price);
                }
                else
                {
                    CompaniesList.List[CompaniesList.FindNumber(sell[j].CompanyName)].CurrentQuantity += qty;
                }
                if (buy[i].CurrentQuantity == 0)
                {
                    i++;
                    continue;
                }
                if (sell[j].CurrentQuantity == 0)
                {
                    j++;
                    continue;
                }
            }
            foreach (var packet in buy)
            {
                BuyList.List.Add(packet);
            }
            BuyList.RemoveEmptyEntries();
            foreach (var packet in sell)
            {
                SellList.List.Add(packet);
            }
            SellList.RemoveEmptyEntries();
        }

        public void OnBuy(SharePacket request, int quantity, decimal price)
        {
            SharePacket SP1 = new SharePacket();
            {
                SP1.CompanyName = request.CompanyName;
                SP1.Price = price;
                SP1.CurrentQuantity = quantity;
                SP1.Owner = "BUY";
            }
            SharePacket SP2 = new SharePacket();
            {
                SP2.CompanyName = request.CompanyName;
                SP2.CurrentQuantity = quantity;
            }
            //decimal temp = request.Price;
            //request.Owner = "BUY";
            //request.CurrentQuantity = quantity;                        
            for (int i = 0; i < Player.Current.History.List.Count; i++)
            {
                if (Player.Current.History.List[i].Owner == "REQUEST" && Player.Current.History.List[i].CompanyName == request.CompanyName && Player.Current.History.List[i].Price == request.Price)
                {
                    //request.Price = price;
                    Player.Current.Money += (Player.Current.History.List[i].Price - price);
                    Player.Current.History.List[i].CurrentQuantity -= quantity;
                    Player.Current.History.RemoveEmptyEntries();
                    //request.Price = temp;
                }
            }
            //request.Price = price;
            Player.Current.History.List.Add(SP1);
            Player.Current.Pocket.List.Add(SP2);
            //request.Price = temp;
        }

        public void OnSell(SharePacket offer, int quantity, decimal price)
        {
            Player.Current.Money += quantity * offer.Price;
            SharePacket pack = new SharePacket();
            {
                pack.CompanyName = offer.CompanyName;
                pack.Price = price;
                pack.CurrentQuantity = quantity;
                pack.Owner = "SELL";
            }
            Player.Current.History.List.Add(pack);
            for (int i = 0; i < Player.Current.History.List.Count; i++)
            {
                if (Player.Current.History.List[i].Owner == "OFFER" && Player.Current.History.List[i].CompanyName == offer.CompanyName && Player.Current.History.List[i].Price == offer.Price)
                {
                    Player.Current.History.List[i].CurrentQuantity -= quantity;
                    Player.Current.History.RemoveEmptyEntries();
                }
            }
        }

        public void RemoveOldOrders()
        {
            int i;
            for (i = 0; i < BuyList.List.Count; i++)
            {
                BuyList.List[i].Age = BuyList.List[i].Age + 1;
                if ((BuyList.List[i].Owner == null) && (BuyList.List[i].Age >= 30))
                {
                    BuyList.List.RemoveAt(i);
                    i--;
                }
            }
            for (i = 0; i < SellList.List.Count; i++)
            {
                SellList.List[i].Age = SellList.List[i].Age + 1;
                if ((SellList.List[i].Owner == null) && (SellList.List[i].Age >= 30))
                {
                    int number = CompaniesList.FindNumber(SellList.List[i].CompanyName);
                    CompaniesList.List[number].CurrentQuantity = CompaniesList.List[number].CurrentQuantity + SellList.List[i].CurrentQuantity;
                    SellList.List.RemoveAt(i);
                    i--;
                }
            }
        }

        public void CombineEquals(ListActions List)
        {
            List<int> Numbers = new List<int>();
            foreach (SharePacket cmp in CompaniesList.List)
            {
                int number = 0;
                Numbers.Clear();
                foreach (SharePacket packet in List.List)
                {
                    if (List.List[number].CompanyName == cmp.CompanyName)
                    {
                        Numbers.Add(number);
                    }
                    number++;
                }
                for (int i = 0; i < Numbers.Count - 1; i++)
                {
                    if (List.List[Numbers[i]].Price == List.List[Numbers[i + 1]].Price)
                    {
                        List.List[Numbers[i]].CurrentQuantity = List.List[Numbers[i]].CurrentQuantity + List.List[Numbers[i + 1]].CurrentQuantity;
                        List.List.RemoveAt(Numbers[i + 1]);
                        Numbers.RemoveAt(Numbers.Count - 1);
                        i--;
                    }
                }
            }
        }
    }
}



