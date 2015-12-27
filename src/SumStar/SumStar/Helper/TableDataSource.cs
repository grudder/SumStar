using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

using Newtonsoft.Json;

namespace SumStar.Helper
{
	/// <summary>
	/// datatables分页表格的数据源。
	/// </summary>
	/// <typeparam name="TEntity">实体类型。</typeparam>
	public class TableDataSource<TEntity> where TEntity : class
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
		public static TableDataSource<TEntity> FromRequest(HttpRequestBase request,
			IDbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> predicate)
		{
			int draw = int.Parse(request.Params["draw"]);
			int start = int.Parse(request.Params["start"]);
			int length = int.Parse(request.Params["length"]);
			string orderColumnIndex = request.Params["order[0][column]"];
			string orderColumnName = request.Params["columns[" + orderColumnIndex + "][data]"];
			string orderDir = request.Params["order[0][dir]"];

			var dataTable = new TableDataSource<TEntity>
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
				int count = query.Count();

				query = (orderDir == "desc")
					? query.OrderByDescending(orderColumnName)
					: query.OrderBy(orderColumnName);
				data = query.Skip(start).Take(length).ToList();

				dataTable.RecordsTotal = count;
				dataTable.RecordsFiltered = count;
				dataTable.Data = data;
			}

			return dataTable;
		}
	}
}
