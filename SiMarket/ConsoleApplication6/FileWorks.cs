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
    class FileWorks : ListActions
    {
        public void ReadCompaniesList()
        {
            String readLine;
            StreamReader fileRead = new StreamReader("CompaniesList.txt"); //"открыть файл"
            readLine = fileRead.ReadLine(); //Чтение первой строки. Начинать сразу в вайле - нельзя
            while (readLine != null)
            {
                SharePacket sp = new SharePacket();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                sp.InitSharePacket(substrings[0], decimal.Parse(substrings[4]), Int32.Parse(substrings[3]), substrings[1], substrings[2]);
                Market.Current.CompaniesList.List.Add(sp);
                readLine = fileRead.ReadLine();
            }
            fileRead.Close(); // Закрыть файл
        }

        public void CreateFileWithTypesAndLocations()   //Моя уметь называть функции
        {
            string writeType = null; //просто так надо вот и всё
            string writeLoc = null;
            StreamWriter writer = new StreamWriter("IDKHowToName.txt");   //Название топовой функции в маркет акшн ещё раз прочитайте
            foreach (SharePacket sp in Market.Current.CompaniesList.List)
            {
                writeType += sp.CompanyType;
                writeLoc += sp.Location;
                writeType += ", ";
                writeLoc += ", ";
            }
            writeLoc = writeLoc.Substring(0, writeLoc.Length - 2); //убрали последние ", "
            writeType = writeType.Substring(0, writeType.Length - 2); //убрали последние ", "
            String[] substringsTypes = writeType.Split(new[] { ", " }, StringSplitOptions.None); //разбили на подстроки для того чтоб убрать одинаковое
            String[] substringsLoc = writeLoc.Split(new[] { ", " }, StringSplitOptions.None); //разбили на подстроки для того чтоб убрать одинаковое
            substringsTypes = Market.Current.RemoveDuplicates(substringsTypes); //убрали одинаковые вхождения
            substringsLoc = Market.Current.RemoveDuplicates(substringsLoc); //убрали одинаковые вхождения
            writeType = String.Join(", ", substringsTypes); //сделали строку обратно
            writeLoc = String.Join(", ", substringsLoc);
            writer.WriteLine(writeType); //записали список типов
            writer.WriteLine(writeLoc); //Записали список локаций
            writer.Close();
        }

        // ЗАГРУЗКА        
        public void Load()
        {
            Market.Current.BuyList.List.Clear();
            Market.Current.SellList.List.Clear();
            Console.WriteLine("Enter save's name");
            string save = Console.ReadLine();
            save += ".save";

            String readLine;
            StreamReader fileRead = new StreamReader(save); //"открыть файл"
            readLine = fileRead.ReadLine();
            Market.Current.Difficult = readLine;
            readLine = fileRead.ReadLine();
            Market.Current.ShowTooltips = bool.Parse(readLine);
            readLine = fileRead.ReadLine();
            Market.Current.DayCounter = Int32.Parse(readLine); //Какой день
            readLine = fileRead.ReadLine();
            Player.Current.Name = readLine;  //Чтение имени игрока
            readLine = fileRead.ReadLine();
            Player.Current.Money = decimal.Parse(readLine); //Чтение кол-ва денег
            readLine = fileRead.ReadLine();

            while (readLine != "History")   //Чтение инвентаря 
            {
                SharePacket sp = new SharePacket();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                sp.InitSharePacket(substrings[0], decimal.Parse(substrings[2]), Int32.Parse(substrings[1])); //инициация sp имя, цена, кол-во
                sp.Owner = Player.Current.Name;  //владелец - игрок
                Player.Current.Pocket.List.Add(sp);                //закинуть в инвентарь
                readLine = fileRead.ReadLine();
            }
            readLine = fileRead.ReadLine();
            while (readLine != "BuyList")   //Чтение истории
            {
                SharePacket sp = new SharePacket();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                sp.InitSharePacket(substrings[0], decimal.Parse(substrings[2]), Int32.Parse(substrings[1])); //инициация sp имя, цена, кол-во
                sp.Owner = substrings[3]; //чтение buy или sell
                Player.Current.History.List.Add(sp);    //закинуть в историю
                readLine = fileRead.ReadLine();
            }
            readLine = fileRead.ReadLine();
            while (readLine != "SellList")   //Чтение BuyList
            {
                SharePacket sp = new SharePacket();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                sp.InitSharePacket(substrings[0], decimal.Parse(substrings[3]), Int32.Parse(substrings[2]), Int32.Parse(substrings[1])); //инициация sp имя, цена, кол-во, возраст
                if (4 < substrings.Count())  //если есть владелец (игрок) присвоить владельца
                {
                    sp.Owner = substrings[4];
                }
                Market.Current.BuyList.List.Add(sp);   //закинуть в BuyList                             
                readLine = fileRead.ReadLine();
            }
            readLine = fileRead.ReadLine();
            while (readLine != "Events")   //Чтение SellList
            {
                SharePacket sp = new SharePacket();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                sp.InitSharePacket(substrings[0], decimal.Parse(substrings[3]), Int32.Parse(substrings[2]), Int32.Parse(substrings[1])); //инициация sp имя, цена, кол-во, возраст
                if (4 < substrings.Count())  //если есть владелец (игрок) присвоить владельца
                {
                    sp.Owner = substrings[4];
                }
                Market.Current.SellList.List.Add(sp); //закинуть в SellList
                readLine = fileRead.ReadLine();
            }
            readLine = fileRead.ReadLine();
            while (readLine != null)   //Чтение ивентов
            {
                Event evnt = new Event();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                evnt.Location = substrings[0];    //локация
                evnt.CompanyType = substrings[1]; //тип
                evnt.Influence = substrings[2];   //множитель
                evnt.Duration = Int32.Parse(substrings[3]);   //длительность
                if (4 < substrings.Count())
                {
                    evnt.CompanyName = substrings[4]; //имя компании если есть
                }
                Market.Current.CurrentEvents.Add(evnt);
                readLine = fileRead.ReadLine();
            }
            foreach (SharePacket cmp in Market.Current.CompaniesList.List) //задать текущее колл-во акций исходя из кол-ва в селку !и кол-ва в инвентаре у игрока!
            {
                string cmpname = cmp.CompanyName;
                int number = Player.Current.Pocket.FindNumber(cmp.CompanyName);
                if (number != -1)
                {
                    cmp.CurrentQuantity = cmp.Quantity - Market.Current.SellList.GetAmountOfShares(cmpname) - Player.Current.Pocket.List[number].CurrentQuantity;
                }
                else
                {
                    cmp.CurrentQuantity = cmp.Quantity - Market.Current.SellList.GetAmountOfShares(cmpname);
                }
            }


            fileRead.Close(); // Закрыть файл
        }

        //СОХРАНЕНИЯ        
        public void Save()
        {
            Console.WriteLine("Enter save's name");
            string save = Console.ReadLine();
            save += ".save";
            StreamWriter sw = new StreamWriter(save);
            sw.WriteLine(Market.Current.Difficult);
            sw.WriteLine(Market.Current.ShowTooltips);
            sw.WriteLine(Market.Current.DayCounter);//День
            sw.WriteLine(Player.Current.Name); //записываем имя
            sw.WriteLine(Player.Current.Money);//записываем деньги
            foreach (SharePacket sp in Player.Current.Pocket.List) //записываем инвентарь
            {
                string towrite;
                towrite = sp.CompanyName;
                towrite += ", ";
                towrite += sp.CurrentQuantity;
                towrite += ", ";
                towrite += sp.Price;
                sw.WriteLine(towrite);
            }
            sw.WriteLine("History");
            foreach (SharePacket sp in Player.Current.History.List) //записываем историю
            {
                string towrite;
                towrite = sp.CompanyName;
                towrite += ", ";
                towrite += sp.CurrentQuantity;
                towrite += ", ";
                towrite += sp.Price;
                towrite += ", ";
                towrite += sp.Owner;
                sw.WriteLine(towrite);
            }
            sw.WriteLine("BuyList");
            foreach (SharePacket sp in Market.Current.BuyList.List) //записываем BuyList
            {
                string towrite;
                towrite = sp.CompanyName;
                towrite += ", ";
                towrite += sp.Age;
                towrite += ", ";
                towrite += sp.CurrentQuantity;
                towrite += ", ";
                towrite += sp.Price;
                if (sp.Owner != null)
                {
                    towrite += ", ";
                    towrite += sp.Owner;
                }
                sw.WriteLine(towrite);
            }
            sw.WriteLine("SellList");
            foreach (SharePacket sp in Market.Current.SellList.List) //записываем SellList
            {
                string towrite;
                towrite = sp.CompanyName;
                towrite += ", ";
                towrite += sp.Age;
                towrite += ", ";
                towrite += sp.CurrentQuantity;
                towrite += ", ";
                towrite += sp.Price;
                if (sp.Owner != null)
                {
                    towrite += ", ";
                    towrite += sp.Owner;
                }
                sw.WriteLine(towrite);
            }
            sw.WriteLine("Events");
            foreach (Event evnt in Market.Current.CurrentEvents) //записываем ивенты
            {
                string towrite;
                towrite = evnt.Location;
                towrite += ", ";
                towrite += evnt.CompanyType;
                towrite += ", ";
                towrite += evnt.Influence;
                towrite += ", ";
                towrite += evnt.Duration;
                if (evnt.CompanyName != null)
                {
                    towrite += ", ";
                    towrite += evnt.CompanyName;
                }
                sw.WriteLine(towrite);
            }
            sw.Close();
        }

        public void FindandAddEvent(Event evnt, string findevent)
        {
            string readline;
            StreamReader sr = new StreamReader("Events.txt");//должно быть try {sr} finally {sr.close;}
            readline = sr.ReadLine();
            while (readline != findevent) //поиск рандомного ивента
            {
                readline = sr.ReadLine();
            }
            readline = sr.ReadLine();   //чтение типа компании
            if (readline == "Random")
            {
                Random rand = new Random();
                string type;
                StreamReader reader = new StreamReader("IDKHowToName.txt"); //открыли список типов и локаций               
                type = reader.ReadLine();
                String[] types = type.Split(new[] { ", " }, StringSplitOptions.None); //получили список всех типов           
                evnt.CompanyType = types[rand.Next(types.Length)]; //присвоили рандомный тип
                evnt.WhatRandom = "Type";
            }
            else if (readline != "")
            {
                evnt.CompanyType = readline;
            }
            readline = sr.ReadLine();   //чтение локации
            if (readline == "Random") //рандомная локация
            {
                if (evnt.CompanyType != null) //если есть тип
                {
                    SharePacket cmp = new SharePacket();
                    List<SharePacket> listwType = new List<SharePacket>();
                    listwType = FindType(evnt.CompanyType); // список пакетов нужного типа
                    Random rand = new Random();
                    cmp = listwType.ElementAt(rand.Next(listwType.Count()));
                    evnt.Location = cmp.Location; //присвоили рандомную локацию                    
                }
                else //если нет типа
                {
                    Random rand = new Random();
                    string loc;
                    StreamReader reader = new StreamReader("IDKHowToName.txt"); //открыли список типов и локаций
                    reader.ReadLine();
                    loc = reader.ReadLine();
                    String[] locs = loc.Split(new[] { ", " }, StringSplitOptions.None); //получили список всех локаций                
                    evnt.Location = locs[rand.Next(locs.Length)]; //присвоили рандомную локацию
                }
                evnt.WhatRandom = "Location";
            }
            else if (readline != "") //не рандомная локация
            {
                evnt.Location = readline;
            }
            readline = sr.ReadLine();   // чтение названия компании
            if (readline == "Random")
            {
                SharePacket cmp = new SharePacket();
                Random rand = new Random();
                cmp = Market.Current.CompaniesList.List.ElementAt(rand.Next(Market.Current.CompaniesList.List.Count()));
                evnt.CompanyName =cmp.CompanyName;
                evnt.WhatRandom = "Company";
                evnt.CompanyType = cmp.CompanyType;
                evnt.Location = cmp.Location;
            }
            else if (readline != "")
            {
                evnt.CompanyName = readline;
            }
            readline = sr.ReadLine();   //чтение множителя
            String[] array = readline.Split(new[] { "-" }, StringSplitOptions.None);
            if (array.Count() == 2)
            {
                Random rand = new Random();
                double Min = Double.Parse(array[0]);
                double Max = Double.Parse(array[1]);
                //  String[] firstnumb = array[0].Split(new[] { "." }, StringSplitOptions.None);
                //  String[] secondnumb = array[1].Split(new[] { "." }, StringSplitOptions.None);
                double Res = rand.NextDouble() * (Max - Min) + Min;
                evnt.Influence += Res;
             //   int fractoffirst = Int32.Parse(firstnumb[1]);
            //    int fractofsecond = Int32.Parse(secondnumb[1]);
            //    evnt.Influence += ".";
           //     if (fractoffirst < fractofsecond)
           //     {
           //         evnt.Influence += rand.Next(fractoffirst, fractofsecond + 1);
           //     }
            //    else if (fractoffirst > fractofsecond)
            //    {
          //          evnt.Influence += rand.Next(fractofsecond, fractoffirst + 1);
           //     }
           //     else
         //       {
           //         evnt.Influence += fractoffirst;
           //     }
            }
            else
            {
                evnt.Influence = readline;
            }

            readline = sr.ReadLine();   //чтение длительности
            if (readline == "forever")
            {
                evnt.Duration = -1;
            }
            else
            {
                evnt.Duration = Int32.Parse(readline);
            }
            readline = sr.ReadLine();
            evnt.Description = readline;
            if (evnt.WhatRandom != null) //если был рандом
            {
                switch (evnt.WhatRandom)
                {
                    case "Type": { evnt.Description += (" " + evnt.CompanyType); } break;
                    case "Location": { evnt.Description += (" " + evnt.Location); } break;
                    case "Company": { evnt.Description += (" " + evnt.CompanyName); } break;
                }
            }
            readline = sr.ReadLine();
            String[] array1 = readline.Split(new[] { "!" }, StringSplitOptions.None);
            if (array1.Count() == 2) //DODELAT
            {
                evnt.EndMessage += (array1[0] + evnt.CompanyName);
            }
            evnt.EndMessage = readline;
            Market.Current.CurrentEvents.Add(evnt); //добавили в current events
            Console.WriteLine("Event happened\n {0}", evnt.Description);
            if (Market.Current.ShowTooltips == true)
            {
                readline = sr.ReadLine();
                evnt.Tooltips = readline;
                Console.WriteLine(evnt.Tooltips);
            }
            Console.ReadKey();
            sr.Close();
        }

        public void StartGame()
        {
            Console.WriteLine("Enter your name:");
            Console.Write("$");
            string input = Console.ReadLine();
            if (input == "Load" || input == "load")
            {
                Load();
            }
            else
            {
                Player.Current.Name = input;
                Player.Current.StartingMoney = 100000;
                Console.WriteLine("Enter difficult");
                Market.Current.Difficult = Console.ReadLine();
                if (Market.Current.Difficult == "Easy" || Market.Current.Difficult == "easy")
                {
                    Market.Current.showTolltips = true;
                }
            }
        }

    }
}
