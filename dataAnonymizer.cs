using System;
using MySql.Data.MySqlClient;

// Jamil Credit Institution - Data Anonymization Techniques
// Ali Jamil 2023

// Project namespace
namespace codeObfuscation
{
    // Main class - all code in here
    class DataAnonymizer
    {
        // Entry point method
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Jamil Credit Institution \u00a9");
            Console.ResetColor();

            Console.WriteLine("Welcome to JCI's customer support system. What is your name?");
            string name = Console.ReadLine();

            Console.WriteLine("Do you have an existing account? (Yes/No)");

            // Reads from the database to check if the user exists
            string userChoice;
            do
            {
                userChoice = Console.ReadLine().ToLower();

                if (userChoice == "yes" || userChoice == "y")
                {
                    // Login with an existing account
                    Login();
                }
                else if (userChoice == "no" || userChoice == "n")
                {
                    // Create a new account and then log in
                    if (Register(name))
                    {
                        Login();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'Yes' or 'No'.");
                }

            } while (userChoice != "yes" && userChoice != "no");
        }

        static void Login()
        {
            Console.WriteLine("Please enter your email:");
            string email = Console.ReadLine();

            Console.WriteLine("Please enter your 4-digit pin:");
            string userPin = Console.ReadLine();

            if (ValidateLogin(email, userPin))
            {
                Console.WriteLine("Login successful! Welcome back.");
            }
            else
            {
                Console.WriteLine("Invalid login credentials. Please try again.");
            }
        }

        static bool Register(string name)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Welcome to JCI! Let's create a new account.");
            Console.ResetColor();

            Console.WriteLine("Please enter your email:");
            string email = Console.ReadLine();

            Console.WriteLine("Let's create a 4-digit pin:");
            string userPin = Console.ReadLine();

            string maskedPin = "";

            if (userPin.Length != 4)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Pin. Pin must be 4 digits.");
                Console.ResetColor();
                return false; // Account creation failed
            }
            else
            {
                maskedPin = new string('*', userPin.Length - 3) + userPin.Substring(userPin.Length - 3);

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(maskedPin);
                Console.ResetColor();

                Console.WriteLine("Database connection established.");

                // Connect to the MySQL database
                string connectionString = "server=localhost;database=dataAnonymizer;uid=root;password=positions;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a table if it doesn't exist
                    string createTableQuery = "CREATE TABLE IF NOT EXISTS Users (Id INT PRIMARY KEY AUTO_INCREMENT, Name VARCHAR(255), Email VARCHAR(255), Pin VARCHAR(255))";
                    using (MySqlCommand createTableCmd = new MySqlCommand(createTableQuery, connection))
                    {
                        createTableCmd.ExecuteNonQuery();
                    }

                    // Obfuscate data
                    string username = email.Substring(0, 3);
                    int maskedUsernameLength = email.IndexOf('@') - 3;
                    string maskedUsername = new string('*', maskedUsernameLength >= 0 ? maskedUsernameLength : 0);
                    string domain = email.Substring(email.IndexOf('@'));

                    // Insert exactly as entered data into the Users table
                    string insertDataQuery = "INSERT INTO Users (Name, Email, Pin) VALUES (@Name, @Email, @Pin)";
                    using (MySqlCommand insertDataCmd = new MySqlCommand(insertDataQuery, connection))
                    {
                        insertDataCmd.Parameters.AddWithValue("@Name", name);
                        insertDataCmd.Parameters.AddWithValue("@Email", email); // Store the email exactly as entered
                        insertDataCmd.Parameters.AddWithValue("@Pin", ObfuscatePin(userPin));
                        insertDataCmd.ExecuteNonQuery();
                    }

                    Console.WriteLine("Account created successfully! You can now log in.");
                    return true; // Account creation succeeded
                }
            }
        }

        static bool ValidateLogin(string email, string userPin)
        {
            string connectionString = "server=localhost;database=dataAnonymizer;uid=root;password=positions;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Users WHERE Email = @Email AND Pin = @Pin";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Pin", ObfuscatePin(userPin));

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        static string ObfuscatePin(string userPin)
        {
            // Obfuscate the pin as needed for display
            // You can customize this method based on your obfuscation requirements for display
            string obfuscatedPin = new string('*', userPin.Length - 1) + userPin[userPin.Length - 1];

            // Store the pin exactly as entered in the database
            return userPin;
        }




        static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char[] charArray = input.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);
            return new string(charArray);
        }
    }
}

