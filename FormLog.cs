﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GitForce
{
    /// <summary>
    /// Implements the Log form.
    /// This form is never closed but is only shown or hidden as requested.
    /// </summary>
    public partial class FormLog : Form
    {
        public FormLog()
        {
            InitializeComponent();
            ClassWinGeometry.Restore(this);

            // WAR: On Linux, remove status bar resizing grip (since it does not work under X)
            if (ClassUtils.IsMono())
                statusStrip.SizingGrip = false;

            // Reuse the same font selected as fixed-pitch
            textBox.Font = Properties.Settings.Default.commitFont;

            // Debug build starts with the log window open!
            Debug("Debug build.");
#if DEBUG
            Properties.Settings.Default.ShowLogWindow = true;
#endif
        }

        /// <summary>
        /// Form is closing.
        /// </summary>
        private void FormLogFormClosing(object sender, FormClosingEventArgs e)
        {
            ClassWinGeometry.Save(this);
        }

        /// <summary>
        /// Form helper function that shows or hides a form
        /// </summary>
        public void ShowWindow(bool toShow)
        {
            if( Visible==false && toShow)
                Show();

            if(Visible && toShow==false)
                Hide();
        }

        /// <summary>
        /// Adds a text string to the end of the text box.
        /// For performance reasons, only up to 120 characters of text are added in one call.
        /// This is a thread-safe call.
        /// </summary>
        public void Print(string text)
        {
            if(textBox.InvokeRequired)
                textBox.BeginInvoke((MethodInvoker)(() => Print(text)));
            else
            {
                int len = Math.Min(text.Length, 120);
                textBox.Text += text.Substring(0, len).Trim() + Environment.NewLine;

                // Scroll to the bottom and move carret position
                textBox.SelectionStart = textBox.TextLength;
                textBox.ScrollToCaret();                
            }
        }

        /// <summary>
        /// Prints a message only in debug build
        /// This is a thread-safe call.
        /// </summary>
        [Conditional("DEBUG")]
        public void Debug(string text)
        {
            Print(text);
        }

        #region Context menu handlers: Copy, Select All and Clear

        private void CopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            textBox.Copy();
        }

        private void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            textBox.SelectAll();
        }

        private void ClearToolStripMenuItemClick(object sender, EventArgs e)
        {
            textBox.Clear();
        }

        #endregion
    }
}
