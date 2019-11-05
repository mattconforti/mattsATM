using System;
using System.IO;
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

        public int presentMenu()
        {
            Console.WriteLine(" --------------------------");
            Console.WriteLine("|                          |");
            Console.WriteLine("|  Welcome to Matt's ATM!  |");
            Console.WriteLine("|                          |");
            Console.WriteLine(" --------------------------\n");

            Console.WriteLine("Please choose from the following options:\n");
            Console.WriteLine("1. Log In");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Exit");

            Console.WriteLine("\nEnter your choice below. ");
            Console.Write("> ");
            int usrChoice = Convert.ToInt32(Console.ReadLine());
            return usrChoice;
        }

        public void printInfo()
        {
            Console.WriteLine("ATM INFO -------");
            Console.WriteLine($"Bank: {this.bankName}");
            Console.WriteLine($"Location: {this.location}");
        }

        /// <summary>
        /// creates an 11 digit random identifier
        /// </summary>
        /// <returns></returns>
        public string idGen()
        {
            // get random file name from Systen.Path
            // this method uses RNGCryptoServiceProvider to get cryptographic strength"
            string randomPathName = Path.GetRandomFileName();
            randomPathName = randomPathName.Replace(".", "");
            return randomPathName;
        }


        public void newUserSignUp()
        {
            Console.WriteLine("\nPlease enter your full name.");
            Console.Write("> ");
            string userFullName = Console.ReadLine();

            string userID = idGen();
            Console.WriteLine($"\nYour auto-generated UseID is {userID}\n");


            Console.WriteLine("Please enter a new 4 digit pin below.");
            Console.WriteLine("Remember this pin for future use.");
            Console.Write("> ");
            string userPin = Console.ReadLine();


        }

        public void userLogIn(User registeredUser)
        {
            string userID = registeredUser.getID();
        }

    }

    class User
    {

        private string name;

        private string ID;

        private int pin;

        public User(string inName, string inID, int inPin)
        {
            this.name = inName;
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
            if (newAtm.debug)
            {
                newAtm.printInfo();
            }

            int userSelection = newAtm.presentMenu();

            if (userSelection == 2)
            {
                newAtm.newUserSignUp();
            }
        }

    }
}
