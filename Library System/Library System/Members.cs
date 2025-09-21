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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Library_System
{
    public partial class Members : Form
    {
        public Members()
        {
            InitializeComponent();
        }

        private void LoadMembers()
        {

            string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM  members";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Update member
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {

                string fullname = textBox2.Text.Trim();
                string email = textBox3.Text.Trim();
                string address = textBox4.Text.Trim();

                if (string.IsNullOrWhiteSpace(fullname) ||
               string.IsNullOrWhiteSpace(email) ||
               string.IsNullOrWhiteSpace(address))
                {
                    MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Invalid email format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open();
                        string query = "UPDATE members SET fullname=@fullname, email=@email, address=@address";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("fullname", fullname);
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Parameters.AddWithValue("@address", address);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Member updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadMembers();
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a Member to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Members_Load(object sender, EventArgs e)
        {
            LoadMembers();
            ThemeManager.ApplyTheme(this);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.ReadOnly = true; // Makes the grid read-only
            dataGridView1.AllowUserToAddRows = false; // Prevents adding new rows directly


            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox2.Text = row.Cells["fullname"].Value.ToString();
                textBox3.Text = row.Cells["email"].Value.ToString();
                textBox4.Text = row.Cells["address"].Value.ToString();


            }
        }

        //Delete member
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int memberID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["memberID"].Value);

                string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM members WHERE memberID=@memberID";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@memberID", memberID);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Member deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadMembers();
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a Member to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {


        }

        //Search member
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchValue = textBox1.Text;
            string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM members WHERE fullname LIKE @search OR email LIKE @search OR address LIKE @search OR memberID LIKE @search OR joindate LIKE @search";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);


                    dataGridView1.DataSource = dataTable;

                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Add member
        private void button4_Click_1(object sender, EventArgs e)
        {
            string fullname = textBox2.Text.Trim();
            string email = textBox3.Text.Trim();
            string address = textBox4.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullname) ||
             string.IsNullOrWhiteSpace(email) ||
             string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO members (fullname, email, address) VALUES (@fullname, @email, @address)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@fullname", fullname);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@address", address);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Member added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadMembers();
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
