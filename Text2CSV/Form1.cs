using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Utilities;

namespace Text2CSV
{
    public partial class Form1 : Form
    {
        string rootFolder;
        List<FileInfo> files;
        public Form1()
        {
            InitializeComponent();
            files = new List<FileInfo>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }
        private void OpenFolder()
        {
            if (this.folderBrowserDialog1.ShowDialog().Equals(DialogResult.OK))
            {
                rootFolder = this.folderBrowserDialog1.SelectedPath;
                files.Clear();
                GetFiles(rootFolder);
                this.listBox1.Items.Clear();
                for (int i = 0; i < Math.Min(files.Count, 1000);i++ )
                {
                    this.listBox1.Items.Add(files[i].Name);
                }
            }
        }
        private void GetFiles(string folderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileInfo[] fs = dir.GetFiles("*.txt");
            if (fs != null && fs.Length > 0) files.AddRange(fs);
            DirectoryInfo[] subdirs = dir.GetDirectories();
            if (subdirs != null && subdirs.Length > 0)
                foreach (DirectoryInfo d in subdirs)
                    GetFiles(d.FullName);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            using (CsvFileWriter cw = new CsvFileWriter(this.rootFolder + "\\result.csv"))
            {
                CsvRow header = new CsvRow();
                //header.Add("Debate");
                header.Add("TextID");
                header.Add("Text");
                cw.WriteRow(header);
             //   foreach (string folder in folders)
               // {
                  //  DirectoryInfo dir = new DirectoryInfo(this.rootFolder);
                  //  FileInfo[] files = dir.GetFiles("*.txt");
                    foreach (FileInfo f in files)
                    {
                        using (StreamReader sr = new StreamReader(f.FullName))
                        {
                            string text = sr.ReadToEnd().Trim().Replace("\r", " ").Replace("\n", " ").Trim();
                            CsvRow row = new CsvRow();
                            //row.Add(dir.Name);
                            row.Add(f.Name.Replace(".txt", ""));
                            row.Add(text);
                            cw.WriteRow(row);
                        }
                    }
                }
            //}
           // this.textBox1.Text = "done!";
            MessageBox.Show("Done! Saved to " + this.rootFolder + "\\result.csv");
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFolder();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex < 0) return;
            if (this.files == null || this.listBox1.SelectedIndex >= this.files.Count) return;
            this.richTextBox1.Clear();
            this.richTextBox1.LoadFile(this.files[this.listBox1.SelectedIndex].FullName, RichTextBoxStreamType.PlainText);
        }
    }
}
