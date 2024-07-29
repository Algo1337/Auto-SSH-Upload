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
        public char key = ' ';

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
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string file_name = Convert.ToString(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["Filename"].Value);
            richTextBox1.Text = File.ReadAllText(directory_path + "\\" + file_name);
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            this.dataGridView1.DefaultCellStyle.Font = new Font("Ariel", 10);
            richTextBox1.SelectionTabs = new int[] { 100, 200, 300, 400 };
            SSHLogin login = new SSHLogin();
            login.ShowDialog();

            this.client = login.c;
            this.ip = login.ip;
            this.port = login.port;

            label6.Text = $"SSH Status: {this.ip}";

        }

        private void richTextBox1_DoubleClick(object sender, EventArgs e)
        {
            this.client.Connect();
            if (textBox1.Text == "Server_Path_To_File" || textBox1.Text.Trim() == "")
                return;

            string file_name = Convert.ToString(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["Filename"].Value);

            File.WriteAllText($"{this.directory_path}\\{file_name}", richTextBox1.Text);
            Thread.Sleep(1000);

            if (textBox1.Text.EndsWith("/"))
            {
                Stream fs = File.OpenRead($"{this.directory_path}\\{file_name}");
                this.client.UploadFile(fs, textBox1.Text + file_name);
                fs.Close();
            }
            else
            {
                Stream fs = File.OpenRead($"{this.directory_path}\\{file_name}");
                this.client.UploadFile(fs, textBox1.Text + "/" + file_name);
                fs.Close();
            }

            MessageBox.Show("File uploaded to server!", "Upload");
            this.client.Disconnect();
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

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            directory_path = folderBrowserDialog1.SelectedPath;
            label4.Text = $"Current Directory: {directory_path}";

            string[] directories = Directory.GetDirectories(directory_path);
            string[] files = Directory.GetFiles(directory_path);
            if (files.Length == 0)
                return;

            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add("...", " ");
            foreach (string directory in directories)
                dataGridView1.Rows.Add(directory.Replace($"{directory_path}\\", ""), "Dir");

            foreach (string file in files)
                dataGridView1.Rows.Add(file.Replace($"{directory_path}\\", ""), "File");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            string file_name = Convert.ToString(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["filename"].Value);
            string file_type = Convert.ToString(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["filetype"].Value);
            if(file_type == "Dir")
            {
                this.directory_path += "\\" + file_name;
                label4.Text = $"Current Directory: {directory_path}";

                string[] directories = Directory.GetDirectories(directory_path);
                string[] files = Directory.GetFiles(directory_path);
                if (files.Length == 0)
                    return;

                dataGridView1.Rows.Clear();
                dataGridView1.Rows.Add("...", " ");
                foreach (string directory in directories)
                    dataGridView1.Rows.Add(directory.Replace($"{directory_path}\\", ""), "Dir");

                foreach (string file in files)
                    dataGridView1.Rows.Add(file.Replace($"{directory_path}\\", ""), "File");

                return;
            } else if(file_name == "...")
            {
                string[] dir_args = this.directory_path.Split('\\');
                this.directory_path = this.directory_path.Replace($"\\{dir_args[dir_args.Length - 1]}", "");
                label4.Text = $"Current Directory: {directory_path}";

                string[] directories = Directory.GetDirectories(directory_path);
                string[] files = Directory.GetFiles(directory_path);
                if (files.Length == 0)
                    return;

                dataGridView1.Rows.Clear();
                dataGridView1.Rows.Add("...", " ");
                foreach (string directory in directories)
                    dataGridView1.Rows.Add(directory.Replace($"{directory_path}\\", ""), "Dir");

                foreach (string file in files)
                    dataGridView1.Rows.Add(file.Replace($"{directory_path}\\", ""), "File");

                return;
            }

            richTextBox1.Text = File.ReadAllText(directory_path + "\\" + file_name);
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            this.key = e.KeyChar;
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Tab)
                richTextBox1.Text += "\t";
        }
    }
}
