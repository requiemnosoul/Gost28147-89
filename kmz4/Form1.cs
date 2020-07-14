using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kmz4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[] key = new string[8];
        string[] K = new string[8];
        char[,] sbox = { 
                {'4','A','9','2','D','8','0','E','6','B','1','C','7','F','5','3'},
                {'E','B','4','C','6','D','F','A','2','3','8','1','0','7','5','9'},
                {'5','8','1','D','A','3','4','2','E','F','C','7','6','0','9','B'},
                {'7','D','A','1','0','8','9','F','E','4','6','C','B','2','5','3'},
                {'6','C','7','1','5','F','D','8','4','A','9','E','0','3','B','2'},
                {'4','B','A','0','7','2','1','D','3','6','8','5','9','C','F','E'},
                {'D','B','4','1','3','F','5','9','0','A','E','7','6','8','2','C'},
                {'1','F','D','0','5','7','A','4','9','2','3','E','6','B','8','C'} };
        private void Button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = Crypt();
        }
        
        string Crypt()
        {
            string Result = "";
            string text = textBox1.Text;
            while(text.Length % 8 != 0 || text.Length<8)
            {
                text += "1";
            }
            //byte[] b = Encoding.UTF8.GetBytes(text);
            for (int i = 0; i < text.Length; i+=8)
            {
                string A = "" + text[i] + text[i + 1] + text[i + 2] + text[i + 3];
                string B = "" + text[i + 4] + text[i + 5] + text[i + 6] + text[i + 7];
                string binA = Bin(A);
                string binB = Bin(B);
                for (int j = 0; j < 24; j++)
                {
                    string f = F(binA, K[j % 8]);
                    string xor = Xor(binB,f);
                    binB = binA;
                    binA = xor;
                }
                for (int j = 7; j >=0; j--)
                {
                    string f = F(binA, K[j]);
                    string xor = Xor(binB, f);
                    binB = binA;
                    binA = xor;
                }
                Result += BinToDec(binB)+BinToDec(binA);
            }
            return (Result);
        }
        string Decrypt()
        {
            string Result = "";
            string text = textBox1.Text;
            //byte[] b = Encoding.UTF8.GetBytes(text);
            for (int i = 0; i < text.Length; i += 8)
            {
                string A = "" + text[i] + text[i + 1] + text[i + 2] + text[i + 3];
                string B = "" + text[i + 4] + text[i + 5] + text[i + 6] + text[i + 7];
                string binA = Bin(A);
                string binB = Bin(B);
                for (int j = 0; j < 8; j++)
                {
                    string f = F(binA, K[j]);
                    string xor = Xor(binB, f);
                    binB = binA;
                    binA = xor;
                }
                for (int j = 23; j >= 0; j--)
                {
                    string f = F(binA, K[j%8]);
                    string xor = Xor(binB, f);
                    binB = binA;
                    binA = xor;
                }
                Result += BinToDec(binB) + BinToDec(binA);
            }
            return (Result);
        }
        string BinToDec (string s1)
        {
            string bbb = "";
            char[] slovo = new char[4];
            string[] res = new string[4];
            int[] aaaa = new int[4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    res[i] += s1[i*8+j];
                }
                slovo[i] = (char)(Convert.ToInt32(res[i],2));
                bbb += slovo[i];
            }
            return (bbb);
        }
        string Xor(string s1, string s2)
        {
            string res = "";
            for (int i = 0; i < 32; i++)
            {
                res += (((Int32)s1[i] + (Int32)s2[i])%2).ToString();
            }
            return (res);
        }
        string Bin(string str)
        {
            string res = "";
            string s = "";
            for (int i = 0; i < str.Length; i++)
            {
                s += Convert.ToString((byte)str[i],2);
                while(s.Length % 8 != 0)
                    s = "0" + s;
                res += s;
                s = "";
            }
            return (res);
        }
        void KeyGen()
        {
            int z = 0;
            string key1 = "";
            string binkey = "";
            Random rnd = new Random();
            for(int i = 0; i<32; i++)
            {
                string k = "";
                for (int j = 0; j < 8; j++)
                {
                    k += Convert.ToString(rnd.Next(0, 2));
                }
                binkey += k;
                k = Convert.ToInt32(k, 2).ToString("X");
                if (k.Length < 2)
                    k = "0" + k;
                key1 += k;
                if ((i+1) % 4 == 0)
                {
                    key[z] = key1;
                    key1 = "";
                    textBox3.Text += key[z]+" ";
                    z++;
                }
            }
            z = 0;
            for (int i = 0; i < 256; i++)
            {
                K[z] += binkey[i];
                if ((i+1) % 32 == 0)
                    z++;
            }
        }
        string F(string Ai, string Xi)
        {
            string f = mod2v32(Ai,Xi);
            int z = 0;
            string t;
            string res = "";
            string[] time = new string[32];

            for (int i = 7; i>=0;i--)
            {
                t = "" + f[i * 4 + 0] + f[i * 4 + 1] + f[i * 4 + 2] + f[i * 4 + 3];
                t = Convert.ToString(Convert.ToInt32(sbox[z, Convert.ToInt32(t,2)].ToString(),16),2);
                while(t.Length < 4)
                    t = "0" + t;
                res += t;
                z++;
            }
            for (int i = 0; i < 32; i++) // Циклический сдвиг на 11 бит
            {
                time[i] = res[(i + 11)%32].ToString();
            }
            res = "";
            for (int i = 0; i < time.Length; i++)
            {
                res += time[i];
            }
            return (res);
        }
        string mod2v32(string a, string x)
        {
            int k = 0;
            int z = 0;
            string mod = "";
            for(int i = a.Length-1; i>=0; i--)
            {
                z = k + Convert.ToInt32(Convert.ToString(a[i])) + Convert.ToInt32(Convert.ToString(x[i]));
                switch(z)
                {
                    case 0:
                        k = 0;
                        z = 0;
                        break;
                    case 1:
                        k = 0;
                        z = 1;
                        break;
                    case 2:
                        k = 1;
                        z = 0;
                        break;
                    case 3:
                        k = 1;
                        z = 1;
                        break;
                }
                mod = z.ToString()+mod;
            }
            return (mod);
        }
        private void Button4_Click(object sender, EventArgs e)
        {
            textBox3.Clear();
            KeyGen();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox2.Text;
            textBox2.Clear();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = Decrypt();
        }
    }
}
