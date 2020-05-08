using System;

namespace Eternity.Share.Config.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldDesc : Attribute
    {
        public string BriefDesc { get; set; }
        public string DetailDesc { get; set; }
        public FieldDesc(string detail = "", string brief = "")
        {
            DetailDesc = detail;
            BriefDesc = brief;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldOrder : Attribute
    {
        public int Order { get; set; }
        public FieldOrder(int order)
        {
            Order = order;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldReadonly : Attribute
    {
        public FieldReadonly()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldShow : Attribute
    {
        public FieldShow() { }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldHide : Attribute
    {
        public FieldHide() { }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldMultilineText : Attribute
    {
        public int Height { get; set; }
        public FieldMultilineText(int h = 40)
        {
            Height = h;
        }
    }
}
