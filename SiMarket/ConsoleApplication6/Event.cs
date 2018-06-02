using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiMarket
{
    class Event
    {
        private string companyName;
        private string location;
        private string companyType;
        private string description;
        private string influence;
        private int duration;
        private string tooltips;
        private string whatRandom;
        private string endMessage;

        public string EndMessage
        {
            get
            {
                return endMessage;
            }

            set
            {
                endMessage = value;
            }
        }
        public string WhatRandom
        {
            get
            {
                return whatRandom;
            }
            set
            {
                whatRandom = value;
            }
        }

        public string Tooltips
        {
            get
            {
                return tooltips;
            }
            set
            {
                tooltips = value;
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

        public string Influence
        {
            get
            {
                return influence;
            }

            set
            {
                influence = value;
            }
        }

        public int Duration
        {
            get
            {
                return duration;
            }

            set
            {
                duration = value;
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

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public void InitEvent(string location, string companyType)
        {
            CompanyType = companyType;
            Location = location;
        }
    }
}
