﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor;


namespace X.Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(params string[] args)
        {
            TypeCatalog.Instance.Add(new FileInfo(typeof(Program).Assembly.Location).Directory);


            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var frm = new Main(args);
            System.Windows.Forms.Application.Run(frm);
        }
    }
}
