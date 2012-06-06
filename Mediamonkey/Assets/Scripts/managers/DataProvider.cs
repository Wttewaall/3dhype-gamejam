using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataProvider<T> : ICloneable {
	
	/**
	 * TODO
	 * keep track of the direction for wrapmode, also for previousItem and nextItem getters
	 * 
	 */
	
	// events
	public event CollectionEventHandler		OnCollectionPreChange;
	public event CollectionEventHandler		OnCollectionChange;
	public event IndexChangeEventHandler	OnIndexChange;
	public event IndexChangeEventHandler	OnIndexChanging;
	
	#region - variables -
	
	public WrapMode wrapMode = WrapMode.Loop;
	
	#endregion
	#region - getters & setters -
	
	private List<T> _data;
	private int _selectedIndex = -1;
	private int _capacity = 0;
	
	public List<T> data {
		get {
			return _data;
		}
		set {
			if (_data == value) return;
			
			// TODO - add listeners? do a sort? set selectedIndex?
			
			_data = value;
		}
	}
	
	public int selectedIndex {
		get { return _selectedIndex; }
		set {
			if (_selectedIndex == value) return;
			
			int oldIndex = _selectedIndex;
			dispatchIndexChangingEvent(IndexChangeEventType.CHANGING, oldIndex, value);
			
			int newIndex = wrapIndex(oldIndex, value);
			
			_selectedIndex = newIndex;
			
			// dispatch, even if the index hasn't changed (possible in some wrapMode cases)
			dispatchIndexChangeEvent(IndexChangeEventType.CHANGE, oldIndex, newIndex);
		}
	}
	
	public T selectedItem {
		get {
			return (data != null && selectedIndex != -1)
				? data[selectedIndex]
				: default(T);
		}
		set {
			selectedIndex = data.IndexOf(value);
		}
	}
	
	public T nextItem {
		get {
			var index = wrapIndex(selectedIndex, selectedIndex+1);
			return GetItemAt(index);
		}
	}
	
	public T previousItem {
		get {
			var index = wrapIndex(selectedIndex, selectedIndex-1);
			return GetItemAt(index);
		}
	}
	
	public int capacity {
		get { return _capacity; }
	}
	
	public int length {
		get { return (data != null) ? data.Count : 0; }
	}
	
	public int numItems {
		get { return (data != null) ? data.Count : 0; }
	}
	
	public int firstEmptyIndex {
		get {
			if (capacity > 0) {
				for (int i=0; i<data.Count; i++) {
					if (data[i] == null) return i;
				}
				
			} else {
				return data.Count;
			}
			return -1;
		}
	}
	
	public int lastEmptyIndex {
		get {
			if (capacity > 0) {
				int i = data.Count;
				while (i-- > 0) {
					if (data[i] == null) return i;
				}
				
			} else {
				return data.Count;
			}
			return -1;
		}
	}
	
	protected int maximum {
		get { return Mathf.Max(capacity, data.Count); }
	}
	
	#endregion
	#region - constructors -
	
	public DataProvider() {
		data = new List<T>();
	}
	
	public DataProvider(int capacity) {
		if (capacity <= 0) throw new UnityException("capacity must be > 0");
		this._capacity = capacity;
		
		data = new List<T>(capacity);
		
		// fill with null
		for (int i=0; i<capacity; i++) {
			data.Add(default(T));
		}
	}
	
	public DataProvider(object source) {
		
		if (source is DataProvider<T>) {
			var other = source as DataProvider<T>;
			this._capacity = other.capacity;
			this._data = other.ToList();
			
		} else {
			// this may throw an exception
			data = getDataFromObject(source);
		}
	}
	
	#endregion
	#region - public methods -
	
	public bool AddItem(T item) {
		return AddItem(item, false);
	}
	
	public bool AddItem(T item, bool dispatch) {
		//Utils.trace(parent, "\tDP AddItem, capacity:", capacity, "length:", length);
		
		if (capacity > 0) {
			int index = firstEmptyIndex;
			
			//Utils.trace("\tAddItem firstEmptyIndex:", index);
			if (index > -1) return AddItemAt(item, index);
			
		} else {
			return AddItemAt(item, data.Count);
		}
		
		return false;
	}
	
	public bool AddItemAt(T item, int index) {
		return AddItemAt(item, index, true);
	}
	
	// does not overwrite not-null value at index
	public bool AddItemAt(T item, int index, bool dispatch) {
		validateIndex(index);
		
		if (dispatch) dispatchPreChangeEvent(CollectionEventKind.ADD, item, index, index);
		
		bool result = true;
		
		if (GetItemAt(index) == null) {
			
			if (capacity > 0) {
				//Utils.trace("\tDP set:", item, index);
				data[index] = item;
				
			} else {
				//Utils.trace("\tDP Add:", item, index);
				data.Add(item);
			}
			
			result = true;
			
		} else {
			//Utils.trace("\tDP AddItemAt FAILURE", item, index);
			result = false;
		}
		
		if (dispatch && result) dispatchChangeEvent(CollectionEventKind.ADD, item, index, index);
		return result;
	}
	
	public bool AddItems(object items) {
		return AddItems(items, true);
	}
	
	public bool AddItems(object items, bool dispatch) {
		List<T> list = getDataFromObject(items);
		
		bool success = true;
		for (int i=0; i<list.Count; i++) {
			success = success && AddItem(list[i], dispatch);
		}
		
		return success;
	}
	
	public bool AddItemsAt(object items, int index) {
		return AddItemsAt(items, index, true);
	}
	
	public bool AddItemsAt(object items, int index, bool dispatch) {
		List<T> list = getDataFromObject(items);
		
		bool success = true;
		for (int i=0; i<list.Count; i++) {
			success = success && AddItemAt(list[i], index + i, dispatch);
		}
		
		return success;
	}
	
	public T GetItemAt(int index) {
		try { return data[index]; }
		catch { return default(T); }
	}
	
	public List<T> GetItemsAt(int beginIndex, int endIndex) {
		validateIndex(beginIndex);
		validateIndex(endIndex);
		
		int count = endIndex - beginIndex;
		if (count > 0) return data.GetRange(beginIndex, count);
		else return new List<T>();
	}
	
	public int GetItemIndex(T item) {
		// TODO: return data.FindIndex(new Predicate<T>(item));
		
		for (int i=0; i<data.Count; i++) {
			if (data[i] != null && data[i].Equals(item)) return i;
		}
		return -1;
	}
	
	/*public void Merge(object newData) {
		List<T> arr = getDataFromObject(newData);
		int l = arr.Count;
		int startLength = data.Count;
		
		dispatchPreChangeEvent("add",data.slice(startLength,data.Count),startLength,this.data.Count-1);
		
		for (int i=0; i<l; i++) {
			object item = arr[i];
			if (GetItemIndex(item) == -1) {
				data.push(item);
			}
		}
		if (data.Count > startLength) {
			dispatchChangeEvent("add",data.slice(startLength,data.Count),startLength,this.data.Count-1);
		} else {
			//dispatchChangeEvent("add",[],-1,-1);
		}
	}*/
	
	public T RemoveItemAt(int index) {
		return RemoveItemAt(index, true);
	}
	
	public T RemoveItemAt(int index, bool dispatch) {
		T oldItem = GetItemAt(index);
		if (dispatch) dispatchPreChangeEvent(CollectionEventKind.REMOVE, oldItem, index, index);
		
		if (capacity > 0) data[index] = default(T);
		else data.RemoveAt(index);
		
		if (dispatch) dispatchChangeEvent(CollectionEventKind.REMOVE, oldItem, index, index);
		return oldItem;
	}
	
	public T RemoveItem(T item) {
		int index = GetItemIndex(item);
		if (index != -1) {
			return RemoveItemAt(index);
		}
		return default(T);
	}
	
	public void RemoveAll() {
		RemoveAll(true);
	}
	
	public void RemoveAll(bool dispatch) {
		T[] all = data.ToArray();
		if (dispatch) dispatchPreChangeEvent(CollectionEventKind.REMOVE_ALL, all, 0, all.Length);
		
		data.Clear();
		
		if (dispatch) dispatchChangeEvent(CollectionEventKind.REMOVE_ALL, all, 0, all.Length);
	}
	
	public T ReplaceItem(T newItem, T oldItem) {
		int index = GetItemIndex(oldItem);
		if (index != -1) {
			return ReplaceItemAt(newItem, index);
		}
		return default(T);
	}
	
	public T ReplaceItemAt(T newItem, int index) {
		return ReplaceItemAt(newItem, index, true);
	}
	
	public T ReplaceItemAt(T newItem, int index, bool dispatch) {
		T oldItem = GetItemAt(index);
		if (dispatch) dispatchPreChangeEvent(CollectionEventKind.REPLACE, oldItem, index, index);
		
		data[index] = newItem;
		
		if (dispatch) dispatchChangeEvent(CollectionEventKind.REPLACE, oldItem, index, index);
		return oldItem;
	}
	
	public void ReplaceItemsAt(object items, int index) {
		ReplaceItemsAt(items, index, true);
	}
	
	public void ReplaceItemsAt(object items, int index, bool dispatch) {
		List<T> list = getDataFromObject(items);
		int endIndex = index+list.Count;
		
		if (dispatch) dispatchPreChangeEvent(CollectionEventKind.REPLACE, list, index, endIndex);
		
		// TODO - data.SetRange(index, list);
		
		if (dispatch) dispatchChangeEvent(CollectionEventKind.REPLACE, list, index, endIndex);
	}
	
	/*public function Sort(...sortArgs:Array):* {
		dispatchPreChangeEvent("sort",data.slice(0),0,data.Count-1);
		var returnValue:Array = data.sort.apply(data,sortArgs);
		dispatchChangeEvent("sort",data.slice(0),0,data.Count-1);
		return returnValue;
	}*/
	
	/*public function SortOn(fieldName:Object,options:Object=null):* {
		dispatchPreChangeEvent("sort",data.slice(0),0,data.Count-1);
		var returnValue:Array = data.sortOn(fieldName,options);
		dispatchChangeEvent("sort",data.slice(0),0,data.Count-1);
		return returnValue;
	}*/
	
	public void Refresh() {
		Refresh(true);
	}
	
	public void Refresh(bool dispatch) {
		var items = ToList();
		if (dispatch) dispatchChangeEvent(CollectionEventKind.RESET, items, 0, items.Count);
	}
	
	public System.Object Clone() {
		return new DataProvider<T>(data);
	}
	
	public List<T> ToList() {
		List<T> items = new List<T>();
		items.AddRange(data);
		return items;
	}
	
	override public string ToString() {
		string output = "";
		
		for (int i = 0; i < length; i++) {
			string item = (data[i] == null) ? "null" : data[i].ToString();
			output += item + (i < length-1 ? "," : "");
		}
		
		return "DataProvider ["+output+"]";
	}
	
	#endregion
	#region - protected methods -
	
	// convert object to List<T>
	protected List<T> getDataFromObject(object source) {
		
		if (source == null) {
			return new List<T>();
		
		} else if (source is T) {
			return new List<T>{(T) source};
			
		} else if (source is ICollection) {
			var list = new List<T>();
			
			var collection = new ArrayList(source as ICollection);
			
			for (int i = 0; i<collection.Count; i++) {
				list.Add( (T) collection[i]);
			}
			
			return list;
			
		} else if (source is DataProvider<T>) {
			return (source as DataProvider<T>).ToList();
			
		} else {
			throw new UnityException("Error: Type Coercion failed: cannot convert "+source+" to Array or DataProvider.");
		}
	}
	
	protected int previousDirection;
	
	protected int wrapIndex(int oldIndex, int newIndex) {
		if (data == null) return -1;
		
		int direction = (newIndex < 0) ? -1 : (newIndex >= data.Count) ? 1 : 0;
		if (direction == 0) return newIndex;
		
		/**
		 * WrapMode.Default: Read's the wrap mode from the clip (default for a clip is Once).
		 * WrapMode.Once: Stops the animation when time reaches the end.
		 * WrapMode.Loop: Starts at the beginning when time reaches the end.
		 * WrapMode.PingPong: Ping Pong's back and forth between beginning and end.
		 * WrapMode.ClampForever: Plays back the animation. When it reaches the end, it will keep sampling the last frame. 
		**/
		
		// direction is left or right beyond the list
		// else would've resulted in an IndexOutOfRangeException
	
		switch (wrapMode) {
		
		default:
			goto case WrapMode.Default;
			
		case WrapMode.Default:
			goto case WrapMode.Once;
			
		case WrapMode.Once:
			if (direction == previousDirection) {
				// we still are going in the right direction
				// keep going until the end
				newIndex = oldIndex + direction;
				
			} else {
				
			}
			break;
			
		case WrapMode.Loop:
			if (direction < 0) newIndex = (data.Count > 0) ? data.Count : -1;
			else if (direction > 0) newIndex = (data.Count > 0) ? 0 : -1;
			break;
		
		case WrapMode.PingPong:
			break;
		
		/*case WrapMode.Clamp:
			if (direction < 0) newIndex = (data.Count > 0) ? 0 : -1;
			else if (direction > 0) newIndex = (data.Count > 0) ? data.Count : -1;
			break;*/
		
		case WrapMode.ClampForever:
			newIndex = Mathf.Clamp(newIndex, -1, data.Count);
			break;
		}
		
		previousDirection = direction;
		return newIndex;
	}
	
	protected void validateIndex(int index) {
		if (index < 0 || index > maximum) {
			throw new IndexOutOfRangeException("DataProvider index ("+index+") is not in acceptable range (0 - "+maximum+")");
		}
	}
	
	protected void validateIndex(int index, int maximum) {
		if (index < 0 || index > maximum) {
			throw new IndexOutOfRangeException("DataProvider index ("+index+") is not in acceptable range (0 - "+maximum+")");
		}
	}
	
	protected void dispatchChangeEvent(CollectionEventKind kind, object items, int startIndex, int endIndex) {
		if (OnCollectionChange != null) OnCollectionChange(kind, this, items, startIndex, endIndex);
	}
	
	protected void dispatchPreChangeEvent(CollectionEventKind kind, object items, int startIndex, int endIndex) {
		if (OnCollectionPreChange != null) OnCollectionPreChange(kind, this, items, startIndex, endIndex);
	}
	
	protected void dispatchIndexChangeEvent(IndexChangeEventType type, int oldIndex, int newIndex) {
		if (OnIndexChange != null) OnIndexChange(type, this, oldIndex, newIndex);
	}
	
	protected void dispatchIndexChangingEvent(IndexChangeEventType type, int oldIndex, int newIndex) {
		if (OnIndexChanging != null) OnIndexChanging(type, this, oldIndex, newIndex);
	}
	
	#endregion
	#region - untested methods -
	
	// TODO - switch (wrapMode)
	public T Next() {
		selectedIndex++;
		return selectedItem;
	}
	
	public T Previous() {
		selectedIndex--;
		return selectedItem;
	}
	
	public bool hasNext {
		get { return (selectedIndex + 1 <= length-1); }
	}
	
	public bool hasPrevious {
		get { return (selectedIndex - 1 >= 0); }
	}
	
	#endregion
	
	// ---- delegates ----
	
	public delegate void CollectionEventHandler(CollectionEventKind kind, DataProvider<T> currentTarget, object items, int startIndex, int endIndex);
	public delegate void IndexChangeEventHandler(IndexChangeEventType type, DataProvider<T> currentTarget, int oldIndex, int newIndex);
}

public enum CollectionEventKind {
	// TODO: MOVE, REFRESH, UPDATE
	ADD, REMOVE, REPLACE, REMOVE_ALL, RESET
}

public enum IndexChangeEventType {
	CHANGE, CHANGING
}