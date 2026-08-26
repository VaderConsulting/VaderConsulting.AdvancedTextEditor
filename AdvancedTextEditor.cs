using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VaderConsulting
{
    [Serializable()]
    public partial class AdvancedTextEditor : UserControl
    {
        private delegate void PrintDialogHelperDelegate(); // Helper delegate for PrintDialog bug

        private int _CheckPrint = 0;
        private string _Path = "";

        public AdvancedTextEditor()
        {
            InitializeComponent();

            this.mnuRuler.Checked = true;
            this.mnuMainToolbar.Checked = true;
            this.mnuFormatting.Checked = true;

            System.Drawing.Text.InstalledFontCollection col = new System.Drawing.Text.InstalledFontCollection();

            this.cmbFontName.Items.Clear();

            foreach (FontFamily ff in col.Families)
            {
                this.cmbFontName.Items.Add(ff.Name);
            }

            col.Dispose();

            this._Editor.Select(0, 0);
            this._Ruler.LeftMargin = 19;
            this._Ruler.LeftIndent = 19;
            this._Ruler.LeftHangingIndent = 19;
            this._Ruler.RightIndent = 19;
            this._Ruler.RightMargin = 19;
            this._Editor.SelectionIndent = 0;
            this._Editor.SelectionRightIndent = 0;
            this._Editor.SelectionHangingIndent = 0;
        }

        #region Properties

        [
        Category("Components"),
        Description("Ruler Sub-Component")
        ]
        public TextRuler Ruler
        {
            get
            {
                return _Ruler;
            }
            set
            {
                _Ruler = value;
            }
        }

        [
        Category("Components"),
        Description("Editor Sub-Component")
        ]

        public ExtendedRichTextBox Editor
        {
            get
            {
                return _Editor;
            }
            set
            {
                _Editor = value;
            }
        }

        //public new Font SelectionFont
        //{
        //    get
        //    {
        //        return Editor.SelectionFont;
        //    }

        //    set
        //    {
        //        Editor.SelectionFont = value;
        //    }
        //}

        #endregion

        #region Event Handlers

        private void AdvancedTextEditor_Load(object sender, EventArgs e)
        {
            //code below will cause refreshing formatting by adding and removing (changing) text
            this._Editor.Select(0, 0);
            this._Editor.AppendText("some text");
            this._Editor.Select(0, 0);
            this._Editor.Clear();
            this._Editor.SetLayoutType(ExtendedRichTextBox.LayoutModes.WYSIWYG);
        }

        private void btnAlignCenter_Click(object sender, EventArgs e)
        {
            PerformAlignCenter();
        }

        private void btnAlignLeft_Click(object sender, EventArgs e)
        {
            PerformAlignLeft();
        }

        private void btnAlignRight_Click(object sender, EventArgs e)
        {
            PerformAlignRight();
        }

        private void btnBold_Click(object sender, EventArgs e)
        {
            PerformBold(this._Editor.SelectionCharStyle.Bold);
        }

        private void btnBulletedList_Click(object sender, EventArgs e)
        {
            PerformBulletedList(this.btnBulletedList.Checked);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            PerformCopy();
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            PerformCut();
        }

        private void btnItalic_Click(object sender, EventArgs e)
        {
            PerformItalic(this._Editor.SelectionCharStyle);
        }

        private void btnJustify_Click(object sender, EventArgs e)
        {
            PerformJustify();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PerformFileNew();
        }

        private void btnNumberedList_Click(object sender, EventArgs e)
        {
            PerformNumberedList(this.btnNumberedList.Checked);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            PerformOpen();
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            PerformPaste();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PerformPrint();
        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            PerformPrintPreview();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            PerformRedo();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            PerformSave(false);
        }

        private void btnStrikeThrough_Click(object sender, EventArgs e)
        {
            PerformStrikeThrough(this._Editor.SelectionCharStyle);
        }

        private void btnUnderline_Click(object sender, EventArgs e)
        {
            PerformUnderline(this._Editor.SelectionCharStyle);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            PerformUndo();
        }

        private void cmbDateTimeFormats_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbDateTimeFormats.SelectedIndex == 0)
            {
                return;
            }

            PerformChangeDateTimeFormat(this.cmbDateTimeFormats.SelectedItem.ToString());
        }

        private void cmbFontName_KeyUp(object sender, KeyEventArgs e)
        {
            PerformFontChangeByKeyUp(e, this.cmbFontName.Text, Convert.ToInt32(this.cmbFontSize.Text));
        }

        private void cmbFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.cmbFontName.Focused) return;
            PerformChangeFontName(this.cmbFontName.Text, Convert.ToInt32(this.cmbFontSize.Text));
        }

        private void cmbFontSize_KeyDown(object sender, KeyEventArgs e)
        {
            PerformFontChangeKeyDown(e);
        }

        private void cmbFontSize_KeyUp(object sender, KeyEventArgs e)
        {
            PerformFontChangeKeyUp(e);
        }

        private void cmbFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this.cmbFontSize.Focused) return;
                PerformChangeFontNameAndSize(this.cmbFontName.Text, Convert.ToInt32(this.cmbFontSize.Text), this._Editor.SelectionFont.Style);
            }
            catch (Exception)
            {

            }
        }

        private void mnuCopy_Click(object sender, EventArgs e)
        {
            PerformCopy();
        }

        private void mnuCut_Click(object sender, EventArgs e)
        {
            PerformCut();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            PerformExit();
        }

        private void mnuFind_Click(object sender, EventArgs e)
        {
            PerformFind();
        }

        private void mnuFont_Click(object sender, EventArgs e)
        {
            PerformChangeFont(this._Editor.SelectionFont);
        }

        private void mnuFormatting_Click(object sender, EventArgs e)
        {
            PerformFormattingMenuClick();
        }

        private void mnuHighlightColor_Click(object sender, EventArgs e)
        {
            PerformHighlightMenuClick();
        }

        private void mnuInsertDateTime_DropDownOpening(object sender, EventArgs e)
        {
            PerformInsertDateTimeClick();
        }

        private void mnuInsertPicture_Click(object sender, EventArgs e)
        {
            PerformInsertPictureMenuClick();
        }

        private void mnuMainToolbar_Click(object sender, EventArgs e)
        {
            PerformMainToolbarMenuClick();
        }

        private void mnuNew_Click(object sender, EventArgs e)
        {
            PerformFileNew();
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            PerformOpen();
        }

        private void mnuPageSettings_Click(object sender, EventArgs e)
        {
            PerformPageSettingsMenuClick();
        }

        private void mnuPaste_Click(object sender, EventArgs e)
        {
            PerformPaste();
        }

        private void mnuPrint_Click(object sender, EventArgs e)
        {
            PerformPrint();
            //this.PrintWnd.ShowDialog(this);
        }

        private void mnuPrintPreview_Click(object sender, EventArgs e)
        {
            PerformPrintPreview();
        }

        private void mnuRedo_Click(object sender, EventArgs e)
        {
            PerformRedo();
        }

        private void mnuRuler_Click(object sender, EventArgs e)
        {
            PerformRulerMenuClick();
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            PerformSave(false);
            //Save(false);
        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            PerformSave(true);
        }

        private void mnuTextColor_Click(object sender, EventArgs e)
        {
            PerformTextColorMenuClick();
        }

        private void mnuULineSolid_Click(object sender, EventArgs e)
        {
            PerformDashDotDotUnderlineMenuClick();
        }

        private void mnuULWave_Click(object sender, EventArgs e)
        {
            PerformWavyUnderlineMenuClick();
        }

        private void mnuUndo_Click(object sender, EventArgs e)
        {
            PerformUndo();
        }

        private void prtDoc_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            _CheckPrint = 0;
        }

        private void prtDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            _CheckPrint = this._Editor.Print(_CheckPrint, this._Editor.TextLength, e);

            if (_CheckPrint < this._Editor.TextLength)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;
        }

        private void Ruler_BothLeftIndentsChanged(int LeftIndent, int HangIndent)
        {
            this._Editor.SelectionIndent = (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
            this._Editor.SelectionHangingIndent = (int)(this.Ruler.LeftHangingIndent * this.Ruler.DotsPerMillimeter) - (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
        }

        private void Ruler_LeftHangingIndentChanging(int NewValue)
        {
            try
            {
                this._Editor.SelectionHangingIndent = (int)(this.Ruler.LeftHangingIndent * this.Ruler.DotsPerMillimeter) - (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_LeftIndentChanging(int NewValue)
        {
            try
            {
                this._Editor.SelectionIndent = (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
                this._Editor.SelectionHangingIndent = (int)(this.Ruler.LeftHangingIndent * this.Ruler.DotsPerMillimeter) - (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_RightIndentChanging(int NewValue)
        {
            try
            {
                this._Editor.SelectionRightIndent = (int)(this.Ruler.RightIndent * this.Ruler.DotsPerMillimeter);
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_TabAdded(TextRuler.TabEventArgs args)
        {
            try
            {
                this._Editor.SelectionTabs = this.Ruler.TabPositionsInPixels.ToArray();
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_TabChanged(TextRuler.TabEventArgs args)
        {
            try
            {
                this._Editor.SelectionTabs = this.Ruler.TabPositionsInPixels.ToArray();
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_TabRemoved(TextRuler.TabEventArgs args)
        {
            try
            {
                this._Editor.SelectionTabs = this.Ruler.TabPositionsInPixels.ToArray();
            }
            catch (Exception)
            {
            }
        }

        private void TextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.B && e.Control == true)
            {
                this.btnBold.PerformClick();
            }

            if (e.Control == true && e.KeyCode == Keys.I)
            {
                this.btnItalic.PerformClick();
                e.SuppressKeyPress = true;
            }

            if (e.Control == true && e.KeyCode == Keys.U)
            {
                this.btnUnderline.PerformClick();
            }
        }

        private void TextEditor_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(e.LinkText);
            }
            catch (Exception)
            {
            }
        }

        private void TextEditor_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void TextEditor_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this._Editor.SelectionType == RichTextBoxSelectionTypes.Object ||
                    this._Editor.SelectionType == RichTextBoxSelectionTypes.MultiObject)
                {
                    MessageBox.Show(Convert.ToString(this._Editor.SelectedObject().sizel.Width));
                }
            }
        }

        private void TextEditor_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                #region Alignment
                if (_Editor.SelectionAlignment == ExtendedRichTextBox.RichTextAlign.Left)
                {
                    this.btnAlignLeft.Checked = true;
                    this.btnAlignCenter.Checked = false;
                    this.btnAlignRight.Checked = false;
                    this.btnJustify.Checked = false;
                }
                else if (_Editor.SelectionAlignment == ExtendedRichTextBox.RichTextAlign.Center)
                {
                    this.btnAlignLeft.Checked = false;
                    this.btnAlignCenter.Checked = true;
                    this.btnAlignRight.Checked = false;
                    this.btnJustify.Checked = false;
                }
                else if (_Editor.SelectionAlignment == ExtendedRichTextBox.RichTextAlign.Right)
                {
                    this.btnAlignLeft.Checked = false;
                    this.btnAlignCenter.Checked = false;
                    this.btnAlignRight.Checked = true;
                    this.btnJustify.Checked = false;
                }
                else if (_Editor.SelectionAlignment == ExtendedRichTextBox.RichTextAlign.Justify)
                {
                    this.btnAlignLeft.Checked = false;
                    this.btnAlignRight.Checked = false;
                    this.btnAlignCenter.Checked = false;
                    this.btnJustify.Checked = true;
                }
                else
                {
                    this.btnAlignLeft.Checked = true;
                    this.btnAlignCenter.Checked = false;
                    this.btnAlignRight.Checked = false;
                }

                #endregion

                #region Tab positions
                this.Ruler.SetTabPositionsInPixels(this._Editor.SelectionTabs);
                #endregion

                #region Font
                try
                {
                    if (this._Editor.SelectionFont2.Size == 0)
                    {
                        this.cmbFontSize.Text = "0";
                    }
                    else
                    {
                        this.cmbFontSize.Text = Convert.ToInt32(this._Editor.SelectionFont2.Size).ToString();
                    }

                }
                catch
                {
                    this.cmbFontSize.Text = "0";
                }

                try
                {
                    this.cmbFontName.Text = this._Editor.SelectionFont2.Name;
                }
                catch
                {
                    this.cmbFontName.Text = "0";
                }

                if (this.cmbFontName.Text != "0")
                {
                    FontFamily ff = new FontFamily(this.cmbFontName.Text);
                    if (ff.IsStyleAvailable(FontStyle.Bold) == true)
                    {
                        this.btnBold.Enabled = true;
                        this.btnBold.Checked = this._Editor.SelectionCharStyle.Bold;
                    }
                    else
                    {
                        this.btnBold.Enabled = false;
                        this.btnBold.Checked = false;
                    }

                    if (ff.IsStyleAvailable(FontStyle.Italic) == true)
                    {
                        this.btnItalic.Enabled = true;
                        this.btnItalic.Checked = this._Editor.SelectionCharStyle.Italic;
                    }
                    else
                    {
                        this.btnItalic.Enabled = false;
                        this.btnItalic.Checked = false;
                    }

                    if (ff.IsStyleAvailable(FontStyle.Underline) == true)
                    {
                        this.btnUnderline.Enabled = true;
                        this.btnUnderline.Checked = this._Editor.SelectionCharStyle.Underline;
                    }
                    else
                    {
                        this.btnUnderline.Enabled = false;
                        this.btnUnderline.Checked = false;
                    }

                    if (ff.IsStyleAvailable(FontStyle.Strikeout) == true)
                    {
                        this.btnStrikeThrough.Enabled = true;
                        this.btnStrikeThrough.Checked = this._Editor.SelectionCharStyle.Strikeout;
                    }
                    else
                    {
                        this.btnStrikeThrough.Enabled = false;
                        this.btnStrikeThrough.Checked = false;
                    }

                    ff.Dispose();
                }
                else
                {
                    this.btnBold.Checked = false;
                    this.btnItalic.Checked = false;
                    this.btnUnderline.Checked = false;
                    this.btnStrikeThrough.Checked = false;
                }
                #endregion

                if (this._Editor.SelectionLength < this._Editor.TextLength - 1)
                {
                    this.Ruler.LeftIndent = (int)(this._Editor.SelectionIndent / this.Ruler.DotsPerMillimeter); //convert pixels to millimeter

                    this.Ruler.LeftHangingIndent = (int)((float)this._Editor.SelectionHangingIndent / this.Ruler.DotsPerMillimeter) + this.Ruler.LeftIndent; //convert pixels to millimeters

                    this.Ruler.RightIndent = (int)(this._Editor.SelectionRightIndent / this.Ruler.DotsPerMillimeter); //convert pixels to millimeters                
                }

                switch (this._Editor.SelectionListType.Type)
                {
                    case ExtendedRichTextBox.ParaListStyle.ListType.None:
                        this.btnNumberedList.Checked = false;
                        this.btnBulletedList.Checked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.SmallLetters:
                        this.btnNumberedList.Checked = false;
                        this.btnBulletedList.Checked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.CapitalLetters:
                        this.btnNumberedList.Checked = false;
                        this.btnBulletedList.Checked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.SmallRoman:
                        this.btnNumberedList.Checked = false;
                        this.btnBulletedList.Checked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.CapitalRoman:
                        this.btnNumberedList.Checked = false;
                        this.btnBulletedList.Checked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.Bullet:
                        this.btnNumberedList.Checked = false;
                        this.btnBulletedList.Checked = true;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.Numbers:
                        this.btnNumberedList.Checked = true;
                        this.btnBulletedList.Checked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.CharBullet:
                        this.btnNumberedList.Checked = true;
                        this.btnBulletedList.Checked = false;
                        break;
                    default:
                        break;
                }

                this._Editor.UpdateObjects();
            }
            catch (Exception)
            {
            }
        }

        private void txtCustom_Enter(object sender, EventArgs e)
        {
            txtCustom.Text = "";
        }

        private void txtCustom_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.txtCustom.Text == "")
                {
                    return;
                }

                try
                {
                    this._Editor.AppendText(Environment.NewLine + DateTime.Now.ToString(txtCustom.Text));
                }
                catch (Exception)
                {
                }
                txtCustom.Text = "specify custom date/time format";
                this.mnuInsert.DropDown.Close();
            }
        }

        private void txtCustom_Leave(object sender, EventArgs e)
        {
            txtCustom.Text = "specify custom date/time format";
            this.mnuInsert.DropDown.Close();
        }

        private void txtCustom_MouseDown(object sender, MouseEventArgs e)
        {
            txtCustom.Text = "";
        }

#endregion

        #region Old style formatting

        private FontStyle SwitchBold()
        {
            FontStyle fs = new FontStyle();

            fs = FontStyle.Regular;

            if (this._Editor.SelectionFont.Italic == true)
            {
                fs = FontStyle.Italic;
            }

            if (this._Editor.SelectionFont.Underline == true)
            {
                fs = fs | FontStyle.Underline;
            }

            if (this._Editor.SelectionFont.Strikeout == true)
            {
                fs = fs | FontStyle.Strikeout;
            }

            if (this._Editor.SelectionFont.Bold == false)
            {
                fs = fs | FontStyle.Bold;
            }

            return fs;
        }

        private FontStyle SwitchItalic()
        {
            FontStyle fs = new FontStyle();

            fs = FontStyle.Regular;

            if (this._Editor.SelectionFont.Underline == true)
            {
                fs = fs | FontStyle.Underline;
            }

            if (this._Editor.SelectionFont.Strikeout == true)
            {
                fs = fs | FontStyle.Strikeout;
            }

            if (this._Editor.SelectionFont.Bold == true)
            {
                fs = fs | FontStyle.Bold;
            }

            if (this._Editor.SelectionFont.Italic == false)
            {
                fs = fs | FontStyle.Italic;
            }

            return fs;
        }

        private FontStyle SwitchStrikeout()
        {
            FontStyle fs = new FontStyle();

            fs = FontStyle.Regular;

            if (this._Editor.SelectionFont.Bold == true)
            {
                fs = fs | FontStyle.Bold;
            }

            if (this._Editor.SelectionFont.Italic == true)
            {
                fs = fs | FontStyle.Italic;
            }

            if (this._Editor.SelectionFont.Underline == true)
            {
                fs = fs | FontStyle.Underline;
            }

            if (this._Editor.SelectionFont.Strikeout == false)
            {
                fs = fs | FontStyle.Strikeout;
            }

            return fs;
        }

        private FontStyle SwitchUnderline()
        {
            FontStyle fs = new FontStyle();

            fs = FontStyle.Regular;

            if (this._Editor.SelectionFont.Strikeout == true)
            {
                fs = fs | FontStyle.Strikeout;
            }

            if (this._Editor.SelectionFont.Bold == true)
            {
                fs = fs | FontStyle.Bold;
            }

            if (this._Editor.SelectionFont.Italic == true)
            {
                fs = fs | FontStyle.Italic;
            }

            if (this._Editor.SelectionFont.Underline == false)
            {
                fs = fs | FontStyle.Underline;
            }

            return fs;
        }

#endregion

        #region Private Methods

        private void Clear()
        {
            _Path = "";
            this._Editor.Clear();

            //set indents to default positions
            this._Editor.Select(0, 0);
            this.Ruler.LeftIndent = 0;
            this.Ruler.LeftHangingIndent = 0;
            this.Ruler.RightIndent = 0;
            this._Editor.SelectionIndent = 0;
            this._Editor.SelectionRightIndent = 0;
            this._Editor.SelectionHangingIndent = 0;

            //clear tabs on the ruler
            this.Ruler.SetTabPositionsInPixels(null);
            this._Editor.SelectionTabs = null;

            ExtendedRichTextBox.ParaListStyle pls = new ExtendedRichTextBox.ParaListStyle();

            pls.Type = ExtendedRichTextBox.ParaListStyle.ListType.None;
            pls.Style = ExtendedRichTextBox.ParaListStyle.ListStyle.NumberAndParenthesis;

            this._Editor.SelectionListType = pls;
        }

        private Color GetColor(Color initColor)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = initColor;
                cd.AllowFullOpen = true;
                cd.AnyColor = true;
                cd.FullOpen = true;
                cd.ShowHelp = false;
                cd.SolidColorOnly = false;
                if (cd.ShowDialog() == DialogResult.OK)
                    return cd.Color;
                else
                    return initColor;
            }
        }

        private string GetFilePath()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Multiselect = false;
            o.RestoreDirectory = true;
            o.ShowReadOnly = false;
            o.ReadOnlyChecked = false;
            o.Filter = "RTF (*.rtf)|*.rtf|TXT (*.txt)|*.txt";
            if (o.ShowDialog(this) == DialogResult.OK)
            {
                return o.FileName;
            }
            else
            {
                return "";
            }
        }

        private Font GetFont(Font initFont)
        {
            using (FontDialog fd = new FontDialog())
            {
                fd.Font = initFont;
                fd.AllowSimulations = true;
                fd.AllowVectorFonts = true;
                fd.AllowVerticalFonts = true;
                fd.FontMustExist = true;
                fd.ShowHelp = false;
                fd.ShowEffects = true;
                fd.ShowColor = false;
                fd.ShowApply = false;
                fd.FixedPitchOnly = false;

                if (fd.ShowDialog() == DialogResult.OK)
                    return fd.Font;
                else
                    return initFont;
            }
        }

        private string GetImagePath()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Multiselect = false;
            o.ShowReadOnly = false;
            o.RestoreDirectory = true;
            o.ReadOnlyChecked = false;
            o.Filter = "Images|*.png;*.bmp;*.jpg;*.jpeg;*.gif;*.tif;*.tiff,*.wmf;*.emf";
            if (o.ShowDialog(this) == DialogResult.OK)
            {
                return o.FileName;
            }
            else
            {
                return "";
            }
        }

        private void Open()
        {
            try
            {
                string file = GetFilePath();

                if (file != "")
                {
                    Clear();
                    try
                    {
                        this._Editor.Rtf = System.IO.File.ReadAllText(file, System.Text.Encoding.Default);
                    }
                    catch (Exception) //error occured, that means we loaded invalid RTF, so load as plain text
                    {
                        this._Editor.Text = System.IO.File.ReadAllText(file, System.Text.Encoding.Default);
                    }
                    _Path = file;
                }
                file = null;
            }
            catch (Exception)
            {
                Clear();
            }
        }

        public void PerformFileNew()
        {
            Clear();
        }

        /// <summary>
        /// Shows the print dialog (invoked from a different thread to get the focus to the dialog)
        /// </summary>
        private void PrintDialogHelper()
        {
            if (PrintWnd.ShowDialog(this) == DialogResult.OK)
            {
                this.prtDoc.Print();
            }
        }

        /// <summary>
        /// Helper thread which sole purpose is to invoke PrintDialogHelper function
        /// to circumvent the PrintDialog focus problem reported on
        /// https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=234179
        /// </summary>
        private void PrintHelpThread()
        {
            if (InvokeRequired)
            {
                PrintDialogHelperDelegate d = new PrintDialogHelperDelegate(PrintHelpThread);
                Invoke(d);
            }
            else
            {
                PrintDialogHelper();
            }
        }

        private void Save(bool SaveAs)
        {
            try
            {
                if (SaveAs == true)
                {
                    string file = SetFilePath();

                    if (file != "")
                    {
                        this._Editor.SaveFile(file, RichTextBoxStreamType.RichText);
                        _Path = file;
                        file = null;
                    }
                }
                else
                {
                    if (_Path == "")
                    {
                        string file = SetFilePath();

                        if (file != "")
                        {
                            this._Editor.SaveFile(file, RichTextBoxStreamType.RichText);
                            _Path = file;
                            file = null;
                        }
                    }
                    else
                    {
                        this._Editor.SaveFile(_Path, RichTextBoxStreamType.RichText);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private string SetFilePath()
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "RTF (*.rtf)|*.rtf|TXT (*.txt)|*.txt";
            if (s.ShowDialog(this) == DialogResult.OK)
            {
                return s.FileName;
            }
            else
            {
                return "";
            }
        }

#endregion

        #region Public Methods

        public void PerformAlignCenter()
        {
            this._Editor.SelectionAlignment = ExtendedRichTextBox.RichTextAlign.Center;
            this.btnAlignLeft.Checked = false;
            this.btnAlignRight.Checked = false;
            this.btnAlignCenter.Checked = true;
            this.btnJustify.Checked = false;
        }

        public void PerformAlignLeft()
        {
            this._Editor.SelectionAlignment = ExtendedRichTextBox.RichTextAlign.Left;
            this.btnAlignLeft.Checked = true;
            this.btnAlignRight.Checked = false;
            this.btnAlignCenter.Checked = false;
            this.btnJustify.Checked = false;
        }

        public void PerformAlignRight()
        {
            this._Editor.SelectionAlignment = ExtendedRichTextBox.RichTextAlign.Right;
            this.btnAlignLeft.Checked = false;
            this.btnAlignRight.Checked = true;
            this.btnAlignCenter.Checked = false;
            this.btnJustify.Checked = false;
        }

        public void PerformBold(bool Bold)
        {
            if (Bold == true)
            {
                this.btnBold.Checked = false;
                ExtendedRichTextBox.CharStyle cs = this._Editor.SelectionCharStyle;
                cs.Bold = false;
                this._Editor.SelectionCharStyle = cs;
                cs = null;
            }
            else
            {
                this.btnBold.Checked = true;
                ExtendedRichTextBox.CharStyle cs = this._Editor.SelectionCharStyle;
                cs.Bold = true;
                this._Editor.SelectionCharStyle = cs;
                cs = null;
            }
        }

        public void PerformBulletedList(bool Checked)
        {
            try
            {
                if (Checked)
                {
                    this.btnBulletedList.Checked = false;
                    this.btnNumberedList.Checked = false;
                    ExtendedRichTextBox.ParaListStyle pls = new ExtendedRichTextBox.ParaListStyle();

                    pls.Type = ExtendedRichTextBox.ParaListStyle.ListType.None;
                    pls.Style = ExtendedRichTextBox.ParaListStyle.ListStyle.NumberAndParenthesis;

                    this._Editor.SelectionListType = pls;
                }
                else
                {
                    this.btnBulletedList.Checked = true;
                    this.btnNumberedList.Checked = false;
                    ExtendedRichTextBox.ParaListStyle pls = new ExtendedRichTextBox.ParaListStyle();

                    pls.Type = ExtendedRichTextBox.ParaListStyle.ListType.Bullet;
                    pls.Style = ExtendedRichTextBox.ParaListStyle.ListStyle.NumberAndParenthesis;

                    this._Editor.SelectionListType = pls;
                }
            }
            catch (Exception)
            {
            }
        }

        public void PerformChangeDateTimeFormat(string DateFormat)
        {
            this._Editor.AppendText(Environment.NewLine + DateFormat);
            this.mnuInsert.DropDown.Close();
        }

        public void PerformChangeFont(Font Font)
        {
            try
            {
                this._Editor.SelectionFont2 = GetFont(Font);
            }
            catch (Exception)
            {
            }
        }

        public void PerformChangeFontName(string FontName, Int32 FontSize)
        {
            try
            {
                this._Editor.SelectionFont2 = new Font(FontName, FontSize);
            }
            catch (Exception)
            {
            }
        }

        public void PerformChangeFontNameAndSize(string FontName, Int32 FontSize, FontStyle Style)
        {
            try
            {
                if (FontSize < 1)
                {
                    FontSize = 1;
                }

                this._Editor.SelectionFont2 = new Font(FontName, FontSize, Style);
            }
            catch (Exception)
            {

            }
        }

        public void PerformCopy()
        {
            this._Editor.Copy();
        }

        public void PerformCut()
        {
            this._Editor.Cut();
        }

        public void PerformDashDotDotUnderlineMenuClick()
        {
            this._Editor.SelectionUnderlineStyle = ExtendedRichTextBox.UnderlineStyle.DashDotDot;
        }

        public void PerformExit()
        {
            Environment.Exit(0);
        }

        public void PerformFind()
        {
            dlgFind find = new dlgFind();
            find.txtFindThis.Text = this._Editor.SelectedText;
            find.Caller = this;
            find.Show(this);
        }

        public void PerformFontChangeByKeyUp(KeyEventArgs e, string FontName, Int32 FontSize)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    PerformChangeFontName(FontName, FontSize);
                    this._Editor.Focus();
                }
            }
            catch (Exception)
            {
            }
        }

        public void PerformFontChangeKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.D1 || e.KeyCode == Keys.D2 ||
                e.KeyCode == Keys.D3 || e.KeyCode == Keys.D4 || e.KeyCode == Keys.D5 ||
                e.KeyCode == Keys.D6 || e.KeyCode == Keys.D7 || e.KeyCode == Keys.D8 ||
                e.KeyCode == Keys.D9 || e.KeyCode == Keys.NumPad0 || e.KeyCode == Keys.NumPad1 ||
                e.KeyCode == Keys.NumPad2 || e.KeyCode == Keys.NumPad3 || e.KeyCode == Keys.NumPad4 ||
                e.KeyCode == Keys.NumPad5 || e.KeyCode == Keys.NumPad6 || e.KeyCode == Keys.NumPad7 ||
                e.KeyCode == Keys.NumPad8 || e.KeyCode == Keys.NumPad9 || e.KeyCode == Keys.Back ||
                e.KeyCode == Keys.Enter || e.KeyCode == Keys.Delete)
            {
                //allow key
            }
            else
            {
                e.SuppressKeyPress = true;
            }
        }

        public void PerformFontChangeKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    this._Editor.SelectionFont2 = new Font(this.cmbFontName.Text, Convert.ToSingle(this.cmbFontSize.Text));
                    this._Editor.Focus();
                }
                catch (Exception)
                {
                }
            }
        }

        public void PerformFormattingMenuClick()
        {
            if (this.Formatting.Visible == true)
            {
                this.Formatting.Visible = false;
                this.mnuFormatting.Checked = false;
            }
            else
            {
                this.Formatting.Visible = true;
                this.mnuFormatting.Checked = true;
            }
        }

        public void PerformHighlightMenuClick()
        {
            try
            {
                this._Editor.SelectionBackColor2 = GetColor(this._Editor.SelectionBackColor);
            }
            catch (Exception)
            {
            }
        }

        public void PerformInsertDateTimeClick()
        {
            this.cmbDateTimeFormats.Items.Clear();

            this.cmbDateTimeFormats.Items.Add("Select date/time format");
            this.cmbDateTimeFormats.Items.Add(DateTime.Now.ToString("D"));
            this.cmbDateTimeFormats.Items.Add(DateTime.Now.ToString("f"));
            this.cmbDateTimeFormats.Items.Add(DateTime.Now.ToString("F"));
            this.cmbDateTimeFormats.Items.Add(DateTime.Now.ToString("g"));
            this.cmbDateTimeFormats.Items.Add(DateTime.Now.ToString("G"));
            this.cmbDateTimeFormats.Items.Add(DateTime.Now.ToString("m"));
            this.cmbDateTimeFormats.Items.Add(DateTime.Now.ToString("t"));
            this.cmbDateTimeFormats.Items.Add(DateTime.Now.ToString("T"));

            this.cmbDateTimeFormats.SelectedIndex = 0;
        }

        public void PerformInsertPictureMenuClick()
        {
            string _imgPath = GetImagePath();

            if (_imgPath == "")
                return;
            this._Editor.InsertImage(_imgPath);
        }

        public void PerformItalic(ExtendedRichTextBox.CharStyle CharacterStyle)
        {
            try
            {
                if (CharacterStyle.Italic == true)
                {
                    this.btnItalic.Checked = false;
                    ExtendedRichTextBox.CharStyle cs = CharacterStyle;
                    cs.Italic = false;
                    this._Editor.SelectionCharStyle = cs;
                    cs = null;
                }
                else
                {
                    this.btnItalic.Checked = true;
                    ExtendedRichTextBox.CharStyle cs = CharacterStyle;
                    cs.Italic = true;
                    this._Editor.SelectionCharStyle = cs;
                    cs = null;
                }
            }
            catch (Exception)
            {
            }
        }

        public void PerformJustify()
        {
            this._Editor.SelectionAlignment = ExtendedRichTextBox.RichTextAlign.Justify;
            this.btnAlignLeft.Checked = false;
            this.btnAlignRight.Checked = false;
            this.btnAlignCenter.Checked = false;
            this.btnJustify.Checked = true;
        }

        public void PerformMainToolbarMenuClick()
        {
            if (this.Toolbox.Visible == true)
            {
                this.Toolbox.Visible = false;
                this.mnuMainToolbar.Checked = false;
            }
            else
            {
                this.Toolbox.Visible = true;
                this.mnuMainToolbar.Checked = true;
            }
        }

        public void PerformNumberedList(bool Checked)
        {
            try
            {
                if (Checked)
                {
                    this.btnBulletedList.Checked = false;
                    this.btnNumberedList.Checked = false;
                    ExtendedRichTextBox.ParaListStyle pls = new ExtendedRichTextBox.ParaListStyle();

                    pls.Type = ExtendedRichTextBox.ParaListStyle.ListType.None;
                    pls.Style = ExtendedRichTextBox.ParaListStyle.ListStyle.NumberAndParenthesis;

                    this._Editor.SelectionListType = pls;
                }
                else
                {
                    this.btnBulletedList.Checked = false;
                    this.btnNumberedList.Checked = true;
                    ExtendedRichTextBox.ParaListStyle pls = new ExtendedRichTextBox.ParaListStyle();

                    pls.Type = ExtendedRichTextBox.ParaListStyle.ListType.Numbers;
                    pls.Style = ExtendedRichTextBox.ParaListStyle.ListStyle.NumberInPar;

                    this._Editor.SelectionListType = pls;
                }
            }
            catch (Exception)
            {
            }
        }

        public void PerformOpen()
        {
            Open();
        }

        public void PerformPageSettingsMenuClick()
        {
            this.PageSettings.ShowDialog(this);
        }

        public void PerformPaste()
        {
            this._Editor.Paste();
        }

        public void PerformPrint()
        {
            Thread t = new Thread(PrintHelpThread);
            t.Start();
        }

        public void PerformPrintPreview()
        {
            this.DocPreview.ShowDialog(this);
        }

        public void PerformRedo()
        {
            this._Editor.Redo();
        }

        public void PerformRulerMenuClick()
        {
            if (this.Ruler.Visible == true)
            {
                this.Ruler.Visible = false;
                this.mnuRuler.Checked = false;
            }
            else
            {
                this.Ruler.Visible = true;
                this.mnuRuler.Checked = true;
            }
        }

        public void PerformSave(bool SaveAs)
        {
            Save(SaveAs);
        }

        private void PerformStrikeThrough(ExtendedRichTextBox.CharStyle CharacterStyle)
        {
            try
            {
                if (CharacterStyle.Strikeout == true)
                {
                    this.btnStrikeThrough.Checked = false;
                    ExtendedRichTextBox.CharStyle cs = CharacterStyle;
                    cs.Strikeout = false;
                    this._Editor.SelectionCharStyle = cs;
                    cs = null;
                }
                else
                {
                    this.btnStrikeThrough.Checked = true;
                    ExtendedRichTextBox.CharStyle cs = CharacterStyle;
                    cs.Strikeout = true;
                    this._Editor.SelectionCharStyle = cs;
                    cs = null;
                }
            }
            catch (Exception)
            {
            }
        }

        public void PerformTextColorMenuClick()
        {
            try
            {
                this._Editor.SelectionColor2 = GetColor(this._Editor.SelectionColor);
            }
            catch (Exception)
            {
            }
        }

        public void PerformUnderline(ExtendedRichTextBox.CharStyle CharacterStyle)
        {
            try
            {
                if (CharacterStyle.Underline == true)
                {
                    this.btnUnderline.Checked = false;
                    ExtendedRichTextBox.CharStyle cs = CharacterStyle;
                    cs.Underline = false;
                    this._Editor.SelectionCharStyle = cs;
                    cs = null;
                }
                else
                {
                    this.btnUnderline.Checked = true;
                    ExtendedRichTextBox.CharStyle cs = CharacterStyle;
                    cs.Underline = true;
                    this._Editor.SelectionCharStyle = cs;
                    cs = null;
                }
            }
            catch (Exception)
            {
            }
        }

        public void PerformUndo()
        {
            this._Editor.Undo();
        }

        public void PerformWavyUnderlineMenuClick()
        {
            this._Editor.SelectionUnderlineStyle = ExtendedRichTextBox.UnderlineStyle.Wave;
        }

#endregion

    }
}
