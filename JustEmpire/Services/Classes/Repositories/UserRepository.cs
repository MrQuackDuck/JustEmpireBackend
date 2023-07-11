using System.Security.Cryptography;
using System.Text;
using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;

namespace JustEmpire.Services;

public class UserRepository
{
    private DatabaseContext _dbContext;
    
    public UserRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Create(User user)
    {
        try
        {
            bool success = _dbContext.Users.Add(user) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Update(User user)
    {
        try
        {
            bool success = _dbContext.Users.Update(user) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Delete(User user)
    {
        try
        {
            bool success = _dbContext.Users.Remove(user) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            var target = _dbContext.Users.FirstOrDefault(user => user.Id == id);
            bool success = _dbContext.Users.Remove(target) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public List<User> GetAll()
    {
        return _dbContext.Users
            .OrderByDescending(u => u.Id)
            .ToList();
    }

    public int GetTotalCount()
    {
        return _dbContext.Users.Count();
    }

    public User GetById(int id)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Id == id) ?? null;
    }

    public User GetByUsername(string username)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Username == username) ?? null;
    }
    
    public bool IsPasswordCorrect(string username, string password)
    {
        var user = GetByUsername(username);
        if (user == null) return false;
        if (user.PasswordHash == Sha256(password)) return true;

        return false;
    }

    public bool DoesUserExist(string username)
    {
        var target = _dbContext.Users.FirstOrDefault(user => user.Username == username);
        return !(target == null);
    }
    
    public string Sha256(string inputString)
    {
        var crypt = new SHA256Managed();
        string hash = String.Empty;
        byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(inputString));
        foreach (byte theByte in crypto)
        {
            hash += theByte.ToString("x2");
        }
        return hash;
    }
}