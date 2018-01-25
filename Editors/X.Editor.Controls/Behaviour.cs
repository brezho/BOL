//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace X.Editor.Controls
//{
//    public class Behaviour
//    {
//        public Control Target { get; private set; }
//        public Behaviour(Control target)
//        {
//            Target = target;
//        }
//    }

//    public class MoveBehaviour : Behaviour
//    {
//        public MoveBehaviour(Control target):base(target)
//        {
//            target.GotFocus += Target_GotFocus;
//            target.LostFocus += Target_LostFocus;
//        }

//        private void Target_LostFocus(object sender, EventArgs e)
//        {

//        }

//        private void Target_GotFocus(object sender, EventArgs e)
//        {

//        }
//    }
//}
