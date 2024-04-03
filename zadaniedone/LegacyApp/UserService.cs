using System;

namespace LegacyApp
{
    public class UserService
    {
        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        { if (IsNameNotEmpty(firstName) || IsSecondNameNotEmpty(lastName))
            {
                return false;
            }

            if (IsEmailCorect(email))
            {
                return false;
            }

            var now = DateTime.Now;
            int age = CalculateAge(dateOfBirth, now);
            if (WhetherItWasHisBirthdayYet(dateOfBirth, now)) age--;

            if (age < 21)
            {
                return false;
            }

            var clientRepository = new ClientRepository();
            var client = clientRepository.GetById(clientId);

            var user = new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName
            };

            if (IsClientVip(client))
            {
                user.HasCreditLimit = false;
            }
            else if (IsClientNormal(client))
            {
                using (var userCreditService = new UserCreditService())
                {
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    creditLimit = creditLimit * 2;
                    user.CreditLimit = creditLimit;
                }
            }
            else
            {
                user.HasCreditLimit = true;
                using (var userCreditService = new UserCreditService())
                {
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    user.CreditLimit = creditLimit;
                }
            }

            if (WhatIsUserLimitOnCredit(user))
            {
                return false;
            }

            UserDataAccess.AddUser(user);
            return true;
        }

        private static bool WhatIsUserLimitOnCredit(User user)
        {
            return user.HasCreditLimit && user.CreditLimit < 500;
        }

        private static bool WhetherItWasHisBirthdayYet(DateTime dateOfBirth, DateTime now)
        {
            return now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day);
        }

        private static bool IsClientNormal(Client client)
        {
            return client.Type == "ImportantClient";
        }

        private static bool IsClientVip(Client client)
        {
            return client.Type == "VeryImportantClient";
        }

        private static int CalculateAge(DateTime dateOfBirth, DateTime now)
        {
            return now.Year - dateOfBirth.Year;
        }

        private static bool IsEmailCorect(string email)
        {
            return !email.Contains("@") && !email.Contains(".");
        }

        private static bool IsSecondNameNotEmpty(string lastName)
        {
            return string.IsNullOrEmpty(lastName);
        }

        private static bool IsNameNotEmpty(string firstName)
        {
            return string.IsNullOrEmpty(firstName);
        }
    }
}
