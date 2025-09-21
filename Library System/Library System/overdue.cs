﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Library_System
{
    public partial class overdue : Form
    {
        public overdue()
        {
            InitializeComponent();
        }

        private void loadOverDueBooks()
        {
            string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string fineRateStr = ConfigurationManager.AppSettings["FineRate"];
                    double fineRate = double.Parse(fineRateStr);

                    string query = $@"SELECT m.memberID, m.fullname, m.email, b.title, bb.DueDate,
                   DATEDIFF(DAY, bb.DueDate, GETDATE()) AS OverdueDays,
                   (CASE WHEN DATEDIFF(DAY, bb.DueDate, GETDATE()) > 0 
                         THEN DATEDIFF(DAY, bb.DueDate, GETDATE()) * {fineRate} 
                         ELSE 0 END) AS FineAmount
            FROM BorrowedBooks bb
            JOIN members m ON bb.memberID = m.memberID
            JOIN Books b ON bb.book_ID = b.book_ID
            WHERE bb.DueDate < GETDATE() AND bb.Status = 'Borrowed'";
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

        private void SendEmailReminder(string recipientEmail, string memberName, string bookTitle, DateTime dueDate)
        {
            try
            {
                string emailSender = ConfigurationManager.AppSettings["SMTP_email"];
                string emailPassword = ConfigurationManager.AppSettings["SMTP_password"];

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(emailSender, emailPassword),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(emailSender),
                    Subject = "Library Overdue Book Reminder",
                    Body = $"Dear {memberName},\n\nYou have an overdue book:\n\nBook: {bookTitle}\nDue Date: {dueDate.ToShortDateString()}\n\nPlease return it as soon as possible.\n\nThank you,\nLibrary Management System",
                    IsBodyHtml = false
                };
                mail.To.Add(recipientEmail);
                smtp.Send(mail);

                MessageBox.Show($"Reminder sent to {memberName} ({recipientEmail})", "Email sent!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message, "Email Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No overdue books found!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["library"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT m.email, m.fullname, b.title, bb.DueDate
                         FROM BorrowedBooks bb
                         JOIN members m ON bb.memberID = m.memberID
                         JOIN books b ON bb.book_ID = b.book_ID
                         WHERE bb.DueDate < GETDATE() AND bb.ReturnDate IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string email = reader["email"].ToString();
                            string memberName = reader["fullname"].ToString();
                            string bookTitle = reader["title"].ToString();
                            DateTime dueDate = Convert.ToDateTime(reader["DueDate"]);

                            SendEmailReminder(email, memberName, bookTitle, dueDate);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void overdue_Load(object sender, EventArgs e)
        {

            loadOverDueBooks();
            ThemeManager.ApplyTheme(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string connString = ConfigurationManager.ConnectionStrings["Library"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    int memberID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["memberID"].Value);
                    string memberName = dataGridView1.SelectedRows[0].Cells["fullname"].Value.ToString();
                    string bookTitle = dataGridView1.SelectedRows[0].Cells["title"].Value.ToString();
                    DateTime dueDate = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["DueDate"].Value);
                    double fineAmount = Convert.ToDouble(dataGridView1.SelectedRows[0].Cells["FineAmount"].Value);
                    DateTime paymentDate = DateTime.Now;

                    if (fineAmount > 0)
                    {
                        DialogResult result = MessageBox.Show($"Confirm payment of ${fineAmount}?", "Confirm Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string query = "UPDATE BorrowedBooks SET Status = 'Returned' WHERE memberID = @memberID AND Status = 'Borrowed'";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@memberID", memberID);
                                cmd.ExecuteNonQuery();
                            }

                            GenerateFineReceipt(memberName, bookTitle, dueDate, fineAmount, paymentDate);
                            MessageBox.Show("Fine paid successfully. Receipt generated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            loadOverDueBooks();
                        }
                    }
                    else
                    {
                        MessageBox.Show("No fine to pay.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Select an overdue record first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GenerateFineReceipt(string memberName, string bookTitle, DateTime dueDate, double fineAmount, DateTime paymentDate)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"FineReceipt_{memberName.Replace(" ", "")}{DateTime.Now.Ticks}.pdf");


                Document doc = new Document();
                PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                doc.Open();

                // Add receipt title
                iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                Paragraph title = new Paragraph("Library Fine Payment Receipt\n\n", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                doc.Add(title);

                // Receipt details
                iTextSharp.text.Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                doc.Add(new Paragraph($"Date: {paymentDate}\n", normalFont));
                doc.Add(new Paragraph($"Member: {memberName}\n", normalFont));
                doc.Add(new Paragraph($"Book Title: {bookTitle}\n", normalFont));
                doc.Add(new Paragraph($"Due Date: {dueDate.ToShortDateString()}\n", normalFont));
                doc.Add(new Paragraph($"Fine Amount: ${fineAmount}\n\n", normalFont));
                // Signature line
                doc.Add(new Paragraph("Signature: ______________________\n\n", normalFont));

                doc.Close();
                MessageBox.Show($"Receipt saved: {filePath}", "Receipt Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating receipt: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
