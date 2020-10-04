using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CipherAll
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)

        {  
            //int x = Convert.ToInt32(textBox1.Text);
            string message = textBox2.Text;
            
            if(comboBox1.Text == "Vegenere")
            {
                string x = textBox1.Text;
                textBox3.Text = VegenereCipher.encrypt(message, x);
            }

            if (comboBox1.Text == "Ceaser")
            {
                int x = Convert.ToInt32(textBox1.Text);
                textBox3.Text = ceasercipher.encrypt(message, x);
            }

            if (comboBox1.Text == "RailFence")
            {
                int x = Convert.ToInt32(textBox1.Text);
                textBox3.Text = RailfenceCipher.encrypt(message, x);
            }

            if (comboBox1.Text == "PlayFair")
            {
                string x = textBox1.Text;
                textBox3.Text = Playfair.encrypt(message, x, false);
            }


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

public class ceasercipher
{
    public static string encrypt(string text, int key)
    {
        string result = " ";

        for(int i = 0; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
            {
                result += Convert.ToChar(Convert.ToInt32(text[i] + key - 65) % 26 + 65);
            }
            else
            {

                result += Convert.ToChar(Convert.ToInt32(text[i] + key - 97) % 26 + 97);
            }
        }
        return result;
    }
}

public class VegenereCipher
{
    public static string encrypt(string text, string key)
    {
        string result = "";

         for (int i = 0; i < text.Length; i++)
         {
          result += Convert.ToChar(((Convert.ToInt32(text[i]) - 97)+(Convert.ToInt32(key[i])- 97))%26 +97) ;
         }

        return result;
    }
}



public class RailfenceCipher
{
    public static string encrypt(string text, int key)
    {
        string result = string.Empty;

        char[][] matrix = BuildCleanMatrix(key, text.Length);

        int rowIncrement = 1;
        for (int row = 0, col = 0; col < matrix[row].Length; col++)
        {
            if (
                row + rowIncrement == matrix.Length ||
                row + rowIncrement == -1
                )
            {
                rowIncrement *= -1;
            }

            matrix[row][col] = text[col];

            row += rowIncrement;
        }

        result = BuildStringFromMatrix(matrix);

        return result;
    }  
        public static char[][] BuildCleanMatrix(int rows, int cols)
        {
            char[][] result = new char[rows][];

            for (int row = 0; row < result.Length; row++)
            {
                result[row] = new char[cols];
            }

            return result;
        }

        public static string BuildStringFromMatrix(char[][] matrix)
        {
            string result = string.Empty;

            for (int row = 0; row < matrix.Length; row++)
            {
                for (int col = 0; col < matrix[row].Length; col++)
                {
                    if (matrix[row][col] != '\0')
                    {
                        result += matrix[row][col];
                    }
                }
            }

            return result;
        }

        public static char[][] Transpose(char[][] matrix)
        {
            char[][] result =
                BuildCleanMatrix(matrix[0].Length, matrix.Length);

            for (int row = 0; row < matrix.Length; row++)
            {
                for (int col = 0; col < matrix[row].Length; col++)
                {
                    result[col][row] = matrix[row][col];
                }
            }

            return result;
        }

    
}



public class Playfair
{
    public static string encrypt(string input, string key, bool encipher)
    {
        string retVal = string.Empty;
        char[,] keySquare = GenerateKeySquare(key);
        string tempInput = RemoveOtherChars(input);
        int e = encipher ? 1 : -1;

        if ((tempInput.Length % 2) != 0)
            tempInput += "X";

        for (int i = 0; i < tempInput.Length; i += 2)
        {
            int row1 = 0;
            int col1 = 0;
            int row2 = 0;
            int col2 = 0;

            GetPosition(ref keySquare, char.ToUpper(tempInput[i]), ref row1, ref col1);
            GetPosition(ref keySquare, char.ToUpper(tempInput[i + 1]), ref row2, ref col2);

            if (row1 == row2 && col1 == col2)
            {
                retVal += new string(SameRowColumn(ref keySquare, row1, col1, e));
            }
            else if (row1 == row2)
            {
                retVal += new string(SameRow(ref keySquare, row1, col1, col2, e));
            }
            else if (col1 == col2)
            {
                retVal += new string(SameColumn(ref keySquare, col1, row1, row2, e));
            }
            else
            {
                retVal += new string(DifferentRowColumn(ref keySquare, row1, col1, row2, col2));
            }
        }

        retVal = AdjustOutput(input, retVal);

        return retVal;
    }

    public static int Mod(int a, int b)
    {
        return (a % b + b) % b;
    }

    public static List<int> FindAllOccurrences(string str, char value)
    {
        List<int> indexes = new List<int>();

        int index = 0;
        while ((index = str.IndexOf(value, index)) != -1)
            indexes.Add(index++);

        return indexes;
    }

    public static string RemoveAllDuplicates(string str, List<int> indexes)
    {
        string retVal = str;

        for (int i = indexes.Count - 1; i >= 1; i--)
            retVal = retVal.Remove(indexes[i], 1);

        return retVal;
    }

    public static char[,] GenerateKeySquare(string key)
    {
        char[,] keySquare = new char[5, 5];
        string defaultKeySquare = "ABCDEFGHIKLMNOPQRSTUVWXYZ";
        string tempKey = string.IsNullOrEmpty(key) ? "CIPHER" : key.ToUpper();

        tempKey = tempKey.Replace("J", "");
        tempKey += defaultKeySquare;

        for (int i = 0; i < 25; ++i)
        {
            List<int> indexes = FindAllOccurrences(tempKey, defaultKeySquare[i]);
            tempKey = RemoveAllDuplicates(tempKey, indexes);
        }

        tempKey = tempKey.Substring(0, 25);

        for (int i = 0; i < 25; ++i)
            keySquare[(i / 5), (i % 5)] = tempKey[i];

        return keySquare;
    }

    public static void GetPosition(ref char[,] keySquare, char ch, ref int row, ref int col)
    {
        if (ch == 'J')
            GetPosition(ref keySquare, 'I', ref row, ref col);

        for (int i = 0; i < 5; ++i)
            for (int j = 0; j < 5; ++j)
                if (keySquare[i, j] == ch)
                {
                    row = i;
                    col = j;
                }
    }

    public static char[] SameRow(ref char[,] keySquare, int row, int col1, int col2, int encipher)
    {
        return new char[] { keySquare[row, Mod((col1 + encipher), 5)], keySquare[row, Mod((col2 + encipher), 5)] };
    }

    public static char[] SameColumn(ref char[,] keySquare, int col, int row1, int row2, int encipher)
    {
        return new char[] { keySquare[Mod((row1 + encipher), 5), col], keySquare[Mod((row2 + encipher), 5), col] };
    }

    public static char[] SameRowColumn(ref char[,] keySquare, int row, int col, int encipher)
    {
        return new char[] { keySquare[Mod((row + encipher), 5), Mod((col + encipher), 5)], keySquare[Mod((row + encipher), 5), Mod((col + encipher), 5)] };
    }

    public static char[] DifferentRowColumn(ref char[,] keySquare, int row1, int col1, int row2, int col2)
    {
        return new char[] { keySquare[row1, col2], keySquare[row2, col1] };
    }

    public static string RemoveOtherChars(string input)
    {
        string output = input;

        for (int i = 0; i < output.Length; ++i)
            if (!char.IsLetter(output[i]))
                output = output.Remove(i, 1);

        return output;
    }

    public static string AdjustOutput(string input, string output)
    {
        StringBuilder retVal = new StringBuilder(output);

        for (int i = 0; i < input.Length; ++i)
        {
            if (!char.IsLetter(input[i]))
                retVal = retVal.Insert(i, input[i].ToString());

            if (char.IsLower(input[i]))
                retVal[i] = char.ToLower(retVal[i]);
        }

        return retVal.ToString();
    }
}