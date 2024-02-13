using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Word = Microsoft.Office.Interop.Word;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace EMIS_WEBSITE_VER._02
{
    public partial class Enrollment_Form : Form
    {
      
        public Enrollment_Form()
        {
            InitializeComponent();
            openToolStripMenuItem.Click += openToolStripMenuItem_Click_1;
            addStudentToolStripMenuItem.Click += addStudentToolStripMenuItem_Click;
            viewToolStripMenuItem.Click += viewToolStripMenuItem_Click;
            cmb_track.Items.AddRange(new string[] { "ABM", "EIM", "ICT" , "HUMMS"});


        }
        

        private int labelID;
        public void AutoincrementID()
        {

            MySqlConnection con = db_connection.GetConnection();
            MySqlCommand cmd = new MySqlCommand("select count(student_number) from student_information ", con);
            int j = Convert.ToInt32(cmd.ExecuteScalar());
            j++;
            lbl_autoID.Text = labelID + j.ToString();

            con.Close();
        }
        private void btn_exit_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Hide();
        }
        private void openToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
          
             string pdfFilePath = @"C:\Users\Dell\Desktop\Enrollment_form.pdf";
             axAcroPDF1.src = pdfFilePath;
          
        }
        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }
        private void addStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }
    
        private void ClearTextboxes(Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is TextBox)
                {
                    ((TextBox)ctrl).Clear();
                }

                // Recursively clear textboxes in child controls (e.g., panels, group boxes)
                if (ctrl.HasChildren)
                {
                    ClearTextboxes(ctrl);
                }
            }
        }
        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearTextboxes(this);
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbx_schoolYear.Text != string.Empty || cmb_gradeEnroll.Text != string.Empty
                    || tbx_father_lastName.Text != string.Empty || tbx_father_firstName.Text != string.Empty
                    || tbx_father_middleName.Text != string.Empty || tbx_Age.Text != string.Empty)
                {
                    using (MySqlConnection con = db_connection.GetConnection())
                    {

                        string query = "INSERT INTO student_information " +
                                       "(birth_certificate, lrn, lastName, firstName, middleName, extensionName, " +
                                       "birthDate, sex, age, indigenous, 4psBeneficiary,4psIDnumber ,disability, typeOfDisability, student_number) " +
                                       "VALUES (@BirthCertificate, @LRN, @LastName, @FirstName, @MiddleName, @ExtensionName," +
                                       "@BirthDate, @Sex, @Age, @Indigenous, @Is4psBeneficiary, @4psIDnumber, @HasDisability, @TypeOfDisability, @Number);" +
                                        "Insert into current_address (house_No,Barangay, sitio, municipality, province, country, zipCode, student_number) " +
                                        "Values( @House_No, @Barangay, @Sitio, @Municipality, @Province, @Country, @Zipcode, @Number); " +
                                        "Insert into permanent_address(house_No, Barangay, sitio, municipality, province, country, zipCode, student_number) " +
                                        "Values( @perm_House_No, @perm_Barangay, @perm_Sitio, @perm_Municipality, @perm_Province, @perm_Country, @perm_Zipcode, @Number);" +
                                        "Insert into parents_guardian_info(fathers_name, fathers_contact, mothers_name, mothers_contact, legalGuardian_name, legalGuardian_contact , student_number)" +
                                        "Values(@FatherName, @FatherContact, @MotherName, @MotherContact, @LegalGuardianName, @LegalGuardianContact, @Number);" +
                                        "Insert Into date_enrolled (schoolYear, gradeEnroll, withLRN, ifReturnee, date, student_number)" +
                                        "Values (@schoolYear, @gradeEnroll,@withLRN, @ifReturnee, @date, @Number);" +
                                        "Insert into senior_high_learners (semester, track, strand, learning_reference)" +
                                        "Values (@semester, @track, @strand, @learning_reference);" +
                                        "Insert into returning_learner(lastGradeCompleted,lastSchoolCompleted,lastSchoolAattended,schoolID) " +
                                        "Values (@lastGradeCompleted,@lastSchoolCompleted,@lastSchoolAattended,@schoolID)";


                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@BirthCertificate", tbx_birthCertificate.Text);
                            cmd.Parameters.AddWithValue("@LRN", tbx_LRN_NUMBER.Text);
                            cmd.Parameters.AddWithValue("@LastName", tbx_lastName.Text);
                            cmd.Parameters.AddWithValue("@FirstName", tbx_firstName.Text);
                            cmd.Parameters.AddWithValue("@MiddleName", tbx_middleName.Text);
                            cmd.Parameters.AddWithValue("@ExtensionName", tbx_extensionName.Text);
                            cmd.Parameters.AddWithValue("@BirthDate", dtp_birthDate.Value);
                            cmd.Parameters.AddWithValue("@Sex", rbt_sex_MALE.Checked ? "Male" : "Female");
                            cmd.Parameters.AddWithValue("@Age", tbx_Age.Text);
                            cmd.Parameters.AddWithValue("@Indigenous", tbx_indegenous.Text);
                            cmd.Parameters.AddWithValue("@Is4psBeneficiary", chk_4PS_YES.Checked ? "Yes" : "No");
                            cmd.Parameters.AddWithValue("@@4psIDnumber", tbx_4PS_ID_NUMBER.Text);
                            cmd.Parameters.AddWithValue("@HasDisability", chk_Disability_YES.Checked ? "Yes" : "No");
                            if (chk_visual_Impairment.Checked)
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "Visual Impairment");
                            else if (chk_multiple_disorder.Checked)
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "Multiple Disorder");
                            else if (chk_intellectual_disability.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "Intellectual Disability");
                            }
                            else if (chk_emotional_disorder.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "Emotional- Behavioral Disorder");
                            }
                            else if (chk_chronic_disease.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "Special Health Problem/ Chronic Disease");
                            }
                            else if (chk_hearing_Impairment.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "Hearing Impairment ");
                            }
                            else if (chk_blind.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "blind");
                            }
                            else if (chk_autism_disorder.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "Autism Spectrum Disorder");
                            }
                            else if (chk_cerebral_palsy.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", " Cerebral Palsy ");
                            }
                            else if (chk_learning_disability.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "Learning Disability");
                            }
                            else if (chk_low_vision.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "low vision");
                            }
                            else if (chk_speech_disorder.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", " Speech/Language Disorder");
                            }
                            else if (chk_physical_handicap.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "Orthopedic Physical Handicap");
                            }
                            else if (chk_cancer.Checked)
                            {
                                cmd.Parameters.AddWithValue("@TypeOfDisability", "cancer");
                            }
                            cmd.Parameters.AddWithValue("@Number", lbl_autoID.Text);

                            cmd.Parameters.AddWithValue("@House_No", tbx_Current_house_No.Text);
                            cmd.Parameters.AddWithValue("@Barangay", tbx_Current_barangay.Text);
                            cmd.Parameters.AddWithValue("@Sitio", tbx_Current_Street_Name.Text);
                            cmd.Parameters.AddWithValue("@Municipality", tbx_Current_city.Text);
                            cmd.Parameters.AddWithValue("@Province", tbx_Current_Province.Text);
                            cmd.Parameters.AddWithValue("@Country", tbx_Current_country.Text);
                            cmd.Parameters.AddWithValue("@Zipcode", tbx_Current_zipcode.Text);

                            cmd.Parameters.AddWithValue("@perm_House_No", tbx_permanent_house_no.Text);
                            cmd.Parameters.AddWithValue("@perm_Barangay", tbx_permanent_Barangay.Text);
                            cmd.Parameters.AddWithValue("@perm_Sitio", tbx_permanent_Street_Name.Text);
                            cmd.Parameters.AddWithValue("@perm_Municipality", tbx_permanent_city.Text);
                            cmd.Parameters.AddWithValue("@perm_Province", tbx_permanent_Province.Text);
                            cmd.Parameters.AddWithValue("@perm_Country", tbx_permanent_country.Text);
                            cmd.Parameters.AddWithValue("@perm_Zipcode", tbx_permanent_Zipcode.Text);

                            cmd.Parameters.AddWithValue("@FatherName", tbx_father_firstName.Text + " " + tbx_father_middleName.Text + " " + tbx_father_lastName.Text);
                            cmd.Parameters.AddWithValue("@FatherContact", tbx_father_contact.Text);
                            cmd.Parameters.AddWithValue("@MotherName", tbx_mothers_firstName.Text + " " + tbx_mothers_middleName.Text + " " + tbx_mothers_lastName.Text);
                            cmd.Parameters.AddWithValue("@MotherContact", tbx_mothers_contact.Text);
                            cmd.Parameters.AddWithValue("@LegalGuardianName", tbx_guardians_firstName.Text + " " + tbx_guardians_middleName.Text + " " + tbx_guardians_lastName.Text);
                            cmd.Parameters.AddWithValue("@LegalGuardianContact", tbx_guardians_contact.Text);

                            cmd.Parameters.AddWithValue("@schoolYear", tbx_schoolYear.Text);
                            cmd.Parameters.AddWithValue("@gradeEnroll", cmb_gradeEnroll.Text);
                            cmd.Parameters.AddWithValue("@withLRN", chk_LRN_YES.Checked ? "Yes" : "No");
                            cmd.Parameters.AddWithValue("@ifReturnee", chk_RETURNING_YES.Checked ? "Yes" : "No");
                            cmd.Parameters.AddWithValue("@date", dtp_date_enrolled.Value.ToString("yyyy-MM-dd"));

                            cmd.Parameters.AddWithValue("@semester", chk_SeniorHigh_Semester_1ST.Checked ? "1st" : "2nd");
                            cmd.Parameters.AddWithValue("@track", cmb_track.Text);
                            cmd.Parameters.AddWithValue("@strand", cmb_strand.Text);
                            if (chk_Modular_Print.Checked)
                                cmd.Parameters.AddWithValue("@learning_reference", "Modular (Print)");
                            else if (chk_Modular_Digital.Checked)
                                cmd.Parameters.AddWithValue("@learning_reference", "Modular (Digital)");
                            else if (chk_Online.Checked)
                                cmd.Parameters.AddWithValue("@learning_reference", " Online");
                            else if (chk_Blended.Checked)
                                cmd.Parameters.AddWithValue("@learning_reference", "Blended");
                            else if (chk_Television.Checked)
                                cmd.Parameters.AddWithValue("@learning_reference", "Educational Television ");
                            else if (chk_Homeschooling.Checked)
                                cmd.Parameters.AddWithValue("@learning_reference", "Homeschooling");
                            else if (chk_Radio_Based.Checked)
                                cmd.Parameters.AddWithValue("@learning_reference", " Radio-Based Instruction Television ");

                            cmd.Parameters.AddWithValue("@lastGradeCompleted", tbx_grade_level_completed.Text);
                            cmd.Parameters.AddWithValue("@lastSchoolCompleted", tbx_last_school_attended.Text);
                            cmd.Parameters.AddWithValue("@lastSchoolAattended", tbx_last_school_year_completed.Text);
                            cmd.Parameters.AddWithValue("@schoolID", tbx_school_id.Text);

                            cmd.ExecuteNonQuery();
                            AutoincrementID();
                            MessageBox.Show("Added Successfully !!!");
                            ClearTextboxes(this);

                        }
                    }
                }
                else {
                    MessageBox.Show("Failed Ka!!!");
                }

            }
            catch (MySqlException err)
            {
                MessageBox.Show(err.Message);
            }
        }


        private void Enrollment_Form_Load(object sender, EventArgs e)
        {
            display_database();
            AutoincrementID();
            cmb_track.SelectedIndexChanged += cmb_track_SelectedIndexChanged;  
            UpdateStrand();
        }

        private void cmb_track_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStrand();
        }

        private void UpdateStrand()
        {      
            cmb_strand.Items.Clear();  
            string selectedItem = cmb_track.SelectedItem as string;

            // Add items to cmb_strand based on the selected item in cmb_track
            if (selectedItem == "ABM")
            {
                cmb_strand.Items.AddRange(new string[] { "Accountancy", "Business" });
            }
            else if (selectedItem == "EIM")
            {
                cmb_strand.Items.AddRange(new string[] { "Electrical", "Electronic" });
            }
            else if (selectedItem == "ICT")
            {
                cmb_strand.Items.AddRange(new string[] { "Programmer", "Networking" });
            }
            else if (selectedItem == "HUMMS")
            {
                cmb_strand.Items.AddRange(new string[] { "Teaching" , ""});
            }
        }

        private void chk_Disability_YES_CheckedChanged(object sender, EventArgs e)
        {
            chk_Disability_NO.Checked = false;

            chk_autism_disorder.Visible = true;
            chk_blind.Visible = true;
            chk_cancer.Visible = true;
            chk_cerebral_palsy.Visible = true;
            chk_chronic_disease.Visible = true;
            chk_emotional_disorder.Visible = true;
            chk_hearing_Impairment.Visible = true;
            chk_intellectual_disability.Visible = true;
            chk_learning_disability.Visible = true;
            chk_low_vision.Visible = true;
            chk_multiple_disorder.Visible = true;
            chk_physical_handicap.Visible = true;
            chk_speech_disorder.Visible = true;
            chk_visual_Impairment.Visible = true;

        }

        private void chk_Disability_NO_CheckedChanged(object sender, EventArgs e)
        {
            chk_Disability_YES.Checked = false;

            chk_autism_disorder.Visible = false;
            chk_blind.Visible = false;
            chk_cancer.Visible = false;
            chk_cerebral_palsy.Visible = false;
            chk_chronic_disease.Visible = false;
            chk_emotional_disorder.Visible = false;
            chk_hearing_Impairment.Visible = false;
            chk_intellectual_disability.Visible = false;
            chk_learning_disability.Visible = false;
            chk_low_vision.Visible = false;
            chk_multiple_disorder.Visible = false;
            chk_physical_handicap.Visible = false;
            chk_speech_disorder.Visible = false;
            chk_visual_Impairment.Visible = false;

        }


    
        private void tbx_LRN_NUMBER_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            char num = e.KeyChar;
            if (!Char.IsDigit(num) && num != 8 && num != 46)
            {
                e.Handled = true;

            }
        }

        private void tbx_Current_zipcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            char num = e.KeyChar;
            if (!Char.IsDigit(num) && num != 8 && num != 46)
            {
                e.Handled = true;

            }
        }
        public void display()
        {
            string query = "SELECT CONCAT(si.firstName, ' ', si.middleName, ' ', si.lastName) AS FullName , si.sex, si.age, si.birthDate, " +
                "CONCAT(ca.barangay, '', ca.sitio, '', ca.municipality, ' ', ca.province) AS Address, enroll.gradeEnroll " + // Added space after gradeEnroll
                "FROM student_information as si INNER JOIN current_address as ca ON si.student_number = ca.student_number " +
                "INNER JOIN date_enrolled as enroll ON si.student_number = enroll.student_number " +
                "WHERE si.firstName = '" + tbxsearch.Text + "' OR si.firstName LIKE '" + tbxsearch.Text + "%'";
            db_connection.search_student(query, dgvstudent);
        }
        public void display_database()
        {
            string query = "SELECT CONCAT(si.firstName, ' ', si.middleName, ' ', si.lastName) AS FullName , si.sex, si.age, si.birthDate, " +
                   "CONCAT(ca.barangay, '', ca.sitio, '', ca.municipality, ' ', ca.province) AS Address, enroll.gradeEnroll " + // Added space after gradeEnroll
                   "FROM student_information as si INNER JOIN current_address as ca ON si.student_number = ca.student_number " +
                   "INNER JOIN date_enrolled as enroll ON si.student_number = enroll.student_number ";
                 
            db_connection.search_student(query, dgvstudent);
        }


        private void tbxsearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tbxsearch.Text != string.Empty)
                {
                    display();
                }
                else
                {
                    display_database();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        
    }
    }

