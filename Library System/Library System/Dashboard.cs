using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;
using Microsoft.Data.SqlClient;

namespace Library_System
{
    public partial class Dashboard : Form
    {
        private string userRole;
        private string username;
        public Dashboard(string username, string role)
        {
            InitializeComponent();
            this.username = username;
            this.userRole = role;
            ApplyRoleRestrictions();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            openchildform(new registration());
        }

        // Open a child form inside the panel
        private Form activeForm = null;
        private void openchildform(Form childFrom)
        {
            if (activeForm != null)
                activeForm.Close();

            activeForm = childFrom;
            childFrom.TopLevel = false;
            childFrom.FormBorderStyle = FormBorderStyle.None;
            childFrom.Dock = DockStyle.Fill;
            panel1.Controls.Add(childFrom);
            panel1.Tag = childFrom;
            childFrom.BringToFront();
            childFrom.Show();
        }

        //light and dark mode
        private bool isDarkMode = false;

        private void ToggleTheme()
        {
            ThemeManager.IsDarkMode = !ThemeManager.IsDarkMode;

            Properties.Settings.Default.Theme = ThemeManager.IsDarkMode ? "Dark" : "Light";
            Properties.Settings.Default.Save();

            ThemeManager.ApplyTheme(this);

            iconButton7.Text = ThemeManager.IsDarkMode ? "☀ Light Mode" : "🌙 Dark Mode";
        }

        //Restrict Access
        private void ApplyRoleRestrictions()
        {
            if (userRole == "Librarian")
            {
                iconButton1.Visible = false;
                iconButton2.Visible = false;
            }
            AdjustButtonLayout();
        }

        //Adjust button layout
        private void AdjustButtonLayout()
        {
            int yoffset = 10;
            for (int i = panel2.Controls.Count - 1; i >= 0; i--)
            {
                if (panel2.Controls[i] is Button btn && btn.Visible)
                {
                    btn.Location = new Point(btn.Location.X, yoffset);
                    yoffset += btn.Height + 10;
                }
            }
        }

        private void loadDashboardSummary()
        {
            string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Count total books
                string queryTotal = "SELECT COUNT(*) FROM books";
                using (SqlCommand cmd = new SqlCommand(queryTotal, conn))
                {
                    label5.Text = cmd.ExecuteScalar().ToString();
                }

                // Count borrowed books
                string queryBorrowed = "SELECT COUNT(*) FROM BorrowedBooks WHERE Status = 'Borrowed'";
                using (SqlCommand cmd = new SqlCommand(queryBorrowed, conn))
                {
                    label7.Text = cmd.ExecuteScalar().ToString();
                }

                // Count overdue books
                string queryOverdue = "SELECT COUNT(*) FROM BorrowedBooks WHERE DueDate < GETDATE() AND Status = 'Borrowed'";
                using (SqlCommand cmd = new SqlCommand(queryOverdue, conn))
                {
                    label8.Text = cmd.ExecuteScalar().ToString();

                }

                //count members
                string queryTotalMembers = "SELECT COUNT(*) FROM members";
                using (SqlCommand cmd = new SqlCommand(queryTotalMembers, conn))
                {
                    label6.Text = cmd.ExecuteScalar().ToString();
                }

            }
        }

        //open members form
        private void iconButton2_Click(object sender, EventArgs e)
        {
            openchildform(new Members());
        }

        //toggle dark/light themes
        private void iconButton7_Click(object sender, EventArgs e)
        {
            ToggleTheme();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            ThemeManager.IsDarkMode = Properties.Settings.Default.Theme == "Dark" || true;
            ThemeManager.ApplyTheme(this);
            iconButton7.Text = ThemeManager.IsDarkMode ? "☀ Light Mode" : "🌙 Dark Mode";
            AdjustButtonLayout();
            loadDashboardSummary();

        }

        private void iconButton8_Click(object sender, EventArgs e)
        {
            this.Hide();
            login log = new login();
            log.Show();
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            openchildform(new Books());
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            openchildform(new Borrow());
        }

        private void iconButton9_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            openchildform(new Return());
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            openchildform(new overdue());
        }
    }
}
