using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XML2CSV
{
    public class Excuse
    {
        public int id;
        public string name;
        public string author;
        public string gender;
        public string date;
        public string country;
        public string text;
        public List<CommunicateivTactic> tactic;
        public string sources;
        public string additionalInfo;
    }
    public class CommunicateivTactic
    {
        public int index;
        public string text;
    }
}
