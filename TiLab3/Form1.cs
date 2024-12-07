using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Hamming
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public int ERROR_VAL = -1;
        public int EMERGENCY = -1;

        #region HAMMING_CODE
        public BitArray Code(string inMessage)
        {
            var messageArray = new BitArray(inMessage.Length, false);
            for (int i = 0; i < inMessage.Length; i++)
            {
                if (inMessage[i] == '1')
                    messageArray[i] = true;
                else
                    messageArray[i] = false;
            }
            int messageInd = 0;
            int retInd = 0;
            int controlIndex = 1;
            var retArray = new BitArray(messageArray.Length + 1 + (int)Math.Ceiling(Math.Log(messageArray.Length, 2)));
            while (messageInd < messageArray.Length)
            {
                if (retInd + 1 == controlIndex)
                {
                    retInd++;
                    controlIndex = controlIndex * 2;
                    continue;
                }
                retArray.Set(retInd, messageArray.Get(messageInd));
                messageInd++;
                retInd++;
            }
            retInd = 0;
            controlIndex = 1 << (int)Math.Log(retArray.Length, 2);
            while (controlIndex > 0)
            {
                int c = controlIndex - 1;
                int counter = 0;

                while (c < retArray.Length)
                {
                    for (int i = 0; i < controlIndex && c < retArray.Length; i++)
                    {
                        if (retArray.Get(c))
                            counter++;
                        c++;
                    }
                    c += controlIndex;
                }

                if (counter % 2 != 0) retArray.Set(controlIndex - 1, true);
                controlIndex = controlIndex / 2;
            }
            return retArray;
        }

        public BitArray Decode(string inMessage)
        {
            var codedArray = new BitArray(inMessage.Length, false);
            for (int i = 0; i < codedArray.Length; i++)
            {
                if (inMessage[i] == '1')
                    codedArray[i] = true;
                else
                    codedArray[i] = false;
            }
            var decodedArray = new BitArray((int)(codedArray.Count - Math.Ceiling(Math.Log(codedArray.Count, 2))), false);
            int count = 0;
            for (int i = 0; i < codedArray.Length; i++)
            {
                for (int j = 0; j < Math.Ceiling(Math.Log(codedArray.Count, 2)); j++)
                {
                    if (i == Math.Pow(2, j) - 1)
                        i++;
                }
                decodedArray[count] = codedArray[i];
                count++;
            }
            string strDecodedArray = "";
            for (int i = 0; i < decodedArray.Length; i++)
            {
                if (decodedArray[i])
                    strDecodedArray += "1";
                else
                    strDecodedArray += "0";
            }
            var checkArray = Code(strDecodedArray);
            byte[] failBits = new byte[checkArray.Length - decodedArray.Length];
            count = 0;
            bool isMistake = false;
            for (int i = 0; i < checkArray.Length - decodedArray.Length; i++)
            {
                try
                {
                    if (codedArray[(int)Math.Pow(2, i) - 1] != checkArray[(int)Math.Pow(2, i) - 1])
                    {
                        failBits[count] = (byte)(Math.Pow(2, i));
                        count++;
                        isMistake = true;
                    }
                }
                catch { EMERGENCY = 1; }
            }
            if (isMistake)
            {
                int mistakeIndex = 0;
                for (int i = 0; i < failBits.Length; i++)
                    mistakeIndex += failBits[i];
                mistakeIndex--;
                codedArray.Set(mistakeIndex, !codedArray[mistakeIndex]);
                ERROR_VAL = mistakeIndex;
                count = 0;
                for (int i = 0; i < codedArray.Length; i++)
                {
                    for (int j = 0; j < Math.Ceiling(Math.Log(codedArray.Count, 2)); j++)
                    {
                        if (i == Math.Pow(2, j) - 1)
                            i++;
                    }
                    decodedArray[count] = codedArray[i];
                    count++;
                }
            }
            return decodedArray;
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            BitArray code = Decode(textBox1.Text);
            if (ERROR_VAL == -1)
            {
                MessageBox.Show("В коде ошибки нет");
                return;
            }
            richTextBox1.AppendText("Ошибка в " + (ERROR_VAL + 1).ToString() + " бите\n" + "Вот исправленный вариант\n");
            char[] textArray = textBox1.Text.ToCharArray();

            if (textArray.Length > ERROR_VAL)
            {
                if (textArray[ERROR_VAL] == '0')
                    textArray[ERROR_VAL] = '1';
                else if (textArray[ERROR_VAL] == '1')
                    textArray[ERROR_VAL] = '0';
            }

            richTextBox1.AppendText(new string(textArray));

            if (EMERGENCY == 1)
            {
                EMERGENCY = -1;
                richTextBox1.Text = richTextBox1.Text.Substring(0, richTextBox1.Text.Length - 1);
            }
        }

        bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.AppendText("Вот ваша закодированная строка:\n");
            BitArray code = Code(textBox1.Text);
            for (int i = 0; i < code.Length; i++)
                if (IsPowerOfTwo(i + 1))
                {
                    richTextBox1.Select(richTextBox1.TextLength, 0);
                    richTextBox1.AppendText(code[i] ? "1" : "0");
                }
                else
                {
                    richTextBox1.Select(richTextBox1.TextLength, 0);
                    richTextBox1.AppendText(code[i] ? "1" : "0");
                }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}