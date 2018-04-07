using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections;

namespace WindowsFormsApplication5
{
    public partial class Form1 : Form
    {
        public struct Card
        {
            public string name;
            public double price;
            public string set;
            public bool Foil;
            public bool ReverseHolo;
        }
        public struct Set
        {
            public string name;
            public int setNum;
            public string URL;
            public void URLCreation()
            {
                string test;

                if (name.Contains('-')) { test = name.Substring(0, 5).Replace(" ", "") + name.Substring(6).Replace(" ", "-"); }
                else { test = name.Replace(" ", "-"); }
                this.URL = test.Replace(":", "");
            }
        }
        public List<Set> lSets;
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int TTorTCG;
            string data = "";
            string SetNameTemp, SetNameHolder;

            string[] ttarray = new string[] { "https://www.trollandtoad.com/Pokemon/7061.html", "inline smallFont subCats", "<a onclick='openSets()' class='seeAllCats'>" };
            string[] tcgArray = new string[] { "https://shop.tcgplayer.com/pokemon", "<select id=\"SetName\" name=\"SetName\">", "<div class=\"SearchParameter Floating\">" };
            string[][] BothWebsites = { ttarray, tcgArray };
            if (Troll_Toad.Checked) { TTorTCG = 0; } else { TTorTCG = 1; }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BothWebsites[TTorTCG][0]);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                lSets = new List<Set>();
                Set CurrentSet = new Set();
                int i = 1;
                if (response.CharacterSet == null) { readStream = new StreamReader(receiveStream); }
                else { readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet)); }
                while ((data = readStream.ReadLine()).Contains(BothWebsites[TTorTCG][1]) == false) { }
                while ((data = readStream.ReadLine()).Contains(BothWebsites[TTorTCG][2]) == false)
                {
                    try
                    {
                        if (Troll_Toad.Checked)
                        {
                            SetNameHolder = data.Trim().Substring(4);
                            if (SetNameHolder.Contains("inline smallFont subCats")) { SetNameHolder = SetNameHolder.Substring(48); }
                            if (SetNameHolder.Contains("subCatAlphaHeader")) { SetNameHolder = SetNameHolder.Substring(36); }
                            SetNameTemp = "";
                            SetNameTemp = SetNameHolder.Split('>')[1].Trim(); CurrentSet.name = SetNameTemp.Substring(0, SetNameTemp.Length - 3);
                            CurrentSet.URL = data.Split('\'')[1];
                        }
                        else { CurrentSet.name = data.Replace("\"", "#").Replace("&amp;", "&").Split('#')[1]; CurrentSet.URLCreation(); }

                        CurrentSet.setNum = i++;
                        lSets.Add(CurrentSet);
                    }
                    catch { }
                }

                response.Close();
                readStream.Close();

                listBox1.BeginUpdate();
                for (int j = i - 2; j > 0; j--)
                {
                    listBox1.Items.Add(lSets[j].name);
                }
                listBox1.EndUpdate();
                MessageBox.Show(lSets[i-8].URL);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int TTorTCG;
            string data = "";
            string SetNameTemp, SetNameHolder, URL;
            int x = 0;
            Set TempSet;

            string[] ttarray = new string[] { "https://www.trollandtoad.com", "items. Showing items ", " < div class=\"catResultPages\">" };
            string[] tcgArray = new string[] { "https://shop.tcgplayer.com/pokemon", "<select id=\"SetName\" name=\"SetName\">", "<div class=\"SearchParameter Floating\">" };
            string[][] BothWebsites = { ttarray, tcgArray };
            if (Troll_Toad.Checked) { TTorTCG = 0; } else { TTorTCG = 1; }
            SetNameHolder = listBox1.SelectedItem.ToString();
            var es = new SetSearch(SetNameHolder);
            List<Card> lCard;
            //lSets[lSets.FindIndex(es.Matching)].URL
            
            for (int i = 1; i < 20; i++) //Webpage
            {
                URL = BothWebsites[TTorTCG][0] + lSets[lSets.FindIndex(es.Matching)].URL + "?sois=No&pageLimiter=100&showImage=No&PageNum=" + i;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    #region //Stupid setting up stream stuff
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null) { readStream = new StreamReader(receiveStream); }
                    else { readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet)); }
                    #endregion 
                    string failState = "Sorry... Your search has come back with no results. Please try a different combination of search criteria.";
                    data = readStream.ReadLine();
                    while (!data.Contains(failState)  )
                    while ((data = readStream.ReadLine()).Contains(BothWebsites[TTorTCG][1]) == false) { }
                    //+ x + 1 + " - " + x + 100
                    while ((data = readStream.ReadLine()).Contains(BothWebsites[TTorTCG][2]) == false)
                    {
                        try { listBox1.Items.Add(data.Replace("\"", "&").Split('&')[1]); }
                        catch { }
                    }

                    response.Close();
                    readStream.Close();
                }
                else
                {
                    MessageBox.Show("dis shit is fucked up");
                }
            }
            
        }



        public class SetSearch
        {
            String _s;
            public SetSearch(String s) { _s = s; }
            public bool Matching(Set e) { return e.name.StartsWith(_s, StringComparison.InvariantCultureIgnoreCase); }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
