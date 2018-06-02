using System;


namespace SiMarket
{
    class Player
    {
        private Player() { }

        private static readonly Player current = new Player();
        public static Player Current
        {
            get { return current; }
        }

        private string name;
        private decimal startingMoney;
        private decimal money;
        private ListActions pocket = new ListActions();
        private ListActions history = new ListActions();        
        
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public decimal StartingMoney
        {
            get
            {
                return startingMoney;
            }

            set
            {
                startingMoney = value;
                Money = value;
            }
        }

        public decimal Money
        {
            get
            {
                return money;
            }

            set
            {
                money = value;
            }
        }

        public ListActions Pocket
        {
            get
            {
                return pocket;
            }
        }        

        public ListActions History
        {
            get
            {
                return history;
            }
        }
        
        public void DisplayHistory()
        {
            if (History.List.Count > 0)
            {
                Console.WriteLine("\nHistory of operations");
                foreach (SharePacket packet in History.List)
                {
                    Console.WriteLine("{0}: {1} shares; price: {2}         {3}", packet.CompanyName, packet.CurrentQuantity, packet.Price, packet.Owner);
                }
            }
            else
            {
                Console.WriteLine("Your history is empty");
            }
        }

        public decimal ReturnHistoryBalance()
        {
            decimal balance = 0;          
            foreach (SharePacket packet in History.List)
            {
                if (packet.Owner == "BUY")
                {
                    balance += packet.Price * packet.CurrentQuantity;
                }
                else
                {
                    balance -= packet.Price * packet.CurrentQuantity;
                }
            }
            return -balance;
        }

        public void DisplayInfo()
        {
            Console.WriteLine("\n" + Name);
            Console.WriteLine("Current money: " + Money);
            if (Pocket.List.Count > 0)
            {
                Console.WriteLine("You have:");
                foreach (SharePacket packet in Pocket.List)
                {
                    Console.WriteLine("{0}: {1} shares", packet.CompanyName, packet.CurrentQuantity);
                }
            }
            else
            {
                Console.WriteLine("Your pocket is empty");
            }
            Console.WriteLine("Your score: " + ReturnHistoryBalance());
        }

        public void Structurise()
        {           
            foreach (SharePacket packet in Pocket.List)
            {
                packet.Price = 0;
            }
            Pocket.SortCompany();
            for (int i = 0; i < Pocket.List.Count - 1; i++)
            {
                if (Pocket.List[i].CompanyName == Pocket.List[i + 1].CompanyName)
                {
                    Pocket.List[i].CurrentQuantity = Pocket.List[i].CurrentQuantity + Pocket.List[i + 1].CurrentQuantity;
                    Pocket.List.RemoveAt(i + 1);
                    i--;
                }
            }
        }
    }
}
