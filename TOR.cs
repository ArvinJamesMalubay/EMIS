using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;
using System.Drawing.Printing;

namespace EMIS_WEBSITE_VER._02
{
    public partial class TOR : Form
    {
        public TOR()
        {
            InitializeComponent();
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            if (btn_exit.Enabled)
            {
                Form1 form = new Form1();
                form.Show();
                this.Hide();
            }
           
        }

        void clear()
        {
            tbx_idNumber.Clear();
            tbxFname.Clear();
            tbxLname.Clear();
            tbxMname.Clear();
            dtpBdate.Text = "";
            dtpStarted.Text = "";
            tbxTor.Clear();
            pbxTOR.Image = null;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbx_idNumber.Text != string.Empty && tbxFname.Text != string.Empty && tbxLname.Text != string.Empty
                    && tbxMname.Text != string.Empty)
                {
                    using (MySqlConnection con = db_connection.GetConnection())
                    {
                        string querry = "Insert into tor_table (idNumber, firstName, middleName, lastName, birthDate, dateStarted, torFile)" +
                        "values(@idNumber, @fName, @mName, @LName, @date, @dateStarted, @file)";

                        using (MySqlCommand cmd = new MySqlCommand(querry, con))
                        {
                            cmd.Parameters.AddWithValue("@idNumber", tbx_idNumber.Text);
                            cmd.Parameters.AddWithValue("@fName", tbxFname.Text);
                            cmd.Parameters.AddWithValue("@mName", tbxMname.Text);
                            cmd.Parameters.AddWithValue("@LName", tbxLname.Text);
                            cmd.Parameters.AddWithValue("@date", dtpBdate.Value);
                            cmd.Parameters.AddWithValue("@dateStarted", dtpStarted.Value);
                            cmd.Parameters.AddWithValue("@file", File.ReadAllBytes(tbxTor.Text));


                           
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Inserted Successfully!!");
                            clear();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Failed to Insert data!!");
                }
            }
            catch (MySqlException err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFileName = openFileDialog1.FileName;
                tbxTor.Text = selectedFileName;
                ShowImageInPictureBox(selectedFileName);
            }
        }
        private void ShowImageInPictureBox(string fileName)
        {
            try
            {
                // Load the image into a Bitmap
                Bitmap image = new Bitmap(fileName);

                // Display the image in the PictureBox
                pbxTOR.SizeMode = PictureBoxSizeMode.StretchImage;
                pbxTOR.Image = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }
        private void SaveFileNameToDatabase(string fileName)
        {
            // Read image file into a byte array
            byte[] imageBytes = File.ReadAllBytes(fileName);

            try
            {
                using (MySqlConnection con = db_connection.GetConnection())
                {
                    con.Open();

                    // SQL command to insert the file name into your database table
                    string query = "INSERT INTO  tor_table(torFile) VALUES (@FileName)";
                    MySqlCommand command = new MySqlCommand(query, con);
                    command.Parameters.AddWithValue("@FileName", imageBytes);
                    command.ExecuteNonQuery();
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = db_connection.GetConnection())
                {
                    string query = "SELECT idNumber,firstName,middleName ,lastName, torFile FROM tor_table WHERE idNumber = '" + tbxSearch.Text+"'"; 
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                tbx_idNumber.Text = reader["idNumber"].ToString();
                                tbxFname.Text = reader["firstName"].ToString();
                                tbxMname.Text = reader["middleName"].ToString();
                                tbxLname.Text = reader["lastName"].ToString();
                            
                                byte[] imageData = (byte[])reader["torFile"];
                                
                                ShowImageFromDatabase(imageData);
                            }
                            else
                            {
                                MessageBox.Show("No image found in database.");
                            }
                        }
                    }
                }
            }
            catch (MySqlException err)
            {
                MessageBox.Show("Error: " + err.Message);
            }
        }
        private void ShowImageFromDatabase(byte[] imageData)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    Image image = Image.FromStream(ms);
                    pbxTOR.SizeMode = PictureBoxSizeMode.StretchImage;
                    pbxTOR.Image = image;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying image: " + ex.Message);
            }
        }

        private void tbxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            char num = e.KeyChar;
            if (!Char.IsDigit(num) && num != 8 && num != 46)
            {
                e.Handled = true;

            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();

            printDialog.Document = printDocument;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
                printDocument.Print();
            }
        
        }
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {          
            Image imageToPrint = pbxTOR.Image;

            if (imageToPrint != null)
            {
                e.Graphics.DrawImage(imageToPrint, e.MarginBounds);
            }
            else
            {
                e.Graphics.DrawString("No image to print", Font, Brushes.Red, e.MarginBounds);
            }
        }
    }
}
