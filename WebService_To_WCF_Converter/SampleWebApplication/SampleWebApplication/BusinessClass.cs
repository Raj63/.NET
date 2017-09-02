using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebApplication
{
    public class BusinessClass
    {
        public BondObject RetriveBond(string Bond)
        {
            var bond  = new BondObject();
            {
                bond.BondName = "Bond1";
                bond.BondNumber = "1234";
                bond.BondTerm = "4";
                bond.BondEffectiveFrom = DateTime.Now;
                bond.AccountNumber = "111";
            }
            return bond;
        }

        public List<BondObject> GetBondsForAccount(string accountNumber)
        {
            var bondObjects = new List<BondObject>();
            var filteredBonds = new List<BondObject>();
            bondObjects =GetBonds();
            filteredBonds = bondObjects.FindAll(x => x.AccountNumber == accountNumber);
            //foreach (var item in bondObjects)
            //{
            //    if (item.AccountNumber==AccountNumber)
            //    {
            //        filteredBonds.Add(item);
            //    }
            //}
            return filteredBonds;


        }

        public bool IsValidBond(string bondNumber)
        {
            return bondNumber.All(char.IsDigit);

        }

        public void SaveBond(BondObject bond)
        {
           //save to the database

        }

        private List<BondObject> GetBonds()
        {
            var bondObjects = new List<BondObject>();

            var bondObject = new BondObject();

            bondObject.AccountNumber = "111";
            bondObject.BondEffectiveFrom = DateTime.MinValue;
            bondObject.BondLimit = "1000";
            bondObject.BondName = "Bond1";
            bondObject.BondNumber = "001";
            bondObject.BondTerm = "1";
            bondObjects.Add(bondObject);

             bondObject = new BondObject();
            bondObject.AccountNumber = "222";
            bondObject.BondEffectiveFrom = DateTime.MinValue;
            bondObject.BondLimit = "1000";
            bondObject.BondName = "Bond2";
            bondObject.BondNumber = "002";
            bondObject.BondTerm = "1";
            bondObjects.Add(bondObject);

            bondObject = new BondObject();
            bondObject.AccountNumber = "111";
            bondObject.BondEffectiveFrom = DateTime.MinValue;
            bondObject.BondLimit = "1000";
            bondObject.BondName = "Bond3";
            bondObject.BondNumber = "003";
            bondObject.BondTerm = "1";
            bondObjects.Add(bondObject);

            bondObject = new BondObject();
            bondObject.AccountNumber = "222";
            bondObject.BondEffectiveFrom = DateTime.MinValue;
            bondObject.BondLimit = "1000";
            bondObject.BondName = "Bond4";
            bondObject.BondNumber = "004";
            bondObject.BondTerm = "1";
            bondObjects.Add(bondObject);

            bondObject = new BondObject();
            bondObject.AccountNumber = "111";
            bondObject.BondEffectiveFrom = DateTime.MinValue;
            bondObject.BondLimit = "1000";
            bondObject.BondName = "Bond5";
            bondObject.BondNumber = "005";
            bondObject.BondTerm = "1";
            bondObjects.Add(bondObject);

            bondObject = new BondObject();
            bondObject.AccountNumber = "111";
            bondObject.BondEffectiveFrom = DateTime.MinValue;
            bondObject.BondLimit = "1000";
            bondObject.BondName = "Bond6";
            bondObject.BondNumber = "006";
            bondObject.BondTerm = "1";
            bondObjects.Add(bondObject);

            return bondObjects;

        }
    }
}