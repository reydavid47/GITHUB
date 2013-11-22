using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClearCostWeb
{
    /// <summary>
    /// A person who is a dependent of an employee
    /// </summary>
    [Serializable]
    public class Dependent
    {
        public Dependent()
        {
        }
        #region Private Members

        private int cchID = -1;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private DateTime dateOfBirth = DateTime.Parse("1/1/1800");
        private int age = -1;
        private string email = string.Empty;
        private Boolean isAdult = false;
        private Boolean showAccessQuestions = false;
        private Boolean depToUserGranted = false;
        private Boolean userToDepGranted = false;
        private string relationshipText = string.Empty;

        #endregion

        #region Public Properties

        public int CCHID
        {
            get { return cchID; }
            set { cchID = value; }
        }
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        public DateTime DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
        }
        public string DOBDisplay
        {
            get
            {
                string dob = dateOfBirth.ToLongDateString();
                dob = dob.Substring(dob.IndexOf(",") + 1);
                return dob;
            }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public int Age
        {
            get { return age; }
            set { age = value; }
        }
        public Boolean IsAdult { get { return isAdult; } set { isAdult = Boolean.Parse(value.ToString()); } }
        public Boolean ShowAccessQuestions { get { return showAccessQuestions; } set { showAccessQuestions = Boolean.Parse(value.ToString()); } }
        public Boolean DepToUserGranted { get { return depToUserGranted; } set { depToUserGranted = Boolean.Parse(value.ToString()); } }
        public Boolean UserToDepGranted { get { return userToDepGranted; } set { userToDepGranted = Boolean.Parse(value.ToString()); } }
        public string RelationshipText { get { return relationshipText; } set { relationshipText = value; } }
        #endregion
    }
}