using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM
{
    class CRMDatabase
    {
        private readonly string connectionString = @"Server = (localdb)\mssqllocaldb; Database = CRM; Trusted_Connection = True";

        public CRMDatabase()
        {
        }

        public CRMDatabase(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int AddCustomer(string firstName, string lastName, string email)
        {
            var query = @"INSERT INTO customers (first_name, last_name, email_address) " +
                        "VALUES (@first_name, @last_name, @email_address) " +
                        "SELECT SCOPE_IDENTITY()";

            if (email == "")
                query = @"INSERT INTO customers (first_name, last_name) " +
                        "VALUES (@first_name, @last_name) " +
                        "SELECT SCOPE_IDENTITY()";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("first_name", firstName);
                command.Parameters.AddWithValue("last_name", lastName);

                if (email != "")
                    command.Parameters.AddWithValue("email_address", email);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int AddCustomer(Customer customer)
        {
            return AddCustomer(
                customer.FirstName,
                customer.LastName,
                customer.Email
                );
        }

        public void AddPhoneNumbers(int customerID, List<string> listOfPhoneNumbers)
        {
            var query = @"INSERT INTO phone_numbers (customer_id, phone_number) " +
                        "VALUES (@customer_id, @phone_number)";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("customer_id", typeof(int));
                command.Parameters.AddWithValue("phone_number", typeof(string));

                foreach (var phoneNumber in listOfPhoneNumbers)
                {
                    command.Parameters["customer_id"].Value = customerID;
                    command.Parameters["phone_number"].Value = phoneNumber;

                    if (command.ExecuteNonQuery() != 1)
                        throw new Exception("Command affected more than 1 row");
                }
            }
        }

        public void EditCustomer(int customerID, string firstName, string lastName, string email)
        {
            var query = @"UPDATE customers " +
                        "SET first_name = @first_name, last_name = @last_name, email_address = @email_address " +
                        "WHERE id = @id";

            var originalCustomer = GetCustomer(customerID);
            object emailAddress = email;

            if (firstName == "")
                firstName = originalCustomer.FirstName;

            if (lastName == "")
                lastName = originalCustomer.LastName;

            if (email == "")
                emailAddress = (object)originalCustomer.Email ?? DBNull.Value;

            else if (email.Trim().ToLower() == "null")
                emailAddress = DBNull.Value;

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("id", customerID);
                command.Parameters.AddWithValue("first_name", firstName);
                command.Parameters.AddWithValue("last_name", lastName);
                command.Parameters.AddWithValue("email_address", emailAddress);

                if (command.ExecuteNonQuery() != 1)
                    throw new Exception("Command affected more than 1 row");
            }
        }

        public void DeleteCustomer(int customerID)
        {
            var query = @"DELETE FROM customers WHERE id = @id";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("id", customerID);

                if (command.ExecuteNonQuery() != 1)
                    throw new Exception("Command affected more/less than 1 row");
            }
        }

        public Customer GetCustomer(int customerID)
        {
            var query = @"SELECT * FROM customers WHERE id = @id";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("id", customerID);

                var reader = command.ExecuteReader();
                var customer = new Customer();

                if (reader.Read())
                {
                    customer.ID = reader.GetInt32(0);
                    customer.FirstName = reader.GetString(1);
                    customer.LastName = reader.GetString(2);

                    if (!reader.IsDBNull(3))
                        customer.Email = reader.GetString(3);
                }
                
                reader.Close();
                return customer;
            }
        }

        public List<Customer> GetCustomerList()
        {
            var query = @"SELECT * FROM customers";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                var customerList = new List<Customer>();
                
                try
                {
                    while (reader.Read())
                    {
                        customerList.Add(new Customer()
                        {
                            ID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = !reader.IsDBNull(3) ? reader.GetString(3) : ""
                        });
                    }
                }
                finally
                {
                    reader.Close();
                }

                return customerList;
            }
        }

        public bool IsValidCustomerID(int customerID)
        {
            var query = @"SELECT id FROM customers WHERE id = @id";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("id", customerID);

                return command.ExecuteScalar() != null;
            }
        }
    }
}
