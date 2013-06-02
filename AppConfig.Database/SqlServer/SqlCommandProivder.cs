using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;

namespace AppConfig.Database.SqlServer
{
    public class SqlCommandProivder : ICommandProvider
    {
        public string GetCreateTable(Type type)
        {
            var table = TableAttribute.GetTable(type);
            var columns = ColumnAttribute.GetColumns(type);
            var primaryKeyColumns = new List<ColumnAttribute>();
            var expressions = new List<string>();

            foreach (var column in columns)
            {
                expressions.Add("\t[" + column.ColumnName + "] " + GetDataTypeExpression(column));
                if (column.IsInPrimaryKey)
                    primaryKeyColumns.Add(column);
            }

            //Add a primary key if one was specified
            if (primaryKeyColumns.Count > 0)
                expressions.Add("\tprimary key(" + string.Join(",", primaryKeyColumns.Select(a => "[" + a.ColumnName + "]") + ")"));

            var rtn = string.Format(
                "create table [{0}].[{1}] (\r\n\t{2}\r\n);",
                table.SchemaName,                         //0 - SchemaName
                table.TableName,                          //1 - TableName
                string.Join(",\r\n\t", expressions)       //2 - Column Expressions
            );

            return rtn;
        }

        public string GetCreateTableConstraints(Type type)
        {
            return "";
        }

        public IDbCommand GetTableSave(Type type)
        {
            var rtn = new SqlCommand();
            rtn.CommandType = CommandType.Text;

            var table = TableAttribute.GetTable(type);
            var columns = ColumnAttribute.GetColumns(type);
            var names = new List<string>();
            var values = new List<string>();
            var updatevalues = new List<string>();
            var keyWhereValues = new List<string>();

            foreach (var column in columns)
            {
                if (!(column.CanInsert || column.CanUpdate || column.IsInPrimaryKey))
                    continue;

                if (column.CanInsert)
                {
                    names.Add("[" + column.ColumnName + "]");
                    values.Add("@" + column.ColumnName);
                }

                if (column.CanUpdate)
                    updatevalues.Add("\t\t[" + column.ColumnName + "]=@" + column.ColumnName);

                if (column.IsInPrimaryKey)
                    keyWhereValues.Add("[" + column.ColumnName + "]=@" + column.ColumnName);

                rtn.Parameters.Add(GetCommandParameter(column));
            }

            //Check for identity column and return the identity id found
            var identityExpression = "";
            var updateIdentityExpression = "";
            var identityColumn = columns.SingleOrDefault(a => a.IsIdentity);
            if (identityColumn != null)
            {
                identityExpression = "\tselect convert(" + GetDataType(identityColumn) + ", @@Identity) as [" + identityColumn.ColumnName + "];\n";
                updateIdentityExpression = "\tselect @" + identityColumn.ColumnName + ";\n";
            }

            rtn.CommandText = string.Format(
                "\nif not exists (select * from {0} where {5}) begin\n" +
                    "\tinsert into {0}({1})\n" +
                    "\tvalues({2});\n" +
                    "{4}" +
                "end\n" +
                "else begin\n" +
                    "\tupdate {0} set\n" +
                        "{3}\n" +
                    "\twhere {5};\n" +
                    "{6}" +
                "end",
                table.SchemaQualifiedTableName,                         //0 - TableName
                string.Join(",", names.ToArray()),                      //1 - Insert names list
                string.Join(",", values.ToArray()),                     //2 - Insert values list
                string.Join(",\n", updatevalues.ToArray()),             //3 - Update name value pairs
                identityExpression,                                     //4 - Identity Expression
                string.Join(" and ", keyWhereValues),                   //5 - Primary Key Where Statement lookup
                updateIdentityExpression                                //6 - Update Return Identity Id
            );

            return rtn;
        }

        public static SqlParameter GetCommandParameter(ColumnAttribute column)
        {
            var type = column.Property.PropertyType;
            if (type.IsEnum)
            {
                if (column.EnumStorageMethod == EnumStorageMethod.Text)
                    return new SqlParameter(column.ColumnName, SqlDbType.NVarChar, column.Length);
                else if (Enum.GetUnderlyingType(type) == typeof(byte))
                    return new SqlParameter(column.ColumnName, SqlDbType.TinyInt);
                else if (Enum.GetUnderlyingType(type) == typeof(Int16))
                    return new SqlParameter(column.ColumnName, SqlDbType.SmallInt);
                else if (Enum.GetUnderlyingType(type) == typeof(int))
                    return new SqlParameter(column.ColumnName, SqlDbType.Int);
                else
                    throw new NotSupportedException("The underlying enum type '" + Enum.GetUnderlyingType(type).FullName + "' is not supported as an enum type.");
            }
            else if (type == typeof(Guid) || type == typeof(Guid?))
                return new SqlParameter(column.ColumnName, SqlDbType.UniqueIdentifier);
            else if (type == typeof(string))
            {
                if (column.LengthType == LengthType.Fixed)
                    return new SqlParameter(column.ColumnName, SqlDbType.NChar, column.Length);
                else if (column.LengthType == LengthType.Variable)
                    return new SqlParameter(column.ColumnName, SqlDbType.NVarChar, column.Length);
                else if (column.LengthType == LengthType.NoMax)
                    return new SqlParameter(column.ColumnName, SqlDbType.NText, -1);
                else
                    throw new NotSupportedException();
            }
            else if (type == typeof(bool) || type == typeof(bool?))
                return new SqlParameter(column.ColumnName, SqlDbType.Bit);
            else if (type == typeof(int) || type == typeof(int?))
                return new SqlParameter(column.ColumnName, SqlDbType.Int);
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
                return new SqlParameter(column.ColumnName, SqlDbType.DateTime);
            else
                throw new NotSupportedException("The property type '" + type.FullName + "' is not mapped to a '" + typeof(SqlDbType).FullName + "' type.");
        }

        public static string GetDataType(ColumnAttribute column)
        {
            var type = column.Property.PropertyType;
            var rtn = "";

            if (type.IsEnum)
            {
                if (column.EnumStorageMethod == EnumStorageMethod.Text)
                    rtn += "nvarchar";
                else if (Enum.GetUnderlyingType(type) == typeof(byte))
                    rtn += "tinyint";
                else if (Enum.GetUnderlyingType(type) == typeof(Int16))
                    rtn += "smallint";
                else if (Enum.GetUnderlyingType(type) == typeof(int))
                    rtn += "int";
                else
                    throw new NotSupportedException("The underlying enum type '" + Enum.GetUnderlyingType(type).FullName + "' is not supported as an enum type.");
            }
            else if (type == typeof(Guid) || type == typeof(Guid?))
                rtn += "uniqueidentifier";
            else if (type == typeof(string))
            {
                if (column.LengthType == LengthType.Variable)
                    rtn += "nvarchar";
                else if (column.LengthType == LengthType.VariableMax)
                    rtn += "nvarchar(max)";
                else if (column.LengthType == LengthType.Fixed)
                    rtn += "nchar";
                else if (column.LengthType == LengthType.NoMax)
                    rtn += "ntext";
            }
            else if (type == typeof(int) || type == typeof(int?))
                rtn += "int";
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
                rtn += "datetime";
            else if (type == typeof(bool) || type == typeof(bool?))
                rtn += "bit";
            else
                throw new NotSupportedException("The CRL type '" + type.FullName + "' is not mapped to a Sql Server data type.");

            if (column.Length > 0)
                rtn += " (" + column.Length + ")";

            return rtn;
        }

        public static string GetDataTypeExpression(ColumnAttribute column)
        {
            var rtn = GetDataType(column);
            rtn += (column.Nullable) ? " NULL" : " NOT NULL";

            return rtn;
        }

        public IDbCommand CreateSelectCommand<T>(Expression<Func<T, dynamic>> SelectClause, Expression<Func<T, bool>> WhereClause, Expression<Func<T, dynamic>> OrderByClause, int Skip, int Take)
        {
            if (Skip > 0 && OrderByClause == null)
                throw new ArgumentException("When Skip is greater than zero, an order by clause is required.");

            var columns = ColumnAttribute.GetColumns<T>();
            var table = TableAttribute.GetTable<T>();

            //Create a new command from the connection
            var rtn = new SqlCommand();
            rtn.CommandType = CommandType.Text;

            if (Skip <= 0)
            {
                rtn.CommandText = string.Format(
                     "select{0} {1}\r\n" +
                     "from [{2}].[{3}] t1\r\n" +
                     "{4}" +
                     "{5}",
                     (Take > 0) ? " top " + Take : "",
                     (SelectClause != null) ? TranslateSelectClause<T>(SelectClause) : string.Join(", ", columns.Select(a => "t1.[" + a.ColumnName + "]")),
                     table.SchemaName,
                     table.TableName,
                     (WhereClause != null) ? "where " + TranslateWhereClause<T>(WhereClause) + "\r\n" : "",
                     (OrderByClause != null) ? "order by " + TranslateOrderByClause<T>(OrderByClause) + "\r\n" : ""
                );
            }
            else //Skip is greater than 0
            {
                rtn.CommandText = string.Format(
                     "select {0}\r\n" +
                     "from (\r\n" +
                        "\tselect row_number() over (order by {4}) as row_number,\r\n" +
                            "\t\t{0}\r\n" +
                        "\tfrom [{1}].[{2}] t1\r\n" +
                        "{3}" +
                     ") t1\r\n" +
                     "where t1.row_number between @Skip + 1 and @Skip + @Take\r\n" +
                     "order by t1.row_number ;",
                     (SelectClause != null) ? TranslateSelectClause<T>(SelectClause) : string.Join(", ", columns.Select(a => "t1.[" + a.ColumnName + "]")),
                     table.SchemaName,
                     table.TableName,
                     (WhereClause != null) ? "where " + TranslateWhereClause<T>(WhereClause) + "\r\n" : "",
                     OrderByClause
                );
            }
            return rtn;
        }

        public string TranslateSelectClause<T>(Expression<Func<T, dynamic>> SelectClause)
        {
            var rtn = ParseExpression<T>(SelectClause.Body);
            return rtn;
        }

        public string TranslateWhereClause<T>(Expression<Func<T, bool>> WhereClause)
        {
            var rtn = ParseExpression<T>(WhereClause.Body);
            return rtn;
        }

        public string TranslateOrderByClause<T>(Expression<Func<T, dynamic>> OrderByClause)
        {
            var rtn = ParseExpression<T>(OrderByClause.Body);
            return rtn;
        }

        private string ParseExpression<T>(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                {
                    var tExp = exp as BinaryExpression;
                    return ParseExpression<T>(tExp.Left) + " and " + ParseExpression<T>(tExp.Right);
                }
                case ExpressionType.Constant:
                {
                    var tExp = exp as ConstantExpression;
                    return ParseConstantValue(tExp.Value);
                }
                case ExpressionType.Equal:
                {
                    var tExp = exp as BinaryExpression;
                    return ParseExpression<T>(tExp.Left) + " = " + ParseExpression<T>(tExp.Right);
                }
                case ExpressionType.GreaterThan:
                {
                    var tExp = exp as BinaryExpression;
                    return ParseExpression<T>(tExp.Left) + " > " + ParseExpression<T>(tExp.Right);
                }
                case ExpressionType.GreaterThanOrEqual:
                {
                    var tExp = exp as BinaryExpression;
                    return ParseExpression<T>(tExp.Left) + " >= " + ParseExpression<T>(tExp.Right);
                }
                case ExpressionType.LessThan:
                {
                    var tExp = exp as BinaryExpression;
                    return ParseExpression<T>(tExp.Left) + " < " + ParseExpression<T>(tExp.Right);
                }
                case ExpressionType.LessThanOrEqual:
                {
                    var tExp = exp as BinaryExpression;
                    return ParseExpression<T>(tExp.Left) + " <= " + ParseExpression<T>(tExp.Right);
                }                
                case ExpressionType.MemberAccess:
                {
                    var tExp = exp as MemberExpression;
                    var columnAttribute = tExp.Member.CustomAttributes.SingleOrDefault(a => a.AttributeType == typeof(ColumnAttribute));
                    if (tExp.Member.MemberType == MemberTypes.Property && tExp.Member.DeclaringType == typeof(T) && columnAttribute != null)
                        return string.Format("t1.[{0}]", tExp.Member.Name);
                    else if (tExp.Member.MemberType == MemberTypes.Field)
                        return ParseConstantValue(((FieldInfo)tExp.Member).GetValue(((ConstantExpression)tExp.Expression).Value));
                    else
                        throw new NotSupportedException();
                }
                case ExpressionType.New:
                {
                    var tExp = exp as NewExpression;
                    return string.Join(", ", tExp.Arguments.Select(a => ParseExpression<T>(a)));
                }
                case ExpressionType.NewArrayInit:
                {
                    var tExp = exp as NewArrayExpression;
                    return string.Join(", ", tExp.Expressions.Select(a => ParseExpression<T>(a)));
                }
                case ExpressionType.NotEqual:
                {
                    var tExp = exp as BinaryExpression;
                    return ParseExpression<T>(tExp.Left) + " <> " + ParseExpression<T>(tExp.Right);
                }
                case ExpressionType.OrElse:
                {
                    var tExp = exp as BinaryExpression;
                    return ParseExpression<T>(tExp.Left) + " or " + ParseExpression<T>(tExp.Right);
                }
                case ExpressionType.Call:
                {
                    var tExp = exp as MethodCallExpression;
                    if (tExp.Method.DeclaringType == typeof(System.Linq.Enumerable) && tExp.Method.Name == "Contains")
                        return ParseExpression<T>(tExp.Arguments[1]) + " in(" + ParseExpression<T>(tExp.Arguments[0]) + ")";
                    else
                        throw new NotSupportedException("The Method Call '" + tExp.Method.Name + "' for Type '" + tExp.Method.DeclaringType.FullName + "' is not supported.");
                }
                default:
                    throw new NotImplementedException("Expression Type '" + exp.NodeType + "', ClassType '" + exp.GetType().FullName + "' is not supported.");
            }
        }

        private string ParseConstantValue(object Value)
        {
            var type = Value.GetType();
            if (type == typeof(string))
            {
                if (Value == null)
                    return "null";
                else
                    return "'" + ((string)Value).Replace("'", "''") + "'";
            }
            else if (type == typeof(DateTime))
            {
                if (Value == null)
                    return "null";
                else
                    return string.Format("'{0:g}'", (DateTime)Value);
            }
            else if (new Type[] { typeof(int), typeof(double), typeof(Int16), typeof(Int64), typeof(decimal) }.Contains(type))
            {
                if (Value == null)
                    return "null";
                else
                    return Convert.ToString(Value);
            }
            else
                throw new NotImplementedException("A constant expression for type '" + type.FullName + "' is not supported.");
        }
    }
}
