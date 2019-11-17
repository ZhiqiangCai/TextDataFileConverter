using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using System.IO;

namespace CSV2Texts
{
    public partial class Form1 : Form
    {
        DataTable data;
        string selectedColumn = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void openCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.openFileDialog1.ShowDialog().Equals(DialogResult.OK)) return;
            data = new DataTable();
            this.listBox1.Items.Clear();
            using(CsvFileReader cr = new CsvFileReader(this.openFileDialog1.FileName))
            {
                CsvRow header = new CsvRow();
                if (!cr.ReadRow(header)) return;
                this.listBox1.Items.AddRange(header.ToArray());
                for (int i = 0; i < header.Count;i++ )
                {
                    string name = header[i].Trim();
                    if (name == "") name = "V" + i;
                    data.Columns.Add(name);
                }
                int count = 1;
                while(true)
                {
                    CsvRow row = new CsvRow();
                    if (!cr.ReadRow(row)) break;
                    DataRow r = data.NewRow();
                    for (int i = 0; i < row.Count; i++) r[i] = row[i];
                    data.Rows.Add(r);
                    if (count > 100) break;
                    count++;
                }

                    cr.Close();
            }
            this.dataGridView1.DataSource = data;
            this.button1.Enabled=true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex < 0) return;
            selectedColumn = this.listBox1.SelectedItem.ToString();
            this.label2.Text = "Text column selected: \r\n  " + selectedColumn;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (data == null)
            {
                MessageBox.Show("Open a csv file first."); return;
            }
            if (selectedColumn=="")
            {
                MessageBox.Show("Select a text column."); return;
            }
           // DirectoryInfo dir = new DirectoryInfo(this.openFileDialog1.FileName);
            if (!this.folderBrowserDialog1.ShowDialog().Equals(DialogResult.OK)) return;
            string dirname = this.folderBrowserDialog1.SelectedPath + "\\CSV2Texts";
            if(Directory.Exists(dirname))
            {
                MessageBox.Show("Converted texts found at: " + dirname); return;
            }
            int lineCount = 0;
            
            using (StreamReader sr = new StreamReader(this.openFileDialog1.FileName))
            {
                while (sr.Peek() > -1)
                {
                    sr.ReadLine();
                    lineCount++;
                }
                sr.Close();
            }
            int digits = 1;
            while (lineCount > 9)
            {
                digits++;
                lineCount /= 10;
            }
            DirectoryInfo saveDir = Directory.CreateDirectory(dirname);
            using(CsvFileReader cr = new CsvFileReader(this.openFileDialog1.FileName))
            {
                 CsvRow header = new CsvRow();
                if (!cr.ReadRow(header)) return;
                int headIndex=-1;
                for (int i = 0; i < header.Count;i++ )
                {
                    if(header[i]==selectedColumn)headIndex=i;
                }
                if(headIndex<0)
                {
                    MessageBox.Show("Selected text column not found."); cr.Close();return;
                }
                int count=1;
                this.button1.Enabled=false;
                while(true)
                {
                    CsvRow row = new CsvRow();
                    if (!cr.ReadRow(row)) break;
                    string text = row[headIndex];
                    string s = count.ToString();
                    while (s.Length < digits) s = "0" + s;
                    string filename = dirname+"\\"+s+".txt";
                    using(StreamWriter sw = new StreamWriter(filename))
                    {
                        sw.Write(text);
                        sw.Close();
                    }
                    count++;
                }
                MessageBox.Show((count-1)+"files converted. Saved in "+dirname);
                    cr.Close();
            }
             
            }
        }
    }

