using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to a <see cref="DbType.DateTime"/> column that 
	/// stores date &amp; time down to the accuracy of a second.
	/// </summary>
	/// <remarks>
	/// This only stores down to a second, so if you are looking for the most accurate
	/// date and time storage your provider can give you use the <see cref="TimestampType" />. 
	/// or the <see cref="TicksType"/>
	/// </remarks>
	public class DateTimeType : ValueTypeType, IIdentifierType, ILiteralType, IVersionType
	{
		/// <summary></summary>
		internal DateTimeType() : base( new DateTimeSqlType() )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, int index )
		{
			DateTime dbValue = Convert.ToDateTime( rs[ index ] );
			return new DateTime( dbValue.Year, dbValue.Month, dbValue.Day, dbValue.Hour, dbValue.Minute, dbValue.Second );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, string name )
		{
			return Get( rs, rs.GetOrdinal( name ) ); // rs.[name];
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( DateTime ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand st, object value, int index )
		{
			IDataParameter parm = st.Parameters[ index ] as IDataParameter;
			parm.DbType = DbType.DateTime;
			//TODO: figure out if this is a good solution for NULL DATES
			if( ( DateTime ) value < new DateTime( 1753, 1, 1 ) )
			{
				parm.Value = DBNull.Value;
			}
			else
			{
				DateTime dateValue = ( DateTime ) value;
				parm.Value = new DateTime( dateValue.Year, dateValue.Month, dateValue.Day, dateValue.Hour, dateValue.Minute, dateValue.Second );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals( object x, object y )
		{
			if( x == y )
			{
				return true;
			}
			// DateTime can't be null because it is a struct - so comparing 
			// them this way is useless - instead use the magic number...
			//if (x==null || y==null) return false;

			DateTime date1 = ( x == null ) ? DateTime.MinValue : ( DateTime ) x;
			DateTime date2 = ( y == null ) ? DateTime.MinValue : ( DateTime ) y;

			//return date1.Equals(date2);
			return ( date1.Year == date2.Year &&
				date1.Month == date2.Month &&
				date1.Day == date2.Day &&
				date1.Hour == date2.Hour &&
				date1.Minute == date2.Minute &&
				date1.Second == date2.Second );
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "DateTime"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public override string ToXML( object val )
		{
			return ( ( DateTime ) val ).ToShortDateString();
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public object StringToObject( string xml )
		{
			return DateTime.Parse( xml );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString( object value )
		{
			return "'" + value.ToString() + "'";
		}

		#region IVersionType Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="current"></param>
		/// <returns></returns>
		public object Next( object current )
		{
			return Seed;
		}

		/// <summary></summary>
		public object Seed
		{
			get { return DateTime.Now; }
		}

		#endregion
	}
}