using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Kod
{
    public partial class Form1 : Form
    {
        private string textVariable;
        private bool check = false;

        private string str;
        double schet1 = 0;
        double schet2 = 0;
        string[] Res, Res2;
        int m = 0;
        double[] P1;

        public Form1()
        {
            InitializeComponent();
            dataGridView1.Columns.Add("Column3", "Шеннон-Фано");
            dataGridView1.Columns.Add("Column4", "Хаффман");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textVariable != textBox1.Text)
                check = false;

            if (check == false)
                str = textBox1.Text.ToLower();
            else
                str = textVariable.ToLower();


            double h_fan = 0;
            double l_hof = 0;
            double l_fan = 0;
            double d_fan = 0;
            double d_hof = 0;
            int i = 0;
            dataGridView1.Rows.Clear();
            Dictionary<char, int> chars = new Dictionary<char, int>();
            Dictionary<char, double> per = new Dictionary<char, double>();
            foreach (char c in str)
            {
                if (!chars.ContainsKey(c))
                {
                    chars.Add((char)c, 1);
                }
                else
                {
                    chars[c]++;
                }
            }
            foreach (var letter in chars)
            {
                per.Add(letter.Key, Convert.ToDouble(letter.Value) / Convert.ToDouble(str.Length));
            }
            per = per.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach (var letter in per)
            {
                dataGridView1.Rows.Add();
                dataGridView1[0, i].Value = letter.Key;
                dataGridView1[1, i].Value = Math.Round(letter.Value, 4);
                i++;
            }
            P1 = new double[per.Count];
            Array.Copy(per.Values.ToArray<double>(), P1, per.Count);
            Res = new string[per.Count];
            Fano(0, per.Count - 1);
            i = 0;
            foreach (var letter in per)
            {
                dataGridView1[2, i].Value = Res[i];
                l_fan += P1[i] * Res[i].Length;
                i++;
            }
            label4.Text = "l = " + Math.Round(l_fan, 2).ToString();
            HuffmanTree huffmanTree = new HuffmanTree();
            Dictionary<char, double> Frequencies = new Dictionary<char, double>();
            char chr = 'A';
            for (int j = 0; j < P1.Length; j++)
            {
                h_fan += f(P1[j]);
                Frequencies.Add(chr, P1[j]);
                chr++;
            }
            d_fan = (l_fan - h_fan) / l_fan;
            label3.Text = "D = " + Math.Round(d_fan, 2).ToString();
            label2.Text = "H = " + Math.Round(h_fan, 2).ToString();
            label5.Text = "H = " + Math.Round(h_fan, 2).ToString();
            huffmanTree.Build(Frequencies);
            List<string> whatever = huffmanTree.ReturnAlphabet();
            i = 0;
            foreach (var what in whatever.OrderBy(x => x.Length))
            {
                l_hof += P1[i] * what.Length;
                dataGridView1[3, i].Value = what;
                i++;
            }
            label7.Text = "l = " + Math.Round(l_hof, 2).ToString();
            d_hof = (l_hof - h_fan) / l_hof;
            label6.Text = "D = " + Math.Round(d_hof, 2).ToString();
        }

        private void AssignCodes(Node node, Dictionary<char, string> codes, string code)
        {
            if (node.Symbol != '\0')
            {
                codes.Add(node.Symbol, new string(code.Reverse().ToArray()).Replace('0', 'x').Replace('1', '0').Replace('x', '1'));
            }
            else
            {
                AssignCodes(node.Left, codes, code + "0");
                AssignCodes(node.Right, codes, code + "1");
            }
        }

        public double f(double x)
        {
            double y = Math.Log(x, 2);
            return -x * y;
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            check = true;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                string fileText = File.ReadAllText(filePath);
                textVariable = fileText;
                textBox1.Text = textVariable;
            }
        }

        private int Delenie_Posledovatelnosty(int L, int R)
        {
            double schet1 = P1[L];
            double schet2 = 0;

            for (int i = L + 1; i <= R; i++)
            {
                schet2 += P1[i];
            }

            int m = L;
            double minDifference = Math.Abs(schet1 - schet2);

            for (int i = L + 1; i <= R; i++)
            {
                schet1 += P1[i];
                schet2 -= P1[i];

                double currentDifference = Math.Abs(schet1 - schet2);

                if (currentDifference < minDifference)
                {
                    minDifference = currentDifference;
                    m = i;
                }
            }

            return m;
        }

        private void Fano(int L, int R)
        {
            if (L < R)
            {
                int n = Delenie_Posledovatelnosty(L, R);

                for (int i = L; i <= R; i++)
                {
                    if (i <= n)
                    {
                        Res[i] = Res[i] + "1";
                    }
                    else
                    {
                        Res[i] = Res[i] + "0";
                    }
                }

                Fano(L, n);
                Fano(n + 1, R);
            }
        }

        public class HuffmanNode
        {
            public int Frequency { get; set; }
            public char Character { get; set; }
            public HuffmanNode Left { get; set; }
            public HuffmanNode Right { get; set; }
        }

        public class HuffmanTree
        {
            private List<Node> nodes = new List<Node>();
            public Node Root { get; set; }
            public Dictionary<char, double> Frequencies = new Dictionary<char, double>();

            public void Build(Dictionary<char, double> organ_harvest)
            {
                Frequencies = organ_harvest;

                foreach (KeyValuePair<char, double> symbol in Frequencies)
                {
                    nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
                }

                while (nodes.Count > 1)
                {
                    List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();

                    if (orderedNodes.Count >= 2)
                    {
                        List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                        Node parent = new Node()
                        {
                            Symbol = '*',
                            Frequency = taken[0].Frequency + taken[1].Frequency,
                            Left = taken[1],
                            Right = taken[0]
                        };

                        nodes.Remove(taken[0]);
                        nodes.Remove(taken[1]);
                        nodes.Add(parent);
                    }

                    this.Root = nodes.FirstOrDefault();
                }
            }

            public List<string> ReturnAlphabet()
            {
                List<string> tmp = new List<string>();

                foreach (KeyValuePair<char, double> symbol in Frequencies)
                {
                    List<bool> encodedSymbol = this.Root.Traverse(symbol.Key, new List<bool>());
                    tmp.Add(new string(encodedSymbol.Select(x => x ? '0' : '1').ToArray()));
                }

                return tmp;
            }
        }

        public class Node
        {
            public char Symbol { get; set; }
            public double Frequency { get; set; }
            public Node Right { get; set; }
            public Node Left { get; set; }

            public List<bool> Traverse(char symbol, List<bool> data)
            {
                // Leaf
                if (Right == null && Left == null)
                {
                    if (symbol.Equals(this.Symbol))
                    {
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    List<bool> left = null;
                    List<bool> right = null;

                    if (Left != null)
                    {
                        List<bool> leftPath = new List<bool>();
                        leftPath.AddRange(data);
                        leftPath.Add(false);

                        left = Left.Traverse(symbol, leftPath);
                    }

                    if (Right != null)
                    {
                        List<bool> rightPath = new List<bool>();
                        rightPath.AddRange(data);
                        rightPath.Add(true);
                        right = Right.Traverse(symbol, rightPath);
                    }

                    if (left != null)
                    {
                        return left;
                    }
                    else
                    {
                        return right;
                    }
                }
            }
        }
    }
}
