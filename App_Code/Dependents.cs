using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace ClearCostWeb
{
    /// <summary>
    /// A collection of the Dependents of an employee
    /// </summary>
    [Serializable]
    public class Dependents : CollectionBase
    {
        public Dependents()
        {
        }

        #region Public Properties

        public Dependent this[int index]
        {
            get { return (Dependent)List[index]; }
        }
        public Boolean ShowAccessQuestionSave
        {
            get
            {
                Boolean retBool = false;
                foreach (Dependent dep in List)
                {
                    retBool = dep.ShowAccessQuestions;
                    if (retBool) { break; }
                }
                return retBool;
            }
        }
        #endregion

        #region Public Methods

        public void Add(Dependent dependent)
        {
            List.Add(dependent);
        }

        public Dependents GetAdultDependents()
        {
            Dependents adults = new Dependents();

            foreach (Dependent dep in List)
            {
                if (dep.Age >= 18)
                {
                    adults.Add(dep);
                }
            }
            return adults;
        }

        public DataTable AsDataTable()
        {
            DataTable dependentData = new DataTable("DependentData");

            dependentData.Columns.Add("FullName");
            dependentData.Columns.Add("FirstName");
            dependentData.Columns.Add("LastName");
            dependentData.Columns.Add("RelationShip");
            dependentData.Columns.Add("ShowQuestions");
            dependentData.Columns.Add("DepToUser");
            dependentData.Columns.Add("UserToDep");
            dependentData.Columns.Add("Adult");
            dependentData.Columns.Add("CCHID");

            DataRow dependentRow = null;
            foreach (Dependent dependent in List)
            {
                dependentRow = dependentData.NewRow();
                dependentRow["FullName"] = String.Format("{0} {1}", dependent.FirstName, dependent.LastName);
                dependentRow["FirstName"] = dependent.FirstName;
                dependentRow["LastName"] = dependent.LastName;
                dependentRow["RelationShip"] = dependent.RelationshipText;
                dependentRow["ShowQuestions"] = dependent.ShowAccessQuestions;
                dependentRow["DepToUser"] = dependent.DepToUserGranted;
                dependentRow["UserToDep"] = dependent.UserToDepGranted;
                dependentRow["Adult"] = dependent.IsAdult;
                dependentRow["CCHID"] = dependent.CCHID;
                dependentData.Rows.Add(dependentRow);
            }

            return dependentData;
        }

        #endregion

    }
}