using System.Text.Json;
using JustEmpire.Models.Classes;

namespace JustEmpire.Services;

public class RankRepository
{
    private string _filepath = "Ranks.json";

    public Rank GetById(int id)
    {
        return GetRanks().FirstOrDefault(rank => rank.Id == id) ?? null;
    }
    
    public List<Rank> GetRanks()
    {
        string jsonString = File.ReadAllText(_filepath);
        var ranks = JsonSerializer.Deserialize<List<Rank>>(jsonString);

        return ranks;
    }
}