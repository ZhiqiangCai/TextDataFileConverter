using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace XML2CSV
{
    public partial class Form1 : Form
    {
        XmlDocument doc;
        Dictionary<int, string> tacticIndex;
        DataTable table;
        public Form1()
        {
            InitializeComponent();
            LoadXML();
            tacticIndex = getTacticNames();
            initDataTable();
            getExcuses();
            Save();
        }
        void LoadXML()
        {
            FileInfo file = new FileInfo("..\\data\\excuses.xml");
            this.webBrowser1.Url =new Uri(file.FullName);
            doc = new XmlDocument();
            doc.Load("..\\data\\excuses.xml");
        }
        Dictionary<int, string> getTacticNames()
        {
            Dictionary<int, string> ret = new Dictionary<int, string>();
            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes =root.SelectNodes("//tactic");
            foreach(XmlNode n in nodes)
            {
                int index = int.Parse(n.Attributes["index"].Value);
                string name = n.Attributes["name"].Value;
                ret.Add(index, name);
            }
            return ret;
        }
       
        void getExcuses()
        {
            
            XmlNode node = doc.SelectSingleNode("//excuses");
            int rid = 0;
            for(int i=0;i<node.ChildNodes.Count;i++)
            {
                XmlNode excuseNode = node.ChildNodes[i];
                Excuse excuse = new Excuse();
                foreach(XmlNode n in excuseNode.ChildNodes)
                {
                     
                    switch(n.Name)
                    {
                        case "id": excuse.id = int.Parse(n.InnerText); break;
                        case "name": excuse.name = n.InnerText; break;
                        case "author": excuse.author = n.InnerText; break;
                        case "gender": excuse.gender = n.InnerText; break;
                        case "date": excuse.date = n.InnerText; break;
                        case "country": excuse.country = n.InnerText; break;
                        case "text": excuse.text = n.InnerText.Trim().Replace("\n", " # ").Replace("\r", " # "); break;
                        case "additionalInformation": excuse.additionalInfo = n.InnerText.Trim().Replace("\n", " # ").Replace("\r", " # "); break;
                        case "communicativeTactics":
                            excuse.tactic = new List<CommunicateivTactic>();
                            foreach (XmlNode nn in n.ChildNodes)
                            { 
                                CommunicateivTactic c = new CommunicateivTactic();
                                c.index = int.Parse(nn.Attributes["tacticIndex"].Value);
                                c.text = nn.InnerText.Trim().Replace("\n", " # ").Replace("\r", " # ");
                                excuse.tactic.Add(c);
                            }
                            break;
                        case "sources":
                            foreach (XmlNode nn in n.ChildNodes)
                            {
                                excuse.sources += nn.InnerText.Trim() + ";";
                            }
                            break;
                        default: break;
                    }
                }
                    for(int j=0;j<excuse.tactic.Count;j++)
                    {
                        DataRow row = table.NewRow();
                        row["rid"] = rid + "";
                        row["tid"] = excuse.id + "";
                        row["name"] = excuse.name;
                        row["author"] = excuse.author;
                        row["gender"] = excuse.gender;
                        row["date"] = excuse.date;
                        row["country"] = excuse.country;
                        row["text"] = excuse.text;
                        row["tacind"] = excuse.tactic[j].index + "";
                        row["tacname"] = tacticIndex[excuse.tactic[j].index];
                        row["tactext"] = excuse.tactic[j].text;
                        row["sources"] = excuse.sources;
                        row["additionalinfo"] = excuse.additionalInfo;
                        table.Rows.Add(row);
                        rid++;
                    }
                
            }
            dataGridView1.DataSource = table;
        }
        void initDataTable()
        {
            table = new DataTable();
            table.Columns.Add("rid");
            table.Columns.Add("tid");
            table.Columns.Add("name");
            table.Columns.Add("author");
            table.Columns.Add("gender");
            table.Columns.Add("date");
            table.Columns.Add("country");
            table.Columns.Add("text");
            table.Columns.Add("tacind");
            table.Columns.Add("tacname");
            table.Columns.Add("tactext");
            table.Columns.Add("sources");
            table.Columns.Add("additionalinfo");
        }
        void Save()
        {
            using (Utilities.CsvFileWriter cw = new Utilities.CsvFileWriter("..\\data\\excuse.csv"))
            {
                Utilities.CsvRow header = new Utilities.CsvRow();
                for (int i = 0; i < table.Columns.Count; i++) header.Add(table.Columns[i].ColumnName);
                cw.WriteRow(header);
                foreach(DataRow row in table.Rows)
                {
                    Utilities.CsvRow cr = new Utilities.CsvRow();
                    for(int i=0;i<table.Columns.Count;i++)
                    {
                        cr.Add(row[i].ToString());
                    }
                    cw.WriteRow(cr);
                }
                    cw.Close();
            }
        }
    }
}
