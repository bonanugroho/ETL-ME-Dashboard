using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoadDataWebService
{
    
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private DataTable getDataTable(string url, string tableName)
        {
            DataTable dt;

            WebClient web = new WebClient();
            string response = web.DownloadString(url);


            //DataSet ds = new DataSet();
            using (StringReader stringReader = new StringReader(response))
            {
                dsData = new DataSet();
                dsData.ReadXml(stringReader);
            }

            dt = dsData.Tables[tableName];
            return dt;
        }

        public static string CleanInvalidXmlChars(string text)
        {
            // From xml spec valid chars: 
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient web = new WebClient();
            string url = string.Format(textBox1.Text);
            string response = web.DownloadString(url);

            response = CleanInvalidXmlChars(response);

            dsData.Clear();
            //DataSet ds = new DataSet();
            using (StringReader stringReader = new StringReader(response))
            {
                dsData = new DataSet();
                dsData.ReadXml(stringReader);
            }


            cbTables.Items.Clear();
            foreach (DataTable dt in dsData.Tables)
            {

                cbTables.Items.Add(dt.TableName);
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            int jml = dsData.Tables.Count;

            //DataTable dt = dsData.Tables[cbTables.SelectedText];

            gvResponse.DataSource = dsData.Tables[cbTables.SelectedItem.ToString()].DefaultView; //getDataTable(textBox1.Text, cbTables.SelectedText).DefaultView;
            gvResponse.Refresh();

            //gvResponse.DataSource = dsData.Tables[cbTables.SelectedText];
            //gvResponse.Refresh();
            //gvResponse.();
        }

        private DateTime Number2Date(long lNum)
        {
            string strNum = lNum.ToString();
            int iDay, iMon, iYear;

            DateTime ValidDate = DateTime.Now;

            try
            {
                //If date is not in valid format 
                if (strNum.Length == 7)
                    strNum = "0" + strNum;

                iMon = Convert.ToInt32(strNum.Substring(0, 2));
                iDay = Convert.ToInt32(strNum.Substring(2, 2));
                iYear = Convert.ToInt32(strNum.Substring(4, 4));
                ValidDate = new DateTime(iYear, iMon, iDay);
            }

            catch (Exception Ex)
            {
                //Ex.Message
            }

            return ValidDate;
        }



        private void button3_Click(object sender, EventArgs e)
        {
            long number;
            long.TryParse(textBox2.Text, out number);
            DateTime resultDate = Number2Date(number);
            textBox3.Text = resultDate.ToString("yyyy-MMM-dd HH:mm");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // This is an example of a UNIX timestamp for the date/time 11-04-2005 09:25.
            //1422100800000
            //1113211532
            string str = textBox2.Text.Substring(0, 10);
            double timestamp = Convert.ToDouble(str);

            // First make a System.DateTime equivalent to the UNIX Epoch.
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

            // Add the number of seconds in UNIX timestamp to be converted.
            dateTime = dateTime.AddSeconds(timestamp);
            dateTime = dateTime.AddHours(+7);

            // The dateTime now contains the right date/time so to format the string,
            // use the standard formatting methods of the DateTime object.
            string printDate = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();

            // Print the date and time
            textBox3.Text = printDate;
            //System.Console.WriteLine(printDate);
        }
    }
}
