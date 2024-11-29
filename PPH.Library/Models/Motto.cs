using SQLite;

namespace PPH.Library.Models;


public class Motto {
    public string Content { get; set; } = string.Empty;
    
    public string Translation { get; set; } = string.Empty; 
    
    public string Source { get; set; } = string.Empty; 
    
    public string Date { get; set; } = string.Empty;
    
    public string Author { get; set; } = string.Empty;
}