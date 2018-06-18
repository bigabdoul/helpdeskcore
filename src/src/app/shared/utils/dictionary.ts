
export class SimpleDictionary<TKey, TValue> {

  private _unique: boolean = true;
  protected items: KeyValuePair<TKey, TValue>[] = [];

  constructor(...items: KeyValuePair<TKey, TValue>[]) {
    if (items) {
      this.addRange(items);
    }
  }
  
  add(item: KeyValuePair<TKey, TValue>) {
    if (this._unique && this.contains(i => i.key === item.key)) {
      throw 'Invalid operation: the key of the item already exists.';
    }
    this.items.push(item);
    return this;
  }

  addRange(items: KeyValuePair<TKey, TValue>[]) {
    for (let i = 0; i < items.length; i++) {
      this.add(items[i]);
    }
    return this;
  }

  get(key: TKey, cb?: (item: (KeyValuePair<TKey, TValue>), index: number) => void) {
    const items = this.items;
    for (let i = 0; i < items.length; i++) {
      const kvp = items[i];
      if (kvp.key === key) {
        if (cb) {
          cb(kvp, i);
        }
        return kvp;
      }
    }
    return false;
  }

  all(cb: (item: (KeyValuePair<TKey, TValue>), index: number) => void) {
    for (let i = 0; i < this.items.length; i++) {
      cb(this.items[i], i);
    }
    return this;
  }

  remove(key: TKey) {
    this.get(key, (item, i) => {
      this.items.splice(i, 1);
      return true;
    });
    return false;
  }

  clear() {
    this.items = [];
    return this;
  }

  contains(predicate: (item: KeyValuePair<TKey, TValue>) => boolean) {
    const items = this.items;
    for (let i = 0; i < items.length; i++) {
      if (predicate(items[i])) {
        return true;
      }
    }
    return false;
  }

  containsKey(key: TKey) {
    return this.contains(item => item.key === key);
  }

  containsValue(value: TValue) {
    return this.contains(item => item.value === value);
  }

  uniqueKeys(value: boolean) {
    this._unique = value;
    return this;
  }
}

export class KeyValuePair<TKey, TValue> {
  
  constructor(public key: TKey, public value: TValue) {
  }

}
