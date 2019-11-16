using System;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace mattsATM
{
    /// <summary>
    /// The Atm class: a simulation of an automated teller machine
    /// </summary>
    /// <remarks>
    /// Uses a MySQL database to store log-in info, atm transactions, balances etc.
    /// and gives users with valid credentials access to normal banking functions
    /// </remarks>
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

        public void DbOpenConnection()
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

        public void DbCloseConnection()
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

        public int PresentMenu()
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
                Console.WriteLine("\nEnter only a number (1-3).");
                Environment.Exit(0);
            }
            return usrChoice;
        }

        public int PresentLoggedInMenu()
        {
            Console.WriteLine("\nPlease select an option from the menu below:\n");
            Console.WriteLine("1. Withdraw");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Display Balance");
            Console.WriteLine("4. Display Transaction History");
            Console.WriteLine("5. Exit\n");
            Console.Write("> ");
            int usrChoice;
            if (!(Int32.TryParse(Console.ReadLine(), out usrChoice)))
            {
                Console.WriteLine("\nEnter only a number (1-5).");
                Environment.Exit(0);
            }
            return usrChoice;
        }

        public void PrintInfo()
        {
            Console.WriteLine("ATM INFO -------");
            Console.WriteLine($"Bank: {bankName}");
            Console.WriteLine($"Location: {location}");
        }

        /// <summary>
        /// creates an 11 digit random identifier
        /// </summary>
        /// <returns>
        /// randomPathName: an 11 digit randomized string of letters and numbers
        /// </returns>
        public string IdGen()
        {
            // get random file name from Systen.Path
            // this method uses RNGCryptoServiceProvider to get cryptographic strength
            string randomPathName = Path.GetRandomFileName();
            randomPathName = randomPathName.Replace(".", "");
            return randomPathName;
        }

        public User NewUserSignUp()
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
            
            string userID = IdGen();
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

        public void RegisterNewUser(User newUser)
        {
            string sqlString = $"INSERT INTO USERS VALUES (\"{newUser.Name}\", \"{newUser.ID}\"" +
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

        public bool UserLogIn()
        {
            bool isLoggedIn = false;
            Console.WriteLine("\nEnter your ID and Pin below to log-in");
            Console.Write("ID > ");
            string idIn = Console.ReadLine();
            //TODO validate this input (11 characters, 1 word, etc.)
            Console.Write("Pin > ");
            string pinIn = Console.ReadLine();
            int iPinIn = Convert.ToInt32(pinIn);
            //TODO validate this input

            // check if the user's input matches whats in the db
            string idQueryString = "SELECT ID FROM USERS;";
            List<string> dbQueryResults = new List<string>();  // no specific length

            try
            {
                MySqlCommand command = new MySqlCommand(idQueryString, bankDbConnection);
                MySqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    dbQueryResults.Add(dataReader[0].ToString());
                }
                dataReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception:\n{e.ToString()}");
                return isLoggedIn;
            }

            if (debug)
            {
                foreach (var item in dbQueryResults)
                {
                    Console.WriteLine($"\nItem Found: {item}");
                }
            }

            if (dbQueryResults.Contains(idIn))  // if the database contains the ID entered
            {
                if (debug)
                {
                    Console.WriteLine("\nID match!");
                }

                string checkPinQuery = $"SELECT PIN FROM USERS WHERE ID=\"{idIn}\"";

                try
                {
                    MySqlCommand command = new MySqlCommand(checkPinQuery, bankDbConnection);
                    object queryResult = command.ExecuteScalar();

                    int dbPin = Convert.ToInt32(queryResult);
                    if (debug)
                    {
                        Console.WriteLine($"\nPin found: {dbPin}");
                    }

                    if (dbPin == iPinIn)  // if the pin entered matches the one found on our system
                    {
                        if (debug)
                        {
                            Console.WriteLine("\nPin Match!");
                        }
                        Console.WriteLine("\nSuccessful Log-In. Welcome!");
                        isLoggedIn = true;
                        return isLoggedIn;
                    }
                    else
                    {
                        Console.WriteLine("\n** Incorrect Pin **\n");
                        Console.WriteLine("Application quitting for security reasons.");
                        return isLoggedIn;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error:\n{e.ToString()}");
                    return isLoggedIn;
                }

            }
            else
            {
                if (debug)
                {
                    Console.WriteLine("\nNo match.");
                }
                Console.WriteLine("\n**ID not found in our system.**\n");
                Console.WriteLine("If you are a registered user, please try again.");
                Console.WriteLine("\nOtherwise, please select choice 2 (Register) when given the option.");
                return isLoggedIn;
            }
        }

        public string ValidateUserInput(string input)
        {
            string validatedInput = "";

            return validatedInput;
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

    /// <summary>
    /// The User class: represents and stores info for a person using this ATM program
    /// </summary>
    /// <remarks>
    /// Users have a name, ID, and pin. All are necessary to have if wishing to use this ATM.
    /// If a person does not have an ID or pin, he or she must register with the system.
    /// </remarks>
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

    /// <summary>
    /// The main class: contains the main method where our ATM instance is created
    /// and menus are displayed for the user to navigate our services
    /// </summary>
    class MainClass
    {
        public static void Main(string[] args)
        {
            Atm newAtm = new Atm("CitiBank", "New York");
            if (newAtm.debug)
            {
                newAtm.PrintInfo();
            }

            newAtm.DbOpenConnection();
            int userSelection = newAtm.PresentMenu();

            switch (userSelection)
            {
                case 1:
                    bool userLoggedIn = newAtm.UserLogIn();
                    if (userLoggedIn)
                    {
                        int usrChoice = newAtm.PresentLoggedInMenu();

                        switch (usrChoice)
                        {
                            case 1:
                                break;

                            case 2:
                                break;

                            case 3:
                                break;

                            case 4:
                                break;

                            case 5:
                                break;

                            default:
                                Console.WriteLine("\nSelection unknown.");
                                break;
                        }
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                    break;

                case 2:
                    User newUser = newAtm.NewUserSignUp();
                    newAtm.RegisterNewUser(newUser);
                    break;

                case 3:
                    newAtm.DbCloseConnection();
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("\nSelection unknown.");
                    break;
            }
            newAtm.DbCloseConnection();  // close the db connection after break
        }
    }
}
