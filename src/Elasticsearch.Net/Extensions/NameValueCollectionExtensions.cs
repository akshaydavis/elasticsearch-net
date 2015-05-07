﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Elasticsearch.Net.Serialization;

namespace Elasticsearch.Net
{
	internal static class NameValueCollectionExtensions
	{
		internal static void CopyKeyValues(this NameValueCollection source, NameValueCollection dest)
		{
			foreach (var key in source.AllKeys)
			{
				if (dest[key] != null) throw new Exception(string.Format("Attempted to add duplicate key '{0}'", key));

				dest.Add(key, source[key]);
			}
		}

		internal static string ToQueryString(this NameValueCollection self, string prefix = "?")
		{
			if (self == null)
				return null;

			if (self.AllKeys.Length == 0) return string.Empty;

#if DNXCORE50
			//SHOULD BE FINE 4.5 and up?
			return prefix + string.Join("&", self.AllKeys.Select(key => string.Format("{0}={1}", Encode(key), Encode(self[key]))));
#else
			return prefix + string.Join("&", Array.ConvertAll(self.AllKeys, key => string.Format("{0}={1}", Encode(key), Encode(self[key]))));
#endif
		}
		private static string Encode(string s)
		{
			return s == null ? null : Uri.EscapeDataString(s);
		}

		internal static NameValueCollection ToNameValueCollection(this IDictionary<string, object> dict, IElasticsearchSerializer stringifier)
		{
			stringifier.ThrowIfNull("stringifier");
			if (dict == null || dict.Count < 0)
				return null;
			
			var nv = new NameValueCollection();
			foreach (var kv in dict.Where(kv => !kv.Key.IsNullOrEmpty()))
			{
				nv.Add(kv.Key, stringifier.Stringify(kv.Value));
			}
			return nv;
		}
	}
}
