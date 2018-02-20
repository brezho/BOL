using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml.Serialization;

namespace PieceOfWork
{
    // that is a stupid data holder
    // that needs to be extended
    public class Activity : X.DataModel.XObject
    {
        [XmlAttribute]
        public Guid Id { get { return Get(() => Id); } set { Set(() => Id, value); } }

        [XmlAttribute]
        public DateTime CreatedOn { get { return Get(() => CreatedOn); } set { Set(() => CreatedOn, value); } }

        [XmlAttribute]
        public MailAddress AssignedTo { get { return Get(() => AssignedTo, (string x) => new MailAddress(x)); } set { Set(() => AssignedTo, value, x => (x!=null)?x.ToString():(string)null); } }
    }
}