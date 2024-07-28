using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Threading;
using System.Text.RegularExpressions;

namespace Auto_SSH_Upload
{
    public partial class Form1 : Form
    {
        public string directory_path = string.Empty;
        public string[] files = new string[] { };
        public SftpClient client;
        public string ip;
        public int port;
        public static int TogMove;
        public static int MValX;
        public static int MValY;

        private readonly string[] pythonKeywords =
    {
        "def", "return", "if", "else", "elif", "while", "for", "break", "continue",
        "import", "from", "as", "try", "except", "finally", "class", "pass", "with", "yield"
    };

        private const string CommentPattern = @"#.*";
        private const string StringPattern = @"""[^""\r\n]*""|'[^'\r\n]*'";
        private const string KeywordPattern = @"\b(?:def|return|if|else|elif|while|for|break|continue|import|from|as|try|except|finally|class|pass|with|yield)\b";
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show();
        }

        private void label5_Click_1(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            directory_path = folderBrowserDialog1.SelectedPath;
            label4.Text = $"Current Directory: {directory_path}";

            string[] files = Directory.GetFiles(directory_path);
            if (files.Length == 0)
                return;

            listBox1.Items.Clear();
            foreach (string file in files)
                listBox1.Items.Add(file.Replace(directory_path, "").Replace("\\", ""));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string file_name = listBox1.SelectedItem.ToString();
            richTextBox1.Text = File.ReadAllText(directory_path + "\\" + file_name);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SSHLogin login = new SSHLogin();
            login.ShowDialog();

            this.client = login.c;
            this.client.Connect();
            this.ip = login.ip;
            this.port = login.port;

            label6.Text = $"SSH Status: {this.ip}";

        }

        private void richTextBox1_DoubleClick(object sender, EventArgs e)
        {
            if (textBox1.Text == "Server_Path_To_File" || textBox1.Text.Trim() == "")
                return; 

            string file_name = listBox1.SelectedItem.ToString();

            File.WriteAllText($"{this.directory_path}\\{file_name}", richTextBox1.Text);
            Thread.Sleep(1000);
            
            if(textBox1.Text.EndsWith("/"))
                this.client.UploadFile(File.OpenRead($"{this.directory_path}\\{file_name}"), textBox1.Text + file_name);
            else
                this.client.UploadFile(File.OpenRead($"{this.directory_path}\\{file_name}"), textBox1.Text + "/" + file_name);

            MessageBox.Show("File uploaded to server!");
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            TogMove = 1;
            MValX = e.X;
            MValY = e.Y;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (TogMove == 1)
            {
                SetDesktopLocation(Control.MousePosition.X - MValX, Control.MousePosition.Y - MValY);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            TogMove = 0;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void C_HighlightSyntax()
        {
            // Define keyword, comment, and string patterns
            string[] keywords = { "int", "float", "if", "else", "for", "while", "return" };
            string commentPattern = @"//.*";
            string stringPattern = @"""[^""\r\n]*""";

            // Clear previous formatting
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.FromArgb(224, 224, 224);

            // Highlight comments
            C_HighlightPattern(commentPattern, Color.FromArgb(108, 148, 168));

            // Highlight strings
            C_HighlightPattern(stringPattern, Color.Red);

            // Highlight keywords
            foreach (string keyword in keywords)
            {
                C_HighlightKeyword(keyword, Color.FromArgb(25, 88, 120));
            }
        }

        private void C_HighlightPattern(string pattern, Color color)
        {
            Regex regex = new Regex(pattern);
            foreach (Match match in regex.Matches(richTextBox1.Text))
            {
                richTextBox1.Select(match.Index, match.Length);
                richTextBox1.SelectionColor = color;
            }
        }

        private void C_HighlightKeyword(string keyword, Color color)
        {
            int startIndex = 0;
            while (startIndex < richTextBox1.TextLength)
            {
                int index = richTextBox1.Text.IndexOf(keyword, startIndex);
                if (index == -1) break;
                richTextBox1.Select(index, keyword.Length);
                richTextBox1.SelectionColor = color;
                startIndex = index + keyword.Length;
            }
        }

        private void HighlightSyntax()
        {
            // Clear previous formatting
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;

            // Highlight comments
            HighlightPattern(CommentPattern, Color.Green);

            // Highlight strings
            HighlightPattern(StringPattern, Color.Red);

            // Highlight keywords
            foreach (string keyword in pythonKeywords)
            {
                HighlightKeyword(keyword, Color.Blue);
            }
        }

        private void HighlightPattern(string pattern, Color color)
        {
            Regex regex = new Regex(pattern);
            foreach (Match match in regex.Matches(richTextBox1.Text))
            {
                richTextBox1.Select(match.Index, match.Length);
                richTextBox1.SelectionColor = color;
            }
        }

        private void HighlightKeyword(string keyword, Color color)
        {
            int startIndex = 0;
            while (startIndex < richTextBox1.TextLength)
            {
                int index = richTextBox1.Text.IndexOf(keyword, startIndex);
                if (index == -1) break;
                // Check if the keyword is a whole word
                if (IsWholeWord(index, keyword.Length))
                {
                    richTextBox1.Select(index, keyword.Length);
                    richTextBox1.SelectionColor = color;
                }
                startIndex = index + keyword.Length;
            }
        }

        private bool IsWholeWord(int startIndex, int length)
        {
            int endIndex = startIndex + length;
            if (startIndex > 0 && Char.IsLetterOrDigit(richTextBox1.Text[startIndex - 1])) return false;
            if (endIndex < richTextBox1.TextLength && Char.IsLetterOrDigit(richTextBox1.Text[endIndex])) return false;
            return true;
        }

    }
}
