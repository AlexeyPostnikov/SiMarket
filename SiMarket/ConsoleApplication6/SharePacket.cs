namespace SiMarket
{

    class SharePacket
    {

        private int quantity;   //Общее количество акций в обороте
        private int currentQuantity; //Количество доступных для покупки акций
        private string location;
        private string companyType;
        private decimal price;
        private string owner;
        private string companyName;
        private int age;        
        
        public int Quantity
        {
            get
            {                
                return quantity;
            }

            set
            {
                quantity = value;
            }
        }

        public int CurrentQuantity
        {
            get
            {
                return currentQuantity;
            }

            set
            {
                currentQuantity = value;
            }
        }

        public string Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }

        public string CompanyType
        {
            get
            {
                return companyType;
            }

            set
            {
                companyType = value;
            }
        }

        public decimal Price
        {
            get
            {
                return price;
            }

            set
            {
                price = value;
            }
        }

        public string Owner
        {
            get
            {
                return owner;
            }

            set
            {
                owner = value;
            }
        }

        public string CompanyName
        {
            get
            {
                return companyName;
            }

            set
            {
                companyName = value;
            }
        }

        public int Age
        {
            get
            {
                return age;
            }

            set
            {
                age = value;
            }
        }

        public void InitSharePacket(string name, decimal price, int amount, string location, string type)
        {
            CompanyName = name;
            Price = price;
            CurrentQuantity = amount;
            Quantity = amount;
            Age = 0;
            Location = location;
            CompanyType = type;
        }
        public void InitSharePacket(string name, decimal price, int amount, int age)
        {
            CompanyName = name;
            Price = price;
            CurrentQuantity = amount;
            Quantity = amount;
            Age = age;
        }
        public void InitSharePacket(string name, decimal price, int amount)
        {
            CompanyName = name;
            Price = price;
            CurrentQuantity = amount;
        }
    }
}
