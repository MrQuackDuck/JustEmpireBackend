using JustEmpire.Models.Interfaces;

namespace JustEmpire.Services;

public interface IRepository<T> where T : IQueueable
{
    public T Create(T article);
    public T Update(T article);
    public bool Delete(T article);
    public bool Delete(int id);
    public int GetTotalCount();
    public List<T> GetAll();
    public T GetById(int id);
    public T GetByOriginalId(int id);
}