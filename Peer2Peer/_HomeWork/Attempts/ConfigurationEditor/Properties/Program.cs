using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using X.DataModel;

namespace ConfigurationEditor
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var ob = new titi();
            var t = ob.GetType().Name;
            //var tr = new Tata();
            //tr.Nunuche.Set("sdfsdf");


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
//public class Tata : MyObject
//{
//    public XString Nunuche;
//}
public class titi
{
    public new Type GetType()
    {
        return new toto() { };
    }
}

public class toto : System.Reflection.TypeDelegator
{
    public toto() : base(typeof(titi)) { }
    public override string Name
    {
        get
        {
            return base.Name + " hacked";
        }
    }

}
