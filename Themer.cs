using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

/*
 * need to reference:
 *   System.Windows.Forms
 *   System.Xml
 *   System.Runtime.Serialization
 *   System.ServiceModel.Web
 *   System.Drawing
 */

namespace WinFormThemer
{
    public class WinFormThemer
    {
        private WinFormThemeConfig mConfig = null;

        public WinFormThemer()
        {
            mConfig = WinFormThemeConfig.DefaultDarkTheme;
        }

        public WinFormThemer(Form form, WinFormThemeConfig config)
        {
            mConfig = config;
            ApplyTheme(form, config);
        }

        public WinFormThemer(WinFormThemeConfig config)
        {
            mConfig = config;
        }

        public WinFormThemer(Form form)
        {
            mConfig = WinFormThemeConfig.DefaultDarkTheme;
            ApplyTheme(form, mConfig);
        }

        public void ApplyTheme(Form form)
        {
            if (mConfig == null)
                mConfig = WinFormThemeConfig.DefaultDarkTheme;
            ApplyTheme(form, mConfig);
        }
        public void ApplyTheme(Form form, WinFormThemeConfig config)
        {
            form.BackColor = config.formBackgroundColor;
            form.ForeColor = config.formForegroundColor;

            ApplyThemeToChildren(form, config);
            if (config.darkTitleBar)
            {
                form.HandleCreated += Form_HandleCreated;
            }
        }

        private void Form_HandleCreated(object sender, EventArgs e)
        {
            // apply the dark theme to the title bar
            //https://stackoverflow.com/questions/57124243/winforms-dark-title-bar-on-windows-10 (thank you:)
            Form form = sender as Form;
            if (DwmSetWindowAttribute(form.Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(form.Handle, 20, new[] { 1 }, 4);
        }

        [DllImport("DwmApi")] //System.Runtime.InteropServices
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);


        private void ApplyThemeToChildren(Control ctrl, WinFormThemeConfig config)
        {
            foreach (Control c in ctrl.Controls)
            {
                c.BackColor = config.formBackgroundColor;
                c.ForeColor = config.formForegroundColor;

                if (c is TextBox)
                {
                    c.BackColor = config.textBoxBackgroundColor != Color.Empty ? config.formBackgroundColor : c.BackColor;
                    c.ForeColor = config.textBoxForegroundColor != Color.Empty ? config.formForegroundColor : c.ForeColor;
                }
                else if (c is ScrollBar)
                {
                    c.BackColor = config.textBoxBackgroundColor;
                    c.ForeColor = config.textBoxForegroundColor;
                }
                else if (c is GroupBox)
                {
                    c.BackColor = config.textBoxBackgroundColor != Color.Empty ? config.textBoxBackgroundColor : c.BackColor;
                    c.ForeColor = config.textBoxForegroundColor != Color.Empty ? config.textBoxForegroundColor : c.ForeColor;
                }
                else if (c is RadioButton)
                {
                    c.BackColor = config.radioButtonBackgroundColor != Color.Empty ? config.radioButtonBackgroundColor : c.BackColor;
                    c.ForeColor = config.radioButtonForegroundColor != Color.Empty ? config.radioButtonForegroundColor : c.ForeColor;
                }
                else if (c is Button)
                {
                    c.BackColor = config.buttonBackgroundColor != Color.Empty ? config.buttonBackgroundColor : c.BackColor;
                    c.ForeColor = config.buttonForegroundColor != Color.Empty ? config.buttonForegroundColor : c.ForeColor;
                    Button b = c as Button;
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
                }
                else if (c is CheckBox)
                {
                    c.BackColor = config.checkBoxBackgroundColor != Color.Empty ? config.checkBoxBackgroundColor : c.BackColor;
                    c.ForeColor = config.checkBoxForegroundColor != Color.Empty ? config.checkBoxForegroundColor : c.ForeColor;
                }
                else if (c is ListBox)
                {
                    c.BackColor = config.listItemBackgroundColor != Color.Empty ? config.listItemBackgroundColor : c.BackColor;
                    c.ForeColor = config.listItemForegroundColor != Color.Empty ? config.listItemForegroundColor : c.ForeColor;
                    ListBox lb = c as ListBox;
                    lb.DrawMode = DrawMode.OwnerDrawFixed;
                    lb.DrawItem += new DrawItemEventHandler(lb_DrawItem);
                    if (listItemTextBrush == null)
                        listItemTextBrush = new SolidBrush(c.ForeColor);
                    lb.Resize += Lb_Resize;
                }
                else if (c is ComboBox)
                {
                    c.BackColor = config.formBackgroundColor;
                    c.ForeColor = config.formForegroundColor;
                }
                else if (c is TextBoxBase)
                {
                    c.BackColor = config.textBoxBackgroundColor != Color.Empty ? config.textBoxBackgroundColor : c.BackColor;
                    c.ForeColor = config.textBoxForegroundColor != Color.Empty ? config.textBoxForegroundColor : c.ForeColor;
                }
                else if (c is DataGridView)
                {
                    DataGridView dgv = c as DataGridView;
                    //c.BackColor = config.dataGridViewBackgroundColor != Color.Empty ? config.dataGridViewBackgroundColor : c.BackColor;
                    c.ForeColor = config.dataGridViewForegroundColor != Color.Empty ? config.dataGridViewForegroundColor : c.ForeColor;
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    style.BackColor = config.dataGridViewCellBackgroundColor;
                    style.ForeColor = config.dataGridViewForegroundColor;
                    dgv.DefaultCellStyle = style;

                    dgv.EnableHeadersVisualStyles = false;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = config.dataGridViewCellBackgroundColor;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = config.dataGridViewForegroundColor;

                    dgv.GridColor = config.formBackgroundColor;
                    dgv.BackgroundColor = config.dataGridViewBackgroundColor;
                }
                else if (c is MenuStrip)
                {
                    c.BackColor = config.menuBackgroundColor != Color.Empty ? config.menuBackgroundColor : c.BackColor;
                    c.ForeColor = config.menuForegroundColor != Color.Empty ? config.menuForegroundColor : c.ForeColor;
                    MenuStrip ms = c as MenuStrip;
                    ApplyMenuItemTheme(ms);
                    ms.Paint += ContextMenuStrip_Paint;
                }
                else if (c is TabControl)
                {
                    c.BackColor = config.formBackgroundColor;
                    c.ForeColor = config.formForegroundColor;
                    //TabControl tc = c as TabControl;
                    //tc.Paint += Tc_Paint;
                    // Tab control is not easily customizable. Looks like you must override OnMessage  to make it more awesome.
                }

                if (c.ContextMenuStrip != null)
                {
                    c.ContextMenuStrip.BackColor = config.menuBackgroundColor != Color.Empty ? config.menuBackgroundColor : c.ContextMenuStrip.BackColor;
                    c.ContextMenuStrip.ForeColor = config.menuForegroundColor != Color.Empty ? config.menuForegroundColor : c.ContextMenuStrip.ForeColor;
                    ApplyMenuItemTheme(c.ContextMenuStrip);
                    c.ContextMenuStrip.Paint += ContextMenuStrip_Paint;
                }
                if (c.Controls.Count > 0)
                {
                    ApplyThemeToChildren(c, config);
                }
            }
        }

        private SolidBrush mMenuBackgrooundBrush = null;
        private SolidBrush MenuBackgrooundBrush
        {
            get
            {
                if (mMenuBackgrooundBrush == null)
                    mMenuBackgrooundBrush = new SolidBrush(mConfig.formBackgroundColor);
                return mMenuBackgrooundBrush;
            }
        }
        private void ContextMenuStrip_Paint(object sender, PaintEventArgs e)
        {
            ToolStrip menu = sender as ToolStrip;
            //e.Graphics.DrawRectangle(Pens.Black, 0, 0, 20, 80);
            e.Graphics.FillRectangle(MenuBackgrooundBrush, 0, 0, menu.Width, menu.Height);
        }

        private void Lb_Resize(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;
            lb.Invalidate();
        }

        private void ApplyMenuItemTheme(ToolStrip strip)
        {
            if (mConfig.menuBackgroundColor != Color.Empty)
            {
                ToolStripMenuItem dude = null;
                foreach (ToolStripItem item in strip.Items)
                {
                    //if (item.Image == null)
                    {
                        //item.Image = mMenuImge;
                        item.DisplayStyle = ToolStripItemDisplayStyle.Text;
                        item.BackColor = mConfig.menuBackgroundColor;
                        item.ForeColor = mConfig.menuForegroundColor;
                    }
                    dude = item as ToolStripMenuItem;
                    if (dude != null)
                    {
                        ToolStripMenuItem thing = null;
                        for (int i = 0; i < dude.DropDownItems.Count; i++)
                        //foreach( ToolStripMenuItem thing in dude.DropDownItems)
                        {
                            thing = dude.DropDownItems[i] as ToolStripMenuItem;
                            if (thing != null)
                            {
                                thing.BackColor = mConfig.menuBackgroundColor;
                                thing.ForeColor = mConfig.menuForegroundColor;
                            }
                        }
                    }
                }
            }
        }

        private SolidBrush listItemTextBrush = null;// assigned when a listbox is encountered.
        void lb_DrawItem(object sender, DrawItemEventArgs e)
        {
            //https://stackoverflow.com/questions/91747/background-color-of-a-listbox-item-windows-forms (thank you :)
            ListBox lb = sender as ListBox;
            if (e.Index < 0) return;
            //if the item state is selected them change the back color 
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          mConfig.SelectionColor);

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip);

            // Draw the current item text
            e.Graphics.DrawString(lb.Items[e.Index].ToString(), e.Font, listItemTextBrush, e.Bounds,
                fmt);
            //StringFormat.GenericTypographic);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }
    }

    [DataContract]
    public class WinFormThemeConfig
    {
        [DataMember]
        public string formBackground;
        [DataMember]
        public string formForeground;
        [DataMember]
        public bool darkTitleBar = false;
        [DataMember]
        public string buttonBackground;
        [DataMember]
        public string buttonForeground;
        [DataMember]
        public string radioButtonBackground;
        [DataMember]
        public string radioButtonForeground;
        [DataMember]
        public string checkBoxBackground;
        [DataMember]
        public string checkBoxForeground;
        [DataMember]
        public string listItemBackground;
        [DataMember]
        public string listItemForeground;
        [DataMember]
        public string listSelection;
        [DataMember]
        public string dataGridViewCellBackground;
        [DataMember]
        public string dataGridViewBackground;
        [DataMember]
        public string dataGridViewForeground;
        [DataMember]
        public string textBoxBackground;
        [DataMember]
        public string textBoxForeground;
        [DataMember]
        public string menuBackground;
        [DataMember]
        public string menuForeground;
        [DataMember]
        public string selection;
        [DataMember]
        public string selectionText;

        public Color formBackgroundColor { get { return GetColor(formBackground); } }
        public Color formForegroundColor { get { return GetColor(formForeground); } }
        public Color buttonBackgroundColor { get { return GetColor(buttonBackground); } }
        public Color buttonForegroundColor { get { return GetColor(buttonForeground); } }
        public Color radioButtonBackgroundColor { get { return GetColor(radioButtonBackground); } }
        public Color radioButtonForegroundColor { get { return GetColor(radioButtonForeground); } }
        public Color checkBoxBackgroundColor { get { return GetColor(checkBoxBackground); } }
        public Color checkBoxForegroundColor { get { return GetColor(checkBoxForeground); } }
        public Color listItemBackgroundColor { get { return GetColor(listItemBackground); } }
        public Color listItemForegroundColor { get { return GetColor(listItemForeground); } }
        public Color SelectionColor
        {
            get
            {
                Color retVal = GetColor(selection);
                if (retVal == Color.Empty)
                    Color.FromKnownColor(KnownColor.HighlightText);
                return retVal;
            }
        }
        public Color SelectionTextColor
        {
            get
            {
                Color retVal = GetColor(selectionText);
                if (retVal == Color.Empty)
                    Color.FromKnownColor(KnownColor.HighlightText);
                return retVal;
            }
        }
        public Color dataGridViewCellBackgroundColor { get { return GetColor(dataGridViewCellBackground); } }
        public Color dataGridViewBackgroundColor { get { return GetColor(dataGridViewBackground); } }
        public Color dataGridViewForegroundColor { get { return GetColor(dataGridViewForeground); } }
        public Color textBoxBackgroundColor { get { return GetColor(textBoxBackground); } }
        public Color textBoxForegroundColor { get { return GetColor(textBoxForeground); } }
        public Color menuBackgroundColor { get { return GetColor(menuBackground); } }
        public Color menuForegroundColor { get { return GetColor(menuForeground); } }

        private Dictionary<string, Color> colors = new Dictionary<string, Color>(16);

        public static WinFormThemeConfig FromFile(string file)
        {
            WinFormThemeConfig retVal = null;
            using (System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Open))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WinFormThemeConfig));
                retVal = (WinFormThemeConfig)ser.ReadObject(fs);
            }
            return retVal;
        }

        public static string ToJson(WinFormThemeConfig config)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WinFormThemeConfig));
            StringBuilder builder = new StringBuilder();
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, config);
            String retVal = System.Text.Encoding.Default.GetString(ms.ToArray());
            return retVal;
        }


        private Color GetColor(string colorString)
        {
            if (String.IsNullOrEmpty(colorString))
                return Color.Empty;
            if (colors == null)
                colors = new Dictionary<string, Color>();
            if (!colors.ContainsKey(colorString))
            {
                Color c = ParseColor(colorString);
                colors.Add(colorString, c);
            }
            return colors[colorString];
        }

        /// <summary>
        /// Returns a Color from the given string.
        /// Colors can be specified as "#FFCC66" numbers or "0xFFCC66" numbers
        /// or names from the 'System.Drawing.KnownColor' enumeration
        /// </summary>
        private Color ParseColor(string val)
        {
            Color retVal = Color.Empty;
            if (val.StartsWith("0x") || val.StartsWith("0X"))
            {
                string dude = val.ToUpper().Replace("0X", "#");
                retVal = System.Drawing.ColorTranslator.FromHtml(dude);
            }
            else if (val.StartsWith("#"))
            {
                retVal = System.Drawing.ColorTranslator.FromHtml(val);
            }
            else
            {
                retVal = Color.FromName(val);
            }
            return retVal;
        }

        public static WinFormThemeConfig DefaultDarkTheme
        {
            get
            {
                WinFormThemeConfig retVal = new WinFormThemeConfig();
                retVal.formBackground = "#1A1A1A";
                retVal.formForeground = "White";
                retVal.darkTitleBar = true;
                retVal.buttonBackground = "#1A1A1A";
                retVal.buttonForeground = "White";
                retVal.radioButtonBackground = "#1A1A1A";
                retVal.radioButtonForeground = "White";
                retVal.checkBoxBackground = "#1A1A1A";
                retVal.checkBoxForeground = "White";
                retVal.listItemBackground = "#1A1A1A";
                retVal.listItemForeground = "White";
                retVal.selection = "#403582";
                retVal.selectionText = "#403582";
                retVal.dataGridViewCellBackground = "#1A1A1A";
                retVal.dataGridViewBackground = "#252526";
                retVal.dataGridViewForeground = "White";
                retVal.textBoxBackground = "#1A1A1A";
                retVal.textBoxForeground = "White";
                retVal.menuBackground = "#1A1A1A";
                retVal.menuForeground = "White";
                return retVal;
            }
        }
    }

    public class MessageForm : Form
    {
        private static WinFormThemer mDefaultThemer = null;
        public static WinFormThemer DefaultThemer
        {
            get
            {
                if (mDefaultThemer == null)
                {
                    mDefaultThemer = new WinFormThemer();
                }
                return mDefaultThemer;
            }
        }

        public MessageForm()
        {
            InitializeComponent();
            DefaultThemer.ApplyTheme(this);
        }

        public static DialogResult ShowMessage(string message)
        {
            return ShowMessage(message, "", false, SystemIcons.Information);
        }

        public static DialogResult ShowMessage(string message, string title)
        {
            return ShowMessage(message, title, false, SystemIcons.Information);
        }

        public static DialogResult ShowMessage(string message, string title, bool showCancel, Icon icon)
        {
            MessageForm mf = new MessageForm();
            mf.Icon = icon;
            mf.btnCancel.Visible = showCancel;
            mf.Text = title;
            mf.textMessage.Text = message;
            mf.textMessage.ReadOnly = true;
            if (!String.IsNullOrEmpty(message))
            {
                Bitmap bm = new Bitmap(1, 1);
                Graphics g = Graphics.FromImage(bm);
                var size = g.MeasureString(message, mf.textMessage.Font);
                bm.Dispose();
                g.Dispose();
                var w = 20 + size.Width;
                var h = 60 + size.Height;
                if (w > Screen.PrimaryScreen.Bounds.Width)
                    w = Screen.PrimaryScreen.Bounds.Width;
                if (h > Screen.PrimaryScreen.Bounds.Height)
                    h = Screen.PrimaryScreen.Bounds.Height;
                if (w > mf.Width)
                    mf.Width = (int)w;
                if (h > mf.Height)
                    mf.Height = (int)h;
            }
            DialogResult retVal = mf.ShowDialog();
            mf.Dispose();
            return retVal;
        }

        public static String GetString(string initialText, string title)
        {
            String retVal = null;
            MessageForm mf = new MessageForm();
            mf.Icon = SystemIcons.Question;
            mf.btnCancel.Visible = true;
            mf.Text = title;
            mf.textMessage.Text = initialText;
            if (mf.ShowDialog() == DialogResult.OK)
            {
                retVal = mf.textMessage.Text;
            }
            mf.Dispose();
            return retVal;
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textMessage = new System.Windows.Forms.RichTextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textMessage
            // 
            this.textMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textMessage.Location = new System.Drawing.Point(12, 12);
            this.textMessage.Name = "textMessage";
            this.textMessage.Size = new System.Drawing.Size(382, 175);
            this.textMessage.TabIndex = 0;
            this.textMessage.Text = "";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(319, 197);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(228, 197);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // MessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 232);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.textMessage);
            this.Name = "MessageForm";
            this.Text = "Title";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox textMessage;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
