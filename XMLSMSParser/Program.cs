using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace XMLSMSParser
{
    class Program
    {
        static void Main(string[] args)
        {

            XmlDocument document = new XmlDocument();
            document.Load("SMS.xml");

            XmlNodeList SMSParent = document.GetElementsByTagName("smses");
            XmlNodeList SMSes = SMSParent[0].ChildNodes;

            List<SMSmessage> SMSMessages = new List<SMSmessage>();

            foreach(XmlNode node in SMSes){
                if (node.Attributes[0].Name == "protocol"){
                    //is valid SMS, just log message directly
                    string sender = node.Attributes["type"].Value;
                    string mess = node.Attributes["body"].Value;
                    string date = node.Attributes["date"].Value;
                    SMSMessages.Add(new SMSmessage(sender, mess, date));
                   
                } else if (node.Attributes[0].Name == "date"){
                    //is a MMS message, needs to be handled different
                    string sender = node.Attributes["msg_box"].Value;
                    string date = node.Attributes["date"].Value;
                    XmlNodeList MMSChildTemp = node.ChildNodes[0].ChildNodes;
                    foreach(XmlNode nodesmal in MMSChildTemp){
                        if (nodesmal.Attributes["chset"].Value != "null"){
                            string mess = nodesmal.Attributes["text"].Value;
                            SMSMessages.Add(new SMSmessage(sender, mess, date));
                        }
                    }
                }
            }

            SMSMessages.Sort();

            foreach(SMSmessage message in SMSMessages){
                Console.WriteLine(message.sender + " " + message.numDate + " " + message.message);
            }


            using (StreamWriter sw = File.AppendText("file.txt"))
            {
                foreach(SMSmessage message in SMSMessages){
                    sw.WriteLine(message.sender + " | " + message.message);
                }
            }	           
            Console.WriteLine("Writing complete, check program directory");
            Console.ReadKey();
        }
    }


    public class SMSmessage : IComparable<SMSmessage>{
        public string sender;
        public string message;
        public string date;
        public Int64 numDate;
        public SMSmessage(string psender, string pmessgae, string pdate){
            sender = psender;
            message = pmessgae;
            date = pdate;
            numDate = Int64.Parse(pdate);
        }
        public int CompareTo(SMSmessage other){
            return numDate.CompareTo(other.numDate);
        }
    }
}






