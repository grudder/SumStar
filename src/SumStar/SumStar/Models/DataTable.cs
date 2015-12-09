using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

using Newtonsoft.Json;

namespace SumStar.Models
{
	/// <summary>
	/// datatables控件所需的数据表格。
	/// </summary>
	/// <typeparam name="TEntity">实体类型。</typeparam>
	/// <typeparam name="TId">标识类型。</typeparam>
	public class DataTable<TEntity, TId> where TEntity : class
	{
		/// <summary>
		/// The draw counter that this object is a response to.
		/// </summary>
		[JsonProperty("draw")]
		public int Draw
		{
			get;
			private set;
		}

		/// <summary>
		/// Total records, before filtering.
		/// </summary>
		[JsonProperty("recordsTotal")]
		public int RecordsTotal
		{
			get;
			private set;
		}

		/// <summary>
		/// Total records, after filtering.
		/// </summary>
		[JsonProperty("recordsFiltered")]
		public int RecordsFiltered
		{
			get;
			private set;
		}

		/// <summary>
		/// The data to be displayed in the table.
		/// </summary>
		[JsonProperty("data")]
		public IList<TEntity> Data
		{
			get;
			private set;
		}

		/// <summary>
		/// 通过请求产生分页数据。
		/// </summary>
		/// <param name="request">HTTP请求。</param>
		/// <param name="dbSet">数据集。</param>
		/// <param name="predicate">查询断言。</param>
		/// <returns>请求的分页数据。</returns>
		public static DataTable<TEntity, TId> FromRequest(HttpRequestBase request,
			IDbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> predicate)
		{
			int draw = int.Parse(request.Params["draw"]);
			int start = int.Parse(request.Params["start"]);
			int length = int.Parse(request.Params["length"]);
			string orderColumnIndex = request.Params["order[0][column]"];
			string orderColumnName = request.Params["columns[" + orderColumnIndex + "][data]"];
			string orderDir = request.Params["order[0][dir]"];

			var dataTable = new DataTable<TEntity, TId>
			{
				Draw = draw
			};
			IList<TEntity> data;
			if (length == -1)
			{
				data = dbSet.ToList();

				int count = data.Count();
				dataTable.RecordsTotal = count;
				dataTable.RecordsFiltered = count;
				dataTable.Data = data;
			}
			else
			{
				IQueryable<TEntity> query = dbSet.Where(predicate);
				query = (orderDir == "desc")
					? query.Where(predicate).OrderByDescending(i => GetProperty(i, orderColumnName))
					: query.Where(predicate).OrderBy(i => GetProperty(i, orderColumnName));
				data = query.Skip(start).Take(length).ToList();

				int count = data.Count;
				dataTable.RecordsTotal = count;
				dataTable.RecordsFiltered = count;
				dataTable.Data = data;
			}

			return dataTable;
		}

		/// <summary>
		/// 获取指定名称的对象属性。
		/// </summary>
		/// <param name="obj">对象。</param>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>指定名称的对象属性。</returns>
		private static object GetProperty(object obj, string propertyName)
		{
			PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
			return propertyInfo.GetValue(obj, null);
		}
	}
}
