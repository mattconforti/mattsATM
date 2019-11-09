using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace mattsATM
{

    class Atm
    {

        public bool debug = true;

        private string bankName;

        private string location;

        private string connectionString;

        private MySqlConnection bankDbConnection;

        public Atm(string inBankName, string inLocation)
        {
            this.bankName = inBankName;
            this.location = inLocation;
            this.connectionString = "Database=mattsAtmDb; Uid=atmQuery; Pwd=atmPwd;";
            this.bankDbConnection = null;
        }

        public void dbOpenConnection()
        {
            string infoString = ConnectionString;
            MySqlConnection dbConnection = new MySqlConnection(infoString);

            try
            {
                dbConnection.Open();
                // set the bankDbConnection instance var
                bankDbConnection = dbConnection;
                if (debug)
                {
                    Console.WriteLine("\nDb Connection Open -------------------");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }

        public void dbCloseConnection()
        {
            MySqlConnection toBeClosed = bankDbConnection;

            try
            {
                toBeClosed.Close();  // close the connection and set the var to null
                bankDbConnection = null;
                if (debug)
                {
                    Console.WriteLine("\nDb Connection Closed -------------------");
                }
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
            Console.WriteLine($"Bank: {bankName}");
            Console.WriteLine($"Location: {location}");
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


        public User newUserSignUp()
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
            return newUser;
        }

        public void userLogIn(User registeredUser)
        {
            string userID = registeredUser.ID;
        }

        // ConnectionString property
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }

            set
            {
                connectionString = value;
            }
        }

    }

    class User
    {

        private string name;

        private string id;

        private int pin;

        public User(string inName, string inID, int inPin)
        {
            this.name = inName;
            this.id = inID;
            this.pin = inPin;
        }

        // Name property of type string
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

        // ID property
        public string ID
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
          
         }

        // Pin property of type int
        public int Pin
        {
            get
            {
                return pin;
            }
            set
            {
                pin = value;
            }
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

            newAtm.dbOpenConnection();
            int userSelection = newAtm.presentMenu();

            switch (userSelection)
            {
                case 1:
                    Console.WriteLine("Not implemented yet.");
                    break;

                case 2:
                    User newUser = newAtm.newUserSignUp();
                    break;

                case 3:
                    newAtm.dbCloseConnection();
                    System.Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Selection unknown.");
                    break;
            }
            newAtm.dbCloseConnection();  // close the db connection after break
        }
    }
}
