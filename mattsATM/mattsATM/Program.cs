﻿using System;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace mattsATM
{
    /// <summary>
    /// A enumeration of data types to be used in validation of usr input
    /// </summary>
    public enum DesiredDataTypes { stringType, intType, floatType };

    /// <summary>
    /// The Atm class: a simulation of an automated teller machine.
    /// </summary>
    /// <remarks>
    /// Uses a MySQL database to store log-in info, atm transactions, balances etc.
    /// and gives users with valid credentials access to normal banking functions.
    /// </remarks>
    class Atm
    {
        /// <summary>
        /// A boolean value to determine whether to print debug statements or not.
        /// </summary>
        public bool debug = false;

        /// <summary>
        /// The name of the bank. Ex. Chase.
        /// </summary>
        private string bankName;

        /// <summary>
        /// The location of the bank. Ex. 123 Main Street, Buffalo, NY, 14215.
        /// </summary>
        private string location;

        /// <summary>
        /// A MySqlConnection object. Used later to connect to a MySQL database.
        /// </summary>
        private MySqlConnection bankDbConnection;

        /// <summary>
        /// The Atm class constructor.
        /// </summary>
        /// <param name="inBankName"> The name of the bank we are creating. </param>
        /// <param name="inLocation"> The location of the bank we are creating. </param>
        public Atm(string inBankName, string inLocation)
        {
            this.bankName = inBankName;
            this.location = inLocation;
            this.ConnectionString = "Database=mattsAtmDb; Uid=atmQuery; Pwd=atmPwd;";
            this.bankDbConnection = null;
        }

        /// <summary>
        /// ConnectionString Property - Auto-implemented
        /// </summary>
        /// <value>
        /// A string containing the Database, Uid, and Pwd for the designated MySQL db.
        /// </value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Atm method to open a connection to our designated MySQL database.
        /// </summary>
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

        /// <summary>
        /// Atm method to close the connection to our designated MySQL database.
        /// </summary>
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

        /// <summary>
        /// Atm method to present the main menu and give the user choices on how to proceed.
        /// </summary>
        /// <returns>
        /// usrChoice: an integer 1-3 representing the next step in the flow of the program.
        /// </returns>
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
            string usrInput = Console.ReadLine();
            int usrChoice = ValidateUserInput(usrInput, DesiredDataTypes.intType);  // validate input
            return usrChoice;
        }

        /// <summary>
        /// Atm method to present logged-in users with a menu in which they can access their account
        /// </summary>
        /// <returns>
        /// usrChoice: an integer 1-5 representing the next step in the flow of the program.
        /// </returns>
        public int PresentLoggedInMenu()
        {
            Console.WriteLine("\nPlease select an option from the menu below:\n");
            Console.WriteLine("1. Withdraw");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Display Balance");
            Console.WriteLine("4. Display Transaction History");
            Console.WriteLine("5. Exit\n");
            Console.Write("> ");
            string usrInput = Console.ReadLine();
            int usrChoice = ValidateUserInput(usrInput, DesiredDataTypes.intType);
            return usrChoice;
        }

        /// <summary>
        /// Atm method to print the instance variables of the current ATM.
        /// </summary>
        public void PrintInfo()
        {
            Console.WriteLine("ATM INFO -------");
            Console.WriteLine($"Bank: {bankName}");
            Console.WriteLine($"Location: {location}");
        }

        /// <summary>
        /// Atm method to create an 11 digit random identifier for a user.
        /// </summary>
        /// <returns>
        /// randomPathName: an 11 digit randomized string of letters and numbers.
        /// </returns>
        public string IdGen()
        {
            // get random file name from Systen.Path
            // this method uses RNGCryptoServiceProvider to get cryptographic strength
            string randomPathName = Path.GetRandomFileName();
            randomPathName = randomPathName.Replace(".", "");
            return randomPathName;
        }

        /// <summary>
        /// Atm method to read in instance variables from the user and create a new User in the system.
        /// </summary>
        /// <returns>
        /// newUser: the User object created by collecting the necessary data and calling constructor.
        /// </returns>
        public User NewUserSignUp()
        {
            Console.WriteLine("\nPlease enter your First, Middle (if applicable), and Last name below.");
            Console.Write("> ");
            string usrInfoIn = Console.ReadLine();
            string userFullName = ValidateUserInput(usrInfoIn, DesiredDataTypes.stringType);  // validate name

            string userID = IdGen();
            Console.WriteLine($"\nYour auto-generated UserID is {userID}\n");


            Console.WriteLine("Please enter a new 4 digit pin below.");
            Console.WriteLine("\n** REMEMBER YOUR ID AND PIN FOR FUTURE USE **\n");
            Console.Write("> ");
            string userInput = Console.ReadLine();
            int userPin = ValidateUserInput(userInput, DesiredDataTypes.intType);  // validate the pin
            if (userPin.ToString().Length < 4)
            {
                Console.WriteLine("\nPin not accepted.\nApplication quitting for security reasons.");
                Environment.Exit(0);
            }

            User newUser = new User(userFullName, userID, userPin);
            return newUser;  // return the new User with validated info
        }

        /// <summary>
        /// Atm method to store our new user in the MySQL database.
        /// </summary>
        /// <param name="newUser"> The User created in NewUserSignUp() </param>
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
            catch (Exception e)
            {
                Console.WriteLine($"Exception:\n{e.ToString()}");
            }
        }

        /// <summary>
        /// Atm method to log-in an already registered user (user exists in our database).
        /// </summary>
        /// <returns>
        /// isLoggedIn: a boolean describing whether the log-in attempt was successful or not.
        /// </returns>
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

        /// <summary>
        /// Atm method to validate any input that the user enters through the keyboard.
        /// </summary>
        /// <param name="input"> The string input from the user </param>
        /// <param name="desiredDataType"> The desired data type of the input after validation </param>
        /// <returns> validatedInput - of dynamic type </returns>
        public dynamic ValidateUserInput(string input, DesiredDataTypes desiredDataType)
        {
            switch (desiredDataType)
            {
                case DesiredDataTypes.intType:  // case: int
                    int validatedInt;
                    if (!int.TryParse(input, out validatedInt))
                    {
                        Console.WriteLine("\n** Enter only an integer **");
                        Console.WriteLine("Application quitting. Please try again!");
                        Environment.Exit(0);
                    }
                    return validatedInt;

                case DesiredDataTypes.stringType:  // case: string
                    string validatedString;

                    string strippedInput = input.Trim();
                    string[] words = strippedInput.Split(' ');

                    foreach (var word in words)
                    {
                        foreach (var character in word)
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

                    validatedString = "";
                    return validatedString;

                case DesiredDataTypes.floatType:  // case: float
                    float validatedFloat = 0.0f;

                    return validatedFloat;

                default:
                    return "Error";
            }
        }
    }

    /// <summary>
    /// The User class: represents and stores info for a person using this ATM program.
    /// </summary>
    /// <remarks>
    /// Users have a name, ID, and pin. All are necessary to have if wishing to use this ATM.
    /// If a person does not have an ID or pin, he or she must sign-up and register.
    /// </remarks>
    class User
    {
        /// <summary>
        /// The User class constructor.
        /// </summary>
        /// <param name="inName"> Name of the User we are creating. </param>
        /// <param name="inID"> ID of the user we are creating. </param>
        /// <param name="inPin"> Pin of the user we are creating. </param>
        public User(string inName, string inID, int inPin)
        {
            this.Name = inName;
            this.ID = inID;
            this.Pin = inPin;
        }

        /// <summary>
        /// Name Property - Auto-implemented
        /// </summary>
        /// <value>
        /// A string representing the user's first, (middle), last name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// ID Property.
        /// </summary>
        /// <value>
        /// 11 digit sequence of randomized letters and numbers.
        /// </value>
        public string ID { get; set; }

        /// <summary>
        /// Pin Property.
        /// </summary>
        /// <value>
        /// A 4 digit integer used by the user for login.
        /// </value>
        public int Pin { get; set; }
    }

    /// <summary>
    /// The main class: contains the main method where our ATM instance is created.
    /// and menus are displayed for the user to navigate our services.
    /// </summary>
    class MainClass
    {
        /// <summary>
        /// Entry point to the program.
        /// </summary>
        /// <param name="args"> List of command line arguments. </param>
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
