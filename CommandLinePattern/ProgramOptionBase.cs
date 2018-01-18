using System;
using System.Collections.Generic;

namespace CommandLinePattern
{
    public class ProgramOptionBase
    {
        public string Name { get; set; }
        public string Pattern { get; set; }
        public string Description { get; set; }
        public bool IsFlag { get; set; }
        public string InformedValue { get; set; }
        public bool HasValue { get; set; }
        public List<ProgramAcceptedValue> AcceptedValues { get; set; }

        public ProgramOptionBase()
        {
            AcceptedValues = new List<ProgramAcceptedValue>();
        }

        public object ConvertValueToType(Type typeInfo)
        {
            if (!HasValue)
            {
                return null;
            }

            TypeCode typeCode = Type.GetTypeCode(typeInfo);

            try
            {
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        return Convert.ToBoolean(InformedValue);

                    case TypeCode.Char:
                        return Convert.ToChar(InformedValue);

                    case TypeCode.SByte:
                        return Convert.ToSByte(InformedValue);

                    case TypeCode.Single:
                        return Convert.ToSingle(InformedValue);

                    case TypeCode.Double:
                        return Convert.ToDouble(InformedValue);

                    case TypeCode.Decimal:
                        return Convert.ToDecimal(InformedValue);

                    case TypeCode.Byte:
                        return Convert.ToByte(InformedValue);

                    case TypeCode.UInt16:
                        return Convert.ToUInt16(InformedValue);

                    case TypeCode.Int16:
                        return Convert.ToInt16(InformedValue);

                    case TypeCode.Int32:
                        return Convert.ToInt32(InformedValue);

                    case TypeCode.UInt32:
                        return Convert.ToUInt32(InformedValue);

                    case TypeCode.Int64:
                        return Convert.ToInt64(InformedValue);

                    case TypeCode.UInt64:
                        return Convert.ToUInt64(InformedValue);

                    case TypeCode.Object:
                    case TypeCode.String:
                        return InformedValue;

                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception)
            {
                throw new ConvertOptionValueException(Name, typeCode.ToString(), InformedValue);
            }
        }
    }
}
