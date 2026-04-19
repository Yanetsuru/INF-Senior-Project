using INF_Senior_Project.Controllers;
using INF_Senior_Project.Models;
using INF_Senior_Project.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class TestSession : ISession
{
    Dictionary<string, byte[]> store = new();

    public IEnumerable<string> Keys => store.Keys;
    public string Id => Guid.NewGuid().ToString();
    public bool IsAvailable => true;

    public void Clear() => store.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => store.Remove(key);
    public void Set(string key, byte[] value) => store[key] = value;
    public bool TryGetValue(string key, out byte[] value) => store.TryGetValue(key, out value);
}