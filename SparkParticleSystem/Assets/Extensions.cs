using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {



	public static void Fill<T>(this List<T> list, T tofillWith)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = tofillWith;
		}

		if (list.Capacity > list.Count)
		{
			for (int i = list.Count; i < list.Capacity; i++)
				list.Add(tofillWith);
		}
	}

	public static void CycleAdd<T>(this List<T> list, T toAdd)
	{
		int prevCount = list.Count;
		list.Insert(0,toAdd);
		list.RemoveAt(prevCount);
	}
}
