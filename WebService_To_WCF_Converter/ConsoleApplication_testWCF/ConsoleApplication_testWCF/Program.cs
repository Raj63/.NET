using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataContract;


namespace ConsoleApplication_testWCF
{
    class Program
    {
        static void Main(string[] args)
        {
            var name = "kumar";


            //Console.WriteLine(string.Format("hello {0} . you are stupid {2}", name, "gal", "boy"));

            //ServiceReference1.Service1Client t = new ServiceReference1.Service1Client();

            //CompositeType input = new CompositeType { BoolValue = true, StringValue = "Prefix" };

            ////ServiceReference1.CompositeType input = new ServiceReference1.CompositeType { BoolValue = true, StringValue = "Prefix" };
            //var c = t.GetDataUsingDataContract(input);

            //ServiceReference_webservice.SampleWebServiceSoapClient webClient = new ServiceReference_webservice.SampleWebServiceSoapClient();
            //ServiceReference_webservice.BondObject bondWeb = webClient.RetrieveBond("123456");


            //ServiceReference_wcfServiceHost.SampleWebServiceClient newClient = new ServiceReference_wcfServiceHost.SampleWebServiceClient();

            //ServiceReference_wcfServiceHost.BondObject bond = null;
            //bond = newClient.RetrieveBond("123456");

            //Console.WriteLine("wcf multiply 2,3 = " + newClient.Multiply(2, 3));
            //Console.WriteLine("web multiply 2,3 = " + webClient.Multiply(2, 3));
            //Console.WriteLine("Comparing the bond object : \n Web service accntNum = "+ bondWeb.AccountNumber + "\n wcf service accntNum = "+bond.AccountNumber );


            //Console.WriteLine(c.StringValue + " - " + c.BoolValue);
            test();

            Console.ReadKey();
        }

        public static void test()
        {

            var line1 = "1";

            System.Console.WriteLine(line1);



            var N = Int32.Parse(line1);
            for (var i = 0; i < N; i++)
            {
                var line2 = "4 2";
                System.Console.WriteLine(line2);

                var line3 = "4 2 3 5";
                System.Console.WriteLine(line3);

                var condition = line2.Split(' ');
                //var data = line3.Split(' ').Select(Int32.Parse).ToList();

                for (int k=0; k < Int32.Parse(condition[1]); k++)
                {
                    int first = 0;
                    int last = line3.Length - 1;


                    if (line3[first] > line3[last])
                    {
                        line3 = line3.Remove(last, 1);
                    }
                    else
                    {
                        line3 = line3.Remove(first, 1);
                    }

                    line3 = line3.Trim();
                }

                int total = 0;
                if (line3 != null && line3.Length > 0)
                {

                    if (line3.Length > 1)
                    {
                        total = Int32.Parse(line3[0]+"") * Int32.Parse(line3[line3.Length - 1]+"");
                    }
                    else
                    {
                        total = Int32.Parse(line3[0]+"");
                    }
                }


                System.Console.WriteLine(total);

            }
        }
    }
}
