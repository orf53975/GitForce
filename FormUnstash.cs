﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GitForce
{
    /// <summary>
    /// Form implementing the git unstash command
    /// </summary>
    public partial class FormUnstash : Form
    {
        /// <summary>
        /// Currently selected stash string (stash id part only)
        /// </summary>
        private string _stash;

        public FormUnstash()
        {
            InitializeComponent();
            ClassWinGeometry.Restore(this);

            PopulateStashList();
        }

        /// <summary>
        /// Form is closing.
        /// </summary>
        private void FormUnstashFormClosing(object sender, FormClosingEventArgs e)
        {
            ClassWinGeometry.Save(this);
        }

        /// <summary>
        /// Populate list with existing stashes
        /// </summary>
        private void PopulateStashList()
        {
            listStashes.Items.Clear();
            string[] response = App.Repos.Current.Run("stash list").Split(("\n").ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var stash in response)
                listStashes.Items.Add(stash);

            // Disable buttons (in the case the stash list was empty)
            // but re-enable them once we have a stash to select (select the top one by default)
            btApply.Enabled = btRemove.Enabled = btShow.Enabled = false;
            if (listStashes.Items.Count > 0)
            {
                string s = listStashes.Items[0].ToString();
                _stash = s.Substring(0, s.IndexOf(':'));
                listStashes.SelectedIndex = 0;
                btApply.Enabled = btRemove.Enabled = btShow.Enabled = true;
            }
        }

        /// <summary>
        /// User clicked on one of the selected stash entries.
        /// Set the proper control enables.
        /// </summary>
        private void ListStashesSelectedIndexChanged(object sender, EventArgs e)
        {
            string s = listStashes.SelectedItem.ToString();
            _stash = s.Substring(0, s.IndexOf(':'));
        }

        /// <summary>
        /// Apply the selected stash
        /// </summary>
        private void BtUnstashClick(object sender, EventArgs e)
        {
            string cmd = String.Format("stash {0} {1}",
                                       checkKeepStash.Checked ? "apply" : "pop",
                                       _stash);

            App.Repos.Current.RunCmd(cmd);
        }

        /// <summary>
        /// Remove selected stash from the list of stashes
        /// </summary>
        private void BtRemoveClick(object sender, EventArgs e)
        {
            string cmd = "stash drop " + _stash;
            App.Repos.Current.RunCmd(cmd);

            PopulateStashList();
        }

        /// <summary>
        /// Show more information on the selected stash
        /// </summary>
        private void BtShowClick(object sender, EventArgs e)
        {
            FormShowChangelist formShowChangelist = new FormShowChangelist();
            DialogResult result;
            do
            {
                formShowChangelist.LoadChangelist(_stash);

                // Walk the list of stashes up and down
                result = formShowChangelist.ShowDialog();
                if(result==DialogResult.No)
                {
                    if (listStashes.SelectedIndex < listStashes.Items.Count - 1)
                        listStashes.SelectedIndex++;
                }
                if(result==DialogResult.Yes)
                {
                    if (listStashes.SelectedIndex > 0)
                        listStashes.SelectedIndex--;
                }
            } while (result != DialogResult.Cancel);
        }
    }
}
