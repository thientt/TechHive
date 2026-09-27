using System.Collections.Concurrent;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public interface IUserStore
{
    IEnumerable<User> GetAll();
    User? Get(int id);
    User Add(User user);
    bool Update(int id, User user);
    bool Delete(int id);
}

public class InMemoryUserStore : IUserStore
{
    private readonly ConcurrentDictionary<int, User> _users = new();
    private int _nextId;

    public IEnumerable<User> GetAll() => _users.Values;

    public User? Get(int id) => _users.TryGetValue(id, out var user) ? user : null;

    public User Add(User user)
    {
        user.Id = Interlocked.Increment(ref _nextId);
        _users[user.Id] = user;
        return user;
    }

    public bool Update(int id, User user)
    {
        if (!_users.ContainsKey(id))
        {
            return false;
        }

        user.Id = id;
        _users[id] = user;
        return true;
    }

    public bool Delete(int id) => _users.TryRemove(id, out _);
}
