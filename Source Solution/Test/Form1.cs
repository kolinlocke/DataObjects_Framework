using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataObjects_Framework.Common;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var Ex = Do_Methods.Convert_Value<String>(null);
            Debugger.Break();
        }
    }
}
