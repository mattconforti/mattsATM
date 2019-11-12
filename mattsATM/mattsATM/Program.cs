using System;
using System.IO;
using System.Collections.Generic;
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
                Console.WriteLine($"Exception: {e.ToString()}");
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
                Console.WriteLine($"Exception: {e.ToString()}");
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
            int usrChoice;
            if (! (Int32.TryParse(Console.ReadLine(), out usrChoice)))
            {
                Console.WriteLine("\nEnter only a number 1-3.");
                Environment.Exit(0);
            }
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
            Console.WriteLine("\nPlease enter your First, Middle (if applicable), and Last name below.");
            Console.Write("> ");
            string usrInfoIn = Console.ReadLine();
            string userFullName;
            string[] nameArr = usrInfoIn.Split(' ');
            int arrayLen = nameArr.Length;

            if (arrayLen < 2 || arrayLen > 3)  // has to be first, middle (if applicable), last
            {
                Console.WriteLine("\nName not accepted.\nApplication quitting for security reasons.");
                Environment.Exit(0);
            }

            foreach (var name in nameArr)
            {
                foreach (var character in name)
                {
                    if (debug)
                    {
                        Console.WriteLine(character);
                    }

                    if (!(Char.IsLetter(character))) // if any of the characters is not a letter
                    {
                        Console.WriteLine("\nName not accepted.\nApplication quitting for security reasons.");
                        Environment.Exit(0);
                    }
                }
            }
            userFullName = usrInfoIn;  // name is valid
            
            string userID = idGen();
            Console.WriteLine($"\nYour auto-generated UserID is {userID}\n");


            Console.WriteLine("Please enter a new 4 digit pin below.");
            Console.WriteLine("\n** REMEMBER YOUR ID AND PIN FOR FUTURE USE **\n");
            Console.Write("> ");
            string userInput = Console.ReadLine();
            int userPin;
            if (userInput.Length < 4)
            {
                Console.WriteLine("\nPin not accepted.\nApplication quitting for security reasons.");
                Environment.Exit(0);
            }

            if (!(Int32.TryParse(userInput, out userPin))) // if can't be converted to int
            {
                Console.WriteLine("\nPin not accepted.\nApplication quitting for security reasons.");
                Environment.Exit(0);
            }

            User newUser = new User(userFullName, userID, userPin);
            return newUser;  // return the new User with validated info
        }

        public void registerNewUser(User newUser)
        {
            string sqlString = $"INSERT INTO USERINFO VALUES (\"{newUser.Name}\", \"{newUser.ID}\"" +
                $", {newUser.Pin});";

            if (debug)
            {
                Console.WriteLine($"SQL COMMAND: {sqlString}");
            }

            try  // will succeed if bankDbConnection is properly set
            {
                MySqlCommand dbCommand = new MySqlCommand(sqlString, bankDbConnection);
                dbCommand.ExecuteNonQuery();  // use for insert, update, and delete commands
                if (debug)
                {
                    Console.WriteLine("\nCommand Executed.\n");
                }

            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception:\n{e.ToString()}");
            }
        }

        public void userLogIn()
        {
            Console.WriteLine("\nEnter your ID and Pin below to log-in");
            Console.Write("ID > ");
            string idIn = Console.ReadLine();
            //TODO validate this input (11 characters, 1 word, etc.)
            Console.Write("Pin > ");
            string pinIn = Console.ReadLine();
            //TODO validate this input and convert to int etc

            // check if the user's input matches whats in the db
            string idQueryString = "SELECT ID FROM USERINFO;";
            List<string> dbQueryResults = new List<string>();  // do not know a specified length

            try
            {
                MySqlCommand command = new MySqlCommand(idQueryString, bankDbConnection);
                MySqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                { 
                    dbQueryResults.Add(dataReader[0].ToString());
                    //TODO if the ID matches, get the pin for that ID from db and see if it matches
                    // the pin that was entered
                    // if so, this users info was correct and he/she is logged in - open new menu
                }
                dataReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception:\n{e.ToString()}");
            }

            if (debug)
            {
                foreach (var item in dbQueryResults)
                {
                    Console.WriteLine($"Item Found: {item}");
                }
            }

            if (dbQueryResults.Contains(idIn))  // if the database contains the ID entered
            {
                if (debug)
                {
                    Console.WriteLine("\nID match!");
                }
            }
            else
            {
                if (debug)
                {
                    Console.WriteLine("\nNo match.");
                }
            }
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
                    newAtm.userLogIn();
                    break;

                case 2:
                    User newUser = newAtm.newUserSignUp();
                    newAtm.registerNewUser(newUser);
                    break;

                case 3:
                    newAtm.dbCloseConnection();
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("\nSelection unknown.");
                    break;
            }
            newAtm.dbCloseConnection();  // close the db connection after break
        }
    }
}
