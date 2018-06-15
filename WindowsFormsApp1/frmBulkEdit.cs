﻿using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Text;

namespace WindowsFormsApp1
{
    public partial class frmBulkEdit : Form
    {
        string path;
        string selectedtag;
        string selectedval;
        StringBuilder changeTracking = new StringBuilder(); //use to track all changes made...
        public frmBulkEdit()
        {
            InitializeComponent();
        }

        private void frmBulkEdit_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");
            dt.Rows.Add("Name", "name = ");
            dt.Rows.Add("Artist", "artist = ");
            dt.Rows.Add("Album", "album = ");
            dt.Rows.Add("Genre", "genre = ");
            dt.Rows.Add("Year", "year = ");
            dt.Rows.Add("Song Length", "song_length = ");
            dt.Rows.Add("Count", "count = ");
            dt.Rows.Add("Difficulty - Band", "diff_band = ");
            dt.Rows.Add("Difficulty - Guitar", "diff_guitar = ");
            dt.Rows.Add("Difficulty - Bass", "diff_bass = ");
            dt.Rows.Add("Difficulty - Drums", "diff_drums = ");
            dt.Rows.Add("Difficulty - Keys", "diff_keys");
            dt.Rows.Add("Difficulty - Guitar (GHL)", "diff_guitarghl = ");
            dt.Rows.Add("Difficulty - Bass (GHL)", "diff_bassghl = ");
            dt.Rows.Add("Preview Start Time", "preview_start_time = ");
            dt.Rows.Add("Frets/Charter", "frets = ");
            dt.Rows.Add("icon", "icon = ");
            dt.Rows.Add("Album Track #", "album_track = ");
            dt.Rows.Add("Playlist Track #", "playlist_track = ");

            //populate drop down with known values...

            cboField.DataSource = dt;
            cboField.ValueMember = "Value";
            cboField.DisplayMember = "Name";
            cboNewValue.Hide();

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Name");
            dt2.Columns.Add("Value");
            dt2.Rows.Add("N/A", "-1");
            for (int i = 0; i < 7; i++)
            {
                dt2.Rows.Add(i.ToString(), i.ToString());
            }
            cboNewValue.DataSource = dt2;
            cboNewValue.DisplayMember = "Name";
            cboNewValue.ValueMember = "Value";

            FolderBrowserDialog d = new FolderBrowserDialog();
            DialogResult r = d.ShowDialog();
            if (r == DialogResult.OK)
            {
                path = d.SelectedPath;

            }
            else
            {
                this.Close();
            }

        }

        private void cboField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboField.SelectedValue.ToString().Contains("diff_"))
            {
                cboNewValue.Show();
                txtNewValue.Hide();
            }
            else
            {
                txtNewValue.Show();
                cboNewValue.Hide();
            }
            selectedtag = cboField.SelectedValue.ToString();
            
        }

        private void WalkDirectoryTree(System.IO.DirectoryInfo root) //borrowed from https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-iterate-through-a-directory-tree
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("*.ini");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                
            }

            if (files != null)
            {

                
                foreach (System.IO.FileInfo fi in files)
                {
                    // In this example, we only access the existing FileInfo object. If we
                    // want to open, delete or modify the file, then
                    // a try-catch block is required here to handle the case
                    // where the file has been deleted since the call to TraverseTree().

                    //read the file line by line and replace any instances of the tag with the selected new tag.
                    StringBuilder fileContents = new StringBuilder(); //temp store entire contents of file regardless of if there's any changes or not.
                    bool hasChanges = false;

                    foreach (string txtline in File.ReadLines(fi.FullName))
                    {
                        
                        string tmp = "";
                        string tmp2 = "";
                        tmp = txtline.Replace(" ", "");
                        tmp2 = selectedtag.Replace(" ", "");
                        if (tmp.Contains(tmp2))
                        {
                            hasChanges = true;
                            fileContents.AppendLine(selectedtag + selectedval);
                            //also log changes...
                            changeTracking.Append(fi.FullName + "| Old value: " + txtline + "| New value: " + selectedtag + selectedval + Environment.NewLine);
                        }
                        else
                        {
                            fileContents.AppendLine(txtline);
                        }

                    }

                    if (hasChanges == true)
                    {
                        System.IO.File.WriteAllText(fi.FullName, fileContents.ToString());
                    }
                   





                }
                

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo);
                }
            }
            
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            WalkDirectoryTree(dir);
            if (changeTracking.ToString() != "")
            {
                //write all changes to the directory where the application is running in a log file...
                //build out a date string to give each log a unique name to prevent overwriting...
                string dateString = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString();
                string logPath = Environment.CurrentDirectory + "/log " + dateString + ".txt";
                System.IO.File.WriteAllText(logPath, changeTracking.ToString());
            }
            MessageBox.Show("Successfully updated!");
            this.Close();
        }

        private void cboNewValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedval = cboNewValue.SelectedValue.ToString();
        }

        private void txtNewValue_TextChanged(object sender, EventArgs e)
        {
            selectedval = txtNewValue.Text;
        }
    }
}