using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace mattsATM
{

    class Atm
    {

        public bool debug = false;

        private string bankName;

        private string location;

        private string connectionString;

        private MySqlConnection bankDbConnection;

        public Atm(string inBankName, string inLocation)
        {
            this.bankName = inBankName;
            this.location = inLocation;
            this.connectionString = "";
            this.bankDbConnection = null;
        }

        private void dbConnect()
        {
            string infoString = getConnectionString();
            MySqlConnection dbConnection = new MySqlConnection(infoString);

            try
            {
                dbConnection.Open();
                if (debug)
                {
                    Console.WriteLine("\nConnection Open -----------------");
                }
                dbConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }

        }

        public int presentMenu()
        {
            Console.WriteLine(" --------------------------");
            Console.WriteLine("|                          |");
            Console.WriteLine("|  Welcome to Matt's ATM!  |");
            Console.WriteLine("|                          |");
            Console.WriteLine(" --------------------------\n");

            Console.WriteLine("Please choose from the following options:\n");
            Console.WriteLine("1. Log-in");
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
            // this method uses RNGCryptoServiceProvider to get cryptographic strength
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
            Console.WriteLine($"\nYour auto-generated UserID is {userID}\n");


            Console.WriteLine("Please enter a new 4 digit pin below.");
            Console.WriteLine("Remember this pin for future use.");
            Console.Write("> ");
            int userPin = Convert.ToInt32(Console.ReadLine());

            User newUser = new User(userFullName, userID, userPin);
        }

        public void userLogIn(User registeredUser)
        {
            string userID = registeredUser.getID();
        }

        public string getConnectionString()
        {
            return this.connectionString;
        }

        public void setConnectionString(string value)
        {
            this.connectionString = value;
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

        public string getID()
        {
            return this.ID;
        }

        public void setID(string value)
        {
            this.ID = value;
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

            switch (userSelection)
            {
                case 1:
                    Console.WriteLine("Not implemented yet.");
                    break;

                case 2:
                    newAtm.newUserSignUp();
                    break;

                case 3:
                    System.Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Selection unknown.");
                    break;
            }
        }
    }
}
