using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace AmongUs_CE_Installer
{
    public class ControlWriter : TextWriter
    {
        private RichTextBox textbox;
        public ControlWriter(RichTextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            textbox.Invoke((MethodInvoker)(() =>
            {
                textbox.ReadOnly = false;
                textbox.AppendText(value.ToString());
                textbox.ReadOnly = true;
            }));

        }

        public override void Write(string value)
        {
            textbox.Invoke((MethodInvoker)(() =>
            {
                textbox.ReadOnly = false;
                textbox.AppendText(value);
                textbox.ReadOnly = true;
            }));

        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }

}
