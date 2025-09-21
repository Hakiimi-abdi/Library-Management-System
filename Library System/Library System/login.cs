using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Library_System
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        //login button
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();
                    string query = "select Role from login where username = @username AND password = @password";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        string username = textBox1.Text.Trim();
                        string password = textBox2.Text.Trim();

                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            string role = reader["Role"].ToString();
                            this.Hide();
                            Dashboard dash = new Dashboard(textBox1.Text,role);
                            dash.Show();
                        }
                        else
                        {
                            MessageBox.Show("Incorrect username or password.");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //password visibility
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        //close the form
        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void login_Load(object sender, EventArgs e)
        {
            ThemeManager.ApplyTheme(this); //Apply theme settings
        }
    }
}
