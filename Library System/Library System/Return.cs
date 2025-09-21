using Microsoft.Data.SqlClient;
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

namespace Library_System
{
    public partial class Return : Form
    {
        public Return()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book to return!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    int borrowID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["BorrowID"].Value);
                    int memberID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["memberID"].Value);
                    string bookTitle = dataGridView1.SelectedRows[0].Cells["BookTitle"].Value.ToString();

                    // Retrieve overdue days for the member
                    string overdueQuery = "SELECT SUM(DATEDIFF(DAY, DueDate, GETDATE())) FROM BorrowedBooks WHERE memberID = @memberID AND Status = 'Borrowed' AND DueDate < GETDATE()";
                    using (SqlCommand overdueCmd = new SqlCommand(overdueQuery, conn, transaction))
                    {
                        overdueCmd.Parameters.AddWithValue("@memberID", memberID);
                        object overdueDaysResult = overdueCmd.ExecuteScalar();
                        int overdueDays = overdueDaysResult != DBNull.Value ? Convert.ToInt32(overdueDaysResult) : 0;

                        if (overdueDays > 0)
                        {
                            // Retrieve FineRate from config
                            double fineRate;
                            if (!double.TryParse(ConfigurationManager.AppSettings["FineRate"], out fineRate))
                            {
                                MessageBox.Show("FineRate configuration is invalid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                transaction.Rollback();
                                return;
                            }

                            // Calculate fine amount dynamically
                            double fineAmount = overdueDays * fineRate;

                            MessageBox.Show($"You must settle your fine of ${fineAmount} before returning books.", "Return Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            transaction.Rollback();
                            return;
                        }
                    }

                    // Update BorrowedBooks table
                    string updateQuery = "UPDATE BorrowedBooks SET ReturnDate = GETDATE(), Status = 'Returned' WHERE BorrowID = @BorrowID";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction))
                    {
                        updateCmd.Parameters.AddWithValue("@BorrowID", borrowID);
                        updateCmd.ExecuteNonQuery();
                    }

                    // Retrieve book ID for quantity update
                    string getBookIdQuery = "SELECT book_ID FROM books WHERE title = @title";
                    using (SqlCommand cmd = new SqlCommand(getBookIdQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@title", bookTitle);
                        object result = cmd.ExecuteScalar();
                        if (result == null)
                        {
                            MessageBox.Show("Book not found in the database!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            transaction.Rollback();
                            return;
                        }

                        int book_ID = Convert.ToInt32(result);

                        // Update available copies in Books table
                        string updateBookQuery = "UPDATE books SET AvailableCopies = AvailableCopies + 1 WHERE book_ID = @book_ID";
                        using (SqlCommand updateCmd = new SqlCommand(updateBookQuery, conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@book_ID", book_ID);
                            updateCmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    MessageBox.Show($"Book '{bookTitle}' returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadBorrowedBooks();
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void loadBorrowedBooks()
        {
            string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = @"
           SELECT bb.BorrowID, m.memberID, m.fullname, b.title AS BookTitle, bb.BorrowDate, bb.DueDate 
FROM BorrowedBooks bb
JOIN members m ON bb.memberID = m.memberID
JOIN books b ON bb.book_ID = b.book_ID
WHERE bb.Status = 'Borrowed'";

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
        private void Return_Load(object sender, EventArgs e)
        {
            loadBorrowedBooks();
            ThemeManager.ApplyTheme(this);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox1.Text = row.Cells["fullname"].Value.ToString();
                textBox2.Text = row.Cells["BookTitle"].Value.ToString();


            }
        }
    }
}
