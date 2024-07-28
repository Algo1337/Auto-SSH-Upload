using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Auto_SSH_Upload.Properties;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Auto_SSH_Upload
{
    public partial class SSHLogin : Form
    {
        public SftpClient c;
        public string ip;
        public int port;
        public SSHLogin()
        {
            InitializeComponent();
        }

        private void SSHLogin_Load(object sender, EventArgs e)
        {
            if ((bool)Properties.Settings.Default["remember_pw"] == true)
            {
                checkBox1.Checked = true;
                textBox1.Text = Properties.Settings.Default.ssh_ip;
                textBox2.Text = Properties.Settings.Default.ssh_port.ToString();
                textBox3.Text = Properties.Settings.Default.ssh_username;
                textBox4.Text = Properties.Settings.Default.ssh_passwd;
                Properties.Settings.Default["remember_pw"] = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == string.Empty || textBox2.Text == string.Empty || textBox3.Text == string.Empty || textBox4.Text == string.Empty)
            {
                MessageBox.Show("Fill out all fields!", "SSH Login Error");
                return;
            }

            this.c = new SftpClient(textBox1.Text, Convert.ToInt32(textBox2.Text), textBox3.Text, textBox4.Text);
            this.ip = textBox1.Text;
            this.port = Convert.ToInt32(textBox2.Text);


            if (checkBox1.Checked == true)
            {
                Settings.Default.ssh_ip = textBox1.Text;
                Settings.Default.ssh_port = Convert.ToInt32(textBox2.Text);
                Settings.Default.ssh_username = textBox3.Text;
                Settings.Default.ssh_passwd = textBox4.Text;
                Settings.Default.remember_pw = true;
                Settings.Default.Save();
            }
            else
            {
                Settings.Default.ssh_ip = "";
                Settings.Default.ssh_port = 0;
                Settings.Default.ssh_username = "";
                Settings.Default.ssh_passwd = "";
                Settings.Default.remember_pw = false;
                Settings.Default.Save();
            }

            this.Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
