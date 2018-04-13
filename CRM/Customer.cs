using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM
{
    class Customer
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> PhoneNumbers { get; set; }

        public Customer()
        {
        }

        public Customer(int id, string firstName, string lastName, string email, List<string> phoneNumbers)
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumbers = phoneNumbers;
        }

        public override string ToString()
        {
            return $"ID: {ID} - {FirstName} {LastName}, {Email}";
        }
    }
}
