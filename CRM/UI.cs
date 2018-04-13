using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CRM
{
    class UI
    {
        public static void Run()
        {
            var database = new CRMDatabase();
            var loop = true;

            while (loop)
            {
                Console.Clear();

                Console.WriteLine("[1] Add customer\n" +
                                  "[2] Edit customer\n" +
                                  "[3] Remove customer\n" +
                                  "[4] Show all customers\n" +
                                  "[5] Exit\n");
                Console.Write("Option: ");

                int.TryParse(Console.ReadLine(), out var choice);

                switch (choice)
                {
                    case 1:
                        AddCustomer(database);
                        break;
                    case 2:
                        EditCustomer(database);
                        break;
                    case 3:
                        RemoveCustomer(database);
                        break;
                    case 4:
                        ShowAllCustomers(database);
                        break;
                    case 5:
                        loop = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private static void ShowAllCustomers(CRMDatabase database)
        {
            Console.Clear();

            DisplayAllCustomers(database);

            PressAnyKeyToContinue();
        }

        private static void RemoveCustomer(CRMDatabase database)
        {
            Console.Clear();

            DisplayAllCustomers(database);

            Console.Write("\nEnter ID for customer to remove: ");
            int.TryParse(Console.ReadLine(), out var customerID);

            if (database.IsValidCustomerID(customerID))
            {
                var customer = database.GetCustomer(customerID);
                database.DeleteCustomer(customerID);

                WriteLineColor($"Removed customer {customer}", ConsoleColor.Red);
            }

            else if (customerID == 0)
                return;

            else
                WriteLineColor("There is no customer with that ID", ConsoleColor.Red);

            PressAnyKeyToContinue();
        }

        private static void EditCustomer(CRMDatabase database)
        {
            Console.Clear();

            DisplayAllCustomers(database);

            var customerID = GetCustomerToEditFromUser();

            if (database.IsValidCustomerID(customerID))
                EditCustomerFromID(database, customerID);

            else if (customerID == 0)
                return;

            else
                WriteLineColor("There is no customer with that ID", ConsoleColor.Red);

            PressAnyKeyToContinue();
        }

        private static int GetCustomerToEditFromUser()
        {
            Console.Write("\nEnter ID for customer to edit: ");
            int.TryParse(Console.ReadLine(), out var customerID);
            return customerID;
        }

        private static void EditCustomerFromID(CRMDatabase database, int customerID)
        {
            Console.Clear();
            var customer = database.GetCustomer(customerID);
            Console.WriteLine($"\nEdit customer: {customer}\n");

            var firstName = GetFirstNameFromUser();
            var lastName = GetLastNameFromUser();
            var email = GetEmailFromUser();

            database.EditCustomer(customerID, firstName, lastName, email);

            AddPhoneNumbers(database, customerID);
        }

        private static void AddCustomer(CRMDatabase database)
        {
            Console.Clear();

            var firstName = GetFirstNameFromUser();
            var lastName = GetLastNameFromUser();
            var email = GetEmailFromUser();
            var customerID = database.AddCustomer(firstName, lastName, email);
            AddPhoneNumbers(database, customerID);

            WriteLineColor($"\nAdded customer {firstName} {lastName} with ID: {customerID}", ConsoleColor.Green);

            PressAnyKeyToContinue();
        }

        private static string GetEmailFromUser()
        {
            Console.Write("Email address: ");
            return Console.ReadLine();
        }

        private static string GetLastNameFromUser()
        {
            Console.Write("Last name: ");
            return Console.ReadLine();
        }

        private static string GetFirstNameFromUser()
        {
            Console.Write("First name: ");
            return Console.ReadLine();
        }

        private static List<string> GetPhoneNumbersFromUser()
        {
            var listOfPhoneNumbers = new List<string>();

            while (true)
            {
                Console.Write("Enter phone number to add: ");
                var input = Console.ReadLine();

                if (input == "")
                    break;

                listOfPhoneNumbers.Add(input);
            }

            return (listOfPhoneNumbers.Count != 0) ? listOfPhoneNumbers : null;
        }

        private static void AddPhoneNumbers(CRMDatabase database)
        {
            DisplayAllCustomers(database);

            Console.Write("Enter ID for customer to add phone numbers to: ");
            int.TryParse(Console.ReadLine(), out var customerID);

            if (database.IsValidCustomerID(customerID))
                AddPhoneNumbers(database, customerID);

            else if (customerID == 0)
                return;

            else
                WriteLineColor("There is no customer with that ID", ConsoleColor.Red);

            PressAnyKeyToContinue();
        }

        private static void AddPhoneNumbers(CRMDatabase database, int customerID)
        {
            var listOfPhoneNumbers = GetPhoneNumbersFromUser();

            if (listOfPhoneNumbers == null)
                return;

            database.AddPhoneNumbers(customerID, listOfPhoneNumbers);
        }

        private static void DisplayAllCustomers(CRMDatabase database)
        {
            foreach (var customer in database.GetCustomerList())
                Console.WriteLine(customer);
        }

        private static void PressAnyKeyToContinue()
        {
            Console.Write("\nPress any key to continue . . . ");
            Console.ReadKey();
        }

        private static void WriteLineColor(string message, ConsoleColor foregoundcolor)
        {
            Console.ForegroundColor = foregoundcolor;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
