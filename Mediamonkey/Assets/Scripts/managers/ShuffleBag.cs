using System;
using System.Collections;
using System.Collections.Generic;

public sealed class ShuffleBag<T> : IEnumerable<T> {
	
	private Random		generator;
	private List<T>		bag;
	private int			cursor = -1;
	private T			current = default(T);
	
	// ---- constructors ----
	
	public ShuffleBag() : this(10, new Random()) { }
	
	public ShuffleBag(Random generator) : this(10, generator) { }
	
	public ShuffleBag(int initialCapacity) : this(initialCapacity, new Random()) { }
	
	public ShuffleBag(int initialCapacity, Random generator) {
		if (initialCapacity < 0) throw new ArgumentException("Capacity must be a positive integer.");

		this.generator = generator;
		bag = new List<T>(initialCapacity);
	}

	// ---- public methods ----
	
	public void Add(T item) {
		Add(item, 1);
	}
	
	// Adds an item to the bag multiple times.
	public void Add(T item, int quantity) {
		if (quantity <= 0) throw new ArgumentException("Quantity must be a positive integer.");
		
		// add items
		for (int i = 0; i < quantity; i++) bag.Add(item);

		// Resetting the cursor to the end makes it possible to get freshly added values right away,
		// otherwise it would have to finish this run first.
		cursor = bag.Count - 1;
	}

	// Pulls an item out of the bag.
	public T Next() {
		if (cursor < 1) {
			cursor = bag.Count - 1;
			current = bag[0];
			return current;
		}

		int index = generator.Next(cursor);
		current = bag[index];
		bag[index] = bag[cursor];
		bag[cursor] = current;
		cursor--;
		return current;
	}

	// The last element that was returned from Next(). Can be null, if Next() has not been called yet.
	public T Current {
		get { return current; }
	}

	// The current capacity of the underlying storage.
	public int Capacity {
		get { return bag.Capacity; }
	}

	// Reduces the capacity as much as possible to save memory.
	public void TrimExcess() {
		bag.TrimExcess();
	}

	// The number of elements in this bag.
	public int Size {
		get { return bag.Count; }
	}
	
	// ---- enumerators ----
	
	// Returns a sequence of random elements from the bag
	IEnumerator<T> IEnumerable<T>.GetEnumerator() {
		for (int i=0; i<=Size; i++) {
			yield return this.Next();
		}
	}

	// Returns a sequence of random elements from the bag.
	IEnumerator IEnumerable.GetEnumerator() {
		return ((IEnumerable<T>)this).GetEnumerator();
	}
}