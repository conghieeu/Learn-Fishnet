using Bifrost;

public delegate bool OnValidateCallback<T>(T item) where T : ISchema;
