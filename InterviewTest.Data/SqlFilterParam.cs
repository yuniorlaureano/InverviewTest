using System.Data;

namespace InterviewTest.Data
{
    /// <summary>
    /// Represent a parameter to be transformed into a SqlParameter
    /// </summary>
    /// <param name="Param">Parameter name</param>
    /// <param name="Value">Parameter value</param>
    /// <param name="Type">Paramete type</param>
    /// <param name="ParamName">A custom parameter name</param>
    public record SqlFilterParam(string Param, object Value, SqlDbType Type, string ParamName = "");
}
