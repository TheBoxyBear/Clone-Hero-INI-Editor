using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class frmMain : Form
    {
        Dictionary<string, Control> fieldMap = new Dictionary<string, Control>();
        string path;
        bool isNew;
        public frmMain()
        {
            InitializeComponent();

            fieldMap.Add("arsist = ", txtArtist);
            fieldMap.Add("name = ", txtSongName);
            fieldMap.Add("charter = ", txtCharter);
            fieldMap.Add("icon = ", txtIcon);
            fieldMap.Add("album = ", txtAlbum);
            fieldMap.Add("year = ", txtYear);
            fieldMap.Add("genre = ", txtGenre);
            fieldMap.Add("song_length = ", txtSongLength);
            fieldMap.Add("preview_start_time = ", txtPreviewStartTime);
            fieldMap.Add("count = ", txtCount);
            fieldMap.Add("playlist_track = ", txtPlaylistTrack);
            fieldMap.Add("track = ", txtAlbumTrack);
            fieldMap.Add("delay = ", txtDelay);
            fieldMap.Add("loading_phrase = ", txtLoadingPhrase);
            fieldMap.Add("diff_guitar", cboguitarDifficulty);
            fieldMap.Add("diff_band", cboBandDifficulty);
            fieldMap.Add("diff_bass", cboBandDifficulty);
            fieldMap.Add("diff_keys", cboKeysDifficulty);
            fieldMap.Add("diff_drums", cboDrumsDifficulty);
            fieldMap.Add("diff_guitarghl", cboGuitarGHLDifficulty);
            fieldMap.Add("difF_bassghl", cboBandDifficulty);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClearData();

            using OpenFileDialog f = new OpenFileDialog
            {
                Filter = "CH INI Configuration Files |*.ini",
                Multiselect = false
            };

            if (f.ShowDialog() ==  DialogResult.OK)
            {
                if (MessageBox.Show("Would you like to back up the current file before overwriting?", "Back up file first?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string filename = f.FileName;
                    int idx = filename.LastIndexOf("\\");

                    filename = (DateTime.Now.ToString() + "_" + filename.Substring(idx, filename.Length - idx).Replace("\\", "")).Replace("/", "_").Replace(":", " ");

                    Directory.CreateDirectory(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\backup\\");
                    File.Copy(f.FileName, Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\backup\\" + filename);
                }
                path = f.FileName;
                isNew = false;
                StreamReader file = new StreamReader(path);

                foreach (string txtline in File.ReadLines(path))
                {
                    //chop up the output one line at a time to determine what value is selected..this will be a bit ugly..
                    //fix for lines with no space between the equal sign and the label...
                    string line = txtline;
                    if (!line.Contains(" = ") && !line.Contains("]") && !line.Contains("]") && line.Length > 0 && !line.StartsWith(";"))
                    {
                        int position = line.IndexOf("=");
                        string start = line.Substring(0, position).Trim();
                        string end = line.Replace(start, "").Trim().Replace("=", " = ");

                        line = start + end;
                    }

                    //charter and frets are really one in the same....
                    if (line.StartsWith("frets = ") && txtCharter.Text == "")
                        txtCharter.Text = line.Replace("frets = ", "");
                    else if (line.StartsWith(";"))
                        txtComments.Text = txtComments.Text + line.Replace(";", "").Trim() + Environment.NewLine;
                    else foreach (KeyValuePair<string, Control> pair in fieldMap)
                        if (line.StartsWith(pair.Key))
                        {
                            //charter and frets are really one in the same....
                            if (line.StartsWith("frets = ") && txtCharter.Text == "")
                            {
                                txtCharter.Text = line.Replace("frets = ", "");
                                break;
                            }

                            switch (pair.Value)
                            {
                                case TextBox txt:
                                    txt.Text = line.Replace(pair.Key, "");
                                    break;
                                case ComboBox cbo:
                                    if (int.TryParse(line.Replace(pair.Key, ""), out int diff) && diff is > -2 and < 7)
                                        cbo.SelectedIndex = diff + 1;
                                    break;
                            }

                            break;
                        }

                    if (line.StartsWith(";"))
                        txtComments.Text = txtComments.Text + line.Replace(";", "").Trim() + Environment.NewLine;
                }

                file.Close();
                txtComments.Text = txtComments.Text.Trim();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            isNew = true; //by default.  If a file is opened later, this will get swapped..
            DataTable[] dts = new DataTable[7];

            for (int i = 0; i < 7; i++)
                dts[i] = new DataTable();

            dts[0].Columns.Add("Name");
            dts[0].Columns.Add("Value");
            dts[0].Rows.Add("N/A", -1);

            for (int i = 0; i < 7; i++)
                dts[0].Rows.Add(i.ToString(), i);

            foreach (DataTable dt in dts[1..])
            {
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                foreach (DataRow row in dts[0].Rows)
                    dt.Rows.Add(row.ItemArray);
            }

            cboBandDifficulty.DataSource = dts[0];
            cboBandDifficulty.DisplayMember = "Name";
            cboBandDifficulty.ValueMember = "Value";
            cboBassDifficulty.DataSource = dts[1];
            cboBassDifficulty.DisplayMember = "Name";
            cboBassDifficulty.ValueMember = "Value";
            cboBassGHLDifficulty.DataSource = dts[2];
            cboBassGHLDifficulty.DisplayMember = "Name";
            cboBassGHLDifficulty.ValueMember = "Value";
            cboDrumsDifficulty.DataSource = dts[3];
            cboDrumsDifficulty.DisplayMember = "Name";
            cboDrumsDifficulty.ValueMember = "Value";
            cboguitarDifficulty.DataSource = dts[4];
            cboguitarDifficulty.DisplayMember = "Name";
            cboguitarDifficulty.ValueMember = "Value";
            cboGuitarGHLDifficulty.DataSource = dts[5];
            cboGuitarGHLDifficulty.DisplayMember = "Name";
            cboGuitarGHLDifficulty.ValueMember = "Value";
            cboKeysDifficulty.DataSource = dts[6];
            cboKeysDifficulty.DisplayMember = "Name";
            cboKeysDifficulty.ValueMember = "Value";

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Icon))
                txtIcon.Text = Properties.Settings.Default.Icon;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Charter))
                txtCharter.Text = Properties.Settings.Default.Charter;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Charter))
                txtGenre.Text = Properties.Settings.Default.Genre;
        }

        private void cboBandDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ValidateSuggestedFields())
            {
                if (ValidateRequiredFields(out string err))
                {
                    if (isNew)
                    {
                        using SaveFileDialog f = new SaveFileDialog
                        {
                            AddExtension = true,
                            DefaultExt = ".ini",
                            Filter = "CH INI Configuration Files |*.ini"
                        };

                        if (f.ShowDialog() == DialogResult.OK)
                        {
                            path = f.FileName;
                            WriteFile(path);
                        }
                    }
                    else if (MessageBox.Show("This will overwrite the existing file", "Overwrite?", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        WriteFile(path);
                }
                else
                    MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearData();
            isNew = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnSave.PerformClick();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ValidateSuggestedFields())
            {
                if (ValidateRequiredFields(out string err))
                {
                    using SaveFileDialog f = new SaveFileDialog
                    {
                        AddExtension = true,
                        DefaultExt = ".ini",
                        Filter = "CH INI Configuration Files |*.ini"
                    };

                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        path = f.FileName;
                        WriteFile(path);
                    }
                }
                else
                    MessageBox.Show(err, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WriteFile(string path)
        {
            using StreamWriter outputFile = new StreamWriter(path, false);

            outputFile.WriteLine("[Song]");

            foreach (string str in txtComments.Text.Split('\n'))
                outputFile.WriteLine(";" + str);

            foreach (KeyValuePair<string, Control> pair in fieldMap)
            {
                switch (pair.Value)
                {
                    case TextBox txt:
                        if (txt.Text != "" || pair.Key is "name == " or "artist = ")
                            outputFile.WriteLine(pair.Key + txt);
                        break;
                    case ComboBox cbo:
                        outputFile.WriteLine(pair.Key, cbo.SelectedValue);
                        break;
                }
            }

            MessageBox.Show("ini file saved successfully!", "Success!");
        }

        private void ClearData()
        {
            foreach (Control con in fieldMap.Values) 
                switch (con)
                {
                    case TextBox txt:
                        txt.Text = "";
                        break;
                    case ComboBox cbo:
                        cbo.SelectedIndex = 0;
                        break;
                }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Icon))
            {
                txtIcon.Text = Properties.Settings.Default.Icon;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Charter))
            {
                txtCharter.Text = Properties.Settings.Default.Charter;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Genre))
            {
                txtCharter.Text = Properties.Settings.Default.Genre;
            }
        }

        private bool ValidateRequiredFields(out string errMsg)
        {
            if (txtSongName.Text == "")
            {
                errMsg = "Song Title is Required!";
                return false;
            }
            else if (txtArtist.Text == "")
            {
                errMsg = "Artist Name is Required!";
                return false;
            }

            errMsg = null;
            return true;
        }

        private bool ValidateSuggestedFields()
        {
            string suggestedFieldList = "";
            if (txtCharter.Text == "")
            {
                suggestedFieldList = suggestedFieldList + "Charter/Frets" + Environment.NewLine;
            }
            if (txtAlbum.Text == "")
            {
                suggestedFieldList = suggestedFieldList + "Album" + Environment.NewLine;
            }
            if (txtGenre.Text == "")
            {
                suggestedFieldList = suggestedFieldList + "Genre" + Environment.NewLine;
            }
            if (txtYear.Text == "")
            {
                suggestedFieldList = suggestedFieldList + "Year" + Environment.NewLine;
            }
            if (txtIcon.Text == "")
            {
                suggestedFieldList = suggestedFieldList + "Icon" + Environment.NewLine;
            }
            if (cboguitarDifficulty.SelectedValue.ToString() == "-1")
            {
                suggestedFieldList = suggestedFieldList + "Difficulty - Guitar (Set as default)" + Environment.NewLine;
            }
            //now prompt the user if they wish to continue, if so simply return true, else return false.
            return suggestedFieldList != ""
                ? MessageBox.Show("The Following Field(s) are suggested but do not have values, or have been left at defaults!" + Environment.NewLine + suggestedFieldList + Environment.NewLine + "Do you want to Continue?", "Suggested Fields missing or default", MessageBoxButtons.YesNo) == DialogResult.OK
                : true;
        }

        private void txtIcon_TextChanged(object sender, EventArgs e)
        {
            if (txtIcon.Text.EndsWith(".png") || txtIcon.Text.EndsWith(".jpg"))
                {
                txtIcon.Text = txtIcon.Text.Substring(0, txtIcon.Text.Length - 4);
                MessageBox.Show("Icons should not have file extensions after their names.  This has been removed for you");
            }
           

        }

        private void timeCalcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using frmTimeCalculator t = new frmTimeCalculator();
            t.Show();
        }

        private void openMp3oggToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //open the media file and read in ID3 tags, populate accordingly.
            using OpenFileDialog f = new OpenFileDialog()
            {
                Filter = "OGG Audio Files |*.ogg|MP3 Audio Files |*.mp3",
                Multiselect = false,
            };

            if (f.ShowDialog() == DialogResult.OK)
            {
                string audiopath = f.FileName;
                //ID3 library from https://www.nuget.org/packages/taglib
                TagLib.File tag = TagLib.File.Create(audiopath);

                if (string.IsNullOrEmpty(tag.Tag.Title))
                {
                    ClearData(); //only clear data if there's actually tags to update..presumption is that the track will have a title.
                    isNew = true;
                }
                else
                    MessageBox.Show("No data could be found!  Please fill this data in manually!", "Not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                foreach (var artist in tag.Tag.Performers)
                    txtArtist.Text = txtArtist.Text == "" ? artist : txtArtist.Text + ", " + artist;

                txtAlbum.Text = tag.Tag.Album + "";

                foreach (var genre in tag.Tag.Genres)
                    txtGenre.Text = txtGenre.Text == "" ? genre : txtGenre.Text + ", " + genre;

                txtAlbumTrack.Text = tag.Tag.Track + "";
                txtSongName.Text = tag.Tag.Title + "";
                txtYear.Text = tag.Tag.Year.ToString();

                if (txtAlbumTrack.Text == "0")
                    txtAlbumTrack.Text = "";
                if (txtYear.Text == "0")
                    txtYear.Text = "";

                txtSongLength.Text = tag.Properties.Duration.TotalMilliseconds.ToString();
            }
        }

        private void bulkEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will allow for bulk editing of numerous ini's contained within a folder structure.  This will apply changes that cannot easily be undone.  Please make sure to back up your files before continuing.  Do you want to continue?", "Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                using frmBulkEdit b = new frmBulkEdit();
                b.ShowDialog();
            }         
        }

        private void btnSaveCharter_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Charter = txtCharter.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show("Charter/Fretter Saved Successfully!", "Saved Successfully!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSaveIcon_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Icon = txtIcon.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show("Icon Saved Successfully!", "Saved Successfully!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnGenreSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Genre = txtGenre.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show("Genre Saved Successfully!", "Saved Successfully!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
