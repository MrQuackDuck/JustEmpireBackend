using System.Security.Cryptography;
using System.Text;
using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;

namespace JustEmpire.Services.Classes.Repositories;

public class UserRepository
{
    private JustEmpireDbContext _dbContext;
    
    public UserRepository(JustEmpireDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Create(User user)
    {
        try
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return true;
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
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            return true;
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
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
            return true;
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
            _dbContext.Users.Remove(target!);
            _dbContext.SaveChanges();
            return true;
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

    public User? GetById(int id)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Id == id);
    }

    public User? GetByUsername(string username)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Username == username);
    }
    
    public bool ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var user = GetById(userId);
        if (user is null) return false;

        if (!IsPasswordCorrect(user.Username, oldPassword)) return false;

        user.PasswordHash = Sha256(newPassword);
        _dbContext.SaveChanges();
        return true;
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
        var crypt = SHA256.Create();
        string hash = String.Empty;
        byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(inputString));
        foreach (byte theByte in crypto)
        {
            hash += theByte.ToString("x2");
        }
        
        return hash;
    }
}