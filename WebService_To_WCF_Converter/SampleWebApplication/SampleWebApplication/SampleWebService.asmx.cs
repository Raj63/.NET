using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SampleWebApplication
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SampleWebService : WebService
    {

        [WebMethod(Description = "Adds two numbers")]
        public int Add(int num1 , int num2)
        {
            return num1 + num2;
        }

        [WebMethod(Description = "Multiplies two numbers")]
        public int Multiply(int num1, int num2)
        {
            return num1 * num2;
        }

        [WebMethod(Description = "Returns bond object")]
        public BondObject RetrieveBond(string bondNumber)
        {
            var business = new BusinessClass();
            return business.RetriveBond(bondNumber);
        }

        [WebMethod(Description = "Gets bonds for account")]
        public List<BondObject> GetBondsForAccount(string accountNumber)
        {
            var business = new BusinessClass();
            return business.GetBondsForAccount(accountNumber);
        }

        [WebMethod(Description = "Validates Bond")]
        public bool IsValidBond(List<string> bondNumber)
        {
            var business = new BusinessClass();
            return business.IsValidBond(bondNumber.FirstOrDefault());
        }

        [WebMethod(Description = "Saves Bond information")]
        public void SaveBond(ref BondObject bond)
        {
            var business = new BusinessClass();
            business.SaveBond(bond);
        }

        private int testRaj()
        {
            return 63;
        }

        public int testRajesh()
        {
            return 63;
        }
    }
}
