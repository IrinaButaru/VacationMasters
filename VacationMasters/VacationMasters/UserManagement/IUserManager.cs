using System;
using System.Collections.Generic;
using System.Globalization;
using VacationMasters.Essentials;

namespace VacationMasters.UserManagement
{
    public interface IUserManager
    {
        /// <summary>
        /// checks whether a given user satisfies the login conditions
        /// Conditions: existing and not banned
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        int CanLogin(string username, string password);

        /// <summary>
        /// checks whether a given user exists
        /// Conditions: existing and password equivalence
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool CheckIfUserExists(string userName);

        /// <summary>
        /// Checks whether the combination UserName/Password worked or not
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool CheckCredentials(string userName, string password);

        /// <summary>
        /// Checks if two password match.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <returns></returns>
        bool CheckIfPasswordMach(string password, string confirmPassword);

        /// <summary>
        /// Verifies a security question.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        bool CheckAnswer(string username, string answer);
        /// <summary>
        /// Change password of an user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="newPassword"></param>
        bool ChangePassword(string userName, string newPassword);

        /// <summary>
        /// Returns an user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        User GetUser(string userName);

        /// <summary>
        /// Add new user without preferences to the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="type"></param>
        void AddUser(User user, string password, string type = "User");

        /// <summary>
        /// Adds new user with preferences to the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="preferencesId"></param>
        /// <param name="type"></param>
        void AddUser(User user, string password, List<string> preferences, List<string> groups,  string type = "User");

        /// <summary>
        /// Remove user from database
        /// </summary>
        /// <param name="userName"></param>
        void RemoveUser(string userName);

        /// <summary>
        /// Bans a user, restrict access
        /// </summary>
        /// <param name="userName"></param>
        void BanUser(string userName);
        string GetMail(string userName);
        /// 
        string GetPassword(string userName);
        void UpdateUser(string user, bool newsletter, string email, string password, string passwordConfirm, List<string>preferences, List<string>groups);

        int GetNewsletter(string userName);
        /// <summary>
        /// Lifts ban for a user, give access back
        /// </summary>
        /// <param name="userName"></param>
        void UnbanUser(string userName);

        /// Gets all the emails from database
        /// </summary>
        List<String> GetAllEmails();

        List<String> GetStrings(string sql);

        /// Logs the user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        void Login(string username, string password);

        List<Preference> GetPreferencesByUser(string userName);
    }
}
