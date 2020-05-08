using System;

namespace Eternity.Share.Config.Attributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Field|AttributeTargets.Property, 
        AllowMultiple = false, Inherited = true)]
    public class MemberDesc : Attribute
    {
        public string Desc { get; set; }
        public MemberDesc(string desc)
        {
            Desc = desc;
        }
    }
}
