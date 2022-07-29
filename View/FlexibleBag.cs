using System.Collections.Concurrent;

namespace Reference_Enflow_Builder.View {
    public class FlexibleBag<T> : ConcurrentBag<T> {
        public FlexibleBag<T> Remove(T item_to_remove) {
            if (item_to_remove is null) return this;

            FlexibleBag<T> new_bag = new();
            foreach(T item in this) {
                if (item_to_remove.Equals(item)) continue;
                new_bag.Add(item);
            }
            return new_bag;
        }
    }
}
