using System;
using System.Collections.Generic;

namespace mattsATM
{

    class Atm
    {

        public bool debug = true;

        private string bankName;

        private string location;

        // Generic Dictionary Collection
        private Dictionary<int, string> usrInfoDict;

        public Atm(string inBankName, string inLocation)
        {
            this.bankName = inBankName;
            this.location = inLocation;
            this.usrInfoDict = new Dictionary<int, string>();
        }

        public void printInfo()
        {
            Console.WriteLine("ATM INFO -------");
            Console.WriteLine($"Bank: {this.bankName}");
            Console.WriteLine($"Location: {this.location}");
        }

        public void userLogIn(User newUser)
        {
            string userID = newUser.getID();
        }

    }

    class User
    {
        private string ID;

        private int pin;

        public User(string inID, int inPin)
        {
            this.ID = inID;
            this.pin = inPin;
        }

        public void setID(string value)
        {
            this.ID = value;
        }

        public string getID()
        {
            return this.ID;
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            Atm newAtm = new Atm("CitiBank", "New York");
            User mattC = new User("21", 1234);
            newAtm.userLogIn(mattC);

            if (newAtm.debug)
            {
                newAtm.printInfo();
            }
        }
    }
}
