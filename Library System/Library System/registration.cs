using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_System
{
    public partial class registration : Form
    {
        public registration()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        // Login Button
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string role = comboBox1.SelectedItem.ToString();
            string email = textBox3.Text.Trim();
            string address = textBox4.Text.Trim();

            if (string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(address) ||
                string.IsNullOrEmpty(role))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 6 || !Regex.IsMatch(password, @"[A-Z]") || !Regex.IsMatch(password, @"[\d]") || !Regex.IsMatch(password, @"[\W]"))
            {
                MessageBox.Show("Password must be at least 6 characters long and include an uppercase letter, a number, and a special character.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;
            try {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "insert into login(username,password,Role,Email,address) values(@username,@password,@Role,@email,@address)";
                    using SqlCommand cmd = new SqlCommand(query, conn);
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@Role", role);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@address", address);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("User registered successfully", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //Password visibility
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        //Close The Form
        private void label5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Apply Theme Settings
        private void registration_Load(object sender, EventArgs e)
        {
            ThemeManager.ApplyTheme(this);
        }
    }
}
