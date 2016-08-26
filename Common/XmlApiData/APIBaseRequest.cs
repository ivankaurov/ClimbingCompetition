using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace XmlApiData
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class XmlValidableAttribute : Attribute
    {
        public XmlValidableAttribute() { }
    }

    [XmlValidable]
    [XmlInclude(typeof(APICompIDRequest))]
    [XmlInclude(typeof(API_AgeGroupCollection))]
    [XmlInclude(typeof(Comp_AgeGroupApiModel))]
    [XmlInclude(typeof(API_ClimbersCollection))]
    [XmlInclude(typeof(Comp_CompetitorRegistrationApiModel))]
    [XmlInclude(typeof(API_RegionCollection))]
    [XmlInclude(typeof(RegionApiModel))]
    [XmlInclude(typeof(APISimpleRequest))]
    [XmlInclude(typeof(ApiListHeader))]
    [XmlInclude(typeof(ApiListHeaderCollection))]
    [XmlInclude(typeof(ApiListLine))]
    [XmlInclude(typeof(ApiListLineCollection))]
    [XmlType("BaseRequest", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    [XmlRoot("BaseRequest", Namespace = XmlApiDataConstants.NAMESPACE)]
    public abstract class APIBaseRequest
    {
        public APIBaseRequest() { }
    }

    public interface IAPICollection<T>
        where T:APIBaseRequest
    {
        T[] Data { get; set; }
    }

    [XmlType("SimpleRequest", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public sealed class APISimpleRequest : APIBaseRequest
    {
        public String Message { get; set; }
        public APISimpleRequest() { this.Message = String.Empty; }
    }

    [XmlValidable]
    public abstract class APICompIDRequest
    {
        [XmlElement("CompID")]
        public long CompID { get; set; }

        [XmlElement("Request")]
        public APIBaseRequest Request { get; set; }

        public APICompIDRequest(long compID)
        {
            this.CompID = compID;
        }

        public APICompIDRequest() : this(-1) { }
    }

    [XmlRoot("SignedRequest", Namespace = XmlApiDataConstants.NAMESPACE)]
    [XmlType("SignedRequest", IncludeInSchema = true)]
    public sealed class APISignedRequest : APICompIDRequest
    {
        private byte[] signature;
        [XmlElement("Signature")]
        public byte[] Signature
        {
            get { return signature; }
            set { signature = value ?? new byte[0]; }
        }

        public APISignedRequest(long compID)
            : base(compID)
        {
            this.signature = new byte[0];
        }

        public APISignedRequest() : this(-1) { }
    }

    [XmlRoot("PasswordRequest", Namespace = XmlApiDataConstants.NAMESPACE)]
    [XmlType("PasswordRequest", IncludeInSchema = true)]
    public sealed class APIPasswordRequest : APICompIDRequest
    {
        [XmlElement("Password")]
        public String Password { get; set; }

        public APIPasswordRequest(long compID)
            : base(compID)
        {
            this.Password = String.Empty;
        }

        public APIPasswordRequest() : this(-1) { }
    }

    public static class SerializingHelper
    {
        private static readonly Encoding defaultEncoding = Encoding.Unicode;
        private static Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();
        private static object locker = new object();
        public static XmlSerializer GetSerializer(Type t)
        {
            lock (locker)
            {
                if (serializers.ContainsKey(t))
                    return serializers[t];
                /*if (!Attribute.IsDefined(t, typeof(XmlValidableAttribute)))
                    throw new ArgumentException(typeof(XmlValidableAttribute).FullName + " should be defined", "t");*/
                var newSer = new XmlSerializer(t);
                serializers.Add(t, newSer);
                return newSer;
            }
        }
        public static byte[] GetRequestBytes(object o, Encoding enc = null)
        {
            XmlSerializer ser;
            try { ser = GetSerializer(o.GetType()); }
            catch (ArgumentException ex) { throw new ArgumentException("Invalid type", "o", ex); }

            StringBuilder sb = new StringBuilder();
            var currentEncoding = enc ?? defaultEncoding;
            using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings { Encoding = currentEncoding }))
            {
                ser.Serialize(writer, o);
            }
            String temp = sb.ToString();
            return currentEncoding.GetBytes(temp);
        }
    }
}